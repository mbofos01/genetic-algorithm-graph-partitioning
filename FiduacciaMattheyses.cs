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

        public static void UpdateBuckets(Vertex to_be_locked, Solution parent, Graph g, Bucket ActiveBucket, int array_size, Solution starting_parent, List<Vertex> vertices)
        {
            for (int i = 0; i < to_be_locked.connections.Length; i++)
            {
                int connected_index = to_be_locked.connections[i];
                // Console.WriteLine($"Vertex {to_be_locked.id} has connection {connected_index}");
                if (vertices[connected_index - 1].GetFree() && parent.GetPartitioning()[to_be_locked.id - 1] == parent.GetPartitioning()[connected_index - 1])
                {
                    // Console.WriteLine($"\tInvestigating vertex {connected_index}");
                    // Console.WriteLine($"\t\tPrevious solution: {BestSolution.ToString()} with score {BestSolution.Score()}");

                    // copy solution in order to calculate hypothetical score
                    Solution child = parent.Clone();
                    // make an hypothetical move and calculate the score
                    child.SwitchPartitioning(connected_index - 1);
                    // Console.WriteLine($"\t\t---------------------------------------------");
                    // Console.WriteLine($"\t\tSwitching partitioning of {connected_index} to {child.ToString()}");
                    int child_score = g.ScoreBiPartition(child);
                    // Console.WriteLine($"\t\tAnd the score is {child_score}");
                    int new_index = CalculateIndex(parent.Score() - child_score, array_size);
                    // Console.WriteLine($"\t\tAnd the index is {new_index}");
                    // Console.WriteLine($"\t\t---------------------------------------------");
                    child.SetScore(child_score);


                    int past_score = starting_parent.Score();
                    starting_parent.SwitchPartitioning(connected_index - 1);
                    int previous_score = g.ScoreBiPartition(starting_parent);
                    // Console.WriteLine($"\t\tPrevious State: {BestSolution.ToString()}");
                    // Console.WriteLine($"\t\tAnd the score is {previous_score}");
                    int removable_index = CalculateIndex(past_score - previous_score, array_size);
                    // Console.WriteLine($"\t\tAnd the index is {removable_index}");
                    // Console.WriteLine($"\t\t---------------------------------------------");
                    starting_parent.SwitchPartitioning(connected_index - 1);

                    ActiveBucket.RemoveValue(removable_index, connected_index);
                    ActiveBucket.InsertValue(new_index, connected_index);

                }
            }
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
            int array_size = 2 * g.GetMaxDegree() + 1;
            int BestScore = int.MaxValue;
            Stack<Pair> Changes = new Stack<Pair>();

            parent.SetScore(g.ScoreBiPartition(parent));
            Solution starting_parent = parent.Clone();

            List<Vertex> vertices = g.GetVertices();

            if (debug)
                Console.WriteLine("Parent: " + parent.ToString() + " with Score: " + g.ScoreBiPartition(parent) + " and Valid: " + parent.IsValid());

            Bucket A = new Bucket(array_size);
            Bucket B = new Bucket(array_size);

            foreach (Vertex v in vertices)
            {
                // copy solution in order to calculate hypothetical score
                Solution child = parent.Clone();
                // make an hypothetical move and calculate the score
                child.SwitchPartitioning(v.id - 1);
                int child_score = g.ScoreBiPartition(child);
                child.SetScore(child_score);
                // Console.WriteLine($"Vertex {v.id} has score {child_score}");

                // calculate the index of the array -> score + max_degree
                int index = CalculateIndex(parent.Score() - child_score, array_size);

                // Keep in mind that we use the inverse of the current team
                // 0 is team A and 1 is team B

                if (child.GetPartitioning()[v.id - 1] == 1)
                {
                    A.InsertValue(index, v.id);
                }
                else
                {
                    B.InsertValue(index, v.id);
                }
                // arrays are filled with the vertices that can be moved
            }

            // ViewBuckets(A, B);

            while (vertices.Any(v => v.GetFree()))
            {
                // string a = Console.ReadLine();
                if (A.GetLength() == 0 && B.GetLength() == 0)
                {
                    break;
                }
                else
                {
                    Vertex? to_be_locked = null;
                    int to_be_scored = int.MaxValue;
                    bool to_be_validated = false;

                    if (A.GetLength() >= B.GetLength())
                    {
                        int max_pointer = A.GetMaxActiveDegree();
                        int vertex_id_to_move = A.GetValue(max_pointer);

                        parent.SwitchPartitioning(vertex_id_to_move - 1);
                        to_be_scored = g.ScoreBiPartition(parent);
                        to_be_validated = parent.IsValid();
                        parent.SetScore(to_be_scored);

                        to_be_locked = vertices[vertex_id_to_move - 1];
                        A.RemoveValue(max_pointer, vertex_id_to_move);
                        UpdateBuckets(to_be_locked, parent, g, B, array_size, starting_parent, vertices);


                    }
                    else
                    {
                        int max_pointer = B.GetMaxActiveDegree();
                        int vertex_id_to_move = B.GetValue(max_pointer);

                        parent.SwitchPartitioning(vertex_id_to_move - 1);
                        to_be_scored = g.ScoreBiPartition(parent);
                        to_be_validated = parent.IsValid();
                        parent.SetScore(to_be_scored);

                        to_be_locked = vertices[vertex_id_to_move - 1];
                        B.RemoveValue(max_pointer, vertex_id_to_move);
                        UpdateBuckets(to_be_locked, parent, g, A, array_size, starting_parent, vertices);

                    }

                    // ViewBuckets(A, B);

                    to_be_locked.free = false;
                    // Console.WriteLine($"Vertex {to_be_locked.id} scored {to_be_scored}");

                    if (to_be_scored <= BestScore && to_be_validated)
                    {
                        BestScore = to_be_scored;
                    }
                    Changes.Push(new Pair(to_be_locked.id, to_be_scored, to_be_validated));

                }

            }

            if (debug)
            {
                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine($"Minimum Value: {BestScore}");
                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine("Changes: ");
                foreach (Pair p in Changes)
                {
                    Console.WriteLine($"Vertex: {p.vertex} Score: {p.value} Valid: {p.valid}");
                }
                Console.WriteLine("------------------------------------------------------");
            }
            Pair? active = Changes.Pop();
            // Console.WriteLine($"From: {starting_parent.ToString()}");
            // Console.WriteLine($"Starting: {parent.ToString()}");
            while (active.value != BestScore || (active.value == BestScore && active.valid == false))
            {

                // Console.WriteLine($"Pair: {active.vertex} with value {active.value} Valid: {active.valid}");
                parent.SwitchPartitioning(active.vertex - 1);
                parent.SetScore(active.value);
                if (debug)
                    Console.WriteLine($"Active: {parent.ToString()} Change: {active.vertex} Value: {active.value} Valid: {parent.IsValid()}");
                try
                {
                    active = Changes.Pop();
                }
                catch (InvalidOperationException e)
                {
                    break;
                }
            }

            foreach (Vertex v in vertices)
            {
                v.free = true;
            }
            if (parent.IsValid(array_size))
                throw new ValidationException("The solution is not valid");

            parent.SetScore(g.ScoreBiPartition(parent));
            return parent;
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