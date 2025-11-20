namespace Server.Exceptions {
    [Serializable]
    internal class InvalidWordAttemptException : Exception {
        public InvalidWordAttemptException() {
        }

        public InvalidWordAttemptException(string? message) : base(message) {
        }

        public InvalidWordAttemptException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}