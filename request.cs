using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // URL to send request
        string url = "https://10.22.131.1:13456/api/hsm/status";

        // Create HttpClientHandler with credentials
        var handler = new HttpClientHandler
        {
            Credentials = new NetworkCredential("johndoe", "password123"),
            ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true // Handle SSL certificate validation
        };

        // Create HttpClient with handler
        using (var client = new HttpClient(handler))
        {
            try
            {
                // Send GET request
                HttpResponseMessage response = await client.GetAsync(url);

                // Ensure success status code
                response.EnsureSuccessStatusCode();

                // Read response content
                string responseBody = await response.Content.ReadAsStringAsync();

                // Output response
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                // Handle exception
                Console.WriteLine($"Request error: {e.Message}");
            }
        }
    }
}
