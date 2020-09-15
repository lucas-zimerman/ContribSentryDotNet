using ContribSentry.Cache;
using ContribSentry.Enums;
using ContribSentry.Testing;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ContribSentry.SessionTest.Cache
{
    public class EnvelopeCacheTests
    {
        [Fact]
        public void Store_CurrentSession_Saves_In_File()
        {
            using (var folder = new TempFolder())
            {
                var envelopeCache = new EnvelopeCache(new ContribSentryOptions(cacheEnable: true)
                {
                    CacheDirPath = folder.FolderName
                });
                var bytes = new byte[3] { 1, 2, 3 };
                var cachedData = new CachedSentryData(SentryId.Empty, bytes, ESentryType.CurrentSession);
                envelopeCache.Store(cachedData);
                Assert.True(File.Exists($"{folder.FolderName}/session"));
                envelopeCache.Discard(cachedData);
                Assert.False(File.Exists($"{folder.FolderName}/session"));
            }
        }


        [Fact]
        public void Iterator_Return_CurrentSession_If_Previously_Stored()
        {
            using (var folder = new TempFolder())
            {
                var envelopeCache = new EnvelopeCache(new ContribSentryOptions(cacheEnable: true)
                {
                    CacheDirPath = folder.FolderName
                }); ;
                var bytes = new byte[3] { 1, 2, 3 };
                var cachedData = new CachedSentryData(SentryId.Empty, bytes, ESentryType.CurrentSession);
                envelopeCache.Store(cachedData);
                Assert.True(File.Exists($"{folder.FolderName}/session"));

                var retreivedCachedList = envelopeCache.Iterator();
                Assert.Single(retreivedCachedList);
                Assert.Equal(bytes, retreivedCachedList[0].Data);

                foreach (var data in retreivedCachedList)
                {
                    envelopeCache.Discard(data);
                    Assert.False(File.Exists(envelopeCache.GetEnvelopePath(data)));
                }
            }
        }
    }
}
