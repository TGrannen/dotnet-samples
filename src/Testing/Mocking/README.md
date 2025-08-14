# Automated Test Mocking

A practical playground for comparing popular .NET mocking libraries side‑by‑side and showcasing modern testing patterns. It includes focused examples for unit tests and integration tests, including end‑to‑end verification of logging and MediatR pipelines.

- Mocking libraries compared: [Moq](https://github.com/moq/moq4), [NSubstitute](https://github.com/nsubstitute/NSubstitute), and [FakeItEasy](https://fakeiteasy.github.io/)
- Assertion library: [Shouldly](https://docs.shouldly.org/) for readable assertions and helpful failure messages
- Test framework: [xUnit](https://xunit.net/)
- Integration testing: [MediatR](https://github.com/jbogard/MediatR) with Dependency Injection and logging verification
- Deterministic test data generation via a Stable Auto Faker utility

## Solution contents

- Shared contracts and DTOs
  - Common interfaces and models used across tests (e.g., repository/service abstractions and DTOs) to keep the examples realistic and reusable.

- Mocking comparison tests
  - Separate test areas for Moq, NSubstitute, and FakeItEasy, demonstrating:
    - Arranging return values and behaviors
    - Verifying calls/interactions
    - Asserting outcomes with Shouldly
    - Logging verification using helper extensions for ILogger<T> (e.g., verifying that specific log messages at specific levels occurred or did not occur)

- Integration tests with MediatR
  - Validates request/handler flows with the default DI container
  - Verifies side‑effects, including emitted log messages

- Utilities: Stable Auto Faker
  - A deterministic data generator designed for tests. It produces stable, repeatable objects for given types and configurations, helping prevent flaky tests caused by randomness.
  - Supports generating a single instance or lists of instances for any class with a public parameterless constructor.
