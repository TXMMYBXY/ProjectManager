using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Entities.Models;

public abstract class EntityBase
{
    [Key]
    public int Id { get; set; }
}