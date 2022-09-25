namespace Mocking.Mediator.Tests.Exceptions;

public class MediatorFixtureConfigurationException : Exception
{
    public MediatorFixtureConfigurationException() : base("MediatR was not properly initialized in the service collection")
    {
    }
}