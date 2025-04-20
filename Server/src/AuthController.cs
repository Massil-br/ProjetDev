using System.Data.Common;
using System.Formats.Asn1;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Shared;


namespace src;


public enum LoginResultStatus
{
    Success,
    WrongPassword,
    UserNotFound
}


public class LoginResult
{
    public LoginResultStatus Status { get; set; }
    public User? User { get; set; }

    public LoginResult(LoginResultStatus status, User? user = null)
    {
        Status = status;
        User = user;
    }
}


public class RegisterData
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? PasswordConfirm { get; set; }
}




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

        if (data == null || string.IsNullOrEmpty(data.Username) || string.IsNullOrEmpty(data.Email) 
            || string.IsNullOrEmpty(data.Password) || string.IsNullOrEmpty(data.PasswordConfirm))
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



   public static async Task LoginTraitment(HttpListenerContext context){
    if (context.Request.HttpMethod != "POST")
    {
        HttpServer.SendResponse(context, 405, "Méthode non autorisée. Utilisez POST.");
        return;
    }

    string body = HttpServer.ReadRequestBody(context.Request);
    var data = JsonSerializer.Deserialize<RegisterData>(body);

    if (data == null || string.IsNullOrEmpty(data.Email) || string.IsNullOrEmpty(data.Password))
    {
        HttpServer.SendResponse(context, 400, "Champs manquants.");
        return;
    }

    var db = Database.GetInstance();
    var result = await db.GetUserAsync(data.Email, data.Password);

    switch (result.Status)
    {
        case LoginResultStatus.Success:
            HttpServer.SendResponse(context, 200, $"Bienvenue {result.User!.Username} !");

            var token = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddHours(1);

            await db.CreateSessionAsync(result.User!.ID, token, expiresAt);
            HttpServer.SendResponse(context, 200, JsonSerializer.Serialize(new {
                message = $"Bienvenue {result.User.Username} !",
                token
            }));
            break;
        case LoginResultStatus.WrongPassword:
            HttpServer.SendResponse(context, 401, "Mot de passe incorrect.");
            break;
        case LoginResultStatus.UserNotFound:
            HttpServer.SendResponse(context, 404, "Aucun compte trouvé avec cet email.");
            break;
        }
    }  
}

