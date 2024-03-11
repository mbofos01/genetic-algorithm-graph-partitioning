namespace genetic_algorithm_graph_partitioning;
using System;
using System.Reflection.Metadata;

public class Program()
{

    public static Solution MultistartLocalSearch(Graph graph, int LIMIT = 1000, bool debug = false)
    {
        Solution one;
        Solution best = new Solution(graph.GetVertices().Count);

        for (int i = 0; i < LIMIT; i++)
        {
            one = FiduacciaMattheysesHeuristic.FiduacciaMattheyses(graph, false);
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
        // Vertex v1 = new Vertex(1, [2, 5, 6]);
        // Vertex v2 = new Vertex(2, [1, 3, 5]);
        // Vertex v3 = new Vertex(3, [2, 4, 5]);
        // Vertex v4 = new Vertex(4, [3, 5]);
        // Vertex v5 = new Vertex(5, [1, 2, 3, 4]);
        // Vertex v6 = new Vertex(6, [1]);

        // List<Vertex> vertices = [v1, v2, v3, v4, v5, v6];
        Vertex v1 = new Vertex(1, [2,]);
        Vertex v2 = new Vertex(2, [1, 3]);
        Vertex v3 = new Vertex(3, [2, 4]);
        Vertex v4 = new Vertex(4, [3,]);

        List<Vertex> vertices = [v1, v2, v3, v4];

        Graph graph = new Graph(vertices);

        Solution solution = MultistartLocalSearch(graph, 1000, true);

    }
}