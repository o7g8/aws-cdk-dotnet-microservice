# .NET Development on AWS

## Provision Windows dev environment

- Start a Windows instance with at least 70Gb disk space. To run Docker you will need to start a `*.metal` instance, e.g. `m5zn.metal` or `z1d.metal` with the latest `Microsoft Windows Server 2019 Base` AMI.

- Install AWS CLI <https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2-windows.html> if needed.

- Install Docker <https://docs.docker.com/docker-for-windows/install/> (not possible in Workspaces). Docker will complain about obsolete Windows version - you can ignore it. A reboot will be needed during the installation.

- Before reboot open _EC2 Launch Settings_ and check `Run EC2Launch on every boot`.

  - command line installation <https://www.dummies.com/computers/operating-systems/microsoft-windows/how-to-install-containers-on-windows-server-2019/>

- Install SAM <https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install-windows.html>

- Install .NET 3.1 <https://dotnet.microsoft.com/download> (Lambda doesn't support .NET 5.0 yet).

- Install Git <https://git-scm.com/download/win>

```powershell
git config --global user.email "you@example.com"
git config --global user.name "Your Name"
```

- Install Node.js <https://nodejs.org/en/download/>

- Install CDK (answer `A` on `Execution Policy Change`):

```powershell
npm install -g aws-cdk
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted
cdk --version
```

### Install VS 2019 Community

- Download a VS 2019 Community <https://visualstudio.microsoft.com/vs/community/>.

- Install the VS 2019 Community, choose:

  - ASP.NET and web development;

  - .NET Desktop Development;

  - .NET cross-platform development.

- Install AWS Toolkit for Visual Studio 2017 and 2019 <https://marketplace.visualstudio.com/items?itemName=AmazonWebServices.AWSToolkitforVisualStudio2017>:

  - Go to _View -> AWS Explorer (Ctrl+K, A)_;

  - Click on the `+` icon and add a new profile: enter the necessary credentials.

### Install VS Code

- Download and install VS Code <https://code.visualstudio.com/download>

- Install following VS Code extensions:

  - AWS Toolkit for VS Code <https://docs.aws.amazon.com/toolkit-for-vscode/latest/userguide/setup-toolkit.html>;

  - C# <https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp>;

  - vscode-solution-explorer.

I recommend to save the AMI afterwards (.NET DEV).

## Development of a .NET Framework application (the "Monolith")

## Cloud-native development with .NET Core (the "Microservice")

Bootstrap the CDK project for microservice:

```powershell
cd D:\Users\oleg\source\repos\
mkdir microservice
cdk init -l csharp
cdk synth
```

If everything worked well, you should get a CloudFormation template on the console.

Add necessary dependencies either using `dotnet`:

```powershell
cd src
dotnet add Microservice package Amazon.CDK.AWS.SQS
dotnet add Microservice package Amazon.CDK.AWS.Lambda
dotnet add Microservice package Amazon.CDK.AWS.Lambda.EventSources
dotnet add Microservice package Amazon.CDK.AWS.DynamoDB
dotnet add Microservice package Amazon.CDK.AWS.APIGateway
```

or in VS2019 right-click on the project `Microservice`, then `Manage NuGet Packages..` and pick the packages listed above.

Ensure the project uses .NET Core 3.1: list available SDK versions and pin the project to the available version of `3.1` (in my case it's `3.1.140`):

```powershell
dotnet --list-sdks
dotnet new globaljson --sdk-version 3.1.410 --force
```

Create the Lambda project `SavePolicy` as described in <https://docs.aws.amazon.com/toolkit-for-visual-studio/latest/user-guide/lambda-creating-project-in-visual-studio.html> using `SQS` blueprint.

Write the CDK code instantiating a SQS queue, a Lambda saving policies and a DynamoDB table.

TODO: paste the code snippets.

Now you should get a stack with a SQS queue, a Lambda and a DynamoDB table. See if your stack builds and can be deployed:

```powershell
cdk synth
cdk deploy
```

You should get healthy CDK output ending with `Stack ARN`.

### .NET Framework development

- Open and build the project `SampleServer` in VS2019.

- Open the port `8888`:

```powershell
netsh http add urlacl url="http://+:8888/" user="Everyone"
```

- Execute the `./bin/Debug/SampleServer.exe`, you should see messages `Saving policy` in the console.

- Open `http://localhost:8888/` in browser, you should see `Version 0.1`.

- Copy the content of the `Debug` directory into an EC2 instance which can access SQS endpoint.

## Useful CDK commands

- `dotnet build src` compile this app

- `cdk deploy`       deploy this stack to your default AWS account/region

- `cdk diff`         compare deployed stack with current state

- `cdk synth`        emits the synthesized CloudFormation template

## References

- <https://aws.amazon.com/sdk-for-net/>

- <https://aws.amazon.com/visualstudio/>

- <https://github.com/aws/dotnet>

- <https://docs.aws.amazon.com/whitepapers/latest/develop-deploy-dotnet-apps-on-aws/develop-deploy-dotnet-apps-on-aws.html>

- <https://aws.amazon.com/blogs/compute/migrating-net-classic-applications-to-amazon-ecs-using-windows-containers/>

- <https://aws.amazon.com/blogs/compute/applying-the-twelve-factor-app-methodology-to-serverless-applications/>

- How to Containerize .NET Applications and Run Them on Amazon Elastic Container Service (ECS) <https://www.youtube.com/watch?v=nGcQcgZywUM>