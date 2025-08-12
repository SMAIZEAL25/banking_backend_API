namespace BankingApp.Application.DTOs
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public IList<string> RoleList { get; set; }
        public string Roles => string.Join(",", RoleList ?? new List<string>());
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
