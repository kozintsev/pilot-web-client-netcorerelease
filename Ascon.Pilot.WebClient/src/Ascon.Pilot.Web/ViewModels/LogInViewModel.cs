using System.ComponentModel.DataAnnotations;

namespace Ascon.Pilot.Web.ViewModels
{
    /// <summary>
    /// Модель пользователя    
    /// </summary>
    public class LogInViewModel
    {
        /// <summary>
        ///Имя пользвоателя для входа в систему.
        /// </summary>
        [Required]
        [Display(Name = "Имя пользователя")]
        public string Login { get; set; }

        /// <summary>
        /// Пароль для входа в систему.
        /// </summary>
        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
