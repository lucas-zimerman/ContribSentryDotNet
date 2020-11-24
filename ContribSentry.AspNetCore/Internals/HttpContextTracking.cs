using ContribSentry.Interface;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContribSentry.AspNetCore.Internals
{
    internal class HttpContextTracking : ITracingWorkerContext
    {
        private readonly string _requestIdField = "RequestId";

        internal List<KeyValuePair<string, int>> _contextReference = new List<KeyValuePair<string, int>>();

        private object _lock = new object();
        private int _value;
        private int InternalNewId()
        {
            lock (_lock)
            {
                int i = _value++;
                return i;
            }
        }

        private string GetrequestId()
        {
            string id = null;
            SentrySdk.ConfigureScope((scope) => { id = scope.Tags[_requestIdField]; });
            return id;
        }

        public int? GetId()
        {
            var id = GetrequestId();
            if (id == null)
                return null;
            return _contextReference.FirstOrDefault(p => p.Key == id).Value;
        }

        public bool IdRegistered(int id)
        {
            return _contextReference.Any(v => v.Value == id);
        }

        /// <summary>
        /// Link the integer unique id to the Request Id
        /// </summary>
        /// <param name="id"></param>
        public void AssociateId(int id)
        {
            _contextReference.Add(new KeyValuePair<string, int>(GetrequestId(), id));
        }

        public int ReserveNewId()
        {
            return InternalNewId();
        }

        public bool UnsetId(int id)
        {
            return _contextReference.Remove(_contextReference.First(p => p.Value == id));
        }

        public Task WithIsolatedTracing(Func<Task> test, int id)
        {
            return test.Invoke();
        }
    }
}
