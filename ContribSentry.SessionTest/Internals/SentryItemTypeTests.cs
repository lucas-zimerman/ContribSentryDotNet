using ContribSentry.Enums;
using ContribSentry.Internals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ContribSentry.SessionTest.Internals
{
    public class SentryItemTypeTests
    {
        [Fact]
        public void TypeSession_Serializes_To_Json()
        {
            var type = new SentryItemType(ESentryType.Session);
            var json = JsonConvert.SerializeObject(type);
            Assert.Equal("{\"type\":\"session\"}", json);
        }
     
        [Fact]
        public void TypeSession_Serializes_To_Json_With_Special_Config()
        {
            var type = new SentryItemType(ESentryType.Session);
            var serializer = new Serializer();
            var json = JsonConvert.SerializeObject(type,serializer.jsonSettings);
            Assert.Equal("{\"type\":\"session\"}", json);
        }
    }
}
