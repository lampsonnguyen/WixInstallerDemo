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





using System.Net.Http;
using System.Threading.Tasks;

namespace Benbarnett02;

internal class BlogPost{

  public static async Task<string> Post(){
    using (HttpClient myHttpClient = new HttpClient())
    {
       // Most of our code will go in here.
       // Set a base address
      myHttpClient.BaseAddress = new Uri("https://example.com/");
      // Don't put creds in code
      string myCredential = "Bearer password123";
      // Add headers
      myHttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
      myHttpClient.DefaultRequestHeaders.Add("Authorization", myCredential);
      
      // Add content
      string body = "Hi there!";
      StringContent bodyContent = new StringContent(body, Encoding.UTF8, "text/plain");
      
      // Post to example.com/foo with our content
      HttpResponseMessage myHttpResponse = await myHttpClient.PostAsync("/foo", bodyContent);
      // Deal with the reply
      MyObject responseObject = await httpResponse.Content.ReadAsAsync<MyObject>();
    }
  }
}