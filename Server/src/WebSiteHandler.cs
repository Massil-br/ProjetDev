using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace src
{
    public class WebSiteHandler
    {
        public static async Task ServeIndexHtml(HttpListenerContext context)
        {
            var replacements = new Dictionary<string, string>
            {
                { "titre", "Page d'accueil" },
                { "content", "Bienvenue sur notre site web !" }
            };

            string templatePath = "src/templates/index.html";
            await HttpServer.RenderTemplateAsync(context, templatePath, replacements); // Appel à la version asynchrone de RenderTemplate
        }
    }
}
