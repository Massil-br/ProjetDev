


namespace Shared{
    public class Ip{
        public string IpAddress{get;private set;}
        public int Port{get;private set;} 


        public Ip(string ipAddress, int port = 8994)
        {
            if (IsValidIp(ipAddress) && IsValidPort(port))
            {
                IpAddress = ipAddress;
                Port = port;
            }
            else
            {
                throw new ArgumentException("Invalid IP address or port.");
            }
        }

        private bool IsValidIp(string ipAddress)
        {
            return System.Net.IPAddress.TryParse(ipAddress, out _);
        }

        private bool IsValidPort(int port)
        {
            return  port > 0 && port <= 65535;
        }


    }
}