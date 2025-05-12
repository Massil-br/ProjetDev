


using System.Threading.Tasks;
using src;

class Program
{
    public static async Task Main(string[] args)
    {
        // Démarrer le serveur UDP dans une tâche séparée
        UDPServer udpServer = new();
        Task udpTask = Task.Run(() => udpServer.Start());

        // Créer et démarrer l'API server
        var apiServer = ApiServer.CreateApiServer(args);
        Task apiTask = Task.Run(() => apiServer.Run());

        // Attendez que les deux serveurs se terminent (ce qui ne se produira normalement jamais si les serveurs sont en cours d'exécution)
        await Task.WhenAll(udpTask, apiTask);
    }
}