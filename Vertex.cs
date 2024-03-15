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
    /// The previous vertex.
    /// </summary>
    public Vertex? previous;
    /// <summary>
    /// The next vertex.
    /// </summary> 
    public Vertex? next;
    /// <summary>
    /// The gain of the vertex.
    /// </summary>
    public int gain;
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
    /// <summary>
    /// The free status of the vertex.
    /// </summary> 
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Vertex"/> class.
    /// </summary>
    /// 
    /// <param name="id">The ID of the vertex.</param>
    /// <param name="connections">The connections of the vertex.</param>
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

    /// <summary>
    /// Gets the free status of the vertex.
    /// </summary>
    /// <returns>The free status of the vertex.</returns>
    public bool GetFree()
    {
        return free;
    }

    /// <summary>
    /// Sets the free status of the vertex.
    /// </summary>
    /// 
    /// <param name="free">Free status</param>
    public void SetFree(bool free)
    {
        this.free = free;
    }

    /// <summary>
    /// Gets the gain of the vertex.
    /// </summary>
    /// 
    /// <param name="gain">The gain of the vertex.</param>
    /// <returns>The gain of the vertex.</returns>
    public void SetGain(int gain)
    {
        this.gain = gain;
    }

    /// <summary>
    /// Sets the gain of the vertex.
    /// </summary>
    /// 
    /// <returns>The gain of the vertex.</returns>
    public int GetGain()
    {
        return gain;
    }

    /// <summary>
    /// Sets the previous vertex.
    /// </summary>
    /// 
    /// <param name="previous">The previous vertex.</param>
    public void SetPrevious(Vertex? previous)
    {
        this.previous = previous;
    }

    /// <summary>
    /// Gets the previous vertex.
    /// </summary>
    /// 
    /// <returns>The previous vertex.</returns>
    public Vertex? GetPrevious()
    {
        return previous;
    }

    /// <summary>
    /// Sets the next vertex.
    /// </summary>
    /// 
    /// <param name="next">The next vertex.</param>
    public void SetNext(Vertex? next)
    {
        this.next = next;
    }

    /// <summary>
    /// Gets the next vertex.
    /// </summary>
    /// 
    /// <returns>The next vertex.</returns>
    public Vertex? GetNext()
    {
        return next;
    }

}

