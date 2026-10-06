public class CreateClientRequestDto
{
    public int ClientId { get; set; }

    public string ClientName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int CreatedBy { get; set; }

    public string Action { get; set; } = "Insert";
}