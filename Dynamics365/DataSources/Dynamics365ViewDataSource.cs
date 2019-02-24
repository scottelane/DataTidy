using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using ScottLane.DataTidy.Core;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Retrieves records from a Dynamics 365 system or personal view.
    /// </summary>
    [DataSource(typeof(Dynamics365ViewDataSource), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365ViewDataSource.png", typeof(Dynamics365Connection))]
    public class Dynamics365ViewDataSource : Dynamics365FetchXmlDataSourceBase, IDynamics365ViewsProvider, IUrlAddressable, ISampleDataProvider, IRecordCountProvider
    {
        private const ViewType DEFAULT_VIEW_TYPE = ViewType.System;

        #region Properties

        private Dynamics365Entity entity;

        /// <summary>
        /// Gets or sets the entity to retrieve system or personal views from.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365ViewDataSource), nameof(Entity)), GlobalisedDisplayName(typeof(Dynamics365ViewDataSource), nameof(Entity)), GlobalisedDecription(typeof(Dynamics365ViewDataSource), nameof(Entity)), TypeConverter(typeof(Dynamics365EntityConverter)), JsonProperty(Order = 1)]
        public Dynamics365Entity Entity
        {
            get { return entity; }
            set
            {
                if (entity != value)
                {
                    entity = value;
                    View = default(Dynamics365View);
                    OnPropertyChanged(nameof(Entity));
                }
            }
        }

        private ViewType viewType = DEFAULT_VIEW_TYPE;

        /// <summary>
        /// Gets or sets a value indicating whether data will be retrieved from a system or personal view.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365ViewDataSource), nameof(ViewType)), GlobalisedDisplayName(typeof(Dynamics365ViewDataSource), nameof(ViewType)), GlobalisedDecription(typeof(Dynamics365ViewDataSource), nameof(ViewType)), DefaultValue(DEFAULT_VIEW_TYPE), JsonProperty(Order = 2)]
        public ViewType ViewType
        {
            get { return viewType; }
            set
            {
                if (viewType != value)
                {
                    viewType = value;
                    View = default(Dynamics365View);
                    OnPropertyChanged(nameof(ViewType));
                }
            }
        }

        private Dynamics365View view;

        /// <summary>
        /// Gets or sets the view to retrive data from.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365ViewDataSource), nameof(View)), GlobalisedDisplayName(typeof(Dynamics365ViewDataSource), nameof(View)), GlobalisedDecription(typeof(Dynamics365ViewDataSource), nameof(View)), TypeConverter(typeof(Dynamics365ViewConverter)), JsonProperty(Order = 3)]
        public Dynamics365View View
        {
            get { return view; }
            set
            {
                if (view != value)
                {
                    view = value;
                    OnPropertyChanged(nameof(View));
                }
            }
        }

        [Browsable(false), JsonIgnore]
        public Uri Url
        {
            get { return new Uri(string.Format("{0}main.aspx?etn={1}&pagetype=entitylist&viewid=%7b{2}%7d&viewtype={3}", ((Dynamics365Connection)Parent).ServerUrl.ToString(), Entity.LogicalName, View.ID, ViewType == ViewType.System ? 1039 : 4230)); }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Dynamics365ViewDataSource class with the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Dynamics365ViewDataSource(IConnection connection) : base(connection)
        { }

        /// <summary>
        /// Generates a friendly name for the data source.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format("{0}", View?.DisplayName ?? Properties.Resources.Dynamics365ViewDataSourceFriendlyNameView);
        }

        /// <summary>
        /// Counts the number of records in the data source.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The record count.</returns>
        public int GetRecordCount(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            return GetRecordCount(View.FetchXml, cancel, progress);
        }

        /// <summary>
        /// Gets the columns in the data source as a DataColumnCollection.
        /// </summary>
        /// <returns>The columns.</returns>
        public override DataColumnCollection GetDataColumns()
        {
            string preparedXml = PrepareFetchXmlQuery(View.FetchXml);
            List<Dynamics365Field> fields = GetFieldsFromFetchXml(preparedXml);
            DataTable table = CreateEmptyDataTable(fields);
            return table.Columns;
        }

        /// <summary>
        /// Gets the data source records as a DataTable.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The record count.</returns>
        public override DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            return GetFetchXmlData(View.FetchXml, cancel, progress);
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
            return GetFetchXmlData(View.FetchXml, cancel, progress, recordLimit);
        }

        public DataTable GetSampleData(CancellationToken cancel, IProgress<ExecutionProgress> progress, int recordLimit)
        {
            return GetDataTable(cancel, progress, recordLimit);
        }

        /// <summary>
        /// Validates the data source.
        /// </summary>
        /// <returns>The validation result.</returns>
        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();
            try
            {
                result.AddErrorIf(Entity == default(Dynamics365Entity), Properties.Resources.Dynamics365ViewDataSourceValidateEntity, nameof(Entity));
                result.AddErrorIf(View == default(Dynamics365View), Properties.Resources.Dynamics365ViewDataSourceValidateView, nameof(View));
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets a list of Dynamics 365 Views
        /// </summary>
        /// <returns></returns>
        public List<Dynamics365View> GetDynamics365Views()
        {
            List<Dynamics365View> views = new List<Dynamics365View>();

            try
            {
                if (Entity != default(Dynamics365Entity) && Parent != default(Dynamics365Connection))
                {
                    views.AddRange(Dynamics365View.GetViews(Entity, ViewType, (Dynamics365Connection)Parent));
                }
            }
            catch { }

            return views;
        }
    }
}
