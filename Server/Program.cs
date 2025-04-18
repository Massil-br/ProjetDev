


using System.Threading.Tasks;
using src;

class Program{

    public static async Task Main(){
        var database = Database.GetInstance();
        HttpServer httpsServer = new();
        UDPServer udpServer = new();
        Task httpTask = httpsServer.StartAsync();
        Task udpTask = Task.Run(() => udpServer.Start());
        await Task.WhenAll(httpTask, udpTask);
        
    }


}