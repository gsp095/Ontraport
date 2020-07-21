using System.Net;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Provides information about the web environment.
    /// </summary>
    public interface IWebEnvironment
    {
        /// <summary>
        /// Returns the public IP address that is exposed to the internet.
        /// </summary>
        IPAddress PublicIpAddress { get; }
    }
}
