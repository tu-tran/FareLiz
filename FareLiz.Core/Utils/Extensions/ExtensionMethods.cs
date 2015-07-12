using System;

namespace SkyDean.FareLiz.Core.Utils.Extensions
{
    public static class ExtensionMethods
    {
        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException("source");
            foreach (var item in list)
            {
                if (source.Equals(item)) return true;
            }
            return false;
        }

        public static bool IsNull(this object source)
        {
            return source == null;
        }

        public static string Format(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
        {
            if (eventHandler != null)
            {
                eventHandler(sender, e);
            }
        }
    }
}
