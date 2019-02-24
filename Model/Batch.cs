using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ScottLane.SurgeonV2.Model
{
    [Serializable]
    public class Batch
    {
        public string Name { get; set; }

        [Browsable(false)]
        public List<Operation> Steps { get; set; }

        [Browsable(false)]
        public Project Project { get; set; }

        public Batch()
        {
            Steps = new List<Operation>();
        }

        public void Run()
        {
            foreach (Operation step in Steps)
            {
                step.Execute();
            }
        }
    }
}
