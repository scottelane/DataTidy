using System;
using System.ComponentModel;
using System.Xml.Serialization;
using ScottLane.SurgeonV2.Model.DynamicsCrm.Operations;

namespace ScottLane.SurgeonV2.Model
{
    [Serializable]
    [XmlInclude(typeof(DynamicsCrmCreateOperation))]
    public abstract class Operation
    {
        [Category("General")]
        public string Name { get; set; }

        [Category("General")]
        public Connection Connection { get; set; }

        [Category("General"), DisplayName("Data Source")]
        public DataSource DataSource { get; set; }

        [Browsable(false)]
        public Batch Batch { get; set; }

        public Operation()
        {
        }

        public virtual bool Validate()
        {
            return true;
        }

        public abstract void Execute();

        //public abstract void GenerateDefaultName();
    }
}
