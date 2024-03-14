namespace genetic_algorithm_graph_partitioning;

public class FileReader
{
    public static List<Vertex> ReadGraphFromFile(string filePath)
    {
        List<Vertex> vertices = new List<Vertex>();

        try
        {
            using (StreamReader? sr = new StreamReader(filePath))
            {

                while (!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    string[] parts = line.Split(' ');

                    int index = 0;
                    while (parts?[index] == "")
                    {
                        index++;
                    }

                    int id = int.Parse(parts[index]);

                    // Extracting coordinates
                    string[] coordinates = parts[index + 1].Trim('(', ')').Split(',');
                    double x = double.Parse(coordinates[0]);
                    double y = double.Parse(coordinates[1]);

                    index += 2;

                    while (parts[index] == "")
                    {
                        index++;
                    }

                    int connections = int.Parse(parts[index]);
                    index += 2;

                    // Extracting connected vertices
                    int[] conns = new int[connections];
                    for (int i = 0; i < connections; i++)
                    {
                        // connectedVertices.Add(int.Parse(parts[index + i]));
                        conns[i] = int.Parse(parts[index + i]);
                    }

                    Vertex vertex = new Vertex(id, x, y, conns);
                    vertices.Add(vertex);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
        }

        return vertices;
    }
}