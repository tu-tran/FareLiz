using log4net;

namespace SkyDean.FareLiz.Data
{
    public interface IObjectPersist
    {
        ILog Logger { get; set; }
        void ApplyData(object targetObject);
        void SaveData(object targetObject);
    }
}
