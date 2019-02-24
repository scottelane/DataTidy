using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Provides a base implementation for Dynamics 365 operations.
    /// </summary>
    public abstract class Dynamics365Operation : Operation, IConnectionsProvider, IDynamics365EntitiesProvider
    {
        protected Dynamics365Connection connection;

        /// <summary>
        /// Gets or sets the Dynamics 365 organisation to apply the operation to.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365Operation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365Operation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365Operation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter))]
        public virtual Dynamics365Connection Connection
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
        /// Initialises a new instance of the Dynamics365Operation class with the specified parent batch.
        /// </summary>
        /// <param name="parentBatch">The parent batch.</param>
        public Dynamics365Operation(Batch parentBatch) : base(parentBatch)
        { }

        public override ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();

            try
            {
                result.AddErrorIf(Connection == default(Dynamics365Connection), Properties.Resources.Dynamics365OperationConnectionValidateConnection, nameof(Connection));
                if (Connection != default(Dynamics365Connection)) result.Errors.AddRange(Connection.Validate().Errors);
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        #region Interface methods

        public List<IConnection> GetConnections()
        {
            return ParentBatch.ParentProject.Connections.Where(x => x.GetType() == typeof(Dynamics365Connection)).ToList();
        }

        public List<Dynamics365Entity> GetEntities()
        {
            List<Dynamics365Entity> entities = new List<Dynamics365Entity>();

            try
            {
                entities.AddRange(Dynamics365Entity.GetEntities((Dynamics365Connection)Connection));
            }
            catch { }

            return entities;
        }
        #endregion
    }
}
