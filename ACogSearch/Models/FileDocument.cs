using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using System.Text.Json.Serialization;

public class FileDocument
{
    [SimpleField(IsKey = true)]
    public string Id { get; set; }

    [SimpleField]
    [JsonPropertyName("filename")]
    public string FileName { get; set; }

    [SearchableField(AnalyzerName = "en.lucene")]
    public string Content { get; set; }
}
