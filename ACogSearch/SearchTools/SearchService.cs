using ACogSearch.SearchTools;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Options;

public class SearchService
{
    private readonly AzureSearchOptions _options;

    public SearchService(IOptions<AzureSearchOptions> options)
    {
        _options = options.Value;
    }
    public async Task<List<FileDocument>> SearchDocumentsAsync(string query)
    {
        var client = new SearchClient(new Uri(_options.Endpoint),_options.IndexName, new AzureKeyCredential(_options.ApiKey));

        var options = new SearchOptions
        {
            Size = 10,
            HighlightFields = { "content" },
            HighlightPreTag = "<mark>",
            HighlightPostTag = "</mark>"
        };

        var response = await client.SearchAsync<FileDocument>(query, options);
        var results = new List<FileDocument>();

        await foreach (SearchResult<FileDocument> result in response.Value.GetResultsAsync())
        {
            var doc = result.Document;

            // Ersätt content med highlight-utdrag från träffar
            if (result.Highlights != null &&
                result.Highlights.TryGetValue("content", out var snippets))
            {
                doc.Content = string.Join("... ", snippets);
            }
            else
            {
                doc.Content = "[Ingen träff i content]";
            }

            results.Add(doc);
        }

        return results;
    }

    public async Task<List<FileDocument>> GetAllUploadedFilesAsync()
    {
        var client = new SearchClient(
            new Uri(_options.Endpoint),
            _options.IndexName,
            new AzureKeyCredential(_options.ApiKey));

        var options = new SearchOptions
        {
            Size = 100,
            Select = { "filename" },
            OrderBy = { "filename asc" }
        };

        var response = await client.SearchAsync<FileDocument>("*", options);
        var files = new List<FileDocument>();

        await foreach (var result in response.Value.GetResultsAsync())
        {
            files.Add(result.Document);
        }

        return files;
    }

}
