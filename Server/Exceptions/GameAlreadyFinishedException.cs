using System;

namespace Server.Exceptions {
    /// <summary>
    /// Thrown when a game action is attempted after the game has already finished.
    /// </summary>
    [Serializable]
    public class GameAlreadyFinishedException : Exception {
        /// <summary>
        /// Creates a new <see cref="GameAlreadyFinishedException"/> with a custom message.
        /// </summary>
        public GameAlreadyFinishedException(string message)
            : base(message) { }

        /// <summary>
        /// Creates a new <see cref="GameAlreadyFinishedException"/> using a default message.
        /// </summary>
        public GameAlreadyFinishedException()
            : base("This game has already finished and cannot accept further actions.") { }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected GameAlreadyFinishedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
