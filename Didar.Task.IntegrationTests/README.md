# SQL Server integration tests

The concurrency test uses the real SQL Server `sp_getapplock` path. It is skipped unless `DIDAR_TASK_TEST_SQLSERVER` is set. Point it at a dedicated test database; the test applies EF migrations and creates then removes its own expert and requests.

PowerShell:

```powershell
$env:DIDAR_TASK_TEST_SQLSERVER = "Server=.;Database=DidarTask_IntegrationTests;Trusted_Connection=True;TrustServerCertificate=True"
dotnet test Didar.Task.IntegrationTests/Didar.Task.IntegrationTests.csproj
```
