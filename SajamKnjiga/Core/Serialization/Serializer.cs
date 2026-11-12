namespace SajamKnjiga.Core.Serialization;

public class Serializer<T> where T : ISerializable, new()
{
    public const char Delimiter = '|';

    public void ToCSV(string filePath, List<T> objects)
    {
        using StreamWriter writer = new StreamWriter(filePath, false, new System.Text.UTF8Encoding(false));
        foreach (T obj in objects)
        {
            string line = string.Join(Delimiter.ToString(), obj.ToCSV());
            writer.WriteLine(line);
        }
    }

    public List<T> FromCSV(string filePath)
    {
        List<T> objects = new List<T>();
        if (!File.Exists(filePath))
        {
            return objects;
        }

        foreach (string line in File.ReadLines(filePath, System.Text.Encoding.UTF8))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] values = line.Split(Delimiter);
            T obj = new T();
            obj.FromCSV(values);
            objects.Add(obj);
        }

        return objects;
    }
}
