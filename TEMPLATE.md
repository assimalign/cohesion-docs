

```text
Cohesion Documentation
├── Overview.md
├── APIManager/
├── ConfigurationStore/
├── Database/
│   ├── Overview.md
│   ├── SQL/
│   │   ├── Language (SQL)/
│   ├── Document/
│   │   ├── Language (OQL)/
│   ├── Graph/
│   │   ├── Language (GQL)/
│   ├── KeyValuePair/
│   ├── Cache/
│   └── Blob/
├── EmailHub/
├── EventHub/
├── IdentityHub/
├── IoTHub/
├── LoadBalancer/
├── LogSpace/
├── MediaHub/
├── MessageHub/
├── NatGateway/
├── NotificationHub/
├── Rezolvr/
├── Scheduler/
├── SecretStore/
├── VPNGateway/
├── Web/
├── Platforms/
│   ├── Containers/
│   ├── Docker/
│   └── Kubernetes/
└──.NET APIs/                                   # Standard API documentation.
    ├── {Libraries|Resources}/
    │       ├── {Area}.md                       # Describes the default
    │       └── {Area}/
    │           ├── {Assembly}.md
    │           └── {Assembly}/
    │               ├── Design.md
    │               ├── Examples/               # I want actual examples of each API usage
    │               │   └── {Example: Title}.md
    │               ├── {Type}.md
    │               └── {Type}/
    │
    ├── SDKs/
    │    ├── {Area}.md
    │    └── {Area}/
    │        ├── Overview.md
    │        ├── MSBuild/                   # A breakdown of the inner MSBuild setup. This often is not exposed to the developer and makes it hard to setup with .NET projects. I want to actually explain the MSBuild pieces and design.
    │        ├── {Assembly}.md
    │        └── {Assembly}/
    │            ├── Design.md
    └── Resources/
```