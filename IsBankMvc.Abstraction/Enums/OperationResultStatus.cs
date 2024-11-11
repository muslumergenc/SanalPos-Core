namespace IsBankMvc.Abstraction.Enums
{
    public enum OperationResultStatus : byte
    {
        Pending = 1,
        Success = 2,
        NotFound = 3,
        Duplicate = 4,
        Rejected = 5,
        UnAuthorized = 6,
        Validation = 7,
        Failed = 8,
        Expire = 11
    }
}
