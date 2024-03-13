// namespace genetic_algorithm_graph_partitioning;


using System.ComponentModel.Design;

public class Bucket
{
    LinkedList<int>[] bucket;
    int actives = 0;

    public Bucket(int size)
    {
        bucket = new LinkedList<int>[size];
        for (int i = 0; i < size; i++)
        {
            bucket[i] = new LinkedList<int>();
        }
    }

    public int GetValue(int index)
    {
        if (bucket[index].Count == 0)
        {
            return -1;
        }

        return bucket[index].First.Value;
    }

    public void InsertValue(int index, int value)
    {
        bucket[index].AddLast(value);
        actives++;
    }

    public void RemoveValue(int index, int value)
    {
        bool success = bucket[index].Remove(value);
        if (success)
        {
            actives--;
        }
    }

    public LinkedList<int> GetBucket(int index)
    {
        return bucket[index];
    }

    public int GetLength()
    {
        return actives;
    }

    public int GetLength(int index)
    {
        return bucket[index].Count;
    }

    public void PrintBucket()
    {
        int count = 0;
        foreach (LinkedList<int> column in bucket)
        {
            Console.Write($"Bucket {count++}:  ");
            foreach (var item in column)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }
    }

    public int GetMaxActiveDegree()
    {
        int max = 0;
        int count = 0;
        foreach (var item in bucket)
        {
            if (item.Count > 0)
            {
                max = count;
            }
            count++;
        }
        return max;
    }

}