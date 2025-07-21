namespace TeamCollaborationWebAPI.Models
{
    public class RegisterModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RoleType { get; set; } = "User"; // e.g., Manager, Developer
    }
}
