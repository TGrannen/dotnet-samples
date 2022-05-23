// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Infrastructure.ConsoleApp.Models;

public class LocalApp
{
    public string Name { get; init; }
    public string Context { get; init; }
    public string Dockerfile { get; init; }
    public string ImageName { get; init; }
    public int NodePort { get; init; }
}