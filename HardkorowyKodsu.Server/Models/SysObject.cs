using System.ComponentModel.DataAnnotations;
public class SysObject
{
    [Key]
    public int Id { get; set; }

    public string name { get; set; }

    public string type { get; set; }
}