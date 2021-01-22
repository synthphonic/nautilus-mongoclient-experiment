using System;
using Nautilus.Data.Exceptions;

namespace Nautilus.Experiment.DataProvider.Mongo.Exceptions
{
    public class NautilusMongoDbException : NautilusDataExceptionBase
    {
        public NautilusMongoDbException()
        {
        }

        public NautilusMongoDbException(string message) : base(message)
        {
        }

        public NautilusMongoDbException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}