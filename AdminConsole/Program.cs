using StudentServiceClient.UniversalConnector;
using System;

namespace AdminConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceConnector<IAdminConsole> serviceConnector = new ServiceConnector<IAdminConsole>();
                serviceConnector.Connect("net.tcp://localhost:10106/admin-console");
                IAdminConsole proxy = serviceConnector.GetProxy();

                string input = string.Empty;
                Console.Write("Authentication needed!\nEnter username: ");
                string username = Console.ReadLine().Trim();
                Console.Write("Enter password: ");
                string password = Console.ReadLine().Trim();
                string res = proxy.Authenticate(username, password);
                if (res == string.Empty)
                {
                    Console.WriteLine("Invalid username or password");
                    return;
                }
                do
                {
                    Console.WriteLine("Menu\n");
                    Console.WriteLine("1 - List all users");
                    Console.WriteLine("2 - Delete user by email");
                    Console.WriteLine("q - exit ");

                    input = Console.ReadLine();
                    switch (input)
                    {
                        case "1": ListAllUsers(res, proxy); break;
                        case "2": DeleteUserUsers(res, proxy); break;
                    }
                }
                while (input != "q");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();
        }

        private static async void ListAllUsers(string adminKey, IAdminConsole proxy)
        {
            var users = await proxy.ListUsersAsync(adminKey);
            Console.WriteLine("=======================================");
            Console.WriteLine($"{"Email",-20}|{"Name",-15}|{"Last name",-15}|{"Phone number",10}\n");
            foreach (var user in users)
            {
                Console.WriteLine($"{user.Email,-20}|{user.FirstName,-15}|{user.LastName,-15}|{user.PhoneNumber,10}\n");
            }
            Console.WriteLine("=======================================");
        }

        private static async void DeleteUserUsers(string adminKey, IAdminConsole proxy)
        {
            Console.Write("Enter user email to delete: ");
            string emailToDelete = Console.ReadLine();
            var res = await proxy.DeleteByIdAsync(adminKey, emailToDelete);
            Console.WriteLine(res);
        }
    }
}