namespace Card.Application.CQRS.Commands
{
    public class RegisterCardRequest
    {
        public RegisterCardRequest()
        {
            //intentionally left blank
        }

        public string AccountNumber { get; set; }
        public string PIN { get; set; }
        public string SerialNumber { get; set; }
    }
}
