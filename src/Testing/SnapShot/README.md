# Snapshot Testing

Snapshot testing is a type of “output comparison” or “golden master” testing. These tests prevent regressions by
comparing the current characteristics of an application or component with stored “good” values for those
characteristics. Snapshot tests are fundamentally different from unit and functional tests. While these types of tests
make assertions about the correct behavior of an application, snapshot tests only assert that the output now is the same
as the output before; it says nothing about whether the behavior was correct in either case.

## [Verify](https://github.com/VerifyTests/Verify)

Verify is called on the test result during the assertion phase. It serializes that result and stores it in a file that
matches the test name. On the next test execution, the result is again serialized and compared to the existing file. The
test will fail if the two snapshots do not match: either the change is unexpected, or the reference snapshot needs to be
updated to the new result.

This project will spin up a postgres container using
the [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) package to not have to depend external
setup for the database.

#### Packages include

* [Verify](https://github.com/VerifyTests/Verify)
* [Refit](https://github.com/reactiveui/refit)
* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core//)
* [Npgsql](https://www.npgsql.org/)
* [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet)
* [Respawn](https://github.com/jbogard/Respawn)