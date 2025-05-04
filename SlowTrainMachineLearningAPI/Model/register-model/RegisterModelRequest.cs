namespace Card.Application.CQRS.Commands
{
    public class RegisterModelRequest
    {
        public RegisterModelRequest()
        {
            //intentionally left blank
        }

        public string Xs { get; set; }
        public string Ys { get; set; }
    }
}
