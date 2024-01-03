using System;
using MongoDB.Bson;
using System.Collections.Generic;

class Program
{
    private static Customer loggedInCustomer = null;

    static void Main(string[] args)
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the MongoDB Store Application!");

            if (loggedInCustomer == null)
            {
                Console.WriteLine("1. Register a new customer");
                Console.WriteLine("2. Log in");
            }
            else
            {
                Console.WriteLine($"Logged in as: {loggedInCustomer.Name}");
                Console.WriteLine("3. Shop");
                Console.WriteLine("4. View shopping cart");
                Console.WriteLine("5. Go to checkout");
                Console.WriteLine("6. Log out");
            }

            Console.WriteLine("7. Exit");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    RegisterCustomer();
                    break;
                case 2:
                    Login();
                    break;
                case 3:
                    if (loggedInCustomer != null)
                    {
                        Shop();
                    }
                    else
                    {
                        Console.WriteLine("Please log in first.");
                        Console.ReadLine(); 
                    }
                    break;
                case 4:
                    if (loggedInCustomer != null)
                    {
                        Console.Clear();
                        Console.WriteLine(loggedInCustomer);
                        Console.ReadLine(); 
                    }
                    else
                    {
                        Console.WriteLine("Please log in first.");
                        Console.ReadLine(); 
                    }
                    break;
                case 5:
                    if (loggedInCustomer != null)
                    {
                        Checkout();
                    }
                    else
                    {
                        Console.WriteLine("Please log in first.");
                        Console.ReadLine(); 
                    }
                    break;
                case 6:
                    if (loggedInCustomer != null)
                    {
                        Logout();
                    }
                    else
                    {
                        Console.WriteLine("Please log in first");
                        Console.ReadLine(); 
                    }
                    break;
                case 7:
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    Console.ReadLine(); 
                    break;
            }
        }
    }


    static void RegisterCustomer()
    {
        Console.Write("Enter your name: ");
        string name = Console.ReadLine();
        Console.WriteLine("Enter a password: ");
        string password = Console.ReadLine();

        Customer newCustomer = new Customer(name, password);
        MongoOperations.InsertCustomer(newCustomer);
        Console.WriteLine("Customer registered!\n");
    }

    static void Login()
    {
        Console.WriteLine("Enter your name: ");
        string name = Console.ReadLine();
        Console.WriteLine("Enter your password: ");
        string password = Console.ReadLine();

        Customer customer = MongoOperations.GetCustomerByName(name);

        if (customer == null)
        {
            Console.WriteLine("Customer does not exist. Do you want to register a new customer? (yes/no)");
            string response = Console.ReadLine().ToLower();
            if (response == "yes")
            {
                RegisterCustomer();
            }
        }
        else if (!customer.VerifyPassword(password))
        {
            Console.WriteLine("Incorrect password. Please try again. ");
        }
        else
        {
            loggedInCustomer = customer;
            Console.WriteLine("Login successful!");
        }
    }

    static void Logout()
    {
        loggedInCustomer = null;
        Console.WriteLine("Logged out successfully.");
    }

    static void Shop()
    {
        bool shopping = true;
        while (shopping)
        {
            Console.WriteLine($"Welcome, {loggedInCustomer.Name}!");
            Console.WriteLine("1. Display Products");
            Console.WriteLine("2. Buy Product");
            Console.WriteLine("3. View Shopping Cart");
            Console.WriteLine("4. Go to Checkout");
            Console.WriteLine("5. Back to Main Menu");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    DisplayProducts();
                    break;
                case 2:
                    BuyProduct();
                    break;
                case 3:
                    ViewShoppingCart();
                    break;
                case 4:
                    Checkout();
                    shopping = false;
                    break;
                case 5:
                    shopping = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again. ");
                    break;
            }
        }
    }

    private static void ViewShoppingCart()
    {
        Console.Clear();
        Console.WriteLine("Shopping Cart:");

        if (loggedInCustomer.Cart.Count == 0)
        {
            Console.WriteLine("Your shopping cart is empty.");
        }
        else
        {
            var groupedCart = loggedInCustomer.Cart
                .GroupBy(p => new { p.Id, p.Name, p.Price })
                .Select(g => new
                {
                    Product = g.Key,
                    Quantity = g.Count()
                });

            foreach (var cartItem in groupedCart)
            {
                Console.WriteLine($"{cartItem.Product.Name} - {cartItem.Product.Price:C} x{cartItem.Quantity}");
            }

            Console.WriteLine("Options:");
            Console.WriteLine("1. Remove Items");
            Console.WriteLine("2. Empty Cart");
            Console.WriteLine("3. Go Back");
            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    RemoveItemsFromCart();
                    break;
                case 2:
                    EmptyCart();
                    break;
                case 3:
                    break; 
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private static void RemoveItemsFromCart()
    {
        Console.Write("Enter the name of the product to remove: ");
        string productNameToRemove = Console.ReadLine();

        Console.Write("Enter the quantity to remove: ");
        int quantityToRemove = int.Parse(Console.ReadLine());

        var productToRemove = loggedInCustomer.Cart
            .FirstOrDefault(p => p.Name.Equals(productNameToRemove, StringComparison.OrdinalIgnoreCase));

        if (productToRemove != null)
        {
            for (int i = 0; i < quantityToRemove && loggedInCustomer.Cart.Contains(productToRemove); i++)
            {
                loggedInCustomer.Cart.Remove(productToRemove);
            }

            Console.WriteLine($"{quantityToRemove} {productToRemove.Name} has been removed from your shopping cart.\n");
        }
        else
        {
            Console.WriteLine($"Product {productNameToRemove} not found in the cart.");
        }
    }

    private static void EmptyCart()
    {
        loggedInCustomer.Cart.Clear();
        Console.WriteLine("Your shopping cart has been emptied.");
    }



    private static void DisplayProducts()
    {
        Console.Clear();
        Console.WriteLine("Available products:");

        var products = MongoOperations.GetAllProducts();

        for (int i = 0; i < products.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {products[i].Name} - {products[i].Price:C}");
        }
    }

    private static void BuyProduct()
    {
        var products = MongoOperations.GetAllProducts();

        if (products.Count == 0)
        {
            Console.WriteLine("No products available. Please contact the store administrator.");
            return;
        }

        Console.Write("Enter the number of the product you want to buy: ");
        int productIndex = int.Parse(Console.ReadLine()) - 1;

        if (productIndex >= 0 && productIndex < products.Count)
        {
            Product selectedProduct = products[productIndex];

            Console.Write($"Enter the quantity of {selectedProduct.Name} you want to buy: ");
            int quantity = int.Parse(Console.ReadLine());

            if (quantity > 0)
            {
                for (int i = 0; i < quantity; i++)
                {

                    loggedInCustomer.AddCart(new Product(selectedProduct.Name, selectedProduct.Price));
                }

                Console.WriteLine($"{quantity} {selectedProduct.Name} has been added to your shopping cart.\n");
            }
            else
            {
                Console.WriteLine("Quantity must be greater than zero. Please try again.");
            }
        }
        else
        {
            Console.WriteLine("Invalid choice. Please try again.");
        }
    }


    static void Checkout()
    {
        Console.WriteLine("Shopping Cart:");
        foreach (var product in loggedInCustomer.Cart)
        {
            Console.WriteLine($"{product.Name} - {product.Price:C}");
        }

        decimal totalAmount = loggedInCustomer.Cart.Sum(product => product.Price);
        Console.WriteLine($"Total Amount: {totalAmount:C}");

        Console.WriteLine("1. Continue Shopping");
        Console.WriteLine("2. Pay for Your Order");

        int checkoutChoice = int.Parse(Console.ReadLine());

        switch (checkoutChoice)
        {
            case 1:
                Console.WriteLine("Continuing shopping...");
                break;
            case 2:
                Console.WriteLine("Thank you for your purchase!");
                loggedInCustomer.Cart.Clear();
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }
}
