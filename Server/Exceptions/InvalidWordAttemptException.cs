using System;

namespace Server.Exceptions {
    /// <summary>
    /// Thrown when a user attempts to submit an invalid or malformed word guess.
    /// </summary>
    [Serializable]
    public class InvalidWordAttemptException : Exception {
        /// <summary>
        /// Creates a new <see cref="InvalidWordAttemptException"/> with a default message.
        /// </summary>
        public InvalidWordAttemptException()
            : base("The attempted word is invalid.") { }

        /// <summary>
        /// Creates a new <see cref="InvalidWordAttemptException"/> with a custom message.
        /// </summary>
        public InvalidWordAttemptException(string? message)
            : base(message) { }

        /// <summary>
        /// Creates a new <see cref="InvalidWordAttemptException"/> with a message and inner exception.
        /// </summary>
        public InvalidWordAttemptException(string? message, Exception? innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected InvalidWordAttemptException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
