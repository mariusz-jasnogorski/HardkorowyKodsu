using System.ComponentModel.DataAnnotations;

public class SysColumn
{
    [Key]
    public int ordinal_position { get; set; }

    public string column_name { get; set; }

    public string data_type { get; set; }

    public string table_name { get; set; }

    public string table_schema { get; set; }

}
