namespace Card.API.Model.order
{
    public class RegisterCardResponse
    {
        public RegisterCardResponse()
        {
            // intentionally left blank
        }

        public RegisterCardResponse(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
