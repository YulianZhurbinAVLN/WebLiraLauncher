using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace WebLiraLauncher;

public class LisenceProvider
{
    const string REQUEST_URI = "http://192.168.1.103:5283/v1/balancer/keys";
    //Test uris
    //const string REQUEST_URI = "http://192.168.6.239:5283/v1/balancer/keys";
    //const string REQUEST_URI = "http://localhost:5283/v1/balancer/keys";

    public bool IsThereFreeLicense()
    {
        using HttpClient client = new();
        string ipAddress = FindLocalIpAddress();

        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(REQUEST_URI),
            Method = HttpMethod.Get
        };
        request.Headers.Add("X-Forwarded-For", ipAddress);

        string result = GetResultFromResponse(request, client);
        IsKeyAvailableDto? isKeyAvailableDto = JsonConvert.DeserializeObject<IsKeyAvailableDto>(result);
        bool isThereFreeLicense = isKeyAvailableDto?.IsKeyAvailable ?? false;
        return isThereFreeLicense;
    }

    private static string FindLocalIpAddress()
    {
        try
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            foreach (IPAddress ipAddress in hostEntry.AddressList)
            {
                //Находим IPv4 адрес
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    return ipAddress.ToString();
            }

            throw new InvalidOperationException("Не удалось получить доступ к локальному IPv4 адресу.");
        }
        catch (SocketException ex)
        {
            throw new InvalidOperationException(ex.ToString());
        }
    }

    private static string GetResultFromResponse(HttpRequestMessage request, HttpClient client)
    {
        try
        {
            HttpResponseMessage response = client.Send(request);
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return result;
            }
            else
            {
                throw new InvalidOperationException("При запросе лицензии возникла ошибка на стороне сервера");
            }
        }
        catch (HttpRequestException)
        {
            throw new InvalidOperationException("Сервер, предоставляющий лицензии, не отвечает. " +
                "Попробуйте запустить приложение позже");
        }
    }

    private class IsKeyAvailableDto
    {
        public bool IsKeyAvailable { get; set; }
    }
}