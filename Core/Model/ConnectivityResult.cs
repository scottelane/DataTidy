namespace ScottLane.DataTidy.Core
{
    public class ConnectivityResult
    {
        public bool IsAvailable { get; set; }
        public string ErrorMessage { get; set; }

        public ConnectivityResult()
        { }

        public ConnectivityResult(bool isAvailable) : this()
        { }

        public ConnectivityResult(bool isAvailable, string errorMessage) : this(isAvailable)
        {
            ErrorMessage = errorMessage;
        }
    }
}
