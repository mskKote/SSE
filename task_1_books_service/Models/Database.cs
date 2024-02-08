using System.Xml.Serialization;

namespace task_1_books_service.Models;

public sealed class Database
{
    public const string XmlFile = "/data.xml";

    public Book[] Books { get; set; }

    public void Serialize(string xmlFile)
    {
        using var fs = new FileStream(xmlFile, FileMode.Create);
        var serializer = new XmlSerializer(typeof(Database));
        serializer.Serialize(fs, this);
    }

    public static Database Deserialize(string xmlFile)
    {
        var fs = new FileStream(xmlFile, FileMode.Open);
        var serializer = new XmlSerializer(typeof(Database));
        var database = (Database?)serializer.Deserialize(fs);
        fs.Close();
        return database ?? new Database();
    }
}