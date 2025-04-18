using System;
using System.Data.Common;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Window;
using src;

public class HttpServer
{
    private readonly HttpListener _listener;
    private readonly string serverIp ;

    private Database database;

    public HttpServer(string port ="5001")
    {   
        this.database =Database.GetInstance();
        string ip = GetLocalIPAddress();
        Console.WriteLine("IP détectée : " + ip);
        string prefix = $"https://{ip}:{port}/";
        serverIp = prefix;

        _listener = new HttpListener();
        _listener.Prefixes.Add(prefix);  // Par exemple, "https://localhost:5001/"
    }


   private string GetLocalIPAddress()
    {
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus != OperationalStatus.Up)
                continue;

            var props = networkInterface.GetIPProperties();

            foreach (var address in props.UnicastAddresses)
            {
                var ip = address.Address;
                if (ip.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(ip) &&
                    !ip.ToString().StartsWith("169.254")) // Évite APIPA
                {
                    return ip.ToString();
                }
            }
        }

        return "localhost"; // fallback
    }


    public async Task StartAsync(){
         try
        {
            _listener.Start();
            Console.WriteLine($"listening on {serverIp}");
           
            _= database.CleanupExpiredSessionsAsync();

            while (true)
            {
                var context = await _listener.GetContextAsync();
                await HandleRequest(context);
                
                
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur dans le serveur : " + ex.Message);
        }
    }

    private async Task HandleRequest(HttpListenerContext context)
    {

        try
        {
            var response = context.Response;

            // Vérifie le chemin et appelle la méthode correspondante
            string? path = context.Request.Url?.AbsolutePath;

            switch (path)
            {
                case "/":
                    await WebSiteHandler.ServeIndexHtml(context);
                    break;
                case "/registerTraitment":
                    await AuthController.RegisterTraitment(context);
                    break;
                case "/contact":
                   // ServeContactPage(context);
                    break;
                default:
                    ServeNotFound(context);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors du traitement de la requête : " + ex.Message);
        }
    }


    private static void ServeNotFound(HttpListenerContext context)
    {
        var response = context.Response;
        string errorResponse = "404 - Not Found";
        byte[] buffer = Encoding.UTF8.GetBytes(errorResponse);
        response.StatusCode = (int)HttpStatusCode.NotFound;
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }


 
    public static string ReadRequestBody(HttpListenerRequest request) {
        using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
        return reader.ReadToEnd();
    }

     // Méthode utilitaire pour envoyer une réponse
    public  static void SendResponse(HttpListenerContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        using var writer = new StreamWriter(context.Response.OutputStream);
        writer.Write(message);
        context.Response.Close();
    }


    public void Stop()
    {
        _listener.Stop();
    }


    public static async Task RenderTemplateAsync(HttpListenerContext context, string templatePath, Dictionary<string, string>? replacements = null)
    {
        var response = context.Response;
        try
        {
            if (File.Exists(templatePath))
            {
                // Lecture du fichier de manière asynchrone
                string htmlContent = await File.ReadAllTextAsync(templatePath); 

                // Si des remplacements sont fournis, on remplace les placeholders dans le fichier HTML
                if (replacements != null)
                {
                    foreach (var replacement in replacements)
                    {
                        htmlContent = htmlContent.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
                    }
                }

                byte[] buffer = Encoding.UTF8.GetBytes(htmlContent);
                response.ContentType = "text/html";
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentLength64 = buffer.Length;
                
                // Envoi de la réponse de manière asynchrone
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                ServeNotFound(context); // Gérer le cas où le fichier n'existe pas
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors du rendu du fichier HTML : " + ex.Message);
            ServeNotFound(context); // Gérer l'erreur
        }
        finally
        {
            response.OutputStream.Close(); // Assurez-vous de fermer le stream à la fin
        }
    }

}
