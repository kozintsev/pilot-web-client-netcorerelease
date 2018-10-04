using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ascon.Pilot.WebClient.Models
{
    public enum TaskState
    {
        None = 0,
        Assigned = 1,
        InProgress = 2,
        Revoked = 3,
        OnValidation = 4,
        Completed = 5
    }

//    public enum TaskKind
//    {
//        Task,
//        Agreement,
//        Acquaintance
//    }
}
