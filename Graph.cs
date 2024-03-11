namespace genetic_algorithm_graph_partitioning;

using System;
using System.Collections;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

/// <summary>
/// Represents a graph data structure.
/// </summary>
public class Graph
{
    /// <summary>
    /// Gets or sets the list of vertices in the graph.
    /// </summary>
    public List<Vertex> vertices;

    public int max_degree;

    /// <summary>
    /// Initializes a new instance of the <see cref="Graph"/> class with the specified vertices.
    /// </summary>
    /// <param name="vertices">The list of vertices.</param>
    public Graph(List<Vertex> vertices)
    {
        this.vertices = vertices;
        max_degree = 0;
    }

    public List<Vertex> GetVertices()
    {
        return vertices;
    }

    /// <summary>
    /// Returns a string representation of the graph.
    /// </summary>
    /// <returns>A string representation of the graph.</returns>
    public override string ToString()
    {
        string result = "";
        foreach (Vertex v in vertices)
        {
            result += v.ToString() + "\n";
        }
        return result;
    }

    public int ScoreBiPartition(int[] partition)
    {
        int score = 0;
        for (int i = 0; i < vertices.Count; i++)
        {
            Vertex v = vertices[i];
            int part = partition[i];
            foreach (int c in v.connections)
            {
                if (partition[c - 1] != part)
                {
                    score++;
                }
            }
        }
        return score / 2;
    }

    public int GetMaxDegree()
    {
        if (max_degree != 0)
        {
            return max_degree;
        }
        int max = 0;
        foreach (Vertex v in vertices)
        {
            if (v.connections.Count() > max)
            {
                max = v.connections.Count();
            }
        }
        max_degree = max;
        return max;
    }

    public int ScoreBiPartition(Solution sol)
    {
        return ScoreBiPartition(sol.GetPartitioning());
    }



    /// <summary>
    /// The entry point of the program.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    static void Main(string[] args)
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
        FiduacciaMattheysesHeuristic.FiduacciaMattheyses(graph, true);

    }
}

// 1 (0.502987,0.528829)    8    28 102 162 233 360 393 460 500