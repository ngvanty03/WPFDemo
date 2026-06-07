using CommunityToolkit.Mvvm.ComponentModel;
public partial class PagingParameters : ObservableObject
{
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private bool _hasNextPage;
    [ObservableProperty] private bool _hasPrevPage;

    public int PageSize { get; set; } = 20;

    public void UpdateState(int totalCount)
    {        
        PageSize= PageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
        HasPrevPage = CurrentPage > 1;
        HasNextPage = CurrentPage < TotalPages;
    }
    public void NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
        }
    }
    public void PrevPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
        }
    }
}