namespace ACogSearch.Viewmodels
{
    public class SearchViewModel
    {
        public string Query { get; set; }
        public List<FileDocument> Results { get; set; } = new();
        public List<FileDocument> UploadedFiles { get; set; } = new();
    }

}
