using System;
using System.ComponentModel;
using System.Threading;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using ScottLane.DataTidy.Core;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Dynamics365
{
    [Operation(typeof(Dynamics365PublishEntityOperation), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365PublishEntityOperation.png")]
    public class Dynamics365PublishEntityOperation : Dynamics365Operation
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Dynamics 365 organisation to delete records from.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365PublishEntityOperation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365PublishEntityOperation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365PublishEntityOperation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter)), JsonProperty(Order = 1)]
        public override Dynamics365Connection Connection
        {
            get { return base.Connection; }
            set { base.Connection = value; }
        }

        private Dynamics365Entity entity;

        /// <summary>
        /// Gets or sets the entity to publish.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365PublishEntityOperation), nameof(Entity)), GlobalisedDisplayName(typeof(Dynamics365PublishEntityOperation), nameof(Entity)), GlobalisedDecription(typeof(Dynamics365PublishEntityOperation), nameof(Entity)), TypeConverter(typeof(Dynamics365EntityConverter)), JsonProperty(Order = 2)]
        public Dynamics365Entity Entity
        {
            get { return entity; }
            set
            {
                if (entity != value)
                {
                    entity = value;
                    OnPropertyChanged(nameof(Entity));
                }
            }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Dynamics365PublishEntityOperation class with the specified parent batch.
        /// </summary>
        /// <param name="parentBatch"></param>
        public Dynamics365PublishEntityOperation(Batch parentBatch) : base(parentBatch)
        { }

        /// <summary>
        /// Generates a friendly name for the operation.
        /// </summary>
        /// <returns></returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.Dynamics365PublishEntityOperationFriendlyName, Entity?.DisplayName ?? Properties.Resources.Dynamics365PublishEntityOperationFriendlyNameEntity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancel"></param>
        /// <param name="progress"></param>
        public override void Execute(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            OnExecuting(new ExecutableEventArgs(this));
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365PublishEntityOperationPublishing, Entity.DisplayName)));

            PublishXmlRequest request = new PublishXmlRequest()
            {
                ParameterXml = string.Format("<importexportxml><entities><entity>{0}</entity></entities></importexportxml>", Entity.LogicalName)
            };

            using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
            {
                proxy.Execute(request);
            }

            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365PublishEntityOperationPublishingSuccess, Entity.DisplayName)));
            OnExecuted(new ExecutableEventArgs(this));
        }
    }
}
