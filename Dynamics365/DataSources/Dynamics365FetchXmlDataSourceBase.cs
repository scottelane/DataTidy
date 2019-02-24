using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Base class for all FetchXML-based data sources.
    /// </summary>
    public abstract class Dynamics365FetchXmlDataSourceBase : Dynamics365DataSource
    {
        /// <summary>
        /// Initialises a new instance of the Dynamics365FetchXmlDataSourceBase class with the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Dynamics365FetchXmlDataSourceBase(IConnection connection) : base(connection)
        { }

        /// <summary>
        /// Counts the number of records in the data source.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The record count.</returns>
        public int GetRecordCount(string fetchXml, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            int recordCount = 0;
            int pageNumber = 1;
            string pagingCookie = null;

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, recordCount, recordCount));

            FetchExpression expression = CreatePagedFetchExpression(fetchXml, pagingCookie, pageNumber);

            using (OrganizationServiceProxy proxy = ((Dynamics365Connection)Parent).OrganizationServiceProxy)
            {
                EntityCollection entityRecords = proxy.RetrieveMultiple(expression);
                recordCount += entityRecords.Entities.Count;

                while (entityRecords.MoreRecords)
                {
                    pageNumber++;
                    pagingCookie = entityRecords.PagingCookie;
                    expression = CreatePagedFetchExpression(fetchXml, pagingCookie, pageNumber);
                    entityRecords = proxy.RetrieveMultiple(expression);
                    recordCount += entityRecords.Entities.Count;
                    progress?.Report(new ExecutionProgress(ExecutionStage.Extract, recordCount, recordCount));
                    cancel.ThrowIfCancellationRequested();
                }
            }

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, recordCount, recordCount));

            return recordCount;
        }

        /// <summary>
        /// Creates a FetchXML expression that supports paging from a baseline FetchXML query.
        /// </summary>
        /// <param name="fetchXml">The query.</param>
        /// <param name="pagingCookie">The paging cookie.</param>
        /// <param name="pageNumber">The page number to retrive.</param>
        /// <returns>The FetchExpression.</returns>
        private FetchExpression CreatePagedFetchExpression(string fetchXml, string pagingCookie, int pageNumber)
        {
            StringReader stringReader = new StringReader(fetchXml);
            XmlTextReader textReader = new XmlTextReader(stringReader);
            XmlDocument document = new XmlDocument();
            document.Load(textReader);

            XmlAttributeCollection attributes = document.DocumentElement.Attributes;

            if (pagingCookie != null)
            {
                XmlAttribute pagingAttribute = document.CreateAttribute("paging-cookie");
                pagingAttribute.Value = pagingCookie;
                attributes.Append(pagingAttribute);
            }

            XmlAttribute pageAttribute = document.CreateAttribute("page");
            pageAttribute.Value = pageNumber.ToString();
            attributes.Append(pageAttribute);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            document.WriteTo(writer);
            writer.Close();

            string pagedFetchXml = sb.ToString();
            FetchExpression expression = new FetchExpression(pagedFetchXml);

            return expression;
        }


        /// <summary>
        /// Prepares a Fetch XML query for processing by adding and removing fields based on user preferences.
        /// </summary>
        /// <param name="fetchXml">The unprepared query.</param>
        /// <returns>The prepared query.</returns>
        protected string PrepareFetchXmlQuery(string fetchXml)
        {
            string preparedXml = HttpUtility.HtmlDecode(fetchXml);
            StringReader stringReader = new StringReader(preparedXml);
            XmlTextReader textReader = new XmlTextReader(stringReader);
            XmlDocument document = new XmlDocument();
            document.Load(textReader);

            XmlNode entityNode = document.DocumentElement.ChildNodes[0];

            // todo - put any tweaks here

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            document.WriteTo(writer);
            writer.Close();

            preparedXml = sb.ToString();
            return preparedXml;
        }

        /// <summary>
        /// Gets a data table from a FetchXML query.
        /// </summary>
        /// <param name="fetchXml">The query.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The data table.</returns>
        public DataTable GetFetchXmlData(string fetchXml, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            return GetFetchXmlData(fetchXml, cancel, progress, int.MaxValue);
        }

        /// <summary>
        /// Gets a data table containing a limited number of records from a FetchXML query.
        /// </summary>
        /// <param name="fetchXml">The query.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="recordLimit">The record limit.</param>
        /// <returns>The data table.</returns>
        public DataTable GetFetchXmlData(string fetchXml, CancellationToken cancel, IProgress<ExecutionProgress> progress, int recordLimit)
        {
            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, 0, 0));
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365FetchXmlDataSourceBaseExtract, Name)));

            int pageNumber = 1;
            string pagingCookie = null;

            FetchExpression expression = CreatePagedFetchExpression(fetchXml, pagingCookie, pageNumber);
            List<Dynamics365Field> fields = GetFieldsFromFetchXml(fetchXml);
            DataTable dataTable;

            using (OrganizationServiceProxy proxy = ((Dynamics365Connection)Parent).OrganizationServiceProxy)
            {
                EntityCollection entityRecords = proxy.RetrieveMultiple(expression);
                dataTable = CreateEmptyDataTable(fields);
                AppendRows(dataTable, entityRecords, recordLimit);

                progress?.Report(new ExecutionProgress(ExecutionStage.Extract, dataTable.Rows.Count, dataTable.Rows.Count));

                while (entityRecords.MoreRecords)
                {
                    pageNumber++;
                    pagingCookie = entityRecords.PagingCookie;
                    expression = CreatePagedFetchExpression(fetchXml, pagingCookie, pageNumber);
                    entityRecords = proxy.RetrieveMultiple(expression);
                    AppendRows(dataTable, entityRecords, recordLimit);
                    progress?.Report(new ExecutionProgress(ExecutionStage.Extract, dataTable.Rows.Count, dataTable.Rows.Count));
                    cancel.ThrowIfCancellationRequested();
                }
            }

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, dataTable.Rows.Count, dataTable.Rows.Count));

            return dataTable;
        }

        /// <summary>
        /// Gets a list of Dynamics 365 fields present in the FetchXML query.
        /// </summary>
        /// <param name="fetchXml">The query.</param>
        /// <returns>The list of fields.</returns>
        protected List<Dynamics365Field> GetFieldsFromFetchXml(string fetchXml)
        {
            string xml = HttpUtility.HtmlDecode(fetchXml);
            StringReader stringReader = new StringReader(xml);
            XmlTextReader textReader = new XmlTextReader(stringReader);
            XmlDocument document = new XmlDocument();
            document.Load(textReader);

            XmlNode entityNode = document.DocumentElement.ChildNodes[0];
            return GetEntityNodeFields(entityNode);
        }

        /// <summary>
        /// Gets a list of fields in an entity or link-entity node of a FetchXML query.
        /// </summary>
        /// <param name="entityNode">The entity node.</param>
        /// <returns>The fields.</returns>
        private List<Dynamics365Field> GetEntityNodeFields(XmlNode entityNode)
        {
            List<Dynamics365Field> fields = new List<Dynamics365Field>();
            Dynamics365Entity entity = Dynamics365Entity.Create(entityNode.Attributes["name"].Value, (Dynamics365Connection)Parent);
            EntityMetadata entityMetadata = entity.GetEntityMetadata((Dynamics365Connection)Parent);

            foreach (XmlNode node in entityNode.ChildNodes)
            {
                if (node.Name.ToLower() == "attribute")
                {
                    string attributeName = node.Attributes["name"].Value;
                    AttributeMetadata attributeMetadata = entityMetadata.Attributes.FirstOrDefault(findField => findField.LogicalName == attributeName);

                    if (attributeMetadata != default(AttributeMetadata))
                    {
                        fields.Add(Dynamics365Field.CreateFromMetadata(attributeMetadata, (Dynamics365Connection)Parent));
                    }
                    else
                    {
                        throw new ApplicationException(string.Format("The {0} field in {1} does not exist in Dynamics 365", attributeName, Name));
                    }
                }

                if (node.Name.ToLower() == "link-entity")
                {
                    List<Dynamics365Field> linkedFields = GetEntityNodeFields(node);

                    foreach (Dynamics365Field linkedField in linkedFields)
                    {
                        fields.Add(linkedField);
                    }
                }
            }

            return fields;
        }
    }
}
