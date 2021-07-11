# .NET Development on AWS

## Provision Windows dev environment

- Start a Windows instance with at least 70Gb disk space. To run Docker you will need to start a `*.metal` instance, e.g. `m5zn.metal` or `z1d.metal` with the latest `Microsoft Windows Server 2019 Base` AMI. Consider to use a spot instance with a permanent spot request to save costs.

- Install AWS CLI <https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2-windows.html> if needed.

- Install Docker <https://docs.docker.com/docker-for-windows/install/> (not possible in Workspaces). Docker will complain about obsolete Windows version - you can ignore it. A reboot will be needed during the installation.

- Before reboot open _EC2 Launch Settings_ and check `Run EC2Launch on every boot`.

  - command line installation <https://www.dummies.com/computers/operating-systems/microsoft-windows/how-to-install-containers-on-windows-server-2019/>

- Install SAM <https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install-windows.html>

- Install .NET 3.1 <https://dotnet.microsoft.com/download> (Lambda doesn't support .NET 5.0 yet).

  - Install .NET Core Global Tool for Lambda and `Amazon.Lambda.Template`:

    ```powershell
    dotnet tool install -g Amazon.Lambda.Tools
    dotnet new -i Amazon.Lambda.Templates
    ```

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

## What will we build

We will build a "monolith" (a legacy application) producing some data and a "microservice" which will process the data. The "monolith" and "microservice" are decoupled with a SQS queue.

The "monolith" is a .NET Framework based server which "saves" some entities (e.g. insurance policies). The monolith has also a HTTP endpoint on which it can report own version. We will make the "monolith" to send the entities to a SQS queue.

We will develop the "micorservice" with .NET Core and will use CDK/.NET for IaC. The microservice will consist of the SQS queue, a Lambda which will save the entities in a DynamoDB table, and an API endpoint which will invoke another Lambda which will read the entities from the DynamoDB table.

## Development of a .NET Framework application (the "Monolith")

- Open and build the project `SampleServer` in VS2019.

- Open the port `8888`:

```powershell
netsh http add urlacl url="http://+:8888/" user="Everyone"
```

- Execute the `./bin/Debug/SampleServer.exe`, you should see messages `Saving policy` in the console.

- Open `http://localhost:8888/` in browser, you should see `Version 0.1`.

- Now you can switch to the build of the Microservice.

Adding code which will send the policies into the queue.

- Add following packages as dependencies to the `SampleServer` project: `AWSSDK.SQS`, `AWSSDK.SimpleSystemsManagement`, `Newtonsoft.Json`, `RandomNameGeneratorLibrary`.

TODO: add the code snippets:

- read the SSM parameter;

- send the data into the queue.

Run the `\bin\Debug\SampleServer.exe` locally (ensure your AWS profile on the machine allows you to read the SSM parameter and send messages into SQS) and see how the policies are submitted to the queue.

You can also debug the application as usual in VS2019.

Switch to the VS2019 with the `Microservice` and open the `AWS Explorer`, expand the Lambda and double-click on `savePolicy`, then click `Logs` and and download the most recent log stream. You should see the polices send by the "monolith" processed by the Lambda.

## Cloud-native development with .NET Core (the "Microservice")

Bootstrap the CDK project for microservice:

```powershell
cd D:\Users\oleg\source\repos\
mkdir microservice
cdk init -l csharp
cdk bootstrap
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
dotnet add Microservice package Amazon.CDK.AWS.SSM
```

or in VS2019 right-click on the project `Microservice`, then `Manage NuGet Packages..` and pick the packages listed above.

Ensure the project uses .NET Core 3.1: list available SDK versions and pin the project to the available version of `3.1` (in my case it's `3.1.140`):

```powershell
dotnet --list-sdks
dotnet new globaljson --sdk-version 3.1.410 --force
```

Create the Lambda project `SavePolicy` as described in <https://docs.aws.amazon.com/toolkit-for-visual-studio/latest/user-guide/lambda-creating-project-in-visual-studio.html> using `SQS` blueprint.

To see the Lambda handler name right-click on the `SavePolicy` project and choose `Publish tpo AWS Lambda..` - you will see the `Handler` in the bottom of the dialog.

Write the CDK code instantiating a SQS queue, SSM Parameter, a Lambda saving policies and a DynamoDB table.

TODO: paste the code snippets.

You can also debug the CDK code locally in VS2019. Set a breakpoint and start the debugger.

You can also debug the Lambda code locally: set a breakpoint on the `foreach` statement in `Function.cs` of `SavePolicy`. Right-click on the `SavePolicy` project and in the opened browser pick `SQS` in the `Example Requests` drop-down, then click the `Execute Function`. After the execution the result and log output will be shown in the browser window. 

You can configure the debug environment for the Lambda in `src\lambdas\SavePolicy\aws-lambda-tools-defaults.json`.

Now you should get a stack with a SQS queue, SSM Parameter with the queue URL, a Lambda and a DynamoDB table. See if your stack builds and can be deployed:

```powershell
cdk synth
cdk deploy
```

You should get healthy CDK output ending with `Stack ARN`.

Now you can switch to the "Monolith" and add the code which will send the entities into the queue.

## Errata

- If you fiddle with the name of the SSM Parameter, CDK may fail to deploy the changes, in this case you may want to redeploy the stack as:

  ```powershell
  cdk destroy
  cdk deploy
  ```

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

- Lambda in .NET Core <https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-cli.html> 

- Work with SQS with .NET AWS SDK <https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/sqs-apis-intro.html>

## TODO

- UT for CDK

- UT for Lambda:

  - add the SQS function with `dotnet new lambda.SQS -n SavePolicy`

  - more about UT <https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-cli.html>

- "Monolith" deployment:

  - on ECS or EC2;
  
  - create the EC2/ECS role with SQS and SSM access <https://docs.aws.amazon.com/cdk/api/latest/docs/aws-iam-readme.html>.

  - Debugging of the "monolith": local container, remote on EC2, remote on ECS.

- CDK Debugging in VS Code.

- Lambda Debugging in VS Code.

- Simple REACT/S3 (or Amplify) app to serve the content.
