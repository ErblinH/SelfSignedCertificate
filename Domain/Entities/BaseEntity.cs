namespace Domain.Entities;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreationDateTime { get; set; }
    public bool Deleted { get; set; }
}