using System.Runtime.Serialization;

namespace ContribSentry.Enums
{
    public enum ESessionState
    {
        [EnumMember(Value = "ok")]
        Ok,

        [EnumMember(Value = "exited")]
        Exited,

        [EnumMember(Value = "crashed")]
        Crashed,

        [EnumMember(Value = "abnormal")]
        Abnormal // not currently used in this SDK.
    }
}
