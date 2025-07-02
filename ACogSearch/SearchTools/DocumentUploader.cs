using ACogSearch.SearchTools;
using Azure;
using Azure.Search.Documents;
using Microsoft.Extensions.Options;

public class DocumentUploader
{
    private readonly AzureSearchOptions _options;
    private readonly SearchClient _client;

    public DocumentUploader(IOptions<AzureSearchOptions> options)
    {
        _options = options.Value;
        _client = new SearchClient(new Uri(_options.Endpoint), _options.IndexName, new AzureKeyCredential(_options.ApiKey));
    }

    public async Task UploadDocumentAsync(FileDocument doc)
    {
        if (await DocumentExistsAsync(doc.FileName))
        {
            Console.WriteLine($"Skippar: {doc.FileName} finns redan i index.");
            return;
        }

        await _client.UploadDocumentsAsync(new[] { doc });
        Console.WriteLine($"Uppladdat: {doc.FileName}");
    }

    private async Task<bool> DocumentExistsAsync(string fileName)
    {
        var filter = $"filename eq '{fileName.Replace("'", "''")}'";
        var options = new SearchOptions
        {
            Filter = filter,
            Size = 1
        };

        var response = await _client.SearchAsync<FileDocument>("*", options);
        
        await foreach (var _ in response.Value.GetResultsAsync())
        {
            return true; // så fort vi hittar något, returnera true
        }

        return false; // om inget hittades
    }
}
