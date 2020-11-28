using ContribSentry.Enums;
using System;
using System.Collections.Generic;

namespace ContribSentry.Internals
{
    internal static class SpanStatus
    {
        internal static Dictionary<ESpanStatus, string> SpanStatusDictionary = new Dictionary<ESpanStatus, string>(){
            { ESpanStatus.Ok, "ok"},
            { ESpanStatus.DeadlineExceeded, "deadline_exceeded"},
            { ESpanStatus.Unauthenticated, "unauthenticated"},
            { ESpanStatus.PermissionDenied, "permission_denied"},
            { ESpanStatus.NotFound, "not_found"},
            { ESpanStatus.ResourceExhausted, "resource_exhausted"},
            { ESpanStatus.InvalidArgument, "invalid_argument"},
            { ESpanStatus.Unimplemented, "unimplemented"},
            { ESpanStatus.Unavailable, "unavailable"},
            { ESpanStatus.InternalError, "internal_error"},
            { ESpanStatus.UnknownError, "unknown_error"},
            { ESpanStatus.Cancelled, "cancelled"},
            { ESpanStatus.AlreadyExists, "already_exists"},
            { ESpanStatus.FailedPrecondition, "failed_precondition"},
            { ESpanStatus.Aborted, "aborted"},
            { ESpanStatus.OutOfRange, "out_of_range"},
            { ESpanStatus.DataLoss, "data_loss"},
        };


        internal static ESpanStatus FromHttpStatusCode(int? httpStatus)
        {
            if (httpStatus == null)
                return ESpanStatus.UnknownError;

            if (httpStatus < 400)
            {
                return ESpanStatus.Ok;
            }

            if (httpStatus >= 400 && httpStatus < 500)
            {
                switch (httpStatus)
                {
                    case 401:
                        return ESpanStatus.Unauthenticated;
                    case 403:
                        return ESpanStatus.PermissionDenied;
                    case 404:
                        return ESpanStatus.NotFound;
                    case 409:
                        return ESpanStatus.AlreadyExists;
                    case 413:
                        return ESpanStatus.FailedPrecondition;
                    case 429:
                        return ESpanStatus.ResourceExhausted;
                    default:
                        return ESpanStatus.InvalidArgument;
                }
            }

            if (httpStatus >= 500 && httpStatus < 600)
            {
                switch (httpStatus)
                {
                    case 501:
                        return ESpanStatus.Unimplemented;
                    case 503:
                        return ESpanStatus.Unavailable;
                    case 504:
                        return ESpanStatus.DeadlineExceeded;
                    default:
                        return ESpanStatus.InternalError;
                }
            }

            return ESpanStatus.UnknownError;
        }

        internal static ESpanStatus FromException(Exception exception)
        {
            if( exception is OperationCanceledException)
            {
                return ESpanStatus.DeadlineExceeded;
            }
            else if( exception is NotImplementedException)
            {
                return ESpanStatus.Unimplemented;
            }
            else if( exception is NullReferenceException ||
                exception is ArgumentNullException)
            {
                return ESpanStatus.NotFound;
            }
            else if( exception is InsufficientMemoryException ||
                exception is InsufficientExecutionStackException)
            {
                return ESpanStatus.ResourceExhausted;
            }
            return ESpanStatus.UnknownError;
        }

    }
}
