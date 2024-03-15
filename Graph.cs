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
    /// <summary>
    /// The maximum degree of the graph.
    /// </summary> 
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

    /// <summary>
    /// Gets the list of vertices in the graph.
    /// </summary>
    /// 
    /// <returns>The list of vertices in the graph.</returns>
    public List<Vertex> GetVertices()
    {
        return vertices;
    }

    /// <summary>
    /// Returns a string representation of the graph.
    /// </summary>
    /// 
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

    /// <summary>
    /// Scores the bi-partition of the graph.
    /// </summary>
    /// 
    /// <param name="partition">The partition of the graph.</param>
    /// <returns>The score in bi-partitioning</returns>//  
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

    /// <summary>
    /// Gets the maximum degree of the graph.
    /// </summary>
    /// 
    /// <returns>Max degree of the graph</returns>
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

    /// <summary>
    /// Scores the bi-partition of the graph.
    /// </summary>
    /// 
    /// <param name="sol">The solution to score.</param>
    /// <returns>The score in bi-partitioning</returns>
    public int ScoreBiPartition(Solution sol)
    {
        return ScoreBiPartition(sol.GetPartitioning());
    }
}
