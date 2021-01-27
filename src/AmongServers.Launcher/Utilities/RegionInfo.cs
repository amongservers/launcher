using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmongServers.Launcher.Utilities
{
    /// <summary>
    /// Represents region info which is a name and a list of master servers.
    /// </summary>
    public class RegionInfo
    {
        private List<RegionServer> _servers = new List<RegionServer>();

        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ping endpoint.
        /// </summary>
        public IPEndPoint PingEndpoint { get; set; }

        /// <summary>
        /// The servers.
        /// </summary>
        public IList<RegionServer> Servers => _servers;

        /// <summary>
        /// Loads the region info from the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The region info.</returns>
        public static async ValueTask<RegionInfo> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            //TODO: this is not needed at the moment
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the region info from the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The region info.</returns>
        public static async ValueTask<RegionInfo> LoadAsync(string path, CancellationToken cancellationToken = default)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                return await LoadAsync(fs, cancellationToken);
            }
        }

        /// <summary>
        /// Saves the region info to the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public async ValueTask SaveAsync(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream()) {
                // write the server data
                BinaryWriter writer = new BinaryWriter(ms);

                // write header
                writer.Write(0);
                writer.Write(Name);
                writer.Write(PingEndpoint == null ? _servers.Count == 0 ? "" : _servers.First().Endpoint.ToString() : PingEndpoint.ToString());
                writer.Write(_servers.Count);

                foreach(var server in _servers) {
                    writer.Write(Name);
                    writer.Write(server.Endpoint.Address.GetAddressBytes());
                    writer.Write((ushort)server.Endpoint.Port);
                    writer.Write(0);
                }

                // write the memory stream to the stream
                await stream.WriteAsync(ms.GetBuffer().AsMemory(0, (int)ms.Length));
            }
        }

        /// <summary>
        /// Saves the region info to the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The region info.</returns>
        public async ValueTask SaveAsync(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite)) {
                await SaveAsync(fs);
            }
        }

        /// <summary>
        /// Create an empty region info.
        /// </summary>
        public RegionInfo()
        {
        }
    }
}
