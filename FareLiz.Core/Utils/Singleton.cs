namespace SkyDean.FareLiz.Core.Utils
{
    /// <summary>
    /// The singleton.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class Singleton<T>
        where T : new()
    {
        /// <summary>The instance.</summary>
        public static T Instance = new T();
    }
}