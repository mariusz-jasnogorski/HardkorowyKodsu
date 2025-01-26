using System.ComponentModel.DataAnnotations;
public class SysObject
{
    [Key]
    public int object_id { get; set; }

    public string name { get; set; }

    public string type { get; set; }
}