using System;
using System.ComponentModel;
using System.Threading;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Publishes all customisations in a Dynamics 365 instance.
    /// </summary>
    [Operation(typeof(Dynamics365PublishAllOperation), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365PublishAllOperation.png")]
    public class Dynamics365PublishAllOperation : Dynamics365Operation, IExecutable
    {
        /// <summary>
        /// Gets or sets the Dynamics 365 organisation to publish all customisations in.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365PublishAllOperation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365PublishAllOperation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365PublishAllOperation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter))]
        public override Dynamics365Connection Connection
        {
            get { return connection; }
            set
            {
                if (connection != value)
                {
                    connection = value;
                    OnPropertyChanged(nameof(Connection));
                    RefreshName();
                }
            }
        }

        /// <summary>
        /// Initialises a instance of the Dynamics365PublishAllOperation class with the specified parent batch.
        /// </summary>
        /// <param name="parentBatch">The parent batch.</param>
        public Dynamics365PublishAllOperation(Batch parentBatch) : base(parentBatch)
        { }

        /// <summary>
        /// Generates a friendly name for the operation.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.Dynamics365PublishAllOperationFriendlyName, Connection?.Name ?? Properties.Resources.Dynamics365PublishAllOperationFriendlyNameConnection);
        }

        /// <summary>
        /// Executes the operation.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        public override void Execute(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            OnExecuting(new ExecutableEventArgs(this));
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365PublishAllOperationExecute, Connection.Name)));

            PublishAllXmlRequest request = new PublishAllXmlRequest();

            using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
            {
                proxy.Execute(request);
            }

            //progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365PublishAllOperationExecuteSuccessful, Connection.Name)));
            OnExecuted(new ExecutableEventArgs(this));
        }
    }
}
