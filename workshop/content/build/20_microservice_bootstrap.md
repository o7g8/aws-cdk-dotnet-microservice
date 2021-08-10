---
title: "Bootstrap the Microservice"
date: 2021-08-10T12:25:08+02:00
draft: false
weight: 20
---

Let's start building our microservice and its Infrastructure as Code using CDK: both the microservice code and IaC will be in the same CDK project.

We follow the best practices _Infrastructure code and application code lives in the same package_ as described in <https://aws.amazon.com/blogs/devops/best-practices-for-developing-cloud-applications-with-aws-cdk/> and keep the IaC (CDK) and application (Lambda) code in the same repository.

Bootstrap the CDK/.NET project:

```powershell
mkdir microservice
cd .\microservice\
cdk init -l csharp
cdk bootstrap
cdk synth
```

Set the default .NET Core SDK version to 3.1.: list available SDK versions and pin the project to the available version of `3.1` (in my case it's `3.1.140`):

```powershell
dotnet --list-sdks
cd src
dotnet new globaljson --sdk-version 3.1.410 --force
```

Open the solution file `./src/Microservice.sln` in VS2019, open the `Microservice` project properties and ensure the project uses `.NET Core 3.1` as the _Target framework_.

Build the project and ensure the build succeeds.

In our microservice we will use following AWS services: SQS, Lambda, DynamoDB, API Gateway and AWS Systems Manager (SSM), therefore we need to add respective CDK packages into the `Microservice` project. You can do it in CLI with `dotnet` command:

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
