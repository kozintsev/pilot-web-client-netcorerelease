using Ascon.Pilot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ascon.Pilot.WebClient.Extensions
{
    public static class DPersonExtensions
    {
        public static string GetActualName(this DPerson person)
        {
            if (person == null)
                return "Пользователь не найден";
            return !string.IsNullOrEmpty(person.DisplayName) ? person.DisplayName : person.Login;
        }
    }
}
