---
title: "Installation of tools"
date: 2021-08-09T18:09:14+02:00
draft: false
weight: 20
---

Install following tools on your development machine:

- Install [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2-windows.html).

- Install [Docker](https://docs.docker.com/docker-for-windows/install/) (not possible in Workspaces). A reboot will be needed during the installation. You will need to reboot your machine after the installation. (On EC2 Docker will complain about obsolete Windows version - you can ignore it. On EC2 before reboot open _EC2 Launch Settings_ and check _Run EC2Launch on every boot_).

- Install AWS Serverless Application Model ([SAM](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install-windows.html)).

- Install [.NET Core 3.1](https://dotnet.microsoft.com/download) (we use v3.1 because AWS Lambda doesn't yet support .NET Core 5.0).

- Install **.NET Core Global Tool for Lambda** and `Amazon.Lambda.Template` (do in PowerShell):

```powershell
dotnet tool install -g Amazon.Lambda.Tools
dotnet new -i Amazon.Lambda.Templates
```

- Install and configure [Git](https://git-scm.com/download/win) (do in PowerShell)

```powershell
git config --global user.email "you@example.com"
git config --global user.name "Your Name"
```

- Install [Node.js](https://nodejs.org/en/download/).

- Install **CDK** (do in PowerShell, answer `A` on `Execution Policy Change`):

```powershell
npm install -g aws-cdk
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted
cdk --version
```
