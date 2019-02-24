using System.Collections.Generic;

namespace ScottLane.DataTidy.Core
{
    public interface IConnectionsProvider
    {
        /// <summary>
        /// Gets a list of Connection objects.
        /// </summary>
        /// <returns>The list of Connection objects.</returns>
        List<IConnection> GetConnections();
    }
}
