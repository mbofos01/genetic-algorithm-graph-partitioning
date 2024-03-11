namespace genetic_algorithm_graph_partitioning;

using System;

/// <summary>
/// Represents a vertex in a graph.
/// </summary>
public class Vertex
{
    /// <summary>
    /// The ID of the vertex.
    /// </summary>
    public int id;

    /// <summary>
    /// The x-coordinate of the vertex.
    /// </summary>
    public double x;

    /// <summary>
    /// The y-coordinate of the vertex.
    /// </summary>
    public double y;

    /// <summary>
    /// The connections of the vertex.
    /// </summary>
    public int[] connections;

    public bool free = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Vertex"/> class.
    /// </summary>
    /// <param name="id">The ID of the vertex.</param>
    /// <param name="x">The x-coordinate of the vertex.</param>
    /// <param name="y">The y-coordinate of the vertex.</param>
    /// <param name="connections">The connections of the vertex.</param>
    public Vertex(int id, double x, double y, int[] connections)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.connections = connections;
    }

    public Vertex(int id, int[] connections)
    {
        this.id = id;
        this.connections = connections;
    }

    /// <summary>
    /// Returns a string representation of the vertex.
    /// </summary>
    /// <returns>A string representation of the vertex.</returns>
    public override string ToString()
    {
        return "Vertex: " + id + " [" + string.Join(" ", connections) + "]";
    }

    public bool GetFree()
    {
        return free;
    }

    public void SetFree(bool free)
    {
        this.free = free;
    }

}

