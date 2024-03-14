namespace genetic_algorithm_graph_partitioning;
using System;
using System.Reflection.Metadata;
using System.Diagnostics;

public class Program()
{
    public static Solution Mutate(Solution one, bool debug = false)
    {
        return one;
    }
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

    public static Solution Recombination(Solution one, Solution two)
    {
        Solution clone_one = one.Clone();
        Solution clone_two = two.Clone();
        Random r = new Random();
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
        Console.WriteLine("Recombination: " + clone_one.ToString() + " + " + clone_two.ToString() + " child:" + child.ToString());
        return new Solution(child_p);
    }

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
            }

        }
        if (debug)
            Console.WriteLine("Best: " + best.ToString() + " Score: " + best.Score());

        return best;
    }

    // TODO: Implement the Iterated Local Search algorithm
    // we are missing an exiting condition and an explanation
    // on how the solution is mutated
    public static Solution IteratedLocalSearch(Graph graph, bool debug = false, bool deep_debug = false)
    {
        Solution local = FiduacciaMattheysesHeuristic.FiduacciaMattheyses(graph, debug && deep_debug);

        do
        {
            Solution mutated = Mutate(local, debug && deep_debug);

            mutated = FiduacciaMattheysesHeuristic.FiduacciaMattheyses(mutated, graph, debug && deep_debug);

            if (mutated.Score() < local.Score())
            {
                local = mutated.Clone();
            }
        } while (true);

        return local;
    }

    public static void Main(string[] args)
    {
        string filePath = "../../../Graph500.txt"; // Update the file path accordingly
        List<Vertex> vertices = FileReader.ReadGraphFromFile(filePath);

        if (vertices.Count == 0)
        {
            Console.WriteLine("No vertices found in the file");
            return;
        }

        Graph graph = new Graph(vertices);
        Solution solution = MultistartLocalSearch(graph, LIMIT: 10000, debug: true, deep_debug: false);

    }
}