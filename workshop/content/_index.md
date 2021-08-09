---
title: ".NET Development on AWS"
date: 2021-08-09T17:43:42+02:00
draft: false
---

# .NET Development on AWS

The workshop is prepared for .NET developers who can navigate in AWS Console, but don't have experience with coding for AWS. The workshop aims to to give the developers head-start with .NET development on AWS with the toolchain familiar to them: Visual Studio, C#, .NET Framework, .NET Core.

The starting point in the workshop is a .NET Framework-based "monolith" application processing insurance polices. Our goal is to make the insurance policy data available for a new Cloud based micro-service.

We will add to the "monolith" a capability to send the insurance policy data to the Cloud via a SQS message queue. Then we will build a serverless .NET Core-based micro-service based, which will store the insurance policy data in DynamoDB, and another .NET Core micro-service which will serve the insurance policies data via an API Gateway.

We will implement Infrastructure as Code in C# using CDK/.NET and will use it to deploy the micro-services to AWS.
