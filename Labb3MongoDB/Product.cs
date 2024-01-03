using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class Product
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Name { get; set; }
    public decimal Price { get; set; }

    
    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}
