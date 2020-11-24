using ContribSentry.Extensibility;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ContribSentry.TracingTest
{
    public class SentryTracingTests
    {
        [Fact]
        public void Ctor_ValidSentryTracing()
        {
            //Act
            var sentryTracing = new SentryTracing(null, 0);

            //Assert
            Assert.True(sentryTracing != null);
        }

        [Fact]
        public void StartChild_AddsChild()
        {
            //Arrange
            var sentryTracing = new SentryTracing(null, 0);

            //Act
            var span = sentryTracing.StartChild("name","name");

            //Assert
            Assert.Contains(span, sentryTracing.Spans);

        }

        [Fact]
        public void GetSpan_EmptySpan_DisabledSpan()
        {
            //Arrange
            var sentryTracing = new SentryTracing(null, 0);

            //Act
            var span = sentryTracing.GetSpan("name");

            //Assert
            Assert.IsType<DisabledSpan>(span);
        }

        [Fact]
        public void GetSpan_OpExist_SpanWithOp()
        {
            //Arrange
            var sentryTracing = new SentryTracing(null, 0);
            sentryTracing.StartChild("name", "name");

            //Act
            var span = sentryTracing.GetSpan("name");

            //Assert
            Assert.IsType<Span>(span);
            Assert.Equal("name", span.Op);
        }

        [Fact]
        public void GetCurrentSpan_NoSpan_DisabledSpan()
        {
            //Arrange
            var sentryTracing = new SentryTracing(null, 0);

            //Act
            var span = sentryTracing.GetCurrentSpan();

            //Assert
            Assert.IsType<DisabledSpan>(span);
        }

        [Fact]
        public void GetCurrentSpan_FirstSpanOpen_FirstSpan()
        {
            //Arrange
            var sentryTracing = new SentryTracing(null, 0);
            sentryTracing.StartChild("name", "name");

            //Act
            var span = sentryTracing.GetCurrentSpan();

            //Assert
            Assert.IsType<Span>(span);
            Assert.Equal("name", span.Op);
        }

        [Fact]
        public void GetCurrentSpan_FirstSpanOpenAndSecondIsChildofChild_FirstSpan()
        {
            //Arrange
            var sentryTracing = new SentryTracing(null, 0);
            var parent = sentryTracing.StartChild("name", "name");
            parent.StartChild("aaa");

            //Act
            var span = sentryTracing.GetCurrentSpan();

            //Assert
            Assert.IsType<Span>(span);
            Assert.Equal("name", span.Op);
        }

        [Fact]
        public void Finish_NoSpan_CapturesEvent()
        {
            //Arrange
            ContribSentrySdk.Init(new ContribSentryOptions());
            ContribSentrySdk.Options.TracesSampleRate = 1.0;

            var sentryTracing = new SentryTracing(null, 0);
            
            //Act
            sentryTracing.Finish();

            ContribSentrySdk.Close();
        }
    }
}
