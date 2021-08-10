---
title: "TODO"
date: 2021-08-10T20:43:03+02:00
draft: true
---

* PowerPoint deck: CDK, SQS, Lambda, DynamoDB, API GW.

* CI/CD pipeline for the microservice in CDK.

* Full-fledged debugging of Lambda

* UT for CDK

* UT for Lambdas

* UT/Debugging with the locally provisioned DynamoDB.

* Strongly-typed access to DynamoDB tables in Lambdas.

* X-Ray

* Containerize the "monolith" (evt. CI/CD and CDK, evt debugging in the container: local, remote on EB?, ECS?, ...).

* Simple REACT/S3 (or Amplify) app to serve the content.

Unsorted:

- UT for CDK <https://docs.aws.amazon.com/cdk/latest/guide/testing.html#testing_snapshot>

- UT for Lambda:

  - more about UT <https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-cli.html>

  - DI for Lambdas:
  
    - <https://github.com/aws/aws-lambda-dotnet/issues/800>

    - <https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection>

    - <https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0>

    - <https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage>

    - <https://www.tutorialsteacher.com/core/dependency-injection-in-aspnet-core>

    - <https://www.claudiobernasconi.ch/2019/01/24/the-ultimate-list-of-net-dependency-injection-frameworks/>

  - local DynamoDB <https://github.com/itorvo/Webinar.Dynamo/tree/14842fc6edbe8a98e41077a836f57d2347d470fa>. Run UT/IT with local DynamoDB in the container with DynamoDB:

    - <https://joehonour.medium.com/a-guide-to-setting-up-a-net-core-project-using-docker-with-integrated-unit-and-component-tests-a326ca5a0284>

    - <https://wright-development.github.io/post/using-docker-for-net-core/>

- "Monolith" deployment:

  - with App2Container: on App Runner (Java/Linux only) / ECS / EC2 <https://app2container.workshop.aws/en>, <https://aws.amazon.com/blogs/containers/expanding-modernization-scenarios-using-the-aws-app2container-integration-with-aws-app-runner/>;
  
  - create the EC2/ECS role with SQS and SSM access <https://docs.aws.amazon.com/cdk/api/latest/docs/aws-iam-readme.html>.