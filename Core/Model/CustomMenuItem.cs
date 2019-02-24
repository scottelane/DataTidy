using System;
using System.Drawing;
using System.Threading;

namespace ScottLane.DataTidy.Core
{
    public class CustomMenuItem
    {
        public string Text { get; set; }
        public string ToolTip { get; set; }
        public Image Icon { get; set; }
        public Object Item { get; set; }
        public bool Enabled { get; set; } = true;
        public OperationSynchronousEventHandler SynchronousEventHandler;
        public OperationAsynchronousEventHandler AsynchronousEventHandler;
    }

    public delegate void OperationSynchronousEventHandler();
    public delegate void OperationAsynchronousEventHandler(CancellationToken cancel, IProgress<ExecutionProgress> progress);
}
