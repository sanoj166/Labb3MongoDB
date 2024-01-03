using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

public class Customer
{

    public void AddCart(Product product)
    {
        Cart.Add(product);
        MongoOperations.UpdateCustomer(this);
    }

    [BsonId]
    public ObjectId Id { get; set; }

    public string Name { get; set; }
    public string Password { get; set; }

    public List<Product> Cart { get; set; } = new List<Product>();

    public Customer() { }

    public Customer(string name, string password)
    {
        Name = name;
        Password = password;
    }

    public bool VerifyPassword(string password)
    {
        return Password == password;
    }
}
