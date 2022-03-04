using System.Net;

public class Client
{
    static async Task Main(string[] args)
    {
        using var client = new HttpClient();

        var result = await client.GetAsync($"localhost:8080/callExt?param1=Prenom&param2=Nom/");
        Console.WriteLine(result.StatusCode);
    }
}
