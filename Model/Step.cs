using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottLane.SurgeonV2.Model
{
    public class Step
    {
        public Operation Operation { get; set; }

        public DataSource Source { get; set; }

        public DataSource Destination { get; set; }

        public List<FieldMapping> FieldMappings { get; set; }

        public Step()
        {
            FieldMappings = new List<FieldMapping>();
        }
    }
}
