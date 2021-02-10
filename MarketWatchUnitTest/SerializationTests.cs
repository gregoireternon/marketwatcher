using MarketWatcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MarketWatchUnitTest
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void TestDeserialize()
        {
            string response = "{\"d\":[{\"o\":1.725,\"h\":2.135,\"l\":1.425,\"c\":1.615,\"v\":0,\"var\":-0.33,\"qt\":[{\"d\":2102050661,\"o\":1.635,\"h\":1.635,\"l\":1.625,\"c\":1.625,\"v\":0},{\"d\":2102050662,\"o\":1.615,\"h\":1.615,\"l\":1.605,\"c\":1.615,\"v\":0}]}]}";
            
            var res = JsonConvert.DeserializeObject<QuoteHolder>(response);
        }
    }
}
