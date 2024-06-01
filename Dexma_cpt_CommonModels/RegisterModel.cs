namespace Dexma_cpt_CommonModels
{
    public class RegisterModel
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? Nickname { get; set; }
        public string Phone { get; set; }
        public string? Password { get; set; }
        public string? OldPassword { get; set; }
        public RegisterModel() { }
    }
}
