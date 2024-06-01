using System.ComponentModel.DataAnnotations;

namespace Dexma_cpt_CommonModels
{
    public class LoginModel : InitialModel
    {
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public LoginModel() { }
    }
}
