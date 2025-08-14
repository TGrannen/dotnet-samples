namespace Mocking.AutoBogus.StableFaker;

public class StableAutoFaker
{
    private readonly StableAutoFakerConfig _config = new();

    public StableAutoFaker WithConfiguration(Action<StableAutoFakerConfig> func)
    {
        func(_config);
        return this;
    }

    public T Generate<T>() where T : class, new()
    {
        return (T)StableAutoFakerGenerator.GenerateObject(typeof(T), "", _config);
    }

    public List<T> GenerateList<T>(int count) where T : class, new()
    {
        return Enumerable.Range(0, count).Select(_ => Generate<T>()).ToList();
    }
}

public class StableAutoFaker<T> where T : class, new()
{
    private readonly StableAutoFakerConfig _config = new();

    public StableAutoFaker<T> WithConfiguration(Action<StableAutoFakerConfig> func)
    {
        func(_config);
        return this;
    }

    public T Generate()
    {
        return (T)StableAutoFakerGenerator.GenerateObject(typeof(T), "", _config);
    }

    public IEnumerable<T> GenerateList(int count)
    {
        return Enumerable.Range(0, count).Select(_ => Generate()).ToList();
    }
}