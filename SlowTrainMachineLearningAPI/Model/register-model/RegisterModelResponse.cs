namespace Card.API.Model.order
{
    public class RegisterModelResponse
    {
        public RegisterModelResponse()
        {
            // intentionally left blank
        }

        public RegisterModelResponse(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
