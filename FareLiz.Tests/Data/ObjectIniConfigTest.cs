namespace FareLiz.Tests.Data
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Data;
    using SkyDean.FareLiz.Data.Config;
    using SkyDean.FareLiz.WinForm;

    /// <summary>The object ini config test.</summary>
    [TestClass]
    public class ObjectIniConfigTest
    {
        /// <summary>The test method 1.</summary>
        [TestMethod]
        public void TestMethod1()
        {
            var fileName = "UnitTestLogger";
            var logger = LogUtil.GetLogger(fileName);
            var target = new ObjectIniConfig("UnitTest.ini", logger);
            var dept = "Departure Test Location String";
            var dest = "Destination Test Location String";
            var isMin = true;
            var deptDate = DateTime.Now;
            var retDate = DateTime.Now.AddDays(7);

            var obj = new ExecutionInfo
                          {
                              Departure = AirportDataProvider.FromIATA("HEL"), 
                              Destination = AirportDataProvider.FromIATA("SGN"), 
                              IsMinimized = isMin, 
                              DepartureDate = deptDate, 
                              ReturnDate = retDate
                          };
            target.SaveData(obj);
            var actual = File.Exists(fileName);
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