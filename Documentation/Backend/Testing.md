# Unit Tests vs Integration Tests

- Integration tests evaluate an app's components on a broader level than unit tests. Unit tests are used to test isolated software components, such as individual class methods.

# Integration Tests

- Integration tests confirm that two or more app components work together to produce an expected result, possibly including every component required to fully process a request.
- These broader tests are used to test the app's infrastructure and whole framework, often including:

  - Databases
  - File Systems
  - Network Appliances
  - Request-Response Pipelines

- In contrast to Unit tests, integration tests:

  - Use the actual components that the app uses in production
  - Require more code and data processing.
  - Take longer to run.

- More about Integration Tests: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0

# Unit Tests

- Unit tests confirm whether a specific method/component is functioning as intended.
- Unit tests use fabricated components, known as fakes or mock objects, in place of infrasture components

- More about Unit Tests: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test
