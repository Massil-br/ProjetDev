


using System.Threading.Tasks;

class Program{

    public static async Task Main(){
        var database = Database.GetInstance();
        HttpServer httpsServer = new(database);
        await httpsServer.StartAsync();

    }


}