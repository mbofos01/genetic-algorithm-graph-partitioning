namespace genetic_algorithm_graph_partitioning;

public class FileReader
{
    public static List<Vertex> ReadGraphFromFile(string filePath)
    {
        List<Vertex> vertices = new List<Vertex>();

        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] parts = line.Split(' ');

                    int id = int.Parse(parts[0]);
                    string[] coordinates = parts[1].Trim('(', ')').Split(',');
                    double x = double.Parse(coordinates[0]);
                    double y = double.Parse(coordinates[1]);
                    int connectedVerticesCount = int.Parse(parts[2]);

                    List<int> connectedVertices = new List<int>();
                    for (int i = 3; i < parts.Length; i++)
                    {
                        connectedVertices.Add(int.Parse(parts[i]));
                    }

                    Vertex vertex = new Vertex(id, x, y, connectedVertices.ToArray());
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