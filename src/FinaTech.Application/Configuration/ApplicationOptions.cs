namespace FinaTech.Application.Configuration;

public record ApplicationOptions: IApplicationOptions
{
  public string CorsOrigins { get; set; }
}
