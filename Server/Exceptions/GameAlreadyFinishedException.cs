namespace Server.Exceptions {
    public class GameAlreadyFinishedException : Exception {
        public GameAlreadyFinishedException(string message) : base(message) { }

    }
}
