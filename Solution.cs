namespace genetic_algorithm_graph_partitioning;

using System;

/// <summary>
/// Represents a solution for the graph partitioning problem.
/// </summary>
public class Solution
{
    private int[]? partitioning;
    private int partion_count;
    private int score;
    private int fm_passes;
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

    public void SwitchPartitioning(int index)
    {
        if (partitioning != null)
            this.partitioning[index] = (this.partitioning[index] + 1) % partion_count;
    }

    public override string ToString()
    {
        return string.Join(" ", partitioning ?? new int[0]);
    }

    public Solution Clone()
    {
        if (partitioning == null)
            return new Solution(0);

        return new Solution(partitioning, partion_count, score);
    }


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

    public int Score()
    {
        return score;
    }

    public void SetScore(int score)
    {
        this.score = score;
    }

    public void SetFMPasses(int passes)
    {
        this.fm_passes = passes;
    }

    public int GetFMPasses()
    {
        return fm_passes;
    }
}