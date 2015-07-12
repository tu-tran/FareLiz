namespace SkyDean.FareLiz.Core.Config
{
    /// <summary>
    /// Interface for Configuration object
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Validate the configurations
        /// </summary>
        ValidateResult Validate();
    }
}
