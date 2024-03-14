using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace genetic_algorithm_graph_partitioning
{

    public class Pair
    {
        public int vertex;
        public int value;

        public bool valid;
        public Pair(int vertex, int value, bool valid)
        {
            this.vertex = vertex;
            this.value = value;
            this.valid = valid;
        }
    }

    public class FiduacciaMattheysesHeuristic
    {
        /// <summary>
        /// Helper function to calculate the index of the array.
        /// </summary>
        /// 
        /// <param name="score">The score of the current solution.</param>
        /// <param name="max_degree">The maximum degree of the graph.</param>
        /// <returns>The index of the array.</returns> 
        public static int CalculateIndex(int score, int max_degree)
        {
            return score + (max_degree / 2);
        }

        public static int DeCalculateIndex(int index, int max_degree)
        {
            return index - (max_degree / 2);
        }

        public static void ViewBuckets(Bucket A, Bucket B)
        {
            Console.WriteLine("------------------------------ ");

            A.PrintBucket();

            Console.WriteLine("------------------------------ ");

            B.PrintBucket();

            Console.WriteLine("------------------------------ ");
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
            foreach (Vertex v in g.GetVertices())
            {
                v.free = true;
                v.SetGain(0);
                v.next = null;
                v.previous = null;
            }
            Random random = new();
            const int SAME_PARTITION_OFFSET = +2;
            const int DIFFERENT_PARTITION_OFFSET = -2;
            int FM_PASS_COUNTER = 1;

            Solution working = parent.Clone();
            Solution best_solution = parent.Clone();
            do
            {
                parent = best_solution.Clone();
                int array_size = 2 * g.GetMaxDegree() + 1;
                int active_score = g.ScoreBiPartition(parent);
                int BestScore = active_score;
                int changes = 0;
                parent.SetScore(active_score);

                Queue<Pair> Changes = new Queue<Pair>();

                if (debug)
                    Console.WriteLine($"Starting with {parent.ToString()} Score: {BestScore}");



                List<Vertex> vertices = g.GetVertices();

                Bucket A = new(g.GetMaxDegree());
                Bucket B = new(g.GetMaxDegree());

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
                    changes += v.GetGain();
                    if (child.GetPartitioning()[v.id - 1] == 1)
                        A.AddToBucket(v, v.GetGain(), random);
                    else
                        B.AddToBucket(v, v.GetGain(), random);
                    // arrays are filled with the vertices that can be moved
                }

                if (changes <= 0)
                {
                    parent.SetFMPasses(FM_PASS_COUNTER);
                    return parent;
                }

                while (vertices.Any(v => v.GetFree()))
                {
                    if (debug)
                        ViewBuckets(A, B);

                    if (A.GetPopulation() == 0 && B.GetPopulation() == 0)
                        break;
                    else
                    {
                        Vertex? to_be_locked = null;
                        int[] neighbors;

                        if (A.GetPopulation() >= B.GetPopulation())
                        {
                            int max_pointer = A.GetMaxActiveDegree();

                            if (debug)
                                Console.WriteLine($"Max Pointer: {max_pointer}");

                            to_be_locked = A.GetFirstVertex(max_pointer);

                            if (debug)
                                Console.WriteLine($"To be locked: {to_be_locked.id} of {parent.GetPartitioning()[to_be_locked.id - 1]}");

                            neighbors = to_be_locked.connections;

                            if (debug)
                            {
                                Console.WriteLine($"Neighbors: {string.Join(", ", neighbors)}");
                                Console.WriteLine($"Neighbors: {string.Join(", ", neighbors.Select(n => parent.GetPartitioning()[n - 1]))}");
                            }

                            A.RemoveFromBucket(to_be_locked);

                            foreach (int neighbor in neighbors)
                            {
                                Vertex vertex = vertices[neighbor - 1];
                                if (vertex.GetFree() == false)
                                {
                                    continue;
                                }

                                if (parent.GetPartitioning()[neighbor - 1] == 0)
                                {
                                    // same partition as the to_be_locked vertex
                                    A.RemoveFromBucket(vertex);
                                    if (debug)
                                        Console.WriteLine($"Adding vertex {vertex.id} to {vertex.GetGain() - SAME_PARTITION_OFFSET} ");
                                    vertex.SetGain(vertex.GetGain() + SAME_PARTITION_OFFSET);
                                    A.AddToBucket(vertex, vertex.GetGain(), random);
                                }
                                else
                                {
                                    B.RemoveFromBucket(vertex);
                                    if (debug)
                                        Console.WriteLine($"Adding vertex {vertex.id} to {vertex.GetGain() - DIFFERENT_PARTITION_OFFSET} ");
                                    vertex.SetGain(vertex.GetGain() + DIFFERENT_PARTITION_OFFSET);
                                    B.AddToBucket(vertex, vertex.GetGain(), random);
                                }
                            }

                        }
                        else
                        {
                            int max_pointer = B.GetMaxActiveDegree();

                            if (debug)
                                Console.WriteLine($"Max Pointer: {max_pointer}");
                            to_be_locked = B.GetFirstVertex(max_pointer);

                            if (debug)
                                Console.WriteLine($"To be locked: {to_be_locked.id} of {parent.GetPartitioning()[to_be_locked.id - 1]}");

                            neighbors = to_be_locked.connections;

                            if (debug)
                            {
                                Console.WriteLine($"Neighbors: {string.Join(", ", neighbors)}");
                                Console.WriteLine($"Neighbors: {string.Join(", ", neighbors.Select(n => parent.GetPartitioning()[n - 1]))}");
                            }

                            B.RemoveFromBucket(to_be_locked);


                            foreach (int neighbor in neighbors)
                            {
                                Vertex vertex = vertices[neighbor - 1];
                                if (vertex.GetFree() == false)
                                {
                                    continue;
                                }

                                if (parent.GetPartitioning()[neighbor - 1] == 0)
                                {
                                    A.RemoveFromBucket(vertex);
                                    if (debug)
                                        Console.WriteLine($"Adding vertex {vertex.id} to {vertex.GetGain() - DIFFERENT_PARTITION_OFFSET} ");
                                    vertex.SetGain(vertex.GetGain() + DIFFERENT_PARTITION_OFFSET);
                                    A.AddToBucket(vertex, vertex.GetGain(), random);
                                }
                                else
                                {
                                    B.RemoveFromBucket(vertex);
                                    if (debug)
                                        Console.WriteLine($"Adding vertex {vertex.id} to {vertex.GetGain() - SAME_PARTITION_OFFSET} ");
                                    vertex.SetGain(vertex.GetGain() + SAME_PARTITION_OFFSET);
                                    B.AddToBucket(vertex, vertex.GetGain(), random);
                                }
                            }

                        }

                        to_be_locked.SetFree(false);

                        working.SwitchPartitioning(to_be_locked.id - 1);

                        if (debug)
                            Console.WriteLine($"Vertex {to_be_locked.id} scored {active_score - to_be_locked.GetGain()} and is now locked");
                        active_score -= to_be_locked.GetGain();

                        if (active_score <= BestScore && working.IsValid())
                        {
                            BestScore = active_score;
                        }
                        // Changes.Push(new Pair(to_be_locked.id, active_score, working.IsValid()));
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
                }
                if (debug)
                    Console.WriteLine($"Parent: {parent.ToString()} -- Score:{parent.Score()}");
                int counter = Changes.Count;
                Solution parent_clone = parent.Clone();
                for (int i = 0; i < counter; i++)
                {
                    Pair active_change = Changes.Dequeue();

                    parent_clone.SwitchPartitioning(active_change.vertex - 1);
                    if (debug)
                        Console.WriteLine($"Parent: {parent_clone.ToString()} -- Offset: {active_change.value} -- Score:{parent_clone.Score() - active_change.value}  -- Valid: {parent_clone.IsValid()}");
                    parent_clone.SetScore(parent_clone.Score() - active_change.value);

                    if (parent_clone.IsValid() && best_solution.Score() > parent_clone.Score())
                    {
                        best_solution = parent_clone.Clone();
                    }
                }

                foreach (Vertex v in g.GetVertices())
                {
                    v.free = true;
                    v.SetGain(0);
                    v.next = null;
                    v.previous = null;
                }
                if (best_solution.IsValid(array_size))
                    throw new ValidationException("The solution is not valid");


            } while (parent.Score() < best_solution.Score());

            FM_PASS_COUNTER++;
            best_solution.SetFMPasses(FM_PASS_COUNTER);

            return best_solution;
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