using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

public class IndexCreator
{
    public static async Task CreateIndexIfNotExistsAsync(string serviceEndpoint, string apiKey)
    {
        var endpoint = new Uri(serviceEndpoint);
        var credential = new AzureKeyCredential(apiKey);
        var indexClient = new SearchIndexClient(endpoint, credential);

        string indexName = "textfiles";

        // Kontrollera om index redan finns
        await foreach (var name in indexClient.GetIndexNamesAsync())
        {
            if (name.Equals(indexName, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Index '{indexName}' finns redan – ingen åtgärd.");
                return;
            }
        }

        // Skapa fält
        var fields = new List<SearchField>
    {
        new SearchField("id", SearchFieldDataType.String)
        {
            IsKey = true,
            IsFilterable = true,
            IsSearchable = false,
            IsHidden = false
        },
        new SearchField("filename", SearchFieldDataType.String)
        {
            IsSearchable = true,
            IsFilterable = true,
            IsHidden = false
        },
        new SearchField("content", SearchFieldDataType.String)
        {
            IsSearchable = true,
            IsFilterable = true,
            IsHidden = false,
            AnalyzerName = LexicalAnalyzerName.EnLucene
        }
    };

        var index = new SearchIndex(indexName) { Fields = fields };

        await indexClient.CreateIndexAsync(index);
        Console.WriteLine($"Index '{indexName}' skapades i Azure Search.");
    }
}
