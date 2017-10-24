using System;
using System.Linq;
using ProtoBuf;

namespace Ascon.Pilot.Transport
{
    [ProtoContract]
    public class ProtoExceptionInfo
    {
        [ProtoMember(1)]
        public string ExceptionMessage { get; set; }

        [ProtoMember(2)]
        public Type ExceptionType { get; set; }

        public ProtoExceptionInfo()
        {
        }

        public ProtoExceptionInfo(Exception e)
        {
            ExceptionMessage = e.Message;
            ExceptionType = e.GetType();
        }
    }
}
