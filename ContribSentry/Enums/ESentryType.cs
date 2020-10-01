using System.Runtime.Serialization;

namespace ContribSentry.Enums
{
    public enum ESentryType
    {
        [EnumMember(Value = "session")]
        Session,

        [EnumMember(Value = "event")]
        Event,

        [EnumMember(Value = "attachment")]
        Attachment,

        [EnumMember(Value = "attachment")]
        Transaction,

        [EnumMember(Value = "unknown")]
        Unknown,

        [EnumMember(Value = "currentSession")]
        CurrentSession
    }
}
