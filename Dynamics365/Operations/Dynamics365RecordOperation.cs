using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using ScottLane.DataTidy.Core;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Base class for Dynamics 365 operations that perform an action against records in an entity.
    /// </summary>
    public abstract class Dynamics365RecordOperation : Dynamics365Operation, IDataDestinationFieldsProvider, ILookupCriteriaCreator, IDataSourceFieldsProvider, IDynamics365EntityFieldsProvider
    {
        protected const TargetSource DEFAULT_TARGET_SOURCE = TargetSource.DataSourceValue;
        protected const int DEFAULT_BATCH_SIZE = 1;
        protected const int MINIMUM_BATCH_SIZE = 1;
        protected const int MAXIMUM_BATCH_SIZE = 1000;
        protected const int DEFAULT_THREAD_COUNT = 1;
        protected const int MINIMUM_THREAD_COUNT = 1;
        protected const int MAXIMUM_THREAD_COUNT = 50;
        protected const bool DEFAULT_CONTINUE_ON_ERROR = false;

        #region Properties

        protected Dynamics365Entity entity;

        /// <summary>
        /// Gets or sets the entity targeted by the operation.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RecordOperation), nameof(Entity)), GlobalisedDisplayName(typeof(Dynamics365RecordOperation), nameof(Entity)), GlobalisedDecription(typeof(Dynamics365RecordOperation), nameof(Entity)), TypeConverter(typeof(Dynamics365EntityConverter)), Browsable(true)]
        public virtual Dynamics365Entity Entity
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

        protected TargetSource targetSource;

        /// <summary>
        /// Gets or sets the source of the target record identifier.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RecordOperation), nameof(TargetSource)), GlobalisedDisplayName(typeof(Dynamics365RecordOperation), nameof(TargetSource)), GlobalisedDecription(typeof(Dynamics365RecordOperation), nameof(TargetSource)), DefaultValue(DEFAULT_TARGET_SOURCE)]
        public virtual TargetSource TargetSource
        {
            get { return targetSource; }
            set
            {
                SetTargetSource(value);
            }
        }

        /// <summary>
        /// Sets the TargetSource value and makes the appropriate properties browsable.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void SetTargetSource(TargetSource value)
        {
            if (targetSource != value)
            {
                targetSource = value;

                if (targetSource == TargetSource.DataSourceValue)
                {
                    Target = new Dynamics365DataSourceValue(this);
                    CoreUtility.SetBrowsable(this, nameof(Target), true);
                }
                else if (targetSource == TargetSource.LookupValue)
                {
                    Target = new Dynamics365LookupValue(this);
                    CoreUtility.SetBrowsable(this, nameof(Target), true);
                }
                else if (targetSource == TargetSource.UserProvidedValue)
                {
                    Target = new Dynamics365UserProvidedValue(this);
                    CoreUtility.SetBrowsable(this, nameof(Target), true);
                }
                else
                {
                    Target = default(FieldValue);
                    CoreUtility.SetBrowsable(this, nameof(Target), false);
                }

                RefreshBrowsableFields();
                OnPropertyChanged(nameof(TargetSource));
                RefreshName();
                // todo - events if properties within target source change
            }
        }

        protected FieldValue target;

        /// <summary>
        /// Gets or sets the identifier of the record targeted by the operation.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RecordOperation), nameof(Target)), GlobalisedDisplayName(typeof(Dynamics365RecordOperation), nameof(Target)), GlobalisedDecription(typeof(Dynamics365RecordOperation), nameof(Target)), TypeConverter(typeof(ExpandableObjectConverter)), Browsable(true)]
        public virtual FieldValue Target
        {
            get { return target; }
            set
            {
                if (target != value)
                {
                    target = value;
                    OnPropertyChanged(nameof(Target));
                    RefreshName();
                }
            }
        }

        protected IDataSource dataSource;

        /// <summary>
        /// Gets or sets the data source to read records from.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RecordOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(Dynamics365RecordOperation), nameof(DataSource)), GlobalisedDecription(typeof(Dynamics365RecordOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter)), JsonProperty(Order = 1)]
        public virtual IDataSource DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    OnPropertyChanged(nameof(DataSource));
                    RefreshName();
                }
            }
        }

        protected int batchSize = DEFAULT_BATCH_SIZE;

        /// <summary>
        /// Gets or sets the number of records that will be processed per request.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RecordOperation), nameof(BatchSize)), GlobalisedDisplayName(typeof(Dynamics365RecordOperation), nameof(BatchSize)), GlobalisedDecription(typeof(Dynamics365RecordOperation), nameof(BatchSize)), DefaultValue(DEFAULT_BATCH_SIZE)]
        public virtual int BatchSize
        {
            get { return batchSize; }
            set
            {
                if (batchSize != value)
                {
                    batchSize = value;
                    OnPropertyChanged(nameof(BatchSize));
                    RefreshName();
                }
            }
        }

        protected int threadCount = DEFAULT_THREAD_COUNT;

        /// <summary>
        /// Gets or sets the number of records that will be processed per batch if the Batch processing mode is used.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RecordOperation), nameof(ThreadCount)), GlobalisedDisplayName(typeof(Dynamics365RecordOperation), nameof(ThreadCount)), GlobalisedDecription(typeof(Dynamics365RecordOperation), nameof(ThreadCount)), DefaultValue(DEFAULT_THREAD_COUNT)]
        public virtual int ThreadCount
        {
            get { return threadCount; }
            set
            {
                if (threadCount != value)
                {
                    threadCount = value;
                    OnPropertyChanged(nameof(ThreadCount));
                }
            }
        }

        protected bool continueOnError = DEFAULT_CONTINUE_ON_ERROR;

        /// <summary>
        /// Gets or sets a value that determines whether subsequent operations are processed if an error occurs.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RecordOperation), nameof(ContinuteOnError)), GlobalisedDisplayName(typeof(Dynamics365RecordOperation), nameof(ContinuteOnError)), GlobalisedDecription(typeof(Dynamics365RecordOperation), nameof(ContinuteOnError)), DefaultValue(DEFAULT_CONTINUE_ON_ERROR), Browsable(true)]
        public virtual bool ContinuteOnError
        {
            get { return continueOnError; }
            set
            {
                if (continueOnError != value)
                {
                    continueOnError = value;
                    OnPropertyChanged(nameof(ContinuteOnError));
                    RefreshName();
                }
            }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Dynamics365RecordOperation class with the specified parent batch.
        /// </summary>
        /// <param name="parentBatch">The parent batch.</param>
        public Dynamics365RecordOperation(Batch parentBatch) : base(parentBatch)
        {
            SetTargetSource(DEFAULT_TARGET_SOURCE);
        }

        private void RefreshBrowsableFields()
        {
            CoreUtility.SetBrowsable(this, nameof(Target), (targetSource == TargetSource.DataSourceValue || targetSource == TargetSource.LookupValue || targetSource == TargetSource.UserProvidedValue));
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();

            result.AddErrorIf(Entity == default(Dynamics365Entity), Properties.Resources.Dynamics365RecordOperationValidateEntity, nameof(Entity));
            result.AddErrorIf(DataSource == default(IDataSource), Properties.Resources.Dynamics365RecordOperationValidateDataSource, nameof(DataSource));
            if (DataSource != default(IDataSource)) result.Errors.AddRange(DataSource.Validate().Errors);
            result.AddErrorIf(BatchSize < MINIMUM_BATCH_SIZE || BatchSize > MAXIMUM_BATCH_SIZE, string.Format(Properties.Resources.Dynamics365RecordOperationValidateBatchSize, MINIMUM_BATCH_SIZE, MAXIMUM_BATCH_SIZE), nameof(BatchSize));
            result.AddErrorIf(ThreadCount < MINIMUM_THREAD_COUNT || ThreadCount > MAXIMUM_THREAD_COUNT, string.Format(Properties.Resources.Dynamics365RecordOperationValidateThreadCount, MINIMUM_THREAD_COUNT, MAXIMUM_THREAD_COUNT), nameof(ThreadCount));

            if (CanValidateTarget())
            {
                result.AddErrorIf(TargetSource != TargetSource.None && Target == default(FieldValue), Properties.Resources.Dynamics365RecordOperationValidateTargetRecord, nameof(Target));

                if (Target != default(FieldValue))
                {
                    result.Errors.AddRange(Target.Validate().Errors);
                }
            }

            return result;
        }

        public virtual bool CanValidateTarget()
        {
            return true;
        }

        /// <summary>
        /// Executes operations against a Dynamics 365 organisation.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <remarks>
        /// There is a fundamental issue that needs fixing: progress reporting is based on data source rows not the actually loaded rows, however some elements below violate this
        /// It works ok due to one record being created for each source row, but will break if that assumption does not hold in the future
        /// </remarks>
        public override void Execute(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            OnExecuting(new ExecutableEventArgs(this));
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365RecordOperationExecute, Name)));

            DataTable data = GetSourceData(cancel, progress);

            using (OrganizationServiceProxy service = Connection.OrganizationServiceProxy)
            {
                List<OrganizationRequest> requests = new List<OrganizationRequest>();
                int recordsLoaded = 0;

                for (int rowIndex = 0; rowIndex < data.Rows.Count; rowIndex++)
                {
                    try
                    {
                        // transform data source records to create organisation requests
                        requests.AddRange(CreateOrganisationRequests(data.Rows[rowIndex], cancel, progress));
                        progress?.Report(new ExecutionProgress(ExecutionStage.Transform, rowIndex + 1, data.Rows.Count));
                        cancel.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        if (ContinuteOnError)
                        {
                            // report the error, update progress and continue
                            progress?.Report(new ExecutionProgress(NotificationType.Warning, ex.Message));
                            progress?.Report(new ExecutionProgress(ExecutionStage.Transform, rowIndex + 1, data.Rows.Count));
                            progress?.Report(new ExecutionProgress(ExecutionStage.Load, ++recordsLoaded, data.Rows.Count));
                        }
                        else
                        {
                            throw;
                        }
                    }

                    try
                    {
                        // accumulate requests until the batch size or the last requests for the operation are reached
                        // threaded batches run simultaneously so we need to create batches for each thread up-front
                        if (requests.Count >= batchSize * threadCount || rowIndex >= data.Rows.Count - 1)
                        {
                            Task[] tasks = new Task[requests.Count % batchSize == 0 ? requests.Count / batchSize : (requests.Count / batchSize) + 1];

                            for (int taskIndex = 0; taskIndex < tasks.Length; taskIndex++)
                            {
                                List<OrganizationRequest> taskRequests = requests.GetRange(taskIndex * batchSize, requests.Count % batchSize != 0 && taskIndex == tasks.Length - 1 ? requests.Count % batchSize : batchSize);
                                tasks[taskIndex] = Task.Factory.StartNew(() => ExecuteRequests(taskRequests, service, cancel, progress, ref recordsLoaded, data.Rows.Count));
                            }

                            Task.WaitAll(tasks);
                            requests.Clear();
                        }
                    }
                    catch (AggregateException ex)
                    {
                        throw ex.InnerException;
                    }
                    catch (Exception ex)
                    {
                        if (ContinuteOnError)
                        {
                            progress?.Report(new ExecutionProgress(NotificationType.Warning, ex.Message));
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            //progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365RecordOperationExecuteSuccess, Name)));
            OnExecuted(new ExecutableEventArgs(this));
        }

        private void ExecuteRequests(List<OrganizationRequest> requests, OrganizationServiceProxy service, CancellationToken cancel, IProgress<ExecutionProgress> progress, ref int recordsLoaded, int rowCount)
        {
            if (batchSize == MINIMUM_BATCH_SIZE)
            {
                ExecuteIndividualRequests(requests, service, cancel, progress);
            }
            else
            {
                ExecuteBatchedRequests(requests, service, cancel, progress);
            }

            Interlocked.Add(ref recordsLoaded, requests.Count); // todo - this works when 1 output record is created per data source row, but not if more than one request is created
            progress?.Report(new ExecutionProgress(ExecutionStage.Load, recordsLoaded, rowCount));
        }

        /// <summary>
        /// Executes the specified requests individually against Dynamics 365.
        /// </summary>
        /// <param name="requests">The requests to execute.</param>
        /// <param name="service">The organisation service.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        protected virtual void ExecuteIndividualRequests(List<OrganizationRequest> requests, IOrganizationService service, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            for (int requestIndex = 0; requestIndex < requests.Count; requestIndex++)
            {
                try
                {
                    service.Execute(requests[requestIndex]);
                    progress?.Report(new ExecutionProgress(NotificationType.Information, GetRequestDescription(requests[requestIndex])));
                    cancel.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (ContinuteOnError)
                    {
                        progress?.Report(new ExecutionProgress(NotificationType.Warning, ex.Message));
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Executes the specified requests in a batch against Dynamics 365.
        /// </summary>
        /// <param name="requests">The requests to execute.</param>
        /// <param name="service">The organisation service.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        protected void ExecuteBatchedRequests(List<OrganizationRequest> requests, IOrganizationService service, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            ExecuteMultipleRequest executeMultipleRequest = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = ContinuteOnError,
                    ReturnResponses = false
                },
                Requests = new OrganizationRequestCollection()
            };
            executeMultipleRequest.Requests.AddRange(requests.GetRange(0, requests.Count));

            ExecuteMultipleResponse response = (ExecuteMultipleResponse)service.Execute(executeMultipleRequest);

            if (response.IsFaulted)
            {
                List<ExecuteMultipleResponseItem> errors = response.Responses.Where(r => r.Fault != null).ToList();

                if (errors.Count > 0)
                {
                    OrganizationServiceFault error = errors.First().Fault;
                    string errorMessage = string.Format(Properties.Resources.Dynamics365RecordOperationExecuteBatchErrors, errors.Count, executeMultipleRequest.Requests.Count, error.Message);

                    if (ContinuteOnError)
                    {
                        progress?.Report(new ExecutionProgress(NotificationType.Warning, errorMessage));
                    }
                    else
                    {
                        throw new ApplicationException(errorMessage);
                    }
                }
            }
            else
            {
                progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365RecordOperationExecuteBatchSuccess, executeMultipleRequest.Requests.Count)));
            }

            cancel.ThrowIfCancellationRequested();
        }

        protected abstract string GetRequestDescription(OrganizationRequest request);

        protected abstract List<OrganizationRequest> CreateOrganisationRequests(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress);

        protected virtual Entity GetTargetEntity(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            Entity targetEntity = new Entity(Entity.LogicalName);

            if (TargetSource != TargetSource.None)
            {
                targetEntity.Id = (Guid)Target.GetValue(row, cancel, progress);
            }

            return targetEntity;
        }

        /// <summary>
        /// Gets data from the operation's data source.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns></returns>
        protected virtual DataTable GetSourceData(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            return DataSource?.GetDataTable(cancel, progress);
        }

        public override void UpdateParentReferences(Batch parentBatch)
        {
            base.UpdateParentReferences(parentBatch);
            Target?.UpdateParentReferences(this);
        }

        public override IOperation Clone(bool addSuffix)
        {
            Dynamics365RecordOperation clone = (Dynamics365RecordOperation)base.Clone(addSuffix);
            clone.Target = Target?.Clone();
            return clone;
        }

        #region Interface Methods

        public virtual List<DataTableField> GetDataSourceFields()
        {
            List<DataTableField> fields = new List<DataTableField>();
            try
            {
                fields.AddRange(DataTableField.GetDataTableFields(DataSource?.GetDataColumns()));
            }
            catch { }

            return fields;
        }

        public LookupCriteria CreateLookupCriteria(Type type)
        {
            return (LookupCriteria)Activator.CreateInstance(type, new object[] { this, default(LookupValue) });
        }

        public virtual List<Field> GetDataDestinationFields()
        {
            List<Field> fields = new List<Field>();
            try
            {
                fields.AddRange(Entity.GetFields(Connection));
            }
            catch { }

            return fields;
        }

        public IConnection GetConnection()
        {
            return Connection;
        }

        public virtual List<Dynamics365Field> GetDynamics365EntityFields()
        {
            List<Dynamics365Field> fields = new List<Dynamics365Field>();

            try
            {
                fields.AddRange(Dynamics365Field.GetFields(Entity, Connection));
            }
            catch { }

            return fields;
        }

        #endregion
    }

    /// <summary>
    /// Defines the types of supported target sources.
    /// </summary>
    public enum TargetSource
    {
        None,
        DataSourceValue,
        LookupValue,
        UserProvidedValue
    }
}
