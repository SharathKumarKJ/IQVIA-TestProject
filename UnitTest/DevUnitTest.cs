using System;
using IQVIA.ClientRequestor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class DevUnitTest
    {
        [TestMethod]
        public async System.Threading.Tasks.Task TestMethod1Async()
        {
            Client.SetURI("https://badapi.iqvia.io/api/v1/Tweets?");

            var startDate = "06/01/2016 03:39:00";
            var endDate = "07/01/2016 09:19:00";
            var r = await Client.GetSwaggerAsync(startDate, endDate);
        }
    }
}
