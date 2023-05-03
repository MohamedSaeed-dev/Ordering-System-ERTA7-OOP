using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTA7_OOP
{
    class Account
    {
        static int int_id = 1;
        private int ID;
        private string Name;
        private string Password;
        private string AccountType;
        private string Email;
        private long PhoneNumber;
        private string Address;
        private bool IsBlocked;
        public int id { get { return ID; } }
        public string name { get { return Name; } set { Name = value; } }
        public string password
        {
            get { return Password; }
            set
            {
                if (value.Length > 5)
                { Password = value; }
                else
                {
                    throw new Exception("Sorry, Password must be long, 6 characters or more\n");
                }
            }
        }
        public string accountType { get { return AccountType; } set { AccountType = value; } }
        public string email
        {
            get { return Email; }
            set
            {
                if (value.Contains("@") && value.EndsWith(".com"))
                { Email = value; }
                else
                {
                    throw new Exception("Sorry, Email is not in correct format, Try Again\n");
                }
            }
        }
        public long phoneNumber
        {
            get { return PhoneNumber; }
            set
            {
                if (value.ToString().Length > 8 && value.ToString().Length < 11)
                { PhoneNumber = value; }
                else
                {
                    throw new Exception("Sorry, Phone Number must be 9 numbers or 10 numbers\n");
                }
            }
        }
        public string address { get { return Address; } set { Address = value; } }
        public bool isBlocked { get { return IsBlocked; } set { IsBlocked = value; } }

        public static List<Account> accounts;

        static Account()
        {
            accounts = new List<Account>();
        }

        public Account(string accType, string name, string password, string email, long phoNum, string address, bool isblocked)
        {
            ID = int_id++;
            this.name = name;
            this.accountType = accType;
            this.email = email;
            this.phoneNumber = phoNum;
            this.address = address;
            this.password = password;
            this.IsBlocked = isblocked;
        }
        public static void SignIn(string name, string password)
        {
            var account = accounts.Where(x => x.name == name && x.password == password).FirstOrDefault();
            if (account == null)
            {
                Console.WriteLine("Username or Password is Incorrect, Please Try Again\n");
                Console.ReadKey();
            }
            else
            {
                if (account.IsBlocked == false)
                {
                    Console.WriteLine("You have Signed Ip Successfully\n");
                    Console.ReadKey();
                    Program.Interface(account);
                }
                else
                {
                    Console.WriteLine("Sorry, The Account is Blocked\n");
                    Console.ReadKey();
                }
            }
        }
        public static void SignUp(string accType, string name, string password, string email, long phoNum, string address)
        {
            var account = accounts.Where(x => x.name == name && x.accountType == accType && x.email == email).FirstOrDefault();
            if (account != null)
            {
                Console.WriteLine("The Account is Already Exists, Please Enter another Info\n");
                Console.ReadKey();
            }
            else
            {
                if (accType == "customer")
                {
                    Customer customer = new Customer(accType, name, password, email, phoNum, address, false);
                    accounts.Add(customer);
                    Console.WriteLine("You have Signed Up Successfully\n");
                    Console.ReadKey();
                    Program.Interface(customer);
                }
                else if (accType == "sp")
                {
                    ServiceProvider serviceProvider = new ServiceProvider(accType, name, password, email, phoNum, address, false);
                    accounts.Add(serviceProvider);
                    Console.WriteLine("You have Signed Up Successfully\n");
                    Console.ReadKey();
                    Program.Interface(serviceProvider);
                }
                else if (accType == "admin")
                {
                    Admin admin = new Admin(accType, name, password, email, phoNum, address, false);
                    accounts.Add(admin);
                    Console.WriteLine("You have Signed Up Successfully\n");
                    Console.ReadKey();
                    Program.Interface(admin);
                }
                else
                {
                    Console.WriteLine("Error : Enter Valid Account Type [customer - sp - admin]\n");
                    Console.ReadKey();
                }
            }
        }

        public void UpdateAccount(int id, string name, string password, string email, string phoNum, string address)
        {
            var account = accounts.Where(x => x.id == id).FirstOrDefault();

            account.name = (name == "") ? account.name : name;
            account.password = (password == "") ? account.password : password;
            account.email = (email == "") ? account.email : email;
            account.phoneNumber = (phoNum == "") ? account.phoneNumber : long.Parse(phoNum);
            account.address = (address == "") ? account.address : address;
            Console.WriteLine("Your Account Info is Updated Successfully\n");
            Console.ReadKey();
        }

        public void LogOut()
        {
            Console.WriteLine("You have Logged Out Successfully\n");
            Console.ReadKey();
            Program.Main();
        }
    }
    class Customer : Account
    {
        private int Notify;
        private List<Order> MyOrders;
        public int notify { get { return Notify; } set { Notify = value; } }
        public List<Order> myOrders { get { return MyOrders; } }
        public Customer(string accType, string name, string password, string email, long phoNum, string address, bool isblocked) : base(accType, name, password, email, phoNum, address, isblocked)
        {
            MyOrders = new List<Order>();
        }
        public void Order(int id, int quantity, string sellmethod, string location)
        {
            var product = Details.ListOfProducts.Where(x => x.id == id).FirstOrDefault();
            if (product == null)
            {
                Console.WriteLine("No Product with that ID\n");
                Console.ReadKey();
            }
            else
            {
                var sell = product.listOfSellMethod.Where(x => x == sellmethod).FirstOrDefault();
                if (sell == null)
                {
                    Console.WriteLine("No sell method with that name, choose another");
                }
                else
                {
                    if (product.quantity >= quantity)
                    {
                        Order order = new Order(product.name, "Unknown", DateTime.Now, product, quantity, sellmethod, location, product.price, this);
                        myOrders.Add(order);
                        Details.ListOfOrders.Add(order);
                        SendNotification(order, product);
                        Console.WriteLine("The Product is ordered Successfully\n");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine($"The Product has {product.quantity} quantity which isn't Enough, Please Enter valid Quantity\n");
                        Console.ReadKey();
                    }
                }
            }
        }
        public void EditOrder(int id, string quantity, string sellmethod)
        {
            var order = myOrders.Where(x => x.id == id).FirstOrDefault();
            if (order == null)
            {
                Console.WriteLine("No Order with that ID\n");
                Console.ReadKey();
            }
            else
            {
                try
                {
                    order.quantity = (quantity == "") ? order.quantity : int.Parse(quantity);
                    order.sellMethod = (sellmethod.ToString() == "") ? order.sellMethod : sellmethod;

                    var order1 = Details.ListOfOrders.Where(x => x.id == id).FirstOrDefault();
                    order1.quantity = (quantity == "") ? order1.quantity : int.Parse(quantity);
                    order1.sellMethod = (sellmethod.ToString() == "") ? order1.sellMethod : sellmethod;
                    Console.WriteLine("The Order Info is Edited Successfully\n");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
            }
        }
        public void CancelOrder(int id)
        {
            var order = myOrders.Where(x => x.id == id).FirstOrDefault();
            if (order == null)
            {
                Console.WriteLine("No Order with that ID\n");
                Console.ReadKey();
            }
            else
            {
                myOrders.Remove(order);
                Details.ListOfOrders.Remove(order);
                Console.WriteLine("The Order is Cancelled Successfully\n");
                Console.ReadKey();
            }
        }
        public void SendNotification(Order order, Product product)
        {
            product.owner.ReceiveNotification(order);
        }
        public void ReceiveNotification()
        {
            notify++;
        }

    }
    class Order
    {
        static int int_id = 1;
        private int ID;
        private string Name;
        private double Price;
        private int Quantity;
        private string Status;
        private string SellMethod;
        private string Location;
        private Customer Owner;
        private Product Product;
        private DateTime CreatedDate;

        public int id { get { return ID; } }
        public string name { get { return Name; } set { Name = value; } }
        public double price { get { return Price; } 
            set 
            {
                if (value >= 0)
                    Price = value;
                else
                    throw new Exception("Enter valid Price\n");
            }
        }
        public int quantity { get { return Quantity; } 
            set 
            { 
                if(value >= 0)
                    Quantity = value;
                else
                    Quantity = 0;
            }
        }
        public string status { get { return Status; } set { Status = value; } }
        public string sellMethod { get { return SellMethod; } set { SellMethod = value; } }
        public string location { get { return Location; } set { Location = value; } }
        public Product product { get { return Product; } set { Product = value; } }
        public Customer owner { get { return Owner; } set { Owner = value; } }
        public DateTime createdDate { get { return CreatedDate; } set { CreatedDate = value; } }

        public Order(string name, string status, DateTime createdDate, Product product, int quantity, string sellmethod, string location, double price, Customer owner)
        {
            ID = int_id++;
            this.name = name;
            this.price = price;
            this.quantity = quantity;
            this.status = status;
            this.sellMethod = sellmethod;
            this.location = location;
            this.product = product;
            this.owner = owner;
            this.createdDate = createdDate.Date;
        }
    }
    class ServiceProvider : Account
    {
        private int Notify;
        private List<Product> Products;
        private List<Order> NotifyOrders;

        public int notify { get { return Notify; } set { Notify = value; } }
        public List<Product> products { get { return Products; } }
        public List<Order> notifyOrders { get { return NotifyOrders; } }
        public ServiceProvider(string accType, string name, string password, string email, long phoNum, string address, bool isblocked) : base(accType, name, password, email, phoNum, address, isblocked)
        {
            Products = new List<Product>();
            NotifyOrders = new List<Order>();
        }
        public void AddProduct(string name, double price, int quantity, DateTime expire, List<string> listsell)
        {
            if (name == "" || price.ToString() == "" || quantity.ToString() == "" || expire.ToString() == "" || listsell.Count == 0)
            {
                Console.WriteLine("Enter valid values, all fields must not be Empty");
                Console.ReadKey();
            }
            else
            {
                Product product = new Product(name, this, price, quantity, expire, listsell);
                products.Add(product);
                Details.ListOfProducts.Add(product);
                Console.WriteLine("The Product is Added Successfully\n");
                Console.ReadKey();
            }
        }
        public void DeleteProduct(int id)
        {
            var product = products.Where(x => x.id == id).FirstOrDefault();
            if (product == null)
            {
                Console.WriteLine("No Product with that ID\n");
                Console.ReadKey();
            }
            else
            {
                products.Remove(product);
                Details.ListOfProducts.Remove(product);
                Console.WriteLine("The Product is Removed Successfully\n");
                Console.ReadKey();
            }
        }
        public void SendNotification(Customer customer)
        {
            customer.ReceiveNotification();
        }
        public void ReceiveNotification(Order order)
        {
            notify++;
            NotifyOrders.Add(order);
        }
        public void EditProduct(int id, string name, string price, string deadline)
        {
            var product = products.Where(x => x.id == id).FirstOrDefault();
            if (product == null)
            {
                Console.WriteLine("No Product with that ID\n");
                Console.ReadKey();
            }
            else
            {
                try
                {
                    product.name = (name == "") ? product.name : name;
                    product.price = (price == "") ? product.price : int.Parse(price);
                    product.deadLineDate = (deadline == "") ? product.deadLineDate : DateTime.Parse(deadline);

                    var product1 = Details.ListOfProducts.Where(x => x.id == id).FirstOrDefault();
                    product1.name = (name == "") ? product1.name : name;
                    product1.price = (price.ToString() == "") ? product1.price : int.Parse(price);
                    product1.deadLineDate = (deadline.ToString() == "") ? product1.deadLineDate :  DateTime.Parse(deadline);
                    Console.WriteLine("The Product Info is Edited Successfully\n");
                    Console.ReadKey();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
            }
        }
        public void Decision(int id)
        {
            var order = NotifyOrders.Where(x => x.id == id).FirstOrDefault();
            if (order == null)
            {
                Console.WriteLine("No Order with that ID\n");
                Console.ReadKey();
            }
            else
            {
                Console.Write("Accept or Reject? [A | R] : ");
                string decision = Console.ReadLine().ToUpper();
                if (decision == "A")
                {
                    order.status = "Accepted";
                    SendNotification(order.owner);
                    this.notify--;
                    order.product.quantity -= order.quantity;
                    Console.WriteLine("The Owner of Order is notified Successfully\n");
                    Console.ReadKey();
                }
                else if (decision == "R")
                {
                    order.status = "Rejected";
                    SendNotification(order.owner);
                    this.notify--;
                    Console.WriteLine("The Owner of Order is notified Successfully\n");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Enter Valid Input [A | R]\n");
                    Console.ReadKey();
                }
            }
        }
    }
    class Product
    {
        static int int_id = 1;
        private int ID;
        private string Name;
        private double Price;
        private int Quantity;
        private ServiceProvider Owner;
        private DateTime DeadlineDate;
        private List<string> ListOfSellMethod;

        public int id { get { return ID; } }
        public string name { get { return Name; } set { Name = value; } }
        public double price { get { return Price; } set { Price = value; } }
        public int quantity { get { return Quantity; } set { Quantity = value; } }
        public ServiceProvider owner { get { return Owner; } set { Owner = value; } }
        public DateTime deadLineDate { get { return DeadlineDate; } set { DeadlineDate = value; } }
        public List<string> listOfSellMethod { get { return ListOfSellMethod; } }

        public Product(string name, ServiceProvider owner, double price, int quantity, DateTime deadline, List<string> listsell)
        {
            ID = int_id++;
            this.name = name;
            this.price = price;
            this.quantity = quantity;
            this.owner = owner;
            this.deadLineDate = deadline.Date;
            ListOfSellMethod = new List<string>();
            this.listOfSellMethod.AddRange(listsell);
        }
    }
    class Admin : Account
    {
        public Admin(string accType, string name, string password, string email, long phoNum, string address, bool isblocked) : base(accType, name, password, email, phoNum, address, isblocked)
        {

        }
        public void Block(int id)
        {
            var account = accounts.Where(x => x.id == id).FirstOrDefault();
            if (account == null)
            {
                Console.WriteLine("No Account with that ID\n");
                Console.ReadKey();
            }
            else
            {
                account.isBlocked = true;
                Console.WriteLine("The Account is Blocked Successfully\n");
                Console.ReadKey();
            }
        }
        public void DeleteUnwantedProduct(int id)
        {
            var product = Details.ListOfProducts.Where(x => x.id == id).FirstOrDefault();
            if (product == null)
            {
                Console.WriteLine("No Product with that ID\n");
                Console.ReadKey();
            }
            else
            {
                Details.ListOfProducts.Remove(product);
                Console.WriteLine("The Product is Removed Successfully BY ADMIN\n");
                Console.ReadKey();
            }
        }
    }
    static class Details
    {
        public static List<Order> ListOfOrders;
        public static List<Product> ListOfProducts;
        static Details()
        {
            ListOfOrders = new List<Order>();
            ListOfProducts = new List<Product>();
        }
        public static double Calculate(Account account)
        {
            double total = 0;
            if (account.accountType == "customer")
            {
                Customer customer = (Customer)account;
                foreach (Order order in customer.myOrders)
                {
                    total += order.price * order.quantity;
                }
                return total;
            }
            else if (account.accountType == "sp")
            {
                ServiceProvider serviceProvider = (ServiceProvider)account;
                foreach (Product product in serviceProvider.products)
                {
                    total += product.price * product.quantity;
                }
                return total;
            }
            else
            {
                return 0;
            }
        }
    }
    internal class Program
    {
        public static void Interface(Account account)
        {
            if (account.accountType == "admin")
            {
                Admin admin = (Admin)account;
                Console.WriteLine("============= WELCOME TO ERTA7 SYSTEM =============\n\n Your Account : \n");
                Console.WriteLine($" ID : {admin.id}\n" +
                    $" name : {admin.name}\n" +
                    $" AccountType : {admin.accountType}\n" +
                    $" Email : {admin.email}\n" +
                    $" Phone : {admin.phoneNumber}\n" +
                    $" Address : {admin.address}\n" +
                    $"");
            }
            else if (account.accountType == "sp")
            {
                ServiceProvider serviceProvider = (ServiceProvider)account;
                Console.WriteLine("============= WELCOME TO ERTA7 SYSTEM =============\n\n Your Account : \n");
                Console.WriteLine($" ID : {serviceProvider.id}\n" +
                    $" name : {serviceProvider.name}\n" +
                    $" AccountType : {serviceProvider.accountType}\n" +
                    $" Email : {serviceProvider.email}\n" +
                    $" Phone : {serviceProvider.phoneNumber}\n" +
                    $" Address : {serviceProvider.address}\n" +
                    $" Notification : {serviceProvider.notify}\n" +
                    $"");
            }
            else if (account.accountType == "customer")
            {
                Customer customer = (Customer)account;
                Console.WriteLine("============= WELCOME TO ERTA7 SYSTEM =============\n\n Your Account : \n");
                Console.WriteLine($" ID : {customer.id}\n" +
                    $" name : {customer.name}\n" +
                    $" AccountType : {customer.accountType}\n" +
                    $" Email : {customer.email}\n" +
                    $" Phone : {customer.phoneNumber}\n" +
                    $" Address : {customer.address}\n" +
                    $" Notification : {customer.notify}\n" +
                    $"");
            }
            while (true)
            {
                if (account.accountType == "customer")
                {
                    Console.WriteLine("(1)  Order a Product\n" +
                        "(2)  Edit an Order\n" +
                        "(3)  Cancel an Order\n" +
                        "(4)  MyOrders\n" +
                        "(5)  List of Products\n" +
                        "(6)  Update Account\n" +
                        "(7)  LogOut\n");
                    Console.Write("Enter your Choice : ");
                    int choice;
                    try { choice = int.Parse(Console.ReadLine()); } catch { Console.WriteLine("Enter valid value [1-7]\n"); Console.ReadKey(); continue; }
                    Customer customer = (Customer)account;
                    if (choice == 1)
                    {
                        try
                        {
                            Console.Write("Enter ID of Product : ");
                            int idproduct = int.Parse(Console.ReadLine());
                            Console.Write("Enter The Quantity : ");
                            int quantity = int.Parse(Console.ReadLine());
                            Console.Write("Enter The Sell Method : ");
                            string sell = Console.ReadLine();
                            Console.Write("Enter The Location : ");
                            string location = Console.ReadLine();
                            customer.Order(idproduct, quantity, sell, location);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.ReadKey();
                        }
                    }
                    else if (choice == 2)
                    {
                        try
                        {
                            Console.Write("Enter ID of Order : ");
                            int idorder = int.Parse(Console.ReadLine());
                            Console.Write("Enter New Quantity of Order : ");
                            string Newquantity = Console.ReadLine();
                            Console.WriteLine("Enter New Sell Method of Order : ");
                            string Newsell = Console.ReadLine();
                            customer.EditOrder(idorder, Newquantity, Newsell);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.ReadKey();
                        }
                    }
                    else if (choice == 3)
                    {
                        try
                        {
                            Console.Write("Enter ID of Order : ");
                            int id_order = int.Parse(Console.ReadLine());
                            customer.CancelOrder(id_order);
                        }
                        catch
                        {
                            Console.WriteLine("Error : Enter valid value\n");
                            Console.ReadKey();
                        }
                    }
                    else if (choice == 4)
                    {
                        foreach (Order order in customer.myOrders)
                        {
                            Console.WriteLine($"ID :{order.id}\n" +
                                $"Name : {order.name}\n" +
                                $"Owner: { order.owner.name}\n" +
                                $"Price : {order.price}\n" +
                                $"Quantity : {order.quantity}\n" +
                                $"Sell Method : {order.sellMethod}\n" +
                                $"Status : {order.status}\n" +
                                $"Location : {order.location}\n" +
                                $"Created Date : {order.createdDate}\n");
                        }
                        double total = Details.Calculate(customer);
                        Console.WriteLine($"Total Price of Orders : {total}\n");
                        Console.ReadKey();
                    }
                    else if (choice == 5)
                    {
                        foreach (Product product in Details.ListOfProducts)
                        {
                            Console.WriteLine($"ID : {product.id}\n" +
                                $"Name : {product.name}\n" +
                                $"Owner : {product.owner.name}\n" +
                                $"Price : {product.price}\n" +
                                $"Quantity : {product.quantity}\n" +
                                $"Deadline Date : {product.deadLineDate}");
                            Console.Write("Sell Methods : ");
                            foreach (string sell in product.listOfSellMethod)
                            {
                                Console.Write(sell + ", ");
                            }
                            Console.WriteLine("\n");
                        }
                        Console.ReadKey();
                    }
                    else if (choice == 6)
                    {
                        try
                        {
                            Console.Write("Enter New Username : ");
                            string Newname = Console.ReadLine();
                            Console.Write("Enter New Email : ");
                            string Newemail = Console.ReadLine();
                            Console.Write("Enter New Phone Number : ");
                            string Newphone = Console.ReadLine();
                            Console.Write("Enter New Address : ");
                            string Newaddress = Console.ReadLine();
                            Console.Write("Enter Old Password : ");
                            string Oldpassword = Console.ReadLine();
                            string Newpassword = "";
                            if (customer.password == Oldpassword)
                            {
                                Console.Write("Enter The New Password : ");
                                Newpassword = Console.ReadLine();
                            }
                            else if (Oldpassword == "")
                            {
                                Newpassword = "";
                            }
                            else
                            {
                                Console.WriteLine("Password is InCorrect, Try Again");
                            }
                            customer.UpdateAccount(customer.id, Newname, Newpassword, Newemail, Newphone, Newaddress);
                            Interface(customer);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.ReadKey();
                        }

                    }
                    else if (choice == 7)
                    {
                        customer.LogOut();
                        Console.WriteLine("Enter valid Input [1-7]\n");
                        Console.ReadKey();
                    }
                }
                else if (account.accountType == "sp")
                {
                    Console.WriteLine("(1)  Add Product\n" +
                        "(2)  Delete Product\n" +
                        "(3)  Edit Product\n" +
                        "(4)  List of Products\n" +
                        "(5)  Update Account\n" +
                        "(6)  Notification\n" +
                        "(7)  LogOut\n");
                    Console.Write("Enter your Choice : ");
                    int choice;
                    try { choice = int.Parse(Console.ReadLine()); } catch { Console.WriteLine("Enter valid value [1-7]\n"); Console.ReadKey(); continue; }
                    ServiceProvider serviceProvider = (ServiceProvider)account;
                    if (choice == 1)
                    {
                        List<string> sellmethods = new List<string>();
                        try
                        {
                            Console.Write("Enter Name of Product : ");
                            string nameProduct = Console.ReadLine();
                            Console.Write("Enter Price of Product : ");
                            double price = double.Parse(Console.ReadLine());
                            Console.Write("Enter Quantity of Product : ");
                            int quantity = int.Parse(Console.ReadLine());
                            Console.Write("Enter Deadline Date of Product : ");
                            DateTime date = DateTime.Parse(Console.ReadLine());
                            bool check = true;
                            while (check)
                            {
                                Console.Write("Enter Ways of Sell Method : ");
                                string sell = Console.ReadLine();
                                sellmethods.Add(sell);
                                Console.Write("Do you want to add more T or F : ");
                                string check1 = Console.ReadLine().ToUpper();
                                if (check1 == "T")
                                {
                                    check = true;
                                }
                                else if(check1 == "F")
                                {
                                    check = false;
                                }
                                else
                                {
                                    Console.WriteLine("Enter valid value [T - F]\n");
                                }
                            }
                            serviceProvider.AddProduct(nameProduct, price, quantity, date, sellmethods);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.ReadKey();
                        }
                    }
                    else if (choice == 2)
                    {
                        try
                        {
                            Console.Write("Enter ID of Product : ");
                            int id_product = int.Parse(Console.ReadLine());
                            serviceProvider.DeleteProduct(id_product);
                        }
                        catch
                        {
                            Console.WriteLine("Error : Enter valid value\n");
                            Console.ReadKey();
                        }
                    }
                    else if (choice == 3)
                    {
                        try
                        {
                            Console.Write("Enter ID of Product : ");
                            int id_product = int.Parse(Console.ReadLine());
                            Console.Write("Enter New Name of Product : ");
                            string newName = Console.ReadLine();
                            Console.Write("Enter New Price of Product : ");
                            string newPrice = Console.ReadLine();
                            Console.Write("Enter New Deadline Date of Product : ");
                            string newDate = Console.ReadLine();
                            serviceProvider.EditProduct(id_product, newName, newPrice, newDate);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.ReadKey();
                        }
                    }
                    else if (choice == 4)
                    {
                        foreach (Product product in serviceProvider.products)
                        {
                            Console.WriteLine($"ID : {product.id}\n" +
                                $"Name : {product.name}\n" +
                                $"Owner : {product.owner.name}\n" +
                                $"Price : {product.price}\n" +
                                $"Quantity : {product.quantity}\n" +
                                $"Deadline Date : {product.deadLineDate}");
                            Console.Write("Sell Methods : ");
                            foreach (string sell in product.listOfSellMethod)
                            {
                                Console.Write(sell + ", ");
                            }
                            Console.WriteLine("\n");
                        }
                        double total = Details.Calculate(serviceProvider);
                        Console.WriteLine($"Total Price of Products : {total}\n");
                        Console.ReadKey();

                    }
                    else if (choice == 5)
                    {
                        try
                        {
                            Console.Write("Enter New Username : ");
                            string Newname = Console.ReadLine();
                            Console.Write("Enter New Email : ");
                            string Newemail = Console.ReadLine();
                            Console.Write("Enter New Phone Number : ");
                            string Newphone = Console.ReadLine();
                            Console.Write("Enter New Address : ");
                            string Newaddress = Console.ReadLine();
                            Console.Write("Enter Old Password : ");
                            string Oldpassword = Console.ReadLine();
                            string Newpassword = "";
                            if (serviceProvider.password == Oldpassword)
                            {
                                Console.Write("Enter New Password : ");
                                Newpassword = Console.ReadLine();
                            }
                            else if (Oldpassword == "")
                            {
                                Newpassword = "";
                            }
                            else
                            {
                                Console.WriteLine("Password is InCorrect, Try Again");
                            }
                            serviceProvider.UpdateAccount(serviceProvider.id, Newname, Newpassword, Newemail, Newphone, Newaddress);
                            Interface(serviceProvider);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.ReadKey();
                        }
                    }
                    else if (choice == 6)
                    {
                        try
                        {
                            foreach (Order order in serviceProvider.notifyOrders)
                            {
                                Console.WriteLine($"ID : {order.id}\n" +
                                    $"Name : {order.name}\n" +
                                    $"Owner: { order.owner.name}\n" +
                                    $"Quantity : {order.quantity}\n" +
                                    $"Sell Method: { order.sellMethod}\n" +
                                    $"Status : {order.status}\n");
                            }
                            Console.ReadKey();
                            Console.Write("Enter ID of Order or Exit: ");
                            string choose = Console.ReadLine();
                            if (choose == "exit" || choose == "Exit")
                            {
                                continue;
                            }
                            else
                            {
                                serviceProvider.Decision(int.Parse(choose));
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Error : Enter vaild values\n");
                            Console.ReadKey();
                        }


                    }
                    else if (choice == 7)
                    {
                        serviceProvider.LogOut();
                    }
                    else
                    {
                        Console.WriteLine("Enter valid Input [1-7]\n");
                        Console.ReadKey();
                    }
                }
                else if (account.accountType == "admin")
                {
                    Console.WriteLine("(1)  Block Accounts\n" +
                        "(2)  Delete Unwanted Product\n" +
                        "(3)  List of Accounts\n" +
                        "(4)  List of Products\n" +
                        "(5)  List of Orders\n" +
                        "(6)  Update Account\n" +
                        "(7)  LogOut\n");
                    Admin admin = (Admin)account;
                    Console.Write("Enter your Choice : ");
                    int choice;
                    try { choice = int.Parse(Console.ReadLine()); } catch { Console.WriteLine("Enter valid value [1-7]\n"); Console.ReadKey(); continue; }
                    if (choice == 1)
                    {
                        try
                        {
                            Console.Write("Enter ID of Account : ");
                            int id_account = int.Parse(Console.ReadLine());
                            admin.Block(id_account);
                        }
                        catch
                        {
                            Console.WriteLine("Error : Enter valid value\n");
                            Console.ReadKey();
                        }
                    }
                    else if (choice == 2)
                    {
                        try
                        {
                            Console.Write("Enter ID of Product : ");
                            int id_product = int.Parse(Console.ReadLine());
                            admin.DeleteUnwantedProduct(id_product);
                        }
                        catch
                        {
                            Console.WriteLine("Error : Enter valid value\n");
                            Console.ReadKey();
                        }
                    }
                    else if (choice == 3)
                    {
                        try
                        {
                            foreach (Account account1 in Account.accounts)
                            {
                                if (account1.accountType == "admin")
                                {
                                    Admin admin1 = (Admin)account1;
                                    Console.WriteLine($"ID : {admin1.id}\n" +
                                      $"Name : {admin1.name}\n" +
                                      $"Account Type : {admin1.accountType}\n" +
                                      $"Email : {admin1.email}\n" +
                                      $"Phone Number : {admin1.phoneNumber}\n" +
                                      $"Address : {admin1.address}\n" +
                                      $"IsBlocked : {admin1.isBlocked}\n");
                                }
                                else if (account1.accountType == "sp")
                                {
                                    ServiceProvider serviceProvider = (ServiceProvider)account1;
                                    Console.WriteLine($"ID : {serviceProvider.id}\n" +
                                      $"Name : {serviceProvider.name}\n" +
                                      $"Account Type : {serviceProvider.accountType}\n" +
                                      $"Email : {serviceProvider.email}\n" +
                                      $"Phone Number : {serviceProvider.phoneNumber}\n" +
                                      $"Address : {serviceProvider.address}\n" +
                                      $"IsBlocked : {serviceProvider.isBlocked}\n" +
                                      $"Notification : {serviceProvider.notify}\n");
                                }
                                else if (account1.accountType == "customer")
                                {
                                    Customer customer1 = (Customer)account1;
                                    Console.WriteLine($"ID : {customer1.id}\n" +
                                      $"Name : {customer1.name}\n" +
                                      $"Account Type : {customer1.accountType}\n" +
                                      $"Email : {customer1.email}\n" +
                                      $"Phone Number : {customer1.phoneNumber}\n" +
                                      $"Address : {customer1.address}\n" +
                                      $"IsBlocked : {customer1.isBlocked}" +
                                      $"Notification : {customer1.notify}\n");
                                }
                            }
                            Console.ReadKey();
                        }
                        catch
                        {
                            Console.WriteLine("Error : Enter valid values\n");
                            Console.ReadKey();
                        }
                    }
                    else if (choice == 4)
                    {

                        foreach (Product product in Details.ListOfProducts)
                        {
                            Console.WriteLine($"ID : {product.id}\n" +
                                $"Name : {product.name}\n" +
                                $"Owner : {product.owner.name}\n" +
                                $"Price : {product.price}\n" +
                                $"Quantity : {product.quantity}\n" +
                                $"Deadline Date : {product.deadLineDate}");
                            Console.Write("Sell Methods : ");
                            foreach (string sell in product.listOfSellMethod)
                            {
                                Console.Write(sell + ", ");
                            }
                            Console.WriteLine("\n");
                        }
                        Console.ReadKey();

                    }
                    else if (choice == 5)
                    {
                        foreach (Order order in Details.ListOfOrders)
                        {
                            Console.WriteLine($"ID : {order.id}\n" +
                                $"Name : {order.name}\n" +
                                $"Owner: { order.owner.name}\n" +
                                $"Price : {order.price}\n" +
                                $"Quantity : {order.quantity}\n" +
                                $"Sell Method : {order.sellMethod}\n" +
                                $"Status : {order.status}\n" +
                                $"Location : {order.location}\n" +
                                $"Created Date : {order.createdDate}\n");
                        }
                        Console.ReadKey();
                    }
                    else if (choice == 6)
                    {
                        try
                        {
                            Console.Write("Enter New Username : ");
                            string Newname = Console.ReadLine();
                            Console.Write("Enter New Email : ");
                            string Newemail = Console.ReadLine();
                            Console.Write("Enter New Phone Number : ");
                            string Newphone = Console.ReadLine();
                            Console.Write("Enter New Address : ");
                            string Newaddress = Console.ReadLine();
                            Console.Write("Enter Old Password : ");
                            string Oldpassword = Console.ReadLine();
                            string Newpassword = "";
                            if (admin.password == Oldpassword)
                            {
                                Console.Write("Enter New Password : ");
                                Newpassword = Console.ReadLine();
                            }
                            else if(Oldpassword == "")
                            {
                                Newpassword = "";
                            }
                            else
                            {
                                Console.WriteLine("Password is InCorrect, Try Again");
                            }
                            admin.UpdateAccount(admin.id, Newname, Newpassword, Newemail, Newphone, Newaddress);
                            Interface(admin);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.ReadKey();
                        }
                    }
                    else if (choice == 7)
                    {
                        admin.LogOut();
                    }
                    else
                    {
                        Console.WriteLine("Enter valid Input [1-7]\n");
                        Console.ReadKey();
                    }
                }
            }
        }
        public static void Main()
        {
            Console.Title = "ERTA7 System";
            Console.ForegroundColor = ConsoleColor.Yellow;
            while (true)
            {
                Console.WriteLine("This System has been Programmed by [Mohamed Saeed Ali bin Omar] \nGithub : MohamedSaeed-dev, for my other projects");
                Console.WriteLine("=================== WELCOME TO ERTA7 SYSTEM ===================\n");
                Console.WriteLine("(1)  SignIn\n" +
                    "(2)  SignUp\n" +
                    "(3)  Exit\n");
                Console.Write("Enter your Choice : ");
                int choice;
                try { choice = int.Parse(Console.ReadLine()); } catch { Console.WriteLine("Enter valid value [1-3]\n"); Console.ReadKey(); continue; }
                if (choice == 1)
                {
                    try
                    {
                        Console.Write("Enter Username : ");
                        string name = Console.ReadLine();
                        Console.Write("Enter Password : ");
                        string password = Console.ReadLine();
                        Account.SignIn(name, password);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.ReadKey();
                    }
                }
                else if (choice == 2)
                {
                    try
                    {
                        Console.Write("Enter Username : ");
                        string name = Console.ReadLine();
                        Console.Write("Enter Account Type : ");
                        string type = Console.ReadLine().ToLower();
                        Console.Write("Enter Email : ");
                        string email = Console.ReadLine();
                        Console.Write("Enter Phone Number : ");
                        long phone = long.Parse(Console.ReadLine());
                        Console.Write("Enter Address : ");
                        string address = Console.ReadLine();
                        Console.Write("Enter Password : ");
                        string password = Console.ReadLine();
                        Account.SignUp(type, name, password, email, phone, address);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.ReadKey();
                    }

                }
                else if (choice == 3)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Enter valid Input [1-3]\n");
                    Console.ReadKey();
                }
            }
        }
    }
}