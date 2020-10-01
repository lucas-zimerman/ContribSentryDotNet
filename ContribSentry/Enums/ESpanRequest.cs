using System.Runtime.Serialization;

namespace ContribSentry.Enums
{
    public enum ESpanRequest
    {
        [EnumMember(Value = "get")]
        Get,

        [EnumMember(Value = "post")]
        Post,

        [EnumMember(Value = "put")]
        Put,

        [EnumMember(Value = "delete")]
        Delete,

        [EnumMember(Value = "patch")]
        Patch
    }
}