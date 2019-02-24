using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ScottLane.SurgeonV2.Model
{
    [Serializable]
    public abstract class Connection
    {
        [CategoryAttribute("General")]
        public string Name { get; set; }

        public List<DataSource> DataSources { get; set; }

        public abstract object Open();

        public abstract void Close();

        public abstract bool CheckIfAvailable();

        public Connection()
        {
            DataSources = new List<DataSource>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
