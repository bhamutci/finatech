namespace FinaTech.Application.Configuration;

public record ApplicationOptions
{
  public const string Name = "Application";

  public string CorsOrigins { get; set; }
}
