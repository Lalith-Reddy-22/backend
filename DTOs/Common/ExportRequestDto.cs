public class ExportRequestDto
{
    public string FileName { get; set; } = "";

    public string Format { get; set; } = "csv"; // csv,pdf,excel

    public Dictionary<string, string>? Headers { get; set; }
}