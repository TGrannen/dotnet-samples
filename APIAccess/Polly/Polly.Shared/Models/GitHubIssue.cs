namespace Polly.Shared.Models;

public class GitHubIssue
{
    public int id { get; set; }
    public string node_id { get; set; }
    public int number { get; set; }
    public string state { get; set; }
    public string title { get; set; }
    public string body { get; set; }
    public bool locked { get; set; }
    public int comments { get; set; }
    public object closed_at { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}