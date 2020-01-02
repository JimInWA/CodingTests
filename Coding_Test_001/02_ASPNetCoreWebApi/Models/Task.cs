namespace ASPNetCoreWebApi.Models
{
    public class Task : ITask
    {
        // ToDo: Shared libary for shared models
        // ToDo: for the Description property, match DB restriction of 400 character length


        public int Id { get; set; }

        public string Description { get; set; }

        public bool IsComplete { get; set; }
    }
}
