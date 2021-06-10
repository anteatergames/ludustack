using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuduStack.Web.Exceptions
{
    [Serializable]
    public class CustomApplicationException : ApplicationException
    {
        private readonly string resourceName;
        private readonly IList<string> validationErrors;

        public string ResourceName => resourceName;

        public IList<string> ValidationErrors => validationErrors;

        public CustomApplicationException()
        {
        }

        public CustomApplicationException(string message) : base(message)
        {
        }

        public CustomApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CustomApplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            resourceName = info.GetString("CustomApplicationException.ResourceName");
            validationErrors = (IList<string>)info.GetValue("CustomApplicationException.ValidationErrors", typeof(IList<string>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("CustomApplicationException.ResourceName", ResourceName);
            info.AddValue("CustomApplicationException.ValidationErrors", ValidationErrors, typeof(IList<string>));
        }
    }
}