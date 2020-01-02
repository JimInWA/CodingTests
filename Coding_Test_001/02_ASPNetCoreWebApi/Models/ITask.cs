namespace ASPNetCoreWebApi.Models
{
    public interface ITask
    {
        int Id { get; set; }
        string Description { get; set; }
        bool IsComplete { get; set; }
    }
}