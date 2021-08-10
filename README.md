# .NET Development on AWS Workshop

The workshop aims to give .NET developers head-start with .NET development on AWS with the familiar toolchain: Visual Studio, C#, .NET Framework, .NET Core. The participant doesn't need to have experience with AWS but should be able to navigate in AWS Console.

The starting point of the workshop is a fictitious .NET Framework-based "monolith" application processing insurance polices. Our goal is to make the insurance policy data available for a new Cloud-based micro-service and serve the data via REST API.

We will add to the "monolith" a capability to send the insurance policy data to the Cloud via a SQS message queue. Then we will build a serverless .NET Core-based microservice, which will store the insurance policy data in DynamoDB, and another .NET Core microservice which will serve the insurance policies data via an API Gateway.

Here is the target architecture:

[Architecture](architecture.png)

The repo contains submodules therefore clone it with:

```powershell
git clone --recurse-submodules <url>
```

Install [Hugo](http://gohugo.io), serve the workshop content as described in [./workshop](./workshop) and proceed with the workshop instructions.
