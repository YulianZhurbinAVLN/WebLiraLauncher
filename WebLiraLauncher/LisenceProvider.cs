using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace WebLiraLauncher;

public class LisenceProvider
{
    public bool IsThereFreeLicense()
    {
        using HttpClient client = new();
        string ipAdress = FindLocalIpAdress();

        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri("http://192.168.1.103:5283/v1/balancer/keys"),
            //RequestUri = new Uri("http://192.168.6.239:5283/v1/balancer/keys"),
            //RequestUri = new Uri("http://localhost:5283/v1/balancer/keys"),
            Method = HttpMethod.Get
        };
        request.Headers.Add("X-Forwarded-For", ipAdress);

        string? result = GetResultFromResponse(request, client);

        Console.WriteLine(result);

        if (result is null)
        {
            return false;
        }
        else
        {
            IsKeyAvailableDto? isKeyAvailableDto = JsonConvert.DeserializeObject<IsKeyAvailableDto>(result);
            bool isThereFreeLicense = isKeyAvailableDto?.IsKeyAvailable ?? false;

#if DEBUG
            Console.WriteLine("Is there a free lisence? " + isThereFreeLicense);
#endif

            return isThereFreeLicense;
        }

        #region Alternative Version
        //    IsKeyAvailableDto? isKeyAvailableDto = client.GetFromJsonAsync<IsKeyAvailableDto>(
        //"http://192.168.6.239:5283/v1/balancer/keys").GetAwaiter().GetResult();

        //    bool isThereFreeLicense = isKeyAvailableDto?.IsKeyAvailable ?? false;
        //    Console.WriteLine("Is there a free lisence? " + isThereFreeLicense);
        //    return isThereFreeLicense; 
        #endregion
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
        HttpResponseMessage response = client.Send(request);
        return !response.IsSuccessStatusCode ? null : response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    }
}

//public record IsKeyAvailableDto(bool IsKeyAvailable);
public class IsKeyAvailableDto
{
    public bool IsKeyAvailable { get; set; }
}
