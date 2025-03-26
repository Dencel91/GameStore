namespace AuthService.Dtos
{
    public class CreateUserRequest
    {
        public required string Name { get; set; }

        public required string Password { get; set; }

        public required string PasswordConfirmation { get; set; }
    }
}
