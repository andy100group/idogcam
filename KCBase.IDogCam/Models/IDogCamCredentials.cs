using System.ComponentModel.DataAnnotations;

public class IDogCamCredentials
{
    [Required]
    public string ApiKey { get; set; }
    [Required]
    public string KennelId { get; set; }
    [Required]
    public string ErpCode { get; set; }
}