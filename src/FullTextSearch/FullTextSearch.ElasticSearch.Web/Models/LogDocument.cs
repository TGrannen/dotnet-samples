namespace FullTextSearch.ElasticSearch.Web.Models;

public class LogDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Body { get; set; }
}