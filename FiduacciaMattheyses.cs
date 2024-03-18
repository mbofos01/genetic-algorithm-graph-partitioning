using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace genetic_algorithm_graph_partitioning
{

    /// <summary>
    /// Helper class to represent a chunk of data used for the Fiduaccia Mattheyses heuristic.
    /// </summary>
    public class Pair
    {
        /// <summary>
        /// Vertex id.
        /// </summary>
        public int vertex;
        /// <summary>
        /// Offset value of moving the vertex.
        /// </summary>
        public int value;
        /// <summary>
        /// Whether the solution is valid.
        /// </summary>
        public bool valid;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pair"/> class with the specified vertex, value and validity.
        /// </summary>
        /// 
        /// <param name="vertex">The vertex id</param>
        /// <param name="value">The vertex value</param>
        /// <param name="valid">Validity of vertex</param>
        public Pair(int vertex, int value, bool valid)
        {
            this.vertex = vertex;
            this.value = value;
            this.valid = valid;
        }
    }


    /// <summary>
    /// Represents a solution for the graph partitioning problem using the Fiduaccia Mattheyses heuristic.  <a href="https://www.youtube.com/watch?v=CKdc5Ej2jSE">Youtube Video</a> 
    /// </summary>
    public class FiduacciaMattheysesHeuristic
    {
        /// <summary>
        /// Helper function to print the buckets.
        /// </summary>
        /// 
        /// <param name="A">The first bucket.</param>
        /// <param name="B">The second bucket.</param>
        public static void ViewBuckets(Bucket A, Bucket B)
        {
            Console.WriteLine("------------------------------ ");

            A.PrintBucket();

            Console.WriteLine("------------------------------ ");

            B.PrintBucket();

            Console.WriteLine("------------------------------ ");
        }

        /// <summary>
        /// Helper function to clean the vertices. Free them detach them from the linked list and set their gain to 0.
        /// </summary>
        /// 
        /// <param name="vertices">The vertices to clean.</param>
        public static void CleanVertices(List<Vertex> vertices)
        {
            foreach (Vertex v in vertices)
            {
                v.free = true;
                v.SetGain(0);
                v.next = null;
                v.previous = null;
            }
        }

        /// <summary>
        /// Represents a solution for the graph partitioning problem using the Fiduaccia Mattheyses heuristic.  <a href="https://www.youtube.com/watch?v=CKdc5Ej2jSE">Youtube Video</a>
        /// </summary>
        /// 
        /// 
        /// <param name="parent">The initial solution.</param>
        /// <param name="g">The graph to partition.</param>
        /// <param name="debug">Whether to print debug information.</param>
        /// <returns>The best solution found.</returns>
        public static Solution FiduacciaMattheyses(Solution parent, Graph g, bool debug = false)
        {
            const int SAME_PARTITION_OFFSET = +2;
            const int DIFFERENT_PARTITION_OFFSET = -2;

            int FM_PASS_COUNTER = 0;
            int max_degree = g.GetMaxDegree();
            int array_size = 2 * max_degree + 1;

            Random random = new();
            Solution working = parent.Clone();
            Solution best_solution = parent.Clone();
            List<Vertex> vertices = g.GetVertices();

            CleanVertices(vertices);

            do
            {
                parent = best_solution.Clone();
                working = parent.Clone();

                int changes_made_in_this_search = 0;
                parent.SetScore(g.ScoreBiPartition(parent));

                Queue<Pair> Changes = new Queue<Pair>();

                Bucket A = new(max_degree);
                Bucket B = new(max_degree);

                foreach (Vertex v in vertices)
                {
                    // copy solution in order to calculate hypothetical score
                    Solution child = parent.Clone();
                    // make an hypothetical move and calculate the score
                    child.SwitchPartitioning(v.id - 1);
                    int child_score = g.ScoreBiPartition(child);

                    // Keep in mind that we use the inverse of the current team
                    // 0 is team A and 1 is team B
                    v.SetGain(parent.Score() - child_score);
                    changes_made_in_this_search += v.GetGain();
                    if (child.GetPartitioning()[v.id - 1] == 1)
                        A.AddToBucket(v, v.GetGain(), random);
                    else
                        B.AddToBucket(v, v.GetGain(), random);
                    // arrays are filled with the vertices that can be moved
                }

                // if there is no improvement we stop the fm pass ~ we have reached a local optimum
                if (changes_made_in_this_search <= 0)
                {
                    // it is important to clean the vertices before returning the solution
                    // if we don't do this, the next call to the function will have the vertices
                    // attached to the linked list creating a havoc
                    CleanVertices(vertices);

                    if (parent.IsValid(array_size))
                        throw new ValidationException("The solution is not valid");

                    parent.SetFMPasses(FM_PASS_COUNTER);
                    return parent;
                }

                // We count FM pass as valid iff there are possible improvements
                FM_PASS_COUNTER++;

                while (vertices.Any(v => v.GetFree()))
                {
                    if (debug)
                        ViewBuckets(A, B);

                    // if there are no vertices to move we stop the greedy process
                    if (A.GetPopulation() == 0 && B.GetPopulation() == 0)
                        break;
                    else
                    {
                        // this vertex is the chosen vertex we move and update its neighbors
                        Vertex? to_be_locked = null;
                        int[] neighbors;

                        if (A.GetPopulation() >= B.GetPopulation())
                        {
                            // we get the position in the bucket of the vertex with the highest gain ~ in other words its gain but encoded on the array
                            int max_pointer = A.GetMaxActiveDegree();

                            if (debug)
                                Console.WriteLine($"Max Pointer: {max_pointer}");

                            // now that we have the position of the vertex with the highest gain we get the vertex itself
                            to_be_locked = A.GetFirstVertex(max_pointer);

                            if (debug)
                                Console.WriteLine($"To be locked: {to_be_locked.id} of {parent.GetPartitioning()[to_be_locked.id - 1]}");

                            // we get the neighbors of the vertex to update them
                            neighbors = to_be_locked.connections;

                            if (debug)
                            {
                                Console.WriteLine($"Neighbors: {string.Join(", ", neighbors)}");
                                Console.WriteLine($"Neighbors: {string.Join(", ", neighbors.Select(n => parent.GetPartitioning()[n - 1]))}");
                            }

                            // we remove the vertex from the bucket
                            A.RemoveFromBucket(to_be_locked);

                            // we update the neighbors
                            foreach (int neighbor in neighbors)
                            {
                                Vertex vertex = vertices[neighbor - 1];
                                // if the vertex is not free we skip it
                                if (vertex.GetFree() == false)
                                {
                                    continue;
                                }
                                if (parent.GetPartitioning()[neighbor - 1] == 0)
                                {
                                    // our active vertex is in group zero, so if the neighbor is in group zero 
                                    // we remove the neighbor vertex from the bucket
                                    A.RemoveFromBucket(vertex);
                                    if (debug)
                                        Console.WriteLine($"Adding vertex {vertex.id} to {vertex.GetGain() - SAME_PARTITION_OFFSET} ");

                                    // we remove it from the bucket and update its gain by adding 2 to the gain
                                    // the mismatch between the debug message and the actual code is not a bug
                                    // higher gain means less conflicts thus lower score
                                    vertex.SetGain(vertex.GetGain() + SAME_PARTITION_OFFSET);
                                    A.AddToBucket(vertex, vertex.GetGain(), random);
                                }
                                else
                                {
                                    // our active vertex is in group zero, so if the neighbor is in group one
                                    // we remove the neighbor vertex from the bucket
                                    B.RemoveFromBucket(vertex);

                                    if (debug)
                                        Console.WriteLine($"Adding vertex {vertex.id} to {vertex.GetGain() - DIFFERENT_PARTITION_OFFSET} ");

                                    // we remove it from the bucket and update its gain by adding -2 to the gain
                                    // the mismatch between the debug message and the actual code is not a bug
                                    // higher gain means less conflicts thus lower score
                                    vertex.SetGain(vertex.GetGain() + DIFFERENT_PARTITION_OFFSET);
                                    B.AddToBucket(vertex, vertex.GetGain(), random);
                                }
                            }

                        }
                        else
                        {
                            // we get the position in the bucket of the vertex with the highest gain ~ in other words its gain but encoded on the array
                            int max_pointer = B.GetMaxActiveDegree();

                            if (debug)
                                Console.WriteLine($"Max Pointer: {max_pointer}");

                            // now that we have the position of the vertex with the highest gain we get the vertex itself
                            to_be_locked = B.GetFirstVertex(max_pointer);

                            if (debug)
                                Console.WriteLine($"To be locked: {to_be_locked.id} of {parent.GetPartitioning()[to_be_locked.id - 1]}");

                            // we get the neighbors of the vertex to update them
                            neighbors = to_be_locked.connections;

                            if (debug)
                            {
                                Console.WriteLine($"Neighbors: {string.Join(", ", neighbors)}");
                                Console.WriteLine($"Neighbors: {string.Join(", ", neighbors.Select(n => parent.GetPartitioning()[n - 1]))}");
                            }

                            // we remove the vertex from the bucket
                            B.RemoveFromBucket(to_be_locked);

                            // we update the neighbors
                            foreach (int neighbor in neighbors)
                            {
                                Vertex vertex = vertices[neighbor - 1];
                                // if the vertex is not free we skip it
                                if (vertex.GetFree() == false)
                                {
                                    continue;
                                }

                                if (parent.GetPartitioning()[neighbor - 1] == 0)
                                {
                                    // our active vertex is in group one so if the neighbor is in group zero 
                                    // we remove the neighbor vertex from the bucket
                                    A.RemoveFromBucket(vertex);
                                    if (debug)
                                        Console.WriteLine($"Adding vertex {vertex.id} to {vertex.GetGain() - DIFFERENT_PARTITION_OFFSET} ");

                                    // we remove it from the bucket and update its gain by adding -2 to the gain
                                    // the mismatch between the debug message and the actual code is not a bug
                                    // higher gain means less conflicts thus lower score
                                    vertex.SetGain(vertex.GetGain() + DIFFERENT_PARTITION_OFFSET);
                                    A.AddToBucket(vertex, vertex.GetGain(), random);
                                }
                                else
                                {
                                    // our active vertex is in group one so if the neighbor is in group one
                                    // we remove the neighbor vertex from the bucket
                                    B.RemoveFromBucket(vertex);
                                    if (debug)
                                        Console.WriteLine($"Adding vertex {vertex.id} to {vertex.GetGain() - SAME_PARTITION_OFFSET} ");

                                    // we remove it from the bucket and update its gain by adding 2 to the gain
                                    // the mismatch between the debug message and the actual code is not a bug
                                    // higher gain means less conflicts thus lower score
                                    vertex.SetGain(vertex.GetGain() + SAME_PARTITION_OFFSET);
                                    B.AddToBucket(vertex, vertex.GetGain(), random);
                                }
                            }

                        }

                        // we lock the vertex we change its partitioning and we add it to the changes queue
                        to_be_locked.SetFree(false);
                        working.SwitchPartitioning(to_be_locked.id - 1);
                        Changes.Enqueue(new Pair(to_be_locked.id, to_be_locked.GetGain(), working.IsValid()));

                    }

                }

                if (debug)
                {
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("Changes: ");
                    foreach (Pair p in Changes)
                    {
                        Console.WriteLine($"Vertex: {p.vertex} Score: {p.value} Valid: {p.valid}");
                    }
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine($"Parent: {parent.ToString()} -- Score:{parent.Score()}");
                }

                int counter = Changes.Count;
                // we clone the parent solution so that we can make the changes and check if the solution is valid
                // without changing the parent solution which is used later
                Solution parent_clone = parent.Clone();

                for (int i = 0; i < counter; i++)
                {
                    Pair active_change = Changes.Dequeue();

                    // we apply the changes to the parent solution (its clone)
                    parent_clone.SwitchPartitioning(active_change.vertex - 1);
                    if (debug)
                        Console.WriteLine($"Parent: {parent_clone.ToString()} -- Offset: {active_change.value} -- Score:{parent_clone.Score() - active_change.value}  -- Valid: {parent_clone.IsValid()}");

                    // we apply the offset of the change to the score
                    // this is where the complicated notion of +2/-2 kicks in
                    parent_clone.SetScore(parent_clone.Score() - active_change.value);

                    // if the solution is valid and the score is better than the best solution we update the best solution
                    if (parent_clone.IsValid() && best_solution.Score() > parent_clone.Score())
                    {
                        best_solution = parent_clone.Clone();
                    }
                }

                // it is important to clean the vertices before returning the solution
                // if we don't do this, the next call to the function will have the vertices
                // attached to the linked list creating a havoc
                CleanVertices(vertices);

                // if the best solution is invalid we throw an exception ~ simple sanity check
                if (best_solution.IsValid(array_size))
                    throw new ValidationException("The solution is not valid");

            } while (true);

        }

        /// <summary>
        /// Represents a solution for the graph partitioning problem using the Fiduaccia Mattheyses heuristic.  <a href="https://www.youtube.com/watch?v=CKdc5Ej2jSE">Youtube Video</a>
        /// </summary>
        /// 
        /// <param name="g">The graph to partition.</param>
        /// <param name="debug">Whether to print debug information.</param>
        ///  
        /// <returns>The best solution found.</returns>
        public static Solution FiduacciaMattheyses(Graph g, bool debug = false)
        {
            List<Vertex> vertices = g.GetVertices();
            Solution parent = new Solution(vertices.Count);
            return FiduacciaMattheyses(parent, g, debug);
        }
    }
}