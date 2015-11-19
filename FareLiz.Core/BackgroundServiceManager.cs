namespace SkyDean.FareLiz.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using log4net;

    /// <summary>Standard object for managing multiple helper services object</summary>
    [Serializable]
    public sealed class BackgroundServiceManager : IServiceManager
    {
        /// <summary>List of stored background services</summary>
        private readonly Dictionary<Type, IHelperService> services = new Dictionary<Type, IHelperService>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundServiceManager"/> class. Create a new instance of Background Service Manager with an instance
        /// of Logger
        /// </summary>
        /// <param name="logger">
        /// Instance of Logger
        /// </param>
        public BackgroundServiceManager(ILog logger)
        {
            this.Logger = logger;
        }

        /// <summary>Gets or sets the Logger instance</summary>
        public ILog Logger { get; set; }

        /// <summary>
        /// Add helper services to the manager
        /// </summary>
        /// <param name="services">
        /// Target services
        /// </param>
        /// <param name="autoStart">
        /// Auto-start the services after adding
        /// </param>
        public void Add(IEnumerable<IHelperService> services, bool autoStart)
        {
            foreach (var s in services)
            {
                this.Add(s, autoStart);
            }
        }

        /// <summary>
        /// Add a helper service to the manager
        /// </summary>
        /// <param name="service">
        /// Target service
        /// </param>
        /// <param name="autoStart">
        /// Auto-start the service after adding
        /// </param>
        public void Add(IHelperService service, bool autoStart)
        {
            var type = service.GetType();
            IHelperService existService;
            if (this.services.TryGetValue(type, out existService))
            {
                existService.Configuration = service.Configuration;
            }
            else
            {
                this.services.Add(type, service);
                existService = service;
            }

            existService.Logger = this.Logger;

            if (autoStart)
            {
                existService.Initialize();
                existService.Start();
            }
        }

        /// <summary>Stop and remove all services from the manager</summary>
        public void Clear()
        {
            this.StopAll();
            this.services.Clear();
        }

        /// <summary>
        /// Returns the helper service of certain class type
        /// </summary>
        /// <param name="type">
        /// Target service type
        /// </param>
        /// <returns>
        /// The helper service of certain class type
        /// </returns>
        public IHelperService Get(Type type)
        {
            IHelperService result;
            this.services.TryGetValue(type, out result);
            return result;
        }

        /// <summary>Returns list of all stored helped services in the manager</summary>
        /// <returns>List of all stored helped services in the manager</returns>
        public IList<IHelperService> GetAll()
        {
            return this.services.Values.ToList();
        }

        /// <summary>Start all helper services</summary>
        public void StartAll()
        {
            foreach (var s in this.services.Values)
            {
                Start(s);
            }
        }

        /// <summary>Stop all helper services</summary>
        public void StopAll()
        {
            foreach (var s in this.services.Values)
            {
                Stop(s);
            }
        }

        /// <summary>Gets the total number of helper services</summary>
        public int Count
        {
            get
            {
                return this.services.Count;
            }
        }

        /// <summary>
        /// Start a certain service if it is not running
        /// </summary>
        /// <param name="service">
        /// Target service to be started
        /// </param>
        private static void Start(IHelperService service)
        {
            if (service.Status != HelperServiceStatus.Running)
            {
                service.Start();
            }
        }

        /// <summary>
        /// Stop a certain service if it is running
        /// </summary>
        /// <param name="service">
        /// Target service to be stopped
        /// </param>
        private static void Stop(IHelperService service)
        {
            if (service.Status == HelperServiceStatus.Running)
            {
                service.Stop();
            }
        }
    }
}