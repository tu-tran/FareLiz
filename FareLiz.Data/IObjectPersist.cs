namespace SkyDean.FareLiz.Data
{
    using log4net;

    /// <summary>
    /// The ObjectPersist interface.
    /// </summary>
    public interface IObjectPersist
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        ILog Logger { get; set; }

        /// <summary>
        /// The apply data.
        /// </summary>
        /// <param name="targetObject">
        /// The target object.
        /// </param>
        void ApplyData(object targetObject);

        /// <summary>
        /// The save data.
        /// </summary>
        /// <param name="targetObject">
        /// The target object.
        /// </param>
        void SaveData(object targetObject);
    }
}