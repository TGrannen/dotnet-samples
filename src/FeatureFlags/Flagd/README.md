# Flagd Powershell Commands

### Get all flags

```shell
Invoke-RestMethod -Uri "http://localhost:8013/flagd.evaluation.v1.Service/ResolveAll" -Method Post -Body '{}' -ContentType "application/json"

```

### Evaluate sample flag

```shell
Invoke-RestMethod -Uri "http://localhost:8013/flagd.evaluation.v1.Service/ResolveBoolean" -Method Post  -Body '{"flagKey":"show-welcome-banner","context":{}}' -ContentType "application/json"
```

### Evaluate Custom flag with context evalutation

```shell
Invoke-RestMethod -Uri "http://localhost:8013/flagd.evaluation.v1.Service/ResolveBoolean" -Method Post  -Body '{"flagKey":"AllowForMinNumber","context":{"RandomNumber":50}}' -ContentType "application/json"
```