using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Design;
using System.Threading;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.SqlServer
{
    /// <summary>
    /// Provides the ability to load records from SQL Server using a T-SQL query.
    /// </summary>
    [DataSource(typeof(SqlServerQueryDataSource), "ScottLane.DataTidy.SqlServer.Resources.SqlServerQueryDataSource.png", typeof(SqlServerConnection))]
    public class SqlServerQueryDataSource : DataSource, IRecordCountProvider, ISampleDataProvider
    {
        private string query;

        /// <summary>
        /// Gets or sets the T-SQL query.
        /// </summary>
        [GlobalisedCategory("SQL Server"), GlobalisedDisplayName("Query"), GlobalisedDecription("The T-SQL query that retrieves data from SQL Server."), Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Query
        {
            get { return query; }
            set
            {
                if (query != value)
                {
                    query = value;
                    OnPropertyChanged(nameof(Query));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the SqlServerQueryDataSource class with the specified connection.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        public SqlServerQueryDataSource(SqlServerConnection connection) : base(connection)
        { }

        /// <summary>
        /// Generates a friendly name for the data source.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format("SQL Query");
        }

        /// <summary>
        /// Validates the SqlServerQueryDataSource settings.
        /// </summary>
        /// <returns>The validation result.</returns>
        public override ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            try
            {
                // result.Errors.AddRange(ParentConnection.Validate().Errors);   // todo - what to do here?
                result.AddErrorIf(query == default(string), "Query is missing");
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets a data table from the T-SQL query.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The data table.</returns>
        public override DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, 0, 0));

            DataTable dataTable = GetDataTable(query, int.MaxValue);

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, dataTable.Rows.Count, dataTable.Rows.Count));

            return dataTable;
        }

        /// <summary>
        /// Gets a data table from the T-SQL query.
        /// </summary>
        /// <param name="query">The T-SQL query.</param>
        /// <returns>The data table.</returns>
        private DataTable GetDataTable(string query, int recordLimit)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection sqlConnection = ((SqlServerConnection)Parent).Open())
            {
                string topQuery = recordLimit == int.MaxValue ? query : string.Format("SELECT TOP {0} * FROM ({1}) subquery", recordLimit, Query);

                using (SqlCommand command = new SqlCommand(topQuery, sqlConnection))
                {
                    SqlDataReader reader = command.ExecuteReader(); // support command.cancel?
                    dataTable.Load(reader);
                    reader.Close();
                }
            }

            return dataTable;
        }

        public DataTable GetSampleData(CancellationToken cancel, IProgress<ExecutionProgress> progress, int recordLimit)
        {
            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, 0, 0));

            DataTable dataTable = GetDataTable(query, recordLimit);

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, dataTable.Rows.Count, dataTable.Rows.Count));

            return dataTable;
        }

        /// <summary>
        /// Gets the query record count.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The record count.</returns>
        public int GetRecordCount(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            // todo - add alternative count method using extraction to cater for awry queries

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, 0, 0));

            string recordCountQuery = string.Format("SELECT COUNT(1) FROM ({0}) subquery", query);
            int recordCount = int.Parse(GetDataTable(recordCountQuery, int.MaxValue).Rows[0][0].ToString());

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, recordCount, recordCount));

            return recordCount;
        }

        /// <summary>
        /// Gets the columns returned by the T-SQL query.
        /// </summary>
        /// <returns>The columns.</returns>
        public override DataColumnCollection GetDataColumns()
        {
            // todo - could just be a local variable?
            ConnectionCache cache = new ConnectionCache(Parent);
            string cacheKey = string.Format("GetDataColumns:{0}", ID);
            DataColumnCollection columns = (DataColumnCollection)cache[cacheKey];

            if (columns == default(DataColumnCollection))
            {
                columns = GetDataTable(Query, 1).Columns;
                cache[cacheKey] = columns;
            }

            return columns;
        }
    }
}
