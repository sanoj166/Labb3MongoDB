using Labb3MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

public static class MongoOperations
{
    private static IMongoCollection<Customer> CustomerCollection =>
        MongoConnection.Database.GetCollection<Customer>("customers");

    private static IMongoCollection<Product> ProductCollection =>
        MongoConnection.Database.GetCollection<Product>("products");

    public static void InsertCustomer(Customer customer)
    {
        CustomerCollection.InsertOne(customer);
    }

    public static List<Customer> GetAllCustomers()
    {
        return CustomerCollection.Find(_ => true).ToList();
    }

    public static Customer GetCustomerByName(string name)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Name, name);
        return CustomerCollection.Find(filter).FirstOrDefault();
    }

    public static void UpdateCustomer(Customer customer)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Id, customer.Id);
        var update = Builders<Customer>.Update
            .Set(c => c.Name, customer.Name)
            .Set(c => c.Password, customer.Password)
            .Set(c => c.Cart, customer.Cart);

        CustomerCollection.UpdateOne(filter, update);
    }

    public static void DeleteCustomer(ObjectId customerId)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Id, customerId);
        CustomerCollection.DeleteOne(filter);
    }

    

    public static void InsertProduct(Product product)
    {
        ProductCollection.InsertOne(product);
    }

    public static List<Product> GetAllProducts()
    {
        return ProductCollection.Find(_ => true).ToList();
    }

    
}

