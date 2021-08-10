---
title: "Build the Microservice"
date: 2021-08-10T14:01:57+02:00
draft: false
weight: 30
---

We will build a microservice with the following architecture:

![Microservice](30_microservice_arch.png)

We will use following AWS services: SQS, Lambda, DynamoDB, API Gateway and AWS Systems Manager (SSM), therefore we need to add respective CDK packages into the `Microservice` project. You can do it in CLI with `dotnet` command:

```powershell
cd src
dotnet add Microservice package Amazon.CDK.AWS.SQS
dotnet add Microservice package Amazon.CDK.AWS.Lambda
dotnet add Microservice package Amazon.CDK.AWS.Lambda.EventSources
dotnet add Microservice package Amazon.CDK.AWS.DynamoDB
dotnet add Microservice package Amazon.CDK.AWS.APIGateway
dotnet add Microservice package Amazon.CDK.AWS.SSM
```

or in VS2019 UI: right-click on the project `Microservice`, then `Manage NuGet Packages..` and pick the packages listed above.

Rebuild the project once again to ensure everything is OK.

