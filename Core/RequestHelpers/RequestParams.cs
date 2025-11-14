namespace Core.RequestHelpers;

public class RequestParams
{
    private readonly int maxPageSize = 10;
    private int pageSize = 5;

    public int PageSize
    {
        set => pageSize = value > maxPageSize || value < 1 ? maxPageSize : value;
        get => pageSize;
    }

    public int PageNumber { get; set; }

    public string? OrderBy { get; set; } = "joiningdatedesc";
    
    public string? SearchTerm { get; set; }

    public string? IsActive { get; set; }
}