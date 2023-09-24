using System;
using System.Collections.Generic;
using Bogus;
using MySql.Data.MySqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Server=localhost;Port=3306;User=root;Password=root;";

        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            using (var createDbCommand = new MySqlCommand("CREATE DATABASE IF NOT EXISTS mydatabase", connection))
            {
                createDbCommand.ExecuteNonQuery();
            }

            connection.ChangeDatabase("mydatabase");


            using (var createUserTableCommand = new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS Users (Id INT AUTO_INCREMENT PRIMARY KEY, FirstName VARCHAR(255), LastName VARCHAR(255), Age INT)",
                connection))
            {
                createUserTableCommand.ExecuteNonQuery();
            }

            var faker = new Faker<User>()
                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                .RuleFor(u => u.LastName, f => f.Person.LastName)
                .RuleFor(u => u.Age, f => f.Random.Number(5, 70)); 

            var users = faker.Generate(10);


            foreach (var user in users)
            {
                if (user.Age >= 14)
                {
                    using (var insertUserCommand = new MySqlCommand(
                        "INSERT INTO Users (FirstName, LastName, Age) VALUES (@FirstName, @LastName, @Age)",
                        connection))
                    {
                        insertUserCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                        insertUserCommand.Parameters.AddWithValue("@LastName", user.LastName);
                        insertUserCommand.Parameters.AddWithValue("@Age", user.Age);
                        insertUserCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    Console.WriteLine($"Пользователь {user.FirstName} {user.LastName} младше 14 лет и не будет зарегистрирован.");
                }
            }
        }
    }
}

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}
