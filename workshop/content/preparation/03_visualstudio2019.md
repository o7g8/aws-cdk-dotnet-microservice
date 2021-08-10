---
title: "Visual Studio 2019"
date: 2021-08-09T18:30:42+02:00
draft: false
weight: 30
---

- If you don't have Visual Studio 2019, then download [VS 2019 Community](https://visualstudio.microsoft.com/vs/community/).

- In case you are installing VS2019 Community, select following modules (or ensure the modules are provisioned in the existing installation):

  - ASP.NET and web development;

  - .NET Desktop Development;

  - .NET cross-platform development.

- Install [AWS Toolkit for Visual Studio 2017 and 2019](https://marketplace.visualstudio.com/items?itemName=AmazonWebServices.AWSToolkitforVisualStudio2017), then:

  - Go to _View -> AWS Explorer (Ctrl+K, A)_;

  - Click on the `+` icon and add a new profile: enter the AWS credentials.

![AWS Extension](aws_vs_plugin.png)

  TODO: describe the process of obtaining the credentials in EE and in the own account (closer to AWS CLI - register profile there).

