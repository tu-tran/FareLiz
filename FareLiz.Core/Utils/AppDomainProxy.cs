using System;
using System.IO;

namespace SkyDean.FareLiz.Core.Utils
{
    public class AppDomainProxy<T> : IDisposable where T : MarshalByRefObject
    {
        public Action OnDispose { get; set; }
        public T Instance { get; set; }

        void IDisposable.Dispose()
        {
            if (OnDispose != null)
            {
                OnDispose();
                OnDispose = null;
            }
        }

        protected AppDomainProxy() { }

        public static AppDomainProxy<T> GetProxy(string privatePath)
        {
            Type wrappedType = typeof(T);
            string newDomainName = Guid.NewGuid().ToString();
            string basePath = Path.GetDirectoryName(wrappedType.Assembly.Location);

            AppDomainSetup setup = new AppDomainSetup()
            {
                ApplicationName = newDomainName,
                ApplicationBase = basePath,
                PrivateBinPath = privatePath
            };

            var domain = AppDomain.CreateDomain(newDomainName, AppDomain.CurrentDomain.Evidence, setup);
            var instance = (T)domain.CreateInstanceAndUnwrap(wrappedType.Assembly.FullName, wrappedType.FullName);
            return new AppDomainProxy<T>() { OnDispose = () => AppDomain.Unload(domain), Instance = (T)instance };
        }
    }
}
