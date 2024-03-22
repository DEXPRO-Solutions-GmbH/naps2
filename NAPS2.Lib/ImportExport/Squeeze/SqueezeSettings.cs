namespace NAPS2.ImportExport.Squeeze;

public record SqueezeSettings
{
    public string SQZURL { get; init; } = "test";
    public string SQZClient { get; init; } = "test";
    public string SQZUserName { get; init; } = "test";
    public string SQZPassword { get; init; } = "";
    public string SQZClassID { get; init; } = "1";
}