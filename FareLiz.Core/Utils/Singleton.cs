namespace SkyDean.FareLiz.Core.Utils
{
    public class Singleton<T> where T : new()
    {
        public static T Instance = new T();
    }
}
