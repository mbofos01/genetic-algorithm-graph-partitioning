namespace genetic_algorithm_graph_partitioning;

using System;

/// <summary>
/// Represents a solution for the graph partitioning problem.
/// </summary>
public class Solution
{
    /// <summary>
    /// Partitioning of the graph.
    /// </summary>
    private int[]? partitioning;
    /// <summary>
    /// The number of partitions.
    /// </summary>
    private int partion_count;
    /// <summary>
    /// The score of the partitioning.
    /// </summary> 
    private int score;
    /// <summary>
    /// The number of passes in the Fiduccia-Mattheyses algorithm.
    /// </summary>
    private int fm_passes;
    /// <summary>
    /// Random number generator.
    /// </summary> 
    private static Random rnd = new Random();

    /// <summary>
    /// Initializes a new instance of the <see cref="Solution"/> class with a random partitioning.
    /// </summary>
    /// <param name="size">The size of the partitioning.</param>
    /// <param name="partions">The number of partitions (default is 2).</param>
    public Solution(int size, int partions = 2)
    {
        partion_count = partions;
        fm_passes = 0;
        partitioning = new int[size];
        score = Int32.MaxValue;
        int count_ones;
        int count_zeros;
        do
        {
            count_ones = 0;
            count_zeros = 0;
            for (int i = 0; i < partitioning.Count(); i++)
            {
                partitioning[i] = rnd.Next(partion_count);
                if (partitioning[i] == 1)
                    count_ones++;
                else if (partitioning[i] == 0)
                    count_zeros++;
                else
                    throw new Exception("Invalid Random Outcome");
            }
        } while (count_ones != partitioning.Count() / 2 || count_zeros != partitioning.Count() / 2);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Solution"/> class with a given partitioning.
    /// </summary>
    /// <param name="partitioning">The initial partitioning.</param>
    /// <param name="partions">The number of partitions (default is 2).</param>
    public Solution(int[] partitioning, int partions = 2)
    {
        this.partion_count = partions;
        fm_passes = 0;
        score = Int32.MaxValue;
        this.partitioning = new int[partitioning.Count()];
        for (int i = 0; i < partitioning.Count(); i++)
        {
            this.partitioning[i] = partitioning[i];
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Solution"/> class with a given partitioning and score.
    /// </summary>
    /// 
    /// <param name="partitioning">The initial partitioning.</param>
    /// <param name="partions">The number of partitions (default is 2).</param>
    /// <param name="score">The score of the partitioning.</param>
    public Solution(int[] partitioning, int partions, int score)
    {
        this.partion_count = partions;
        this.score = score;
        fm_passes = 0;
        this.partitioning = new int[partitioning.Count()];
        for (int i = 0; i < partitioning.Count(); i++)
        {
            this.partitioning[i] = partitioning[i];
        }
    }

    /// <summary>
    /// Gets the current partitioning.
    /// </summary>
    /// <returns>An array representing the current partitioning.</returns>
    public int[] GetPartitioning()
    {
        if (partitioning == null)
            return new int[0];

        return partitioning;
    }

    /// <summary>
    /// Sets the partitioning to a new value.
    /// </summary>
    /// <param name="new_partioning">The new partitioning.</param>
    public void SetPartitioning(int[] new_partioning)
    {
        this.partitioning = new_partioning;
    }

    /// <summary>
    /// Switches the value of a specific index in the partitioning.
    /// </summary>
    /// <param name="index">The index to switch.</param>
    /// <param name="value">The new value.</param>
    public void SwitchPartitioning(int index, int value)
    {
        if (partitioning != null)
            this.partitioning[index] = value;
    }

    /// <summary>
    /// Switches the value of a specific index in the partitioning.
    /// </summary>
    /// 
    /// <param name="index">The index to switch.</param>
    public void SwitchPartitioning(int index)
    {
        if (partitioning != null)
            this.partitioning[index] = (this.partitioning[index] + 1) % partion_count;
    }

    /// <summary>
    /// Overrides the ToString method.
    /// </summary>
    /// 
    /// <returns>A string representation of the partitioning.</returns>
    public override string ToString()
    {
        return string.Join(" ", partitioning ?? new int[0]);
    }

    /// <summary>
    /// Overrides the Clone method.
    /// </summary>
    /// 
    /// <returns>A clone of a solution object</returns>
    public Solution Clone()
    {
        if (partitioning == null)
            return new Solution(0);

        return new Solution(partitioning, partion_count, score);
    }

    /// <summary>
    /// Checks the validity of a solution.
    /// </summary>
    /// 
    /// <param name="groups">The number of groups to check for validity (default is 2).</param>
    /// <returns>True if the solution is valid, false otherwise.</returns>
    public bool IsValid(int groups = 2)
    {
        if (partitioning == null)
            return false;

        int[] vals = new int[groups];

        for (int i = 0; i < partitioning.Count(); i++)
        {
            vals[partitioning[i]]++;
        }
        int test = vals[0];
        for (int i = 0; i < vals.Count(); i++)
        {
            if (test != vals[i])
                return false;
        }

        return true;
    }

    /// <summary>
    /// Returns the score of the solution.
    /// </summary>
    /// 
    /// <returns>The score of the solution.</returns>
    public int Score()
    {
        return score;
    }

    /// <summary>
    /// Sets the score of the solution.
    /// </summary>
    ///  
    /// <param name="score">Score to set.</param>
    public void SetScore(int score)
    {
        this.score = score;
    }

    /// <summary>
    /// Sets the Fiduccia-Mattheyses passes.
    /// </summary>
    /// 
    /// <param name="passes">The number of passes.</param>
    public void SetFMPasses(int passes)
    {
        this.fm_passes = passes;
    }

    /// <summary>
    /// Gets the Fiduccia-Mattheyses passes.
    /// </summary>
    /// 
    /// <returns>The number of passes.</returns>
    public int GetFMPasses()
    {
        return fm_passes;
    }
}