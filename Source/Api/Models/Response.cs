namespace Api.Models
{
    public class Response
    {
        public Response()
        {
            Status = ResponseStatus.Success;
        }
        public ResponseStatus Status { get; set; }
        public string ErrorText { get; set; }
    }

    public class Response<TWhat> : Response
    {
        public TWhat Data { get; set; }
    }

    public enum ResponseStatus
    {
        Success,
        Failure
    }
}
