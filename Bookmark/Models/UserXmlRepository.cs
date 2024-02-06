using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Bookmark.Models;

public class UserXmlRepository(IHostEnvironment env) : IUserRepository
{
    private const string XmlFile = "/data.xml";
    private readonly string _xmlFile = env.ContentRootPath + XmlFile;

    public User[] GetAll()
    {
        var database = Database.Deserialize(_xmlFile);
        return database.Users;
    }

    public User? GetById(int id)
    {
        var doc = XDocument.Load(_xmlFile);
        var user = doc.XPathSelectElement("//Users/User[Id=" + id + "]");

        if (user == null) return null;

        var serializer = new XmlSerializer(typeof(User));
        var stringReader = new StringReader(user.ToString());
        return (User)serializer.Deserialize(stringReader);
    }

    public int Create(User user)
    {
        var database = Database.Deserialize(_xmlFile);
        var users = database.Users;

        user.Id = users.Length > 0
            ? users.Select(b => b.Id).Max() + 1
            : 1;

        database.Users = users.Append(user).ToArray();
        database.Serialize(_xmlFile);

        return user.Id;
    }

    public void Update(User user)
    {
        var database = Database.Deserialize(_xmlFile);
        var users = database.Users.Where(b => b.Id != user.Id);
        database.Users = users.Append(user).ToArray();
        database.Serialize(_xmlFile);
    }

    public void Delete(int id)
    {
        var database = Database.Deserialize(_xmlFile);
        database.Users = database.Users.Where(b => b.Id != id).ToArray();
        database.Serialize(_xmlFile);
    }
}