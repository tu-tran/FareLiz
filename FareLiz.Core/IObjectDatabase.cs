﻿using System;
using System.Collections.Generic;
using SkyDean.FareLiz.Core.Presentation;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Interface for object-storage database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectDatabase<T>
    {
        /// <summary>
        /// Add list of data into the database
        /// </summary>
        /// <param name="data">List of data</param>
        void AddData(IList<T> data, IProgressCallback callback);

        /// <summary>
        /// Create the data (any existing data will be replaced)
        /// </summary>
        /// <param name="data">List of data</param>
        void ResetData(IList<T> data, IProgressCallback callback);

        /// <summary>
        /// Reset and empty the database
        /// </summary>
        void Reset(IProgressCallback callback);

        /// <summary>
        /// Repair the database structure/data
        /// </summary>
        /// <returns>Success</returns>
        void RepairDatabase(IProgressCallback callback);

        /// <summary>
        /// Validate the database structure/data
        /// </summary>
        /// <returns>Validation result</returns>
        ValidateResult ValidateDatabase(IProgressCallback callback);
    }
}
