using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;


class Program
{
    private const string EndpointUri = "https://166174cosmosdb01.documents.azure.com:443/";
    private const string PrimaryKey = "6GDPxRSZg6RwUx4flphmH52CRDmyTdFzU04oETTHbhFZOUinmvLG789gFr8U8ekbJ7YwSVfxQjqCACDboyeTdg==";
    private const string DatabaseName = "EmployeeDB";
    private const string ContainerName = "Employee";

    private static CosmosClient cosmosClient;
    private static Database database;
    private static Microsoft.Azure.Cosmos.Container container;

    static async Task Main(string[] args)
    {
        // Initialize Cosmos Client
        cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

        // Create or Get Database
        database = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName);

        // Create or Get Container
        container = await database.CreateContainerIfNotExistsAsync(ContainerName, "/id");

        // Perform CRUD operations
      //  await CreateItemAsync();
        await ReadItemAsync("01");
       // await UpdateItemAsync("01", "Updated Employee Name", "Updated Department", 10000);
       // await DeleteItemAsync("01");

        

        // Clean up resources
        cosmosClient.Dispose();
    }

    private static async Task CreateItemAsync()
    {
        var newItem = new Employee
        {
            id = "01",
            EmpName = "John Doe",
            Dept = "Sales",
            Sales = 5000
        };

        try
        {
            ItemResponse<Employee> response = await container.CreateItemAsync(newItem, new PartitionKey(newItem.id));
            Console.WriteLine($"Created Employee: {response.Resource.EmpName}, Dept: {response.Resource.Dept}, Total Sales: {response.Resource.Sales}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating employee: {ex.Message}");
        }
    }

    private static async Task ReadItemAsync(string employeeId)
    {
        try
        {
            ItemResponse<Employee> response = await container.ReadItemAsync<Employee>(employeeId, new PartitionKey(employeeId));
            Console.WriteLine($"Read Employee: {response.Resource.EmpName}, Dept: {response.Resource.Dept}, Total Sales: {response.Resource.Sales}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading employee: {ex.Message}");
        }
    }

    private static async Task UpdateItemAsync(string employeeId, string newName, string newDept, double newTotalSales)
    {
        try
        {
            ItemResponse<Employee> response = await container.ReadItemAsync<Employee>(employeeId, new PartitionKey(employeeId));
            Employee employeeToUpdate = response.Resource;
            employeeToUpdate.EmpName = newName;
            employeeToUpdate.Dept = newDept;
            employeeToUpdate.Sales = newTotalSales;

            response = await container.ReplaceItemAsync(employeeToUpdate, employeeId, new PartitionKey(employeeId));
            Console.WriteLine($"Updated Employee: {response.Resource.EmpName}, Dept: {response.Resource.Dept}, Total Sales: {response.Resource.Sales}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating employee: {ex.Message}");
        }
    }

    private static async Task DeleteItemAsync(string employeeId)
    {
        try
        {
            ItemResponse<Employee> response = await container.DeleteItemAsync<Employee>(employeeId, new PartitionKey(employeeId));
            Console.WriteLine($"Employee with Id:{employeeId} deleted successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting employee: {ex.Message}");
        }
    }

  

}

public class Employee
{
    public string id { get; set; }
    public string EmpName { get; set; }
    public string Dept { get; set; }
    public double Sales { get; set; }
}

