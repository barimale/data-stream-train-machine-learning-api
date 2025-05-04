namespace Card.Application.CQRS.Commands
{
    public class RegisterCardRequest
    {
        public RegisterCardRequest()
        {
            //intentionally left blank
        }

        public string Input { get; set; }
        public string Ys { get; set; }
    }
}
