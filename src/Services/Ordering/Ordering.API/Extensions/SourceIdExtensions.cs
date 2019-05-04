using System;
using EventFlow.Core;

namespace Ordering.API.Extensions
{
    public static class SourceIdExtensions
    {
        public static SourceId ToSourceId(this Guid guid)
        {
            return new SourceId(guid.ToString());
        }
    }
}