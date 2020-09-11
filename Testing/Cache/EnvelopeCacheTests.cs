using ContribSentry.Cache;
using ContribSentry.Enums;
using ContribSentry.Testing;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ContribSentry.TracingTest.Cache
{
    public class EnvelopeCacheTests
    {
        [Fact]
        public void CachedEnvelope_Discart_Not_Found_Doesnt_Break()
        {
            using (var folder = new TempFolder())
            {
                var envelopeCache = new EnvelopeCache(new ContribSentryOptions(cacheEnable: true)
                {
                    CacheDirPath = folder.FolderName
                }); ;
                envelopeCache.Discard(new CachedSentryData(SentryId.Empty, null, ESentryType.Transaction));
            }
        }

        [Fact]
        public void CachedEnvelope_Create_And_Discart_Data()
        {
            using (var folder = new TempFolder())
            {
                var envelopeCache = new EnvelopeCache(new ContribSentryOptions(cacheEnable: true)
                {
                    CacheDirPath = folder.FolderName
                });
                var cachedData = new CachedSentryData(SentryId.Empty, null, ESentryType.Transaction);
                envelopeCache.Store(cachedData);
                Assert.True(File.Exists(envelopeCache.GetEnvelopePath(cachedData)));
                envelopeCache.Discard(cachedData);
                Assert.False(File.Exists(envelopeCache.GetEnvelopePath(cachedData)));
            }
        }

        [Fact]
        public  void CachedEnvelope_Iterator_Return_3_Itens()
        {
            using (var folder = new TempFolder())
            {
                var envelopeCache = new EnvelopeCache(new ContribSentryOptions(cacheEnable: true)
                {
                    CacheDirPath = folder.FolderName
                }); ;
                var cachedData = new List<CachedSentryData>(){
                new CachedSentryData(Guid.NewGuid(), new byte[1]{ 13 }, ESentryType.Transaction),
                new CachedSentryData(Guid.NewGuid(), new byte[1]{ 13 }, ESentryType.Transaction),
                new CachedSentryData(Guid.NewGuid(), new byte[1]{ 13 }, ESentryType.Transaction)
            };
                foreach (var data in cachedData)
                {
                    envelopeCache.Store(data);
                    Assert.True(File.Exists(envelopeCache.GetEnvelopePath(data)));
                }

                var retreivedCachedList = envelopeCache.Iterator();
                Assert.Equal(cachedData.Count, retreivedCachedList.Count);

                foreach (var data in cachedData)
                {
                    envelopeCache.Discard(data);
                    Assert.False(File.Exists(envelopeCache.GetEnvelopePath(data)));
                }
            }
        }

        [Fact]
        public void CachedEnvelope_Doesnt_Save_More_Than_Max_Allowed()
        {
            using (var folder = new TempFolder())
            {
                var envelopeCache = new EnvelopeCache(new ContribSentryOptions(cacheEnable: true)
                {
                    CacheDirPath = folder.FolderName,
                    CacheDirSize = 2
                    
                }); ;
                var discaredGuid = Guid.NewGuid();
                var cachedData = new List<CachedSentryData>(){
                new CachedSentryData(Guid.NewGuid(), new byte[1]{ 13 }, ESentryType.Transaction),
                new CachedSentryData(Guid.NewGuid(), new byte[1]{ 13 }, ESentryType.Transaction),
                new CachedSentryData(discaredGuid, new byte[1]{ 13 }, ESentryType.Transaction)
            };
                foreach (var data in cachedData)
                {
                    envelopeCache.Store(data);
                    if (discaredGuid.Equals(data.EventId))
                    {
                        Assert.False(File.Exists(envelopeCache.GetEnvelopePath(data)));
                    }
                    else
                    {
                        Assert.True(File.Exists(envelopeCache.GetEnvelopePath(data)));
                    }
                }

                var retreivedCachedList = envelopeCache.Iterator();
                Assert.Equal(2, retreivedCachedList.Count);

                foreach (var data in cachedData)
                {
                    envelopeCache.Discard(data);
                    Assert.False(File.Exists(envelopeCache.GetEnvelopePath(data)));
                }
            }
        }

        [Fact]
        public void CachedEnvelope_Iterator_Return_Empty_List_If_No_Events()
        {
            using (var folder = new TempFolder())
            {
                var envelopeCache = new EnvelopeCache(new ContribSentryOptions(cacheEnable: true)
                {
                    CacheDirPath = folder.FolderName
                }); ;
                var retreivedCachedList = envelopeCache.Iterator();
                Assert.Empty(retreivedCachedList);
            }
        }

        [Fact]
        public void CachedEnvelope_Push_And_Iterate_Same_Bytes()
        {
            using (var folder = new TempFolder())
            {
                var envelopeCache = new EnvelopeCache(new ContribSentryOptions(cacheEnable: true)
                {
                    CacheDirPath = folder.FolderName
                });
                var bytes = new byte[2222];
                var rand = new Random();
                for(int i=0; i < 2222; i++)
                {
                    bytes[i] = (byte)rand.Next(0, 255);
                }
                var cache = new CachedSentryData(Guid.NewGuid(), bytes, ESentryType.Transaction);
                envelopeCache.Store(cache);
                var iterated = envelopeCache.Iterator();
                Assert.Equal(bytes, iterated[0].Data);
            }
        }
    }
}
