namespace SkyDean.FareLiz.Core.Utils
{
    using System;

    /// <summary>This attribute indicates that the data is unique and should not be copied on transfering object data</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event)]
    public class UniqueDataAttribute : Attribute
    {
    }
}