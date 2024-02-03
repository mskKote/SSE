using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Bookmark.Models;

public class UserXmlRepository : IUserRepository
{
    private readonly string _xmlFile;

    public UserXmlRepository(IWebHostEnvironment env)
    {
        _xmlFile = env.ContentRootPath + "/data.xml";
    }

    public static string XmlFile = "./data.xml";

    public User[] GetAll()
    {
        var serializer = new XmlSerializer(typeof(Database));
        using (var fs = new FileStream(_xmlFile, FileMode.Open))
        {
            return ((Database)serializer.Deserialize(fs)).Users;
        }
    }

    public User GetById(int id)
    {
        XDocument doc = XDocument.Load(_xmlFile);            
        XElement user = (doc.XPathSelectElement("//Users/User[Id=" + id + "]"));

        if (user == null)
        {
            return null;
        }
            
        var serializer = new XmlSerializer(typeof(User));
        return (User)serializer.Deserialize(new StringReader(user.ToString()));
    }

    public int Create(User user)
    {
        var serializer = new XmlSerializer(typeof(Database));
        var fs = new FileStream(_xmlFile, FileMode.Open);
        var database = (Database)serializer.Deserialize(fs);
        fs.Close();
        var users = database.Users;

        var nextId = users.Length > 0 ? (users.Select(b => b.Id).Max() + 1) : 1;
        user.Id = nextId;

        database.Users = users.Append(user).ToArray();

        using (fs = new FileStream(_xmlFile, FileMode.Create))
        {
            serializer.Serialize(fs, database);
        }

        return nextId;
    }

    public void Update(User user)
    {
        var serializer = new XmlSerializer(typeof(Database));
        var fs = new FileStream(_xmlFile, FileMode.Open);
        var database = (Database)serializer.Deserialize(fs);
        fs.Close();
        var users = database.Users.Where(b => b.Id != user.Id);

        database.Users = users.Append(user).ToArray();
        using(fs = new FileStream(_xmlFile, FileMode.Create))
        {
            serializer.Serialize(fs, database);
        }
    }

    public void Delete(int id)
    {
        var serializer = new XmlSerializer(typeof(Database));
        var fs = new FileStream(_xmlFile, FileMode.Open);
        var database = (Database)serializer.Deserialize(fs);
        fs.Close();

        database.Users = database.Users.Where(b => b.Id != id).ToArray();
        using (fs = new FileStream(_xmlFile, FileMode.Create))
        {
            serializer.Serialize(fs, database);
        }
    }
}