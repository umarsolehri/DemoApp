namespace Domain.Enums
{
    public enum ResponseEnum
    {
        [Description("Request completed successfully.")]
        Success = 1,
        [Description("Data duplication found.")]
        Duplicate,
        [Description("Request failed.")]
        Failed,
        [Description("Data Not Found.")]
        NotFound,
        [Description("Data Deleted.")]
        Deleted
    }
}
