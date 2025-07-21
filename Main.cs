namespace genetic_algorithm_graph_partitioning;

using System;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.Data;

public class Program()
{
    /// <summary>
    /// Mutates a solution.
    /// </summary>
    /// 
    /// <param name="one">The solution to mutate.</param>
    /// <param name="how_many">The number of mutations.</param>
    /// <param name="debug">Debug flag.</param>
    /// <returns> The mutated solution</returns>
    public static Solution Mutate(Solution one, int how_many, bool debug = false)
    {
        Random random = new Random();
        if (how_many % 2 != 0)
        {
            throw new Exception("how_many must be even");
        }
        bool loop_through = false;
        do
        {
            loop_through = false;
            List<int> indexes = new List<int>();
            for (int i = 0; i < how_many; i++)
            {
                int index = -1;
                do
                {
                    index = random.Next(0, one.GetPartitioning().Length);
                } while (indexes.Contains(index));
                one.SwitchPartitioning(index);
                indexes.Add(index);
            }
            if (one.IsValid() == false)
            {
                loop_through = true;
                for (int i = 0; i < indexes.Count; i++)
                {
                    one.SwitchPartitioning(indexes[i]);
                }
            }
        } while (loop_through);

        return one;
    }

    /// <summary>
    /// Calculates the Hamming distance between two solutions.
    /// </summary>
    /// 
    /// <param name="one">The first solution.</param>
    /// <param name="two">The second solution.</param>
    /// <returns> The Hamming distance between the two solutions</returns>
    public static int HammingDistance(Solution one, Solution two)
    {
        int distance = 0;
        int[] one_p = one.GetPartitioning();
        int[] two_p = two.GetPartitioning();
        for (int i = 0; i < one_p.Length; i++)
        {
            if (one_p[i] != two_p[i])
            {
                distance++;
            }
        }

        return distance;
    }

    /// <summary>
    /// Recombines two solutions.
    /// </summary>
    /// 
    /// <param name="one">The first solution.</param>
    /// <param name="two">The second solution.</param>
    /// <param name="debug">Debug flag.</param>
    /// <returns> The recombined solution</returns> 
    public static Solution Recombination(Solution one, Solution two, bool debug = false)
    {
        Solution clone_one = one.Clone();
        Solution clone_two = two.Clone();
        Random r = new Random();
        if (debug)
            Console.WriteLine("Recombination: " + clone_one.ToString() + " + " + clone_two.ToString());

        if (HammingDistance(clone_one, clone_two) > clone_one.GetPartitioning().Length / 2)
            for (int i = 0; i < clone_two.GetPartitioning().Length; i++)
                clone_two.SwitchPartitioning(i);

        int[] c_o_p = clone_one.GetPartitioning();
        int[] c_t_p = clone_two.GetPartitioning();
        int[] child_p = new int[c_o_p.Length];
        do
        {
            for (int i = 0; i < c_o_p.Length; i++)
                if (c_o_p[i] == c_t_p[i])
                    child_p[i] = c_o_p[i];
                else
                    child_p[i] = r.Next(0, 2);

        } while (child_p.Count(n => n == 0) != child_p.Count(n => n == 1));

        Solution child = new Solution(child_p);

        if (debug)
            Console.WriteLine("Recombination: " + clone_one.ToString() + " + " + clone_two.ToString() + " child:" + child.ToString());

        return new Solution(child_p);
    }

    /// <summary>
    /// Multistart local search algorithm.
    /// </summary>
    ///
    /// <param name="graph">Graph to partition.</param>
    /// <param name="LIMIT">Limit of FM passes (default=10000).</param>
    /// <param name="debug">Debug flag.</param>
    /// <param name="deep_debug">Deep debug flag.</param>
    /// <returns> The best solution found by the multistart local search algorithm</returns>  
    public static Solution MultistartLocalSearch(Graph graph, int LIMIT = 10000, bool debug = false, bool deep_debug = false)
    {
        Solution one;
        Solution best = new Solution(graph.GetVertices().Count);

        for (int i = 0; i < LIMIT; i++)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            one = FiduacciaMattheysesHeuristic.FiduacciaMattheyses(graph, debug && deep_debug);
            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;
            i += one.GetFMPasses();
            if (debug)
                Console.WriteLine((i + 1) + "/" + LIMIT + " Score: " + one.Score() + " Time: " + elapsedTime);

            if (one.Score() < best.Score())
            {
                best = one.Clone();
                best.SetScore(one.Score());
            }

        }
        if (debug)
            Console.WriteLine("Best: " + best.ToString() + " Score: " + best.Score());

        return best;
    }

    /// <summary>
    /// Iterated local search algorithm.
    /// </summary>
    /// 
    /// <param name="graph">Graph to partition.</param>
    /// <param name="LIMIT">Limit of FM passes (default=10000).</param>
    /// <param name="permutaton_degree">Degree of permutation (default=2).</param>
    /// <param name="debug">Debug flag.</param>
    /// <param name="deep_debug">Deep debug flag.</param>
    /// <returns> The best solution found by the iterated local search algorithm</returns>
    public static Solution IteratedLocalSearch(Graph graph, int LIMIT = 10000, int permutaton_degree = 2, bool debug = false, bool deep_debug = false)
    {
        Solution local = FiduacciaMattheysesHeuristic.FiduacciaMattheyses(graph, debug && deep_debug);
        int fm_passes = local.GetFMPasses();
        Console.WriteLine($"Score: {local.Score()} FM Passes: {local.GetFMPasses()}/{LIMIT}");
        Solution best = local.Clone();
        int better_scores = 0;
        int runs = 0;

        do
        {
            Solution mutated = Mutate(local, permutaton_degree, debug && deep_debug);

            mutated = FiduacciaMattheysesHeuristic.FiduacciaMattheyses(mutated, graph, debug && deep_debug);
            fm_passes += mutated.GetFMPasses();

            Console.WriteLine($"Score: {mutated.Score()} FM Passes: {fm_passes}/{LIMIT}");

            if (mutated.Score() < local.Score())
            {
                local = mutated.Clone();
                better_scores++;
            }


            if (local.Score() < best.Score())
            {
                best = local.Clone();
            }
            runs++;
        } while (fm_passes < LIMIT);

        if (debug)
            Console.WriteLine("Best: " + best.ToString() + " Score: " + best.Score() + " Improvement: " + better_scores + " Runs: " + runs);

        return best;
    }

    /// <summary>
    /// Genetic local search algorithm.
    /// </summary>
    /// 
    /// <param name="graph">Graph to partition.</param>
    /// <param name="population">Population size (default=50).</param>
    /// <param name="LIMIT">Limit of FM passes (default=10000).</param>
    /// <param name="debug">Debug flag.</param>
    /// <returns> The best solution found by the genetic local search algorithm</returns>
    public static Solution GeneticLocalSearch(Graph graph, int population = 50, int LIMIT = 10000, bool debug = false, bool deep_debug = false)
    {
        List<Solution> solutions = new List<Solution>();
        Solution? last_solution;
        Random picker = new();
        int father = -1;
        int mother = -1;
        int fm_passes = 0;

        for (int i = 0; i < population; i++)
        {
            // Genearate a random local optimal solutions
            solutions.Add(FiduacciaMattheysesHeuristic.FiduacciaMattheyses(graph, debug && deep_debug));
        }
        solutions.Sort((x, y) => x.Score().CompareTo(y.Score()));
        do
        {
            do
            {
                father = picker.Next(0, population);
                mother = picker.Next(0, population);

            } while (father == mother);

            Solution child = Recombination(solutions[father], solutions[mother]);
            // optimize the child
            child = FiduacciaMattheysesHeuristic.FiduacciaMattheyses(child, graph);
            fm_passes += child.GetFMPasses();

            if (debug)
                Console.WriteLine($"Score: {child.Score()} FM Passes: {fm_passes}/{LIMIT}");


            // compare child with the worst solution in the population
            last_solution = solutions.Last();

            if (child.Score() <= last_solution.Score())
            {
                solutions.Remove(last_solution);
                solutions.Add(child);
            }
            solutions.Sort((x, y) => x.Score().CompareTo(y.Score()));

        } while (fm_passes < LIMIT);

        if (debug)
            Console.WriteLine("Best: " + solutions.First().ToString() + " Score: " + solutions.First().Score());

        return solutions.First();
    }
    public static void Main(string[] args)
    {
        string filePath = "Graph5.txt"; // Update the file path accordingly
        List<Vertex> vertices = FileReader.ReadGraphFromFile(filePath);

        if (vertices.Count == 0)
        {
            Console.WriteLine("No vertices found in the file");
            return;
        }

        Graph graph = new Graph(vertices);
        // Solution solution = MultistartLocalSearch(graph, LIMIT: 10000, debug: true, deep_debug: false);
        // Solution s = FiduacciaMattheysesHeuristic.FiduacciaMattheyses(new Solution([1, 0, 1, 0, 1, 0]), graph, debug: true);
        // Console.WriteLine($"Local Best: {s.ToString()} Score: {s.Score()}");
        // Solution solution = IteratedLocalSearch(graph, 10000, permutaton_degree: 2, debug: true, deep_debug: false);

        Solution solution = GeneticLocalSearch(graph, 10, debug: true);
        solution = IteratedLocalSearch(graph, 10, debug: true);
        solution = MultistartLocalSearch(graph, 10, debug: true);


    }
}