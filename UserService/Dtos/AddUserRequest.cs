namespace UserService.Dtos
{
    public class AddUserRequest
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
