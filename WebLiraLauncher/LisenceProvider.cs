using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WebLiraLauncher;

public class LisenceProvider
{
    public bool IsThereFreeLicense()
    {
        using HttpClient client = new();
        //Task<IsKeyAvailiableDto?> task = client.GetFromJsonAsync<IsKeyAvailiableDto>("http://localhost:5283/v1/balancer/keys");
        //Task<IsKeyAvailiableDto?> task = client.GetFromJsonAsync<IsKeyAvailiableDto>("http://192.168.1.103:5283/v1/balancer/keys");
        //bool isThereFreeLicense = task.Result?.IsKeyAvailiable ?? false;

        string ipAdress = FindLocalIpAdress();

        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri("http://192.168.1.103:5283/v1/balancer/keys"),
            //RequestUri = new Uri("http://192.168.6.239:5283/v1/balancer/keys"),
            //RequestUri = new Uri("http://localhost:5283/v1/balancer/keys"),
            Method = HttpMethod.Get
        };
        request.Headers.Add("X-Forwarded-For", ipAdress );

        using var response = client.Send(request);
        IsKeyAvailiableDto? isKeyAvailiableDto = response.Content.ReadFromJsonAsync<IsKeyAvailiableDto>().GetAwaiter().GetResult();
        bool isThereFreeLicense = isKeyAvailiableDto?.IsKeyAvailiable ?? false;
        //bool isThereFreeLicense = false;

#if DEBUG
        Console.WriteLine("Is there a free lisence? " + isThereFreeLicense);
#endif

        return isThereFreeLicense;
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
                    Console.WriteLine($"Локальный IP-адрес: {ipAddress.ToString()}");
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

    record IsKeyAvailiableDto(bool IsKeyAvailiable);
}
