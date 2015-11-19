namespace FareLiz.Tests.Data
{
    using System;
    using System.IO;

    using log4net;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SkyDean.FareLiz.Data;
    using SkyDean.FareLiz.Data.Config;
    using SkyDean.FareLiz.WinForm;

    /// <summary>
    /// The object ini config test.
    /// </summary>
    [TestClass]
    public class ObjectIniConfigTest
    {
        /// <summary>
        /// The test method 1.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            string fileName = "UnitTestLogger";
            var logger = LogManager.GetLogger(fileName);
            var target = new ObjectIniConfig("UnitTest.ini", logger);
            string dept = "Departure Test Location String";
            string dest = "Destination Test Location String";
            bool isMin = true;
            DateTime deptDate = DateTime.Now;
            DateTime retDate = DateTime.Now.AddDays(7);

            var obj = new ExecutionInfo
                          {
                              Departure = AirportDataProvider.FromIATA("HEL"), 
                              Destination = AirportDataProvider.FromIATA("SGN"), 
                              IsMinimized = isMin, 
                              DepartureDate = deptDate, 
                              ReturnDate = retDate
                          };
            target.SaveData(obj);
            bool actual = File.Exists(fileName);
            Assert.AreEqual(true, actual, "INI configuration file should be created");

            var fi = new FileInfo(fileName);
            var length = fi.Length;
            Assert.AreNotEqual(0, length, "INI Configuration file should not be empty");

            target.RemoveAllSections();
            target.Load(fileName);

            var newObj = new ExecutionInfo(); // Initialize empty object
            target.Load(fileName);
            target.ApplyData(newObj);

            Assert.AreEqual(newObj.Departure, dept);
            Assert.AreEqual(newObj.Destination, dest);
            Assert.AreEqual(newObj.IsMinimized, isMin);
            Assert.AreEqual(newObj.DepartureDate, deptDate);
            Assert.AreEqual(newObj.ReturnDate, retDate);
        }
    }
}