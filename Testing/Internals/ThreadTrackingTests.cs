using sentry_dotnet_transaction_addon.Internals;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Testing.Internals
{
    public class ThreadTrackingTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public ThreadTrackingTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private async Task TaskWaiter(int? expectedId, ThreadTracking tracking)
        {
            await Task.Delay(30);
            Assert.True(tracking.Created);
            Assert.Equal(expectedId, tracking.Id);
        }

        [Fact]
        public async Task ThreadTracking_CreateUnique_Numbers()
        {
            var tracker = new ThreadTracking();
            var numbers = new int[1000];
            for (int i = 0; i < 300; i++)
            {
                numbers[i] = ThreadTracking.InternalNewId();
            }
            var task = new Task(() =>
            {
                new Task(() =>
                {
                    for (int i = 300; i < 600; i += 2)
                        numbers[i] = ThreadTracking.InternalNewId();
                }).Start();
                new Task(() =>
                {
                    for (int i = 301; i < 600; i += 2)
                        numbers[i] = ThreadTracking.InternalNewId();
                }).Start();
            });
            var task2 = new Thread(() =>
            {
                new Thread(() =>
                {
                    for (int i = 600; i < 1000; i += 2)
                        numbers[i] = ThreadTracking.InternalNewId();
                }).Start();
                new Thread(() =>
                {
                    for (int i = 601; i < 800; i += 2)
                        numbers[i] = ThreadTracking.InternalNewId();
                }).Start();
            });
            task.Start();
            task2.Start();
            await Task.Delay(100);
            Assert.True(numbers.Distinct().Count() > 0);
        }


        [Fact]
        public void ThreadTracking_UnsafeTracking_Not_Created()
        {
            var tracker = new ThreadTracking();
            var id = tracker.StartUnsafeTrackingId();
            Assert.False(tracker.Created);
            Assert.Null(tracker.Id);
        }



        [Fact]
        public async Task ThreadTracking_CreateTrackTask_newTask_ReturnSameId()
        {
            var tracker = new ThreadTracking();
            await tracker.StartCallbackTrackingIdAsync(async () =>
            {
                Assert.True(tracker.Created);
                var idEqual = tracker.Id;
                Task task = new Task(async () =>
                {
                    await Task.Delay(10);
                    Assert.True(tracker.Created);
                    Assert.Equal(idEqual, tracker.Id);
                });
                task.Start();
                await task;

            });
            Assert.False(tracker.Created);
        }

        [Fact]
        public async Task ThreadTracking_CreateTrackTask_Using_UnsafeId()
        {
            var tracker = new ThreadTracking();
            var unsafeId = tracker.StartUnsafeTrackingId();
            await tracker.StartCallbackTrackingIdAsync(async () =>
            {
                Assert.True(tracker.Created);
                var idEqual = tracker.Id;
                Task task = new Task(async () =>
                {
                    await Task.Delay(10);
                    Assert.True(tracker.Created);
                    Assert.Equal(idEqual, tracker.Id);
                    Assert.Equal(unsafeId, tracker.Id);
                });
                task.Start();
                await task;

            }, unsafeId);
            Assert.False(tracker.Created);
        }

        [Fact]
        public void ThreadTracking_Different_ThreadsCallbacks_Async_Return_DifferentIds()
        {
            var tracker = new ThreadTracking();
            int?[] ids = new int?[3];
            var Semaphores = new Semaphore[3] { new Semaphore(0, 1), new Semaphore(0, 1), new Semaphore(0, 1) };
            new Thread(async () =>
            {
                await tracker.StartCallbackTrackingIdAsync(async () =>
                {

                    ids[0] = tracker.Id;
                    await TaskWaiter(ids[0], tracker);
                    Assert.Equal(ids[0], tracker.Id);
                    Semaphores[0].Release();
                });
            }).Start();
            new Thread(async () =>
            {
                await tracker.StartCallbackTrackingIdAsync(async () =>
                {

                    ids[1] = tracker.Id;
                    await TaskWaiter(ids[1], tracker);
                    Assert.Equal(ids[1], tracker.Id);
                    Semaphores[1].Release();
                });
            }).Start();
            new Thread(async () =>
                {
                    await tracker.StartCallbackTrackingIdAsync(async () =>
                    {

                        ids[2] = tracker.Id;
                        await TaskWaiter(ids[2], tracker);
                        Assert.Equal(ids[2], tracker.Id);
                        Semaphores[2].Release();
                    });
                }).Start();
            Semaphores[0].WaitOne();
            Semaphores[1].WaitOne();
            Semaphores[2].WaitOne();

            Assert.False(tracker.Created);
            Assert.NotNull(ids[0]);
            Assert.NotNull(ids[1]);
            Assert.NotNull(ids[2]);
            Assert.NotEqual(ids[0], ids[1]);
            Assert.NotEqual(ids[2], ids[1]);
        }

        [Fact]
        private async Task ThreadTracking_On_Function_Error()
        {
            var tracker = new ThreadTracking();
            bool receivedError = false;
            try
            {
                await tracker.StartCallbackTrackingIdAsync(() =>
                {
                    throw new Exception(".");
                });
            }
            catch
            {
                receivedError = true;
            }
            finally
            {
                Assert.True(receivedError);
            }
        }
    }
}
