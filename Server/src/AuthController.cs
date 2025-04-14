using System.Formats.Asn1;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualBasic;


namespace src;

public class AuthController
{
  public static async Task RegisterTraitment(HttpListenerContext context)
    {
        if (context.Request.HttpMethod != "POST")
        {
            HttpServer.SendResponse(context, 405, "Méthode non autorisée. Utilisez POST.");
            return;
        }

        string body = HttpServer.ReadRequestBody(context.Request);
        var data = JsonSerializer.Deserialize<RegisterData>(body);

        if (data == null || string.IsNullOrEmpty(data.Username) || string.IsNullOrEmpty(data.Email) || string.IsNullOrEmpty(data.Password))
        {
            HttpServer.SendResponse(context, 400, "Champs manquants.");
            return;
        }

        if (data.Password != data.PasswordConfirm)
        {
            HttpServer.SendResponse(context, 400, "Mot de passe et confirmation de mot de passe ne sont pas les mêmes.");
            return;
        }

        var db = Database.GetInstance();
        var pass = await db.AddUserAsync(data.Username, data.Email, data.Password);
        
        if (!pass)
        {
            HttpServer.SendResponse(context, 400, "Mot de passe ne respecte pas les règles de sécurité.");
            return;
        }

        // Inscription réussie
        HttpServer.SendResponse(context, 201, "Inscription réussie !");
    }

   
}

public class RegisterData
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? PasswordConfirm { get; set; }
}
