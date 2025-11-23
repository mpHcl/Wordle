using System;

namespace Server.Exceptions {
    /// <summary>
    /// Thrown when a requested object cannot be found in storage or the database.
    /// </summary>
    [Serializable]
    public class ObjectNotFoundException : Exception {
        /// <summary>
        /// Creates a new <see cref="ObjectNotFoundException"/> with a custom message.
        /// </summary>
        public ObjectNotFoundException(string message)
            : base(message) { }

        /// <summary>
        /// Creates a new <see cref="ObjectNotFoundException"/> with a default message.
        /// </summary>
        public ObjectNotFoundException()
            : base("The requested object was not found.") { }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected ObjectNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
