namespace SkyDean.FareLiz.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>Interface for managing helper services</summary>
    public interface IServiceManager
    {
        /// <summary>Returns the total number of helper services</summary>
        int Count { get; }

        /// <summary>
        /// Add a helper service to the manager
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="autoStart">
        /// Auto-start the service after adding
        /// </param>
        void Add(IHelperService service, bool autoStart);

        /// <summary>
        /// Add helper services to the manager
        /// </summary>
        /// <param name="services">
        /// Target services
        /// </param>
        /// <param name="autoStart">
        /// Auto-start the services after adding
        /// </param>
        void Add(IEnumerable<IHelperService> services, bool autoStart);

        /// <summary>
        /// Get helper service of certain class type
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="IHelperService"/>.
        /// </returns>
        IHelperService Get(Type type);

        /// <summary>Get all stored helped services in the manager</summary>
        /// <returns>The <see cref="IList" />.</returns>
        IList<IHelperService> GetAll();

        /// <summary>Start all helper services</summary>
        void StartAll();

        /// <summary>Stop all helper services</summary>
        void StopAll();

        /// <summary>Stop and remove all services from the manager</summary>
        void Clear();
    }
}