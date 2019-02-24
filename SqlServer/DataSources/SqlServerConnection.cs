using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Threading;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.SqlServer
{
    /// <summary>
    /// Provides connectivity to Microsoft SQL Server.
    /// </summary>
    [Connection("SQL Server", "Provides connectivity to Microsoft SQL Server", "ScottLane.DataTidy.SqlServer.Resources.SqlServerConnection.png")]
    public class SqlServerConnection : Connection, IAvailableProvider
    {
        #region Properties

        private SqlConnection connection;

        private string connectionString;

        /// <summary>
        /// Gets or sets the encrypted connection string.
        /// </summary>
        [GlobalisedCategory("SQL Server"), GlobalisedDisplayName("Connection String"), GlobalisedDecription("See https://www.connectionstrings.com/sql-server/ for examples."), TypeConverter(typeof(EncryptedStringConverter))]
        public string ConnectionString
        {
            get { return connectionString; }
            set
            {
                if (connectionString != value)
                {
                    connectionString = value;
                    OnPropertyChanged(nameof(ConnectionString));
                }
            }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the SqlServerConnection class.
        /// </summary>
        public SqlServerConnection(Project parent) : base(parent)
        { }

        /// <summary>
        /// Opens a SQL Server connection.
        /// </summary>
        /// <returns>A SqlConnection.</returns>
        public SqlConnection Open()
        {
            connection = new SqlConnection(new AESEncrypter().Decrypt(ConnectionString));
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Validates the connection settings.
        /// </summary>
        /// <returns>The validation result.</returns>
        public override ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            try
            {
                result.AddErrorIf(ConnectionString == default(string), "The Connection String has not been specified");
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Checks whether SQL Server is available.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The connectivity result.</returns>
        public ConnectivityResult IsAvailable(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            ConnectivityResult result = new ConnectivityResult(false);

            try
            {
                using (SqlConnection connection = new SqlConnection(new AESEncrypter().Decrypt(ConnectionString)))
                {
                    connection.Open();
                }

                result.IsAvailable = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }
}
