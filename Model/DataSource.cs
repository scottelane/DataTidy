using System;
using System.Data;

namespace ScottLane.SurgeonV2.Model
{
    [Serializable]
    public abstract class DataSource
    {
        public string Name { get; set; }

        public abstract DataTable GetDataTable(Connection connection);

        public abstract DataTable GetSampleData(Connection connection, int recordCount);

        public override string ToString()
        {
            return Name;
        }
    }
}
