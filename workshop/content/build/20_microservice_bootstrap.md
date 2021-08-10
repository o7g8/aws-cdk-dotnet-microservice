---
title: "Microservice: bootstrap CDK (IaC) project"
date: 2021-08-10T12:25:08+02:00
draft: false
weight: 20
---
We will build a microservice with the following architecture:

![Microservice](30_microservice_arch.png)

Let's start building our microservice and its Infrastructure as Code using CDK: both the microservice code and IaC will be in the same CDK project.

We follow the best practice _Infrastructure code and application code lives in the same package_ as described in [Best practices for developing cloud applications with AWS CDK](https://aws.amazon.com/blogs/devops/best-practices-for-developing-cloud-applications-with-aws-cdk/) and keep the IaC (CDK) and business logic (Lambdas) code in the same solution.

Bootstrap the CDK/.NET project:

```powershell
mkdir microservice
cd .\microservice\
cdk init -l csharp
cdk bootstrap
cdk synth
```

You should see process of CDK bootstrapping and in the end some output resembling a CloudFormation template.

Set the default .NET Core SDK version to 3.1.: list available SDK versions and pin the project to the available version of `3.1` (in my case it's `3.1.140`):

```powershell
dotnet --list-sdks
cd src
dotnet new globaljson --sdk-version 3.1.410 --force
```

Open the solution file `./src/Microservice.sln` in VS2019, open the `Microservice` project properties and ensure the project uses `.NET Core 3.1` as the _Target framework_.

Build the project and ensure the build succeeds.

The microservice is based on AWS services: SQS, Lambda, DynamoDB, API Gateway and AWS Systems Manager (SSM), therefore we need to add respective CDK packages into the `Microservice` project. You can do it in CLI with `dotnet` command:

```powershell
cd src
dotnet add Microservice package Amazon.CDK.AWS.SQS
dotnet add Microservice package Amazon.CDK.AWS.Lambda
dotnet add Microservice package Amazon.CDK.AWS.Lambda.EventSources
dotnet add Microservice package Amazon.CDK.AWS.DynamoDB
dotnet add Microservice package Amazon.CDK.AWS.APIGateway
dotnet add Microservice package Amazon.CDK.AWS.SSM
```

Alternatively you can add the dependencies in VS2019 UI: right-click on the project `Microservice`, then `Manage NuGet Packages..` and pick the packages listed above.

Rebuild the project to ensure everything is OK.

Open file `MicroserviceStack.cs` and add the necessary `using`s:

```csharp
using Amazon.CDK;
using Amazon.CDK.AWS.SQS;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.SSM;
```

Rebuild the project again and ensure everything is OK.

Awesome! We got a skeleton for our microservice. Now it's time to add some "meat" to it and deploy some actual AWS services.

We start with a SQS queue which is used to send the insurance policies from the "monolith" to the cloud and to decouple our shiny microservice from the legacy "monolith".


