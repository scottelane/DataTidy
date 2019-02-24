using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;

namespace ScottLane.SurgeonV2.Model
{
    [Serializable]
    public class FieldMapping
    {
        public string Destination { get; set; }
        public string Source { get; set; }       
        public object Value { get; set; }
        public ConversionType ConversionType { get; set; }

        public FieldMapping()
        {
            ConversionType = ConversionType.SourceValue;
        }
    }
}
