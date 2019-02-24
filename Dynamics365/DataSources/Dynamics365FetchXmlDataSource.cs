using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing.Design;
using System.Threading;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Retrieves records form a Dynamics 365 FetchXML query.
    /// </summary>
    [DataSource("Fetch XML", "Retrieves records form a Dynamics 365 FetchXML query", "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365FetchXmlDataSource.png", typeof(Dynamics365Connection))]
    public class Dynamics365FetchXmlDataSource : Dynamics365FetchXmlDataSourceBase, IRecordCountProvider, ISampleDataProvider
    {
        /// <summary>
        /// Gets or sets the FetchXML query.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("FetchXML"), GlobalisedDecription("The FetchXML query."), Browsable(true), Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string FetchXml { get; set; }

        /// <summary>
        /// Initialises a new instance of the Dynamics365FetchXmlDataSource class with the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Dynamics365FetchXmlDataSource(IConnection connection) : base(connection)
        { }

        /// <summary>
        /// Validates the data source.
        /// </summary>
        /// <returns>The validation result.</returns>
        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();
            try
            {
                result.AddErrorIf(FetchXml == default(string), "Please enter the FetchXML", nameof(FetchXml));
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Generates a friendly name for the data source.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format("FetchXML Query");
        }

        /// <summary>
        /// Counts the number of records in the data source.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The record count.</returns>
        public int GetRecordCount(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            string preparedFetchXml = PrepareFetchXmlQuery(FetchXml);
            return GetRecordCount(preparedFetchXml, cancel, progress);
        }

        /// <summary>
        /// Gets the data source records as a DataTable.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The record count.</returns>
        public override DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            string preparedFetchXml = PrepareFetchXmlQuery(FetchXml);
            return GetFetchXmlData(preparedFetchXml, cancel, progress);
        }

        /// <summary>
        /// Gets a limited number of data source records as a DataTable.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="recordLimit">The number of records to return.</param>
        /// <returns>The data table.</returns>
        private DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress, int recordLimit)
        {
            string preparedFetchXml = PrepareFetchXmlQuery(FetchXml);
            return GetFetchXmlData(preparedFetchXml, cancel, progress, recordLimit);
        }

        public DataTable GetSampleData(CancellationToken cancel, IProgress<ExecutionProgress> progress, int recordLimit)
        {
            return GetDataTable(cancel, progress, recordLimit);
        }

        /// <summary>
        /// Gets the columns in the data source as a DataColumnCollection.
        /// </summary>
        /// <returns>The columns.</returns>
        public override DataColumnCollection GetDataColumns()
        {
            string preparedFetchXml = PrepareFetchXmlQuery(FetchXml);
            List<Dynamics365Field> fields = GetFieldsFromFetchXml(preparedFetchXml);
            DataTable table = CreateEmptyDataTable(fields);
            return table.Columns;
        }
    }
}
