using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ContribSentry.Enums;
using ContribSentry.Extensions;
using ContribSentry.Interface;
using ContribSentry.Internals;

namespace ContribSentry
{
    public class Span : ISpanBase
    {
        #region Properties

        [JsonIgnore]
        private bool _isRequest;
        [JsonIgnore]
        private bool _finished;

        [JsonIgnore]
        internal List<ISpanBase> Spans { get; set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("op")]
        public string Op { get; private set; }

        [JsonProperty("span_id")]
        public string SpanId { get; private set; }

        [JsonProperty("parent_span_id")]
        public string ParentSpanId { get; private set; }

        [JsonProperty("start_timestamp")]
        public DateTimeOffset? StartTimestamp { get; private set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset? Timestamp { get; private set; }

        [JsonProperty("trace_id")]
        public string TraceId { get; private set; }

        [JsonIgnore]
        public bool Error { get; private set; }

        /// <summary>
        /// If set, it'll be based on the ESpanStatus result
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; private set; }

        #endregion
        public Span(string traceId, string spanId, string description, string op = null)
        {
            StartTimestamp = DateTimeOffset.UtcNow;
            Timestamp = StartTimestamp; //In case of not closing the equal time will indicate a problem.
            Description = description;
            Op = op ?? description;
            SpanId = Guid.NewGuid().LimitLength();
            TraceId = traceId;
            ParentSpanId = spanId;
        }

        public Span(string traceId, string spanId, string url, ESpanRequest requestType)
        {
            StartTimestamp = DateTimeOffset.UtcNow;
            Timestamp = StartTimestamp; //In case of not closing the equal time will indicate a problem.
            Description = url;
            Op = requestType.ToString().ToUpper();
            SpanId = Guid.NewGuid().LimitLength();
            TraceId = traceId;
            ParentSpanId = spanId;
            _isRequest = true;
        }

        public ISpanBase StartChild(string description, string op = null)
        {
            var span = new Span(TraceId, SpanId, description, op);
            span.Spans = Spans;
            Spans.Add(span);
            return span;
        }

        public ISpanBase StartChild(string url, ESpanRequest requestType)
        {
            var span = new Span(TraceId, SpanId, url, requestType);
            span.Spans = Spans;
            Spans.Add(span);
            return span;
        }

        public void Finish()
        {
            if (!_finished)
            {
                _finished = true;
                Status = SpanStatus.SpanStatusDictionary[ESpanStatus.Ok];
                Timestamp = DateTimeOffset.UtcNow;
            }
            else
                Timestamp = DateTimeOffset.UtcNow;
        }

        public void Finish(int? httpCode)
        {
            if (!_finished)
            {
                _finished = true;
                Status = SpanStatus.SpanStatusDictionary[SpanStatus.FromHttpStatusCode(httpCode)];
                Timestamp = DateTimeOffset.UtcNow;

                if(Status != "ok")
                {
                    Error = true;
                }
            }
        }

        public void Finish(Exception exception)
        {
            if (!_finished)
            {
                _finished = true;
                Status = SpanStatus.SpanStatusDictionary[SpanStatus.FromException(exception)];
            }
        }

        internal void Finish(string status)
        {
            if (!_finished)
            {
                _finished = true;
                Status = "idle";
                Timestamp = DateTimeOffset.UtcNow;
            }
        }

        public void GetParentSpans(List<ISpanBase> spans)
        {
            Spans = spans;
        }
    }
}