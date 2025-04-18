


using System.Threading.Tasks;
using src;

class Program{

    public static async Task Main(){
        var database = Database.GetInstance();
        HttpServer httpsServer = new(database);
        UDPServer udpServer = new();
        Task httpTask = httpsServer.StartAsync();
        Task udpTask = Task.Run(() => udpServer.Start());
        await Task.WhenAll(httpTask, udpTask);
        
    }


}