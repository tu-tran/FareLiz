namespace SkyDean.FareLiz.Core
{
    using SkyDean.FareLiz.Core.Data;

    /// <summary>
    /// Generic interface for helper objects which are used to synchronize database
    /// </summary>
    /// <typeparam name="T">
    /// Target synchronizable database type
    /// </typeparam>
    public interface IDatabaseSyncer<T> : IDataSyncer<T>, IPackageSyncer<TravelRoute>
        where T : IFareDatabase, ISyncable
    {
    }
}