using System.Net;
using System.Net.Sockets;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Provides information about the web environment.
    /// </summary>
    public class WebEnvironment : IWebEnvironment
    {
        /// <summary>
        /// Returns the public IP address that is exposed to the internet.
        /// </summary>
        public IPAddress PublicIpAddress
        {
            get
            {
                // This method of getting public IP address is described here
                // https://stackoverflow.com/a/27376368/3960200

                IPAddress? localIP = null;
                try
                {

                    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                    {
                        socket.Connect("8.8.8.8", 65530);
                        var endPoint = socket.LocalEndPoint as IPEndPoint;
                        localIP = endPoint?.Address;
                    }
                }
                catch (SocketException) { }
                return localIP ?? IPAddress.Loopback;
            }
        }
    }
}
