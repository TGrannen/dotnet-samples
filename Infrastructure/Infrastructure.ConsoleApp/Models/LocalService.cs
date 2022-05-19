namespace Infrastructure.ConsoleApp.Models;

public class LocalService
{
    public string AppName { get; init; }
    public string Image { get; init; }
    public int ContainerPort { get; init; } = 80;
    public int NodePort { get; init; }
}