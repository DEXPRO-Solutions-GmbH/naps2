namespace NAPS2.ImportExport.Squeeze;

public record SqueezeSettings
{
    private string _sqzurl = "";
    public string SQZURL
    {
        get
        {
            const string appendString = "/api/v2/documents";
            // Ensure there is no trailing slash before appending
            if (_sqzurl.EndsWith("/"))
            {
                _sqzurl = _sqzurl.TrimEnd('/');
            }
            if (!_sqzurl.EndsWith(appendString))
            {
                return _sqzurl + appendString;
            }
            return _sqzurl;
        }
        set
        {
            // Ensure the value does not end with a slash
            _sqzurl = (value ?? "").TrimEnd('/');
        }
    }

    public string SQZClient { get; init; } = "";
    public string SQZUserName { get; init; } = "";
    public string SQZPassword { get; init; } = "";
    public int SQZClassID { get; init; } = 1;
}