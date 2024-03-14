namespace genetic_algorithm_graph_partitioning;

public class Bucket
{
    public bool[]? bucket;
    public Vertex?[]? first_pointer;
    public Vertex?[]? last_pointer;
    public int max_degree;

    public int population;

    public Bucket(int max_d)
    {
        max_degree = max_d;
        bucket = new bool[2 * max_degree + 1];
        first_pointer = new Vertex[2 * max_degree + 1];
        last_pointer = new Vertex[2 * max_degree + 1];
        population = 0;
    }

    /// <summary>
    /// Helper function to calculate the index of the array.
    /// </summary>
    /// 
    /// <param name="gain">The gain of the current solution.</param>
    /// <returns>The index of the array.</returns> 
    public int CalculateIndex(int gain)
    {
        return gain + max_degree;
    }

    public void AddToBucket(Vertex x, int gain, Random rnd)
    {
        population++;
        int index = CalculateIndex(gain);
        int order = 0;// rnd.Next(2);

        if (bucket == null || first_pointer == null || last_pointer == null)
        {
            throw new Exception("Bucket is not initialized");
        }

        if (bucket != null && first_pointer != null && last_pointer != null && bucket[index] == false)
        {
            // bucket is empty
            first_pointer[index] = x;
            last_pointer[index] = x;
            bucket[index] = true;

        }
        else if (bucket != null && first_pointer != null && last_pointer != null && bucket[index] == true)
        {
            Vertex? first_of_gain = first_pointer[index];
            Vertex? last_of_gain = last_pointer[index];

            if (first_of_gain == null)
                throw new Exception("Pointer is null");

            if (order == 0)
            {
                // insert at the beginning
                x.SetNext(first_of_gain);
                x.SetPrevious(null);

                first_of_gain.SetPrevious(x);
                first_pointer[index] = x;

            }
            else
            {
                // insert at the end
                x.SetNext(null);
                x.SetPrevious(last_of_gain);

                if (last_of_gain != null)
                {
                    last_of_gain.SetNext(x);
                }
                last_pointer[index] = x;
            }
        }

    }
    public void RemoveFromBucket(Vertex x)
    {
        Vertex? previous = x.GetPrevious();
        Vertex? next = x.GetNext();
        population--;

        if (bucket == null || first_pointer == null || last_pointer == null)
        {
            throw new Exception("Bucket is not initialized");
        }

        int index = CalculateIndex(x.GetGain());

        Vertex? first_of_gain = first_pointer[index];
        Vertex? last_of_gain = last_pointer[index];

        if (previous == null && next == null)
        {
            // you are the only one in the bucket
            // disable spot and clear your pointers
            bucket[index] = false;
            first_pointer[index] = null;
            last_pointer[index] = null;

            return;
        }
        else
        {
            if (first_of_gain != null && first_of_gain.id == x.id)
            {
                // if I am the first node
                // take my next and set it as first
                // the new first's previous is null
                first_pointer[index] = next;
                if (next != null)
                    next.previous = null;
            }

            if (last_of_gain != null && last_of_gain.id == x.id)
            {
                // if I am the last node
                // take my previous and set it as last
                // the new last's next is null
                last_pointer[index] = previous;
                if (previous != null)
                    previous.next = null;
            }
            // if you are not alone
            if (previous != null)
            {
                // if you have a previous
                // set the previous's next to your next
                previous.SetNext(x.GetNext());
            }
            if (next != null)
            {
                // if you have a next
                // set the next's previous to your previous
                next.SetPrevious(x.GetPrevious());
            }
        }

        x.SetNext(null);
        x.SetPrevious(null);

        for (int i = 0; i < 2 * max_degree + 1; i++)
        {
            if (first_pointer[i] == null && last_pointer[i] == null)
                bucket[i] = false;
        }
    }

    public void PrintBucket()
    {
        for (int i = -max_degree; i <= max_degree; i++)
        {
            int index = CalculateIndex(i);
            if (i < 0)
                Console.Write("Bucket " + i + " : ");
            else
                Console.Write("Bucket  " + i + " : ");

            if (bucket[index] == true)
            {
                Vertex? current = first_pointer[index];
                while (current != null)
                {
                    Console.Write(current.id + " ");
                    current = current.GetNext();
                }
            }
            Console.WriteLine();
        }
    }

    public int GetPopulation()
    {
        return population;
    }

    public int GetMaxActiveDegree()
    {
        int spot = -1;
        if (bucket != null)
        {
            for (int i = 0; i < bucket.Length; i++)
            {
                if (bucket[i] == true)
                    spot = i;
            }
        }
        return spot;
    }

    public Vertex GetFirstVertex(int pointer)
    {
        if (first_pointer == null || first_pointer[pointer] == null)
        {
            throw new Exception("First pointer is null");
        }

        return first_pointer[pointer]!;
    }

    // public static void Main(string[] args)
    // {
    //     int max_degree = 3;
    //     Bucket A = new Bucket(max_degree);
    //     Bucket B = new Bucket(max_degree);

    //     Vertex v1 = new Vertex(1, new int[] { 2, 3, 4 });
    //     Vertex v2 = new Vertex(2, new int[] { 1, 3, 4 });
    //     Vertex v3 = new Vertex(3, new int[] { 1, 2, 4 });
    //     Vertex v4 = new Vertex(4, new int[] { 1, 2, 4 });
    //     v1.SetGain(1);
    //     v2.SetGain(1);
    //     v3.SetGain(3);
    //     v4.SetGain(3);

    //     Random rnd = new Random();

    //     A.AddToBucket(v1, 1, rnd);
    //     A.AddToBucket(v2, 1, rnd);
    //     A.RemoveFromBucket(v2);
    //     A.RemoveFromBucket(v1);
    //     B.AddToBucket(v3, 3, rnd);
    //     B.AddToBucket(v4, 3, rnd);

    //     Console.WriteLine(A.GetFirstVertex(A.GetMaxActiveDegree()));




    // }


}