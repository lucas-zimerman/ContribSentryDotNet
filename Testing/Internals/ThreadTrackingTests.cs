using ContribSentry.Internals;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ContribSentry.TracingTest.Internals
{
    public class ThreadTrackingTests
    {
        private async Task TaskWaiter(int? expectedId, TracingWorkerContext tracking)
        {
            await Task.Delay(30);
            Assert.True(tracking.IdRegistered(0));
            Assert.Equal(expectedId, tracking.GetId());
        }

        [Fact]
        public async Task ThreadTracking_CreateUnique_Numbers()
        {
            var tracker = new TracingWorkerContext();
            var numbers = new int[1000];
            for (int i = 0; i < 300; i++)
            {
                numbers[i] = tracker.InternalNewId();
            }
            var task = new Task(() =>
            {
                new Task(() =>
                {
                    for (int i = 300; i < 600; i += 2)
                        numbers[i] = tracker.InternalNewId();
                }).Start();
                new Task(() =>
                {
                    for (int i = 301; i < 600; i += 2)
                        numbers[i] = tracker.InternalNewId();
                }).Start();
            });
            var task2 = new Thread(() =>
            {
                new Thread(() =>
                {
                    for (int i = 600; i < 1000; i += 2)
                        numbers[i] = tracker.InternalNewId();
                }).Start();
                new Thread(() =>
                {
                    for (int i = 601; i < 800; i += 2)
                        numbers[i] = tracker.InternalNewId();
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
            var tracker = new TracingWorkerContext();
            var id = tracker.InternalNewId();
            Assert.False(tracker.IdRegistered(0));
            Assert.Null(tracker.GetId());
        }



        [Fact]
        public async Task ThreadTracking_CreateTrackTask_newTask_ReturnSameId()
        {
            var tracker = new TracingWorkerContext();
            await tracker.WithIsolatedTracing(async () =>
            {
                Assert.True(tracker.IdRegistered(0));
                var idEqual = tracker.GetId();
                Task task = new Task(async () =>
                {
                    await Task.Delay(10);
                    Assert.True(tracker.IdRegistered(0));
                    Assert.Equal(idEqual, tracker.GetId());
                });
                task.Start();
                await task;

            },0);
            Assert.False(tracker.IdRegistered(0));
        }

        [Fact]
        public async Task ThreadTracking_CreateTrackTask_Using_UnsafeId()
        {
            var tracker = new TracingWorkerContext();
            var unsafeId = tracker.InternalNewId();
            await tracker.WithIsolatedTracing(async () =>
            {
                Assert.True(tracker.IdRegistered(0));
                var idEqual = tracker.GetId();
                Task task = new Task(async () =>
                {
                    await Task.Delay(10);
                    Assert.True(tracker.IdRegistered(0));
                    Assert.Equal(idEqual, tracker.GetId());
                    Assert.Equal(unsafeId, tracker.GetId());
                });
                task.Start();
                await task;

            }, unsafeId);
            Assert.False(tracker.IdRegistered(0));
        }

        [Fact]
        public void ThreadTracking_Different_ThreadsCallbacks_Async_Return_DifferentIds()
        {
            var tracker = new TracingWorkerContext();
            int?[] ids = new int?[3];
            var Semaphores = new Semaphore[3] { new Semaphore(0, 1), new Semaphore(0, 1), new Semaphore(0, 1) };
            new Thread(async () =>
            {
                var myId = tracker.InternalNewId();
                await tracker.WithIsolatedTracing(async () =>
                {

                    ids[0] = tracker.GetId();
                    await TaskWaiter(ids[0], tracker);
                    Assert.Equal(ids[0], tracker.GetId());
                    Semaphores[0].Release();
                }, myId);
            }).Start();
            new Thread(async () =>
            {
                var myId = tracker.InternalNewId();
                await tracker.WithIsolatedTracing(async () =>
                {

                    ids[1] = tracker.GetId();
                    await TaskWaiter(ids[1], tracker);
                    Assert.Equal(ids[1], tracker.GetId());
                    Semaphores[1].Release();
                }, myId);
            }).Start();
            new Thread(async () =>
                {
                    var myId = tracker.InternalNewId();
                    await tracker.WithIsolatedTracing(async () =>
                    {

                        ids[2] = tracker.GetId();
                        await TaskWaiter(ids[2], tracker);
                        Assert.Equal(ids[2], tracker.GetId());
                        Semaphores[2].Release();
                    }, myId);
                }).Start();
            Semaphores[0].WaitOne();
            Semaphores[1].WaitOne();
            Semaphores[2].WaitOne();

            Assert.False(tracker.IdRegistered(0));
            Assert.NotNull(ids[0]);
            Assert.NotNull(ids[1]);
            Assert.NotNull(ids[2]);
            Assert.NotEqual(ids[0], ids[1]);
            Assert.NotEqual(ids[2], ids[1]);
        }

        [Fact]
        private async Task ThreadTracking_On_Function_Error()
        {
            var tracker = new TracingWorkerContext();
            bool receivedError = false;
            try
            {
                await tracker.WithIsolatedTracing(() =>
                {
                    throw new Exception(".");
                },0);
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
