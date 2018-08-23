
using Ascon.Pilot.Core;

namespace Ascon.Pilot.Server.Api.Contracts
{
    public interface IAuthenticationApi
    {
        DPerson Person { get;}
        string DatabaseName { get; }
        string Token { get; }
        /// <summary>
        /// логин
        /// </summary>
        /// <param name="databaseName">имя базы данных</param>
        /// <param name="login">логин пользователя</param>
        /// <param name="protectedPassword">пароль, зашифрованный с помощью Advanced Encryption Standard</param>
        /// <param name="useWindowsAuth">признак использования windows авторизации</param>
        /// <param name="licenseType">тип лицензии</param>
        void Login(string databaseName, string login, string protectedPassword, bool useWindowsAuth, int licenseType);
        /// <summary>
        /// Получить информацию о лицензии
        /// </summary>
        /// <returns>инфо</returns>
        byte[] GetLicenseInformation();

        /// <summary>
        /// Захватить лицензию с типом licenseType
        /// </summary>
        void ConsumeLicense(int licenseType);

        /// <summary>
        /// Отпустить лицензию с типом licenseType
        /// </summary>
        void ReleaseLicense(int licenseType);

    }
}
