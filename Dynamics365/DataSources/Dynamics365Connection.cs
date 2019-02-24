using System;
using System.ComponentModel;
using System.Threading;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Provides connectivity to a Dynamics 365 online or on-premise instance.
    /// </summary>
    [Connection(typeof(Dynamics365Connection), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365Connection.png")]
    public class Dynamics365Connection : Connection, IAvailableProvider, IUrlAddressable
    {
        private readonly TimeSpan PROXY_TIMEOUT_BUFFER = TimeSpan.FromMinutes(30);
        private readonly TimeSpan DEFAULT_CONNECTION_TIMEOUT = TimeSpan.FromMinutes(5);
        private const AuthenticationType DEFAULT_AUTHENTICATION_TYPE = AuthenticationType.Office365;

        #region Properties

        private AuthenticationType authenticationType = DEFAULT_AUTHENTICATION_TYPE;

        /// <summary>
        /// Gets or sets the authentication type.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365Connection), nameof(AuthenticationType)), GlobalisedDisplayName(typeof(Dynamics365Connection), nameof(AuthenticationType)), GlobalisedDecription(typeof(Dynamics365Connection), nameof(AuthenticationType)), DefaultValue(DEFAULT_AUTHENTICATION_TYPE), Browsable(true)]
        public AuthenticationType AuthenticationType
        {
            get { return authenticationType; }
            set
            {
                if (authenticationType != value)
                {
                    authenticationType = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(AuthenticationType));
                    InvalidateProxy();
                    ClearConnectionCache();
                }
            }
        }

        private string userName;

        /// <summary>
        /// Gets or sets the login user name.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365Connection), nameof(UserName)), GlobalisedDisplayName(typeof(Dynamics365Connection), nameof(UserName)), GlobalisedDecription(typeof(Dynamics365Connection), nameof(UserName)), Browsable(true)]
        public string UserName
        {
            get { return userName; }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    OnPropertyChanged(nameof(UserName));
                    InvalidateProxy();
                    ClearConnectionCache();
                }
            }
        }

        private string password;

        /// <summary>
        /// Gets or sets the encrypted login password.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365Connection), nameof(Password)), GlobalisedDisplayName(typeof(Dynamics365Connection), nameof(Password)), GlobalisedDecription(typeof(Dynamics365Connection), nameof(Password)), PasswordPropertyText(true), TypeConverter(typeof(EncryptedStringConverter)), Browsable(true)]
        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                    InvalidateProxy();
                    ClearConnectionCache();
                }
            }
        }

        private string domain;

        /// <summary>
        /// Gets or sets the login domain.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365Connection), nameof(Domain)), GlobalisedDisplayName(typeof(Dynamics365Connection), nameof(Domain)), GlobalisedDecription(typeof(Dynamics365Connection), nameof(Domain)), Browsable(true)]
        public string Domain
        {
            get { return domain; }
            set
            {
                if (domain != value)
                {
                    domain = value;
                    OnPropertyChanged(nameof(Domain));
                    InvalidateProxy();
                    ClearConnectionCache();
                }
            }
        }

        private Uri serverUrl;

        /// <summary>
        /// Gets or sets the Dynamics 365 server URL.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365Connection), nameof(ServerUrl)), GlobalisedDisplayName(typeof(Dynamics365Connection), nameof(ServerUrl)), GlobalisedDecription(typeof(Dynamics365Connection), nameof(ServerUrl)), Browsable(true)]
        public Uri ServerUrl
        {
            get { return serverUrl; }
            set
            {
                if (serverUrl != value)
                {
                    serverUrl = value;
                    OnPropertyChanged(nameof(ServerUrl));
                    InvalidateProxy();
                    ClearConnectionCache();
                }
            }
        }

        private Uri homeRealUrl;

        /// <summary>
        /// Gets or sets the IFD home realm URL.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365Connection), nameof(HomeRealmUrl)), GlobalisedDisplayName(typeof(Dynamics365Connection), nameof(HomeRealmUrl)), GlobalisedDecription(typeof(Dynamics365Connection), nameof(HomeRealmUrl)), Browsable(true)]
        public Uri HomeRealmUrl
        {
            get { return homeRealUrl; }
            set
            {
                if (homeRealUrl != value)
                {
                    homeRealUrl = value;
                    OnPropertyChanged(nameof(HomeRealmUrl));
                    InvalidateProxy();
                    ClearConnectionCache();
                }
            }
        }

        private TimeSpan timeout;

        /// <summary>
        /// Gets or sets the connection command timeout.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365Connection), nameof(Timeout)), GlobalisedDisplayName(typeof(Dynamics365Connection), nameof(Timeout)), GlobalisedDecription(typeof(Dynamics365Connection), nameof(Timeout)), Browsable(true)]
        public TimeSpan Timeout
        {
            get { return timeout; }
            set
            {
                if (timeout != value)
                {
                    timeout = value;
                    OnPropertyChanged(nameof(Timeout));
                    InvalidateProxy();
                }
            }
        }

        private OrganizationServiceProxy proxy;

        /// <summary>
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/dn688174(v=crm.6).aspx
        /// https://msdn.microsoft.com/en-us/library/dn688177.aspx
        /// https://msdn.microsoft.com/en-us/library/microsoft.xrm.sdk.client.organizationserviceproxy.aspx
        /// https://blog.thomasfaulkner.nz/post/2015/03/crm-organization-service-(re)authentication
        /// </remarks>
        [JsonIgnore, Browsable(false)]
        public OrganizationServiceProxy OrganizationServiceProxy
        {
            get
            {
                if (proxy != default(OrganizationServiceProxy) && (proxy.SecurityTokenResponse != default(SecurityTokenResponse) && DateTime.Now.Add(PROXY_TIMEOUT_BUFFER) >= proxy.SecurityTokenResponse.Response.Lifetime.Expires))
                {
                    InvalidateProxy();
                }

                if (proxy == default(OrganizationServiceProxy))
                {
                    CrmServiceClient serviceClient = new CrmServiceClient(GetConnectionString());

                    if (serviceClient.IsReady)
                    {
                        proxy = serviceClient.OrganizationServiceProxy;
                        proxy.Timeout = Timeout;
                        Version = serviceClient.ConnectedOrgVersion;
                    }
                    else
                    {
                        throw new ApplicationException(string.Format(Properties.Resources.Dynamics365ConnectionProxyNotReady, serviceClient.LastCrmError));
                    }
                }

                return proxy;
            }
        }

        /// <summary>
        /// Invalidates the organisation service proxy.
        /// </summary>
        private void InvalidateProxy()
        {
            if (proxy != default(OrganizationServiceProxy))
            {
                proxy.Dispose();
                proxy = default(OrganizationServiceProxy);
            }
        }

        // todo - OAuth settings

        /// <summary>
        /// Gets the Dynamics 365 version.
        /// </summary>
        [Browsable(false)]
        public Version Version { get; private set; }

        [Browsable(false), JsonIgnore]
        public Uri Url
        {
            get { return ServerUrl; }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Dynamics365Connection class.
        /// </summary>
        public Dynamics365Connection(Project parent) : base(parent)
        {
            timeout = DEFAULT_CONNECTION_TIMEOUT;
            RefreshBrowsableFields();
        }

        private void RefreshBrowsableFields()
        {
            CoreUtility.SetBrowsable(this, nameof(Domain), authenticationType == AuthenticationType.ActiveDirectory);
            CoreUtility.SetBrowsable(this, nameof(HomeRealmUrl), authenticationType == AuthenticationType.InternetFacingDeployment);
        }

        /// <summary>
        /// Checks if the connection is available.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The connectivity progress.</param>
        /// <returns>True if the connection is available, false otherwise.</returns>
        public ConnectivityResult IsAvailable(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            ConnectivityResult result = new ConnectivityResult(false);

            try
            {
                CrmServiceClient serviceClient = new CrmServiceClient(GetConnectionString());

                if (serviceClient.IsReady)
                {
                    result.IsAvailable = true;
                }
                else
                {
                    result.ErrorMessage = serviceClient.LastCrmError;
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Validates the connection.
        /// </summary>
        /// <returns>The validation result.</returns>
        public override Core.ValidationResult Validate()
        {
            Core.ValidationResult result = new Core.ValidationResult();
            result.AddErrorIf(string.IsNullOrEmpty(UserName), Properties.Resources.Dynamics365ConnectionValidateUserName, nameof(UserName));
            result.AddErrorIf(string.IsNullOrEmpty(Password), Properties.Resources.Dynamics365ConnectionValidatePassword, nameof(Password));
            result.AddErrorIf(string.IsNullOrEmpty(ServerUrl?.OriginalString), Properties.Resources.Dynamics365ConnectionValidateServerUrl, nameof(ServerUrl));
            result.AddErrorIf(Timeout == default(TimeSpan) || Timeout <= TimeSpan.MinValue, Properties.Resources.Dynamics365ConnectionValidateTimeout, nameof(Timeout));
            return result;
        }

        /// <summary>
        /// Determines whether the specified version is equal to or above the connected Dynamics 365 version.
        /// </summary>
        /// <param name="version">The version to compare.</param>
        /// <returns>True if the specified version is equal to or above the connected Dynamics 365 version, false otherwise.</returns>
        public bool IsVersionOrAbove(CrmVersion version)
        {
            bool result = false;

            if (Version != null && Version.Major >= (int)version)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Gets a connection string for the connection based on the authentication type and connection details.
        /// </summary>
        /// <returns>The connection.</returns>
        private string GetConnectionString()
        {
            string connectionString = string.Empty;

            if (AuthenticationType == AuthenticationType.Office365)
            {
                connectionString = string.Format("AuthType=Office365; Url={0}; Username={1}; Password={2}; RequireNewInstance=True;", ServerUrl, UserName, new AESEncrypter().Decrypt(Password));
            }
            else if (AuthenticationType == AuthenticationType.ActiveDirectory)
            {
                connectionString = string.Format("AuthType=AD;Url={0}; Username={1}; Password={2}; RequireNewInstance=True;", ServerUrl, UserName, new AESEncrypter().Decrypt(Password));
            }
            else if (AuthenticationType == AuthenticationType.InternetFacingDeployment)
            {
                connectionString = string.Format("AuthType=IFD;Url={0}; HomeRealmUri={1}; Username={2}; Password={3}; RequireNewInstance=True;", ServerUrl, HomeRealmUrl, UserName, new AESEncrypter().Decrypt(Password));
            }

            return connectionString;
        }
    }

    /// <summary>
    /// The supported authentication methods.
    /// </summary>
    public enum AuthenticationType
    {
        ActiveDirectory,
        InternetFacingDeployment,
        //OAuth,
        Office365
    }

    /// <summary>
    /// The CRM version.
    /// </summary>
    public enum CrmVersion
    {
        Crm2011 = 5,
        Crm2013 = 6,
        Crm2015 = 7,
        Crm2016 = 8
    }
}
