using ContribSentry.Interface;
using ContribSentry.Internals;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ContribSentry.Test.TestCase
{
    public class TransactionWorkerTests
    {
        [Fact(Timeout = 6000)]
        public async Task StartChildren_WithParents_MatchesParents()
        {
            var worker = new TransactionWorker();
            int i = 0;
            var spansWithZeroParents = 10;
            var spansWithTenParents = 1;
            var spansWithOneParent = (10 + 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1) - spansWithTenParents - spansWithZeroParents;
            var spansTotal = spansWithTenParents + spansWithZeroParents + spansWithOneParent;
            var tasks = new Task[10];
            await worker.StartTransaction("Xunit", null, (tracing) =>
             {
                 DoChild.Invoke(worker, 0);
                 DoChild.Invoke(worker, 1);
                 DoChild.Invoke(worker, 2);
                 DoChild.Invoke(worker, 3);
                 DoChild.Invoke(worker, 4);
                 DoChild.Invoke(worker, 5);
                 DoChild.Invoke(worker, 6);
                 DoChild.Invoke(worker, 7);
                 DoChild.Invoke(worker, 8);
                 DoChild.Invoke(worker, 9);

                 Task.Delay(300).Wait();
                 Assert.Equal(spansTotal, tracing.Spans.Count);

                 foreach (var span in tracing.Spans)
                 {
                     var id = int.Parse(span.Op);
                     var expected = 100 - (id + 1) * 10;
                     var value = span.Timestamp.Value.ToUnixTimeMilliseconds() - span.StartTimestamp.Value.ToUnixTimeMilliseconds();
                     Assert.True(expected - 10 <= value, $"id {id} expected {expected} <= {value}");
                 }

                 //Check if the first Span is parent of 10 spans
                 Assert.Equal(10, tracing.Spans.Where(p => p.ParentSpanId == ((SentryTracing)tracing).Trace.SpanId).Count());

                 //Check if All Spans but not the first have one parent
                 int emptySpans = 0;
                 for (int i = 1; i < tracing.Spans.Count(); i++)
                 {
                     var spans = tracing.Spans.Where(s => s.ParentSpanId == tracing.Spans[i].SpanId);
                     if (spans.Count() == 0)
                     {
                         emptySpans++;
                     }
                     else
                     {
                         Assert.Single(spans);
                     }
                 }

                 Assert.Equal(spansWithOneParent + spansWithTenParents, 
                     tracing.Spans.Where(span => tracing.Spans.Where(parent => parent.SpanId == span.ParentSpanId).Count() == 1).Count());

                 //Check if the last Spans are not parents.
                 Assert.Equal(spansWithZeroParents, tracing.Spans.Where(span => tracing.Spans.Where(parent => parent.SpanId == span.ParentSpanId).Count() == 0).Count());
             });
        }

        public static Action<ITransactionWorker, int> DoChild = (workerTmp, stage) =>
        {
            var worker = (TransactionWorker)workerTmp;
            if (stage < 10)
            {
                worker.StartChild("description", stage.ToString(), (span) =>
                {
                    Assert.Equal(span, worker.CurrentSpan.Value);
                    DoJob(10).Wait();
                    DoChild.Invoke(worker, stage + 1);
                    Assert.Equal(span, worker.CurrentSpan.Value);
                }).Wait();

            }
        };

        public static Task DoJob(int ms)
            => Task.Delay(ms);
    }
}
