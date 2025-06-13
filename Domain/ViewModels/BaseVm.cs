namespace Domain.ViewModels
{
    public class BaseVm
    {
        public DateOnly? CreatedDate { get; set; }
        public DateOnly? UpdatedDate { get; set; }
        public TimeOnly? CreatedTime { get; set; }
        public TimeOnly? UpdatedTime { get; set; }
    }
}
