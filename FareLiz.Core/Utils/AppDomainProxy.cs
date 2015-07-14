namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.IO;

    /// <summary>
    /// The app domain proxy.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class AppDomainProxy<T> : IDisposable
        where T : MarshalByRefObject
    {
        /// <summary>Initializes a new instance of the <see cref="AppDomainProxy{T}" /> class.</summary>
        protected AppDomainProxy()
        {
        }

        /// <summary>Gets or sets the on dispose.</summary>
        public Action OnDispose { get; set; }

        /// <summary>Gets or sets the instance.</summary>
        public T Instance { get; set; }

        /// <summary>The dispose.</summary>
        void IDisposable.Dispose()
        {
            if (this.OnDispose != null)
            {
                this.OnDispose();
                this.OnDispose = null;
            }
        }

        /// <summary>
        /// The get proxy.
        /// </summary>
        /// <param name="privatePath">
        /// The private path.
        /// </param>
        /// <returns>
        /// The <see cref="AppDomainProxy"/>.
        /// </returns>
        public static AppDomainProxy<T> GetProxy(string privatePath)
        {
            var wrappedType = typeof(T);
            var newDomainName = Guid.NewGuid().ToString();
            var basePath = Path.GetDirectoryName(wrappedType.Assembly.Location);

            var setup = new AppDomainSetup { ApplicationName = newDomainName, ApplicationBase = basePath, PrivateBinPath = privatePath };

            var domain = AppDomain.CreateDomain(newDomainName, AppDomain.CurrentDomain.Evidence, setup);
            var instance = (T)domain.CreateInstanceAndUnwrap(wrappedType.Assembly.FullName, wrappedType.FullName);
            return new AppDomainProxy<T> { OnDispose = () => AppDomain.Unload(domain), Instance = instance };
        }
    }
}