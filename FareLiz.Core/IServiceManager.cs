using System;
using System.Collections.Generic;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Interface for managing helper services
    /// </summary>
    public interface IServiceManager
    {
        /// <summary>
        /// Add a helper service to the manager
        /// </summary>
        /// <param name="services">Target service</param>
        /// <param name="autoStart">Auto-start the service after adding</param>
        void Add(IHelperService service, bool autoStart);

        /// <summary>
        /// Add helper services to the manager
        /// </summary>
        /// <param name="services">Target services</param>
        /// <param name="autoStart">Auto-start the services after adding</param>
        void Add(IEnumerable<IHelperService> services, bool autoStart);

        /// <summary>
        /// Get helper service of certain class type
        /// </summary>
        IHelperService Get(Type type);

        /// <summary>
        /// Get all stored helped services in the manager
        /// </summary>
        IList<IHelperService> GetAll();

        /// <summary>
        /// Start all helper services
        /// </summary>
        void StartAll();

        /// <summary>
        /// Stop all helper services
        /// </summary>
        void StopAll();

        /// <summary>
        /// Stop and remove all services from the manager
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns the total number of helper services
        /// </summary>
        int Count { get; }
    }
}
