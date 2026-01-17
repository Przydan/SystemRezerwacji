
namespace Web.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsLockedOut { get; set; }
    }
}
