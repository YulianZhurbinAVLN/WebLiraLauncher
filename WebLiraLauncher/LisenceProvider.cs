using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Media.Protection.PlayReady;

namespace WebLiraLauncher;

public class LisenceProvider
{
    public bool IsThereFreeLicense()
    {
        using HttpClient client = new();
        //Task<IsKeyAvailiableDto?> task = client.GetFromJsonAsync<IsKeyAvailiableDto>("http://localhost:5283/v1/balancer/keys");

        string ipAdress = FindLocalIpAdress();

        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri("http://192.168.1.103:5283/v1/balancer/keys"),
            //RequestUri = new Uri("http://192.168.6.239:5283/v1/balancer/keys"),
            //RequestUri = new Uri("http://localhost:5283/v1/balancer/keys"),
            Method = HttpMethod.Get
        };
        request.Headers.Add("X-Forwarded-For", ipAdress);

        string? json = GetResultFromResponse(request, client);

        if (json is null)
        {
            return false;
        }    
        else
        {
            IsKeyAvailiableDto? isKeyAvailiableDto = JsonConvert.DeserializeObject<IsKeyAvailiableDto>(json);
            bool isThereFreeLicense = isKeyAvailiableDto?.IsKeyAvailiable ?? false;

#if DEBUG
            Console.WriteLine("Is there a free lisence? " + isThereFreeLicense);
#endif

            return isThereFreeLicense;
        }
    }

    private static string FindLocalIpAdress()
    {
        try
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            foreach (IPAddress ipAddress in hostEntry.AddressList)
            {
                // Фильтруем только IPv4 адреса (локальные)
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine($"Локальный IP-адрес: {ipAddress}");
                    return ipAddress.ToString(); // Выходим из цикла после первого найденного IPv4
                }
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Ошибка при получении IP-адреса: {ex.Message}");
        }

        return string.Empty;
    }

    private static string? GetResultFromResponse(HttpRequestMessage request, HttpClient client)
    {
        //HttpResponseMessage response = Task.Run(async () => await client.SendAsync(request)).GetAwaiter().GetResult();
        HttpResponseMessage response = client.Send(request);
        return !response.IsSuccessStatusCode ? null : response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    }
}
public class IsKeyAvailiableDto
{
    public bool IsKeyAvailiable { get; set; }
}
