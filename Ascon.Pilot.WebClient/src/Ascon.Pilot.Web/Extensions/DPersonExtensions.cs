﻿using Ascon.Pilot.DataClasses;

namespace Ascon.Pilot.Web.Extensions
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
