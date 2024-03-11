namespace genetic_algorithm_graph_partitioning;
using System;
using System.Reflection.Metadata;

public class Program()
{

    public static Solution MultistartLocalSearch(Graph graph, int LIMIT = 1000, bool debug = false, bool deep_debug = false)
    {
        Solution one;
        Solution best = new Solution(graph.GetVertices().Count);

        for (int i = 0; i < LIMIT; i++)
        {
            one = FiduacciaMattheysesHeuristic.FiduacciaMattheyses(graph, debug && deep_debug);

            if (one.Score() < best.Score())
            {
                best = one.Clone();
            }

        }
        if (debug)
            Console.WriteLine("Best: " + best.ToString() + " Score: " + best.Score());

        return best;
    }
    public static void Main(string[] args)
    {
        string filePath = "../../../Graph5.txt"; // Update the file path accordingly
        List<Vertex> vertices = FileReader.ReadGraphFromFile(filePath);

        if (vertices.Count == 0)
        {
            Console.WriteLine("No vertices found in the file");
            return;
        }

        Graph graph = new Graph(vertices);
        Console.WriteLine(graph.ToString());
        Solution solution = MultistartLocalSearch(graph: graph, LIMIT: 1000, debug: true, deep_debug: false);

    }
}