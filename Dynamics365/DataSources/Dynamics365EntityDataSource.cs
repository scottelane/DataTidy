using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Retrieves records from a Dynamics 365 entity.
    /// </summary>
    /// <remarks>
    /// See https://dreamingincrm.com/2015/05/21/retrievemultiple-performance-on-large-datasets/ for perf stats
    /// </remarks>
    [DataSource("Entity", "Retrieves records from a Dynamics 365 entity", "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365EntityDataSource.png", typeof(Dynamics365Connection))]
    public class Dynamics365EntityDataSource : Dynamics365DataSource, IDynamics365EntityFieldsProvider, IUrlAddressable, ISampleDataProvider, IRecordCountProvider
    {
        private const int MAXIMUM_PAGE_SIZE = 5000;

        #region Properties

        private Dynamics365Entity entity;

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        [GlobalisedCategory("Entity"), GlobalisedDisplayName("Entity"), GlobalisedDecription("The entity to retrieve data from."), TypeConverter(typeof(Dynamics365EntityConverter)), Browsable(true)]
        public Dynamics365Entity Entity
        {
            get { return entity; }
            set
            {
                if (entity != value)
                {
                    entity = value;
                    OnPropertyChanged(nameof(Entity));
                    RefreshName();
                }
            }
        }

        private BindingList<Dynamics365Field> fields = new BindingList<Dynamics365Field>();

        /// <summary>
        /// Gets or sets the fields to retrieve.
        /// </summary>
        [GlobalisedCategory("Entity"), GlobalisedDisplayName("Fields"), GlobalisedDecription("The fields to retrieve from the entity."), TypeConverter(typeof(Dynamics365EntityFieldConverter)), Editor(typeof(Dynamics365FieldCheckedListBoxEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(true)]
        public BindingList<Dynamics365Field> Fields
        {
            get { return fields; }
            set
            {
                if (fields != value)
                {
                    fields = value;

                    if (fields != null)
                    {
                        fields.ListChanged += Fields_ListChanged;
                    }

                    OnPropertyChanged(nameof(Fields));
                }
            }
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/gg328483.aspx
        /// https://community.dynamics.com/crm/b/powerxrmblog/archive/2016/05/17/addressing-forms-and-views-by-url
        /// </summary>
        [Browsable(false), JsonIgnore, ReadOnly(true)]
        public Uri Url
        {
            get { return new Uri(string.Format("{0}main.aspx?etn={1}&pagetype=entitylist", ((Dynamics365Connection)Parent).ServerUrl.ToString(), Entity.LogicalName)); }
        }

        /// <summary>
        /// Gets or sets the object type code.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("Object Type Code"), GlobalisedDecription("The entity object type code."), Browsable(true), ReadOnly(true)]
        public int? ObjectTypeCode
        {
            get { return Entity?.ObjectTypeCode; }
        }

        private void Fields_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Fields));
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Dynamics365EntityDataSource class with the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Dynamics365EntityDataSource(IConnection connection) : base(connection)
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
                result.AddErrorIf(Entity == default(Dynamics365Entity), "Please select an Entity", nameof(Entity));
                result.AddErrorIf(Fields.Count == 0, "Please select at least one Field", nameof(Fields));
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
            return string.Format("{0} Entity", Entity?.DisplayName ?? "<Entity>");
        }

        /// <summary>
        /// Counts the number of records in the data source.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The record count.</returns>
        public int GetRecordCount(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            int recordCount = 0;
            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, recordCount, recordCount));

            QueryExpression query = new QueryExpression(Entity.LogicalName)
            {
                ColumnSet = new ColumnSet(Entity.PrimaryIdFieldName),
                PageInfo = new PagingInfo() { PageNumber = 1, ReturnTotalRecordCount = false }
            };

            using (OrganizationServiceProxy proxy = ((Dynamics365Connection)Parent).OrganizationServiceProxy)
            {
                EntityCollection records = proxy.RetrieveMultiple(query);
                recordCount += records.Entities.Count;

                while (records.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = records.PagingCookie;
                    records = proxy.RetrieveMultiple(query);
                    recordCount += records.Entities.Count;
                    progress?.Report(new ExecutionProgress(ExecutionStage.Extract, recordCount, recordCount));
                    cancel.ThrowIfCancellationRequested();
                }
            }

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, recordCount, recordCount));
            return recordCount;
        }

        /// <summary>
        /// Gets the data source records as a DataTable.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The record count.</returns>
        public override DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            return GetDataTable(cancel, progress, int.MaxValue);
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
            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, 0, 0));
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365EntityDataSourceExtract, Name)));

            QueryExpression query = new QueryExpression(Entity.LogicalName)
            {
                ColumnSet = new ColumnSet(Fields.Select(f => f.LogicalName).ToArray())
            };
            if (recordLimit == int.MaxValue)
            {
                query.PageInfo = new PagingInfo() { PageNumber = 1, ReturnTotalRecordCount = false, Count = MAXIMUM_PAGE_SIZE };
            }
            else
            {
                query.TopCount = recordLimit;
            }

            DataTable dataTable;

            using (OrganizationServiceProxy proxy = ((Dynamics365Connection)Parent).OrganizationServiceProxy)
            {
                EntityCollection entities = proxy.RetrieveMultiple(query);
                dataTable = CreateEmptyDataTable(Fields.ToList());
                AppendRows(dataTable, entities, recordLimit);
                progress?.Report(new ExecutionProgress(ExecutionStage.Extract, dataTable.Rows.Count, dataTable.Rows.Count));

                while (entities.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = entities.PagingCookie;
                    entities = proxy.RetrieveMultiple(query);
                    AppendRows(dataTable, entities, recordLimit);
                    progress?.Report(new ExecutionProgress(ExecutionStage.Extract, dataTable.Rows.Count, dataTable.Rows.Count));
                    cancel.ThrowIfCancellationRequested();
                }
            }

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, dataTable.Rows.Count, dataTable.Rows.Count));

            return dataTable;
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
            DataTable table = CreateEmptyDataTable(Fields.ToList());
            return table.Columns;
        }

        public List<Dynamics365Field> GetDynamics365EntityFields()
        {
            List<Dynamics365Field> fields = new List<Dynamics365Field>();
            try
            {
                fields.AddRange(Dynamics365Field.GetFields(Entity, (Dynamics365Connection)Parent)); // todo - should call entity.getfields??
            }
            catch { }

            return fields;
        }
    }
}
