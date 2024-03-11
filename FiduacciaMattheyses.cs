namespace genetic_algorithm_graph_partitioning
{

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
            bool KeepGoing = true;
            int BestScore = Int32.MaxValue;
            int array_size = 2 * g.GetMaxDegree() + 1;
            Solution BestSolution = parent.Clone();
            List<Vertex> vertices = g.GetVertices();
            do
            {
                if (debug)
                    Console.WriteLine("Parent: " + parent.ToString());

                do
                {
                    int score = g.ScoreBiPartition(parent);

                    int[,] A;
                    A = new int[vertices.Count, array_size];
                    int[] A_Counter = new int[array_size];
                    int A_Pointer = 0;
                    int A_Length = 0;

                    int[,] B;
                    B = new int[vertices.Count, array_size];
                    int[] B_Counter = new int[array_size];
                    int B_Pointer = 0;
                    int B_Length = 0;
                    int non_improvement = 0;

                    foreach (Vertex v in vertices)
                    {
                        // only use free vertices
                        if (v.GetFree() == false)
                        {
                            continue;
                        }

                        // copy solution in order to calculate hypothetical score
                        Solution child = parent.Clone();
                        // make an hypothetical move and calculate the score
                        child.SwitchPartitioning(v.id - 1);
                        int child_score = g.ScoreBiPartition(child);
                        child.SetScore(child_score);

                        if (child_score <= BestScore)
                            non_improvement++;

                        // calculate the index of the array -> score + max_degree
                        int index = CalculateIndex(score - child_score, array_size);



                        // Keep in mind that we use the inverse of the current team
                        // 0 is team A and 1 is team B

                        if (child.GetPartitioning()[v.id - 1] == 1)
                        {
                            A[A_Counter[index], index] = v.id;
                            A_Counter[index]++;
                            A_Pointer = Math.Max(A_Pointer, index);
                            A_Length++;
                        }
                        else
                        {
                            B[B_Counter[index], index] = v.id;
                            B_Counter[index]++;
                            B_Pointer = Math.Max(B_Pointer, index);
                            B_Length++;
                        }
                        // arrays are filled with the vertices that can be moved
                    }


                    if (non_improvement == 4)
                        KeepGoing = false;

                    if (A_Length == 0 && B_Length == 0)
                    {
                        break;
                    }

                    if (debug)
                        Console.WriteLine("==================================");
                    if (A_Length >= B_Length)
                    {
                        parent.SwitchPartitioning(A[A_Counter[A_Pointer] - 1, A_Pointer] - 1);
                        vertices[A[A_Counter[A_Pointer] - 1, A_Pointer] - 1].free = false;
                        A_Counter[A_Pointer] = Math.Max(1, A_Counter[A_Pointer] - 1);
                        if (debug)
                        {
                            Console.WriteLine("Score: " + g.ScoreBiPartition(parent) + " Solution: " + parent.ToString());
                            Console.WriteLine("==================================");
                        }
                    }
                    else
                    {
                        parent.SwitchPartitioning(B[B_Counter[B_Pointer] - 1, B_Pointer] - 1);
                        vertices[B[B_Counter[B_Pointer] - 1, B_Pointer] - 1].free = false;
                        B_Counter[B_Pointer] = Math.Max(1, B_Counter[B_Pointer] - 1);
                        if (debug)
                        {
                            Console.WriteLine("Score: " + g.ScoreBiPartition(parent) + " Solution: " + parent.ToString());
                            Console.WriteLine("==================================");
                        }
                    }

                    if (g.ScoreBiPartition(parent) <= BestScore && parent.IsValid())
                    {
                        BestScore = g.ScoreBiPartition(parent);
                        BestSolution = parent.Clone();
                    }

                } while (true);


                // free all vertices and update working solution
                foreach (Vertex v in vertices)
                {
                    v.SetFree(true);
                    parent = BestSolution.Clone();

                }
            } while (KeepGoing);

            BestSolution.SetScore(BestScore);

            if (debug)
                Console.WriteLine("Best Solution Found: " + BestSolution.ToString() + " with Score: " + BestSolution.Score() + " and Valid: " + BestSolution.IsValid());

            return BestSolution;
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