namespace Domain.Dtos.Shared
{
    public class CommandResponse
    {
        private CommandResponse() { }

        public int Status { get; private set; }
        public String Error { get; private set; }
        public dynamic Data { get; private set; }

        public static CommandResponse Failure(int status, string error = null) => new CommandResponse { Status = status, Error = error };

        public static CommandResponse Success(dynamic data = null) => new CommandResponse { Status = 200, Data = data };
    }
}
