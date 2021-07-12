using System.Collections.Generic;
using System.Diagnostics;

using Amazon.CDK;
using Amazon.CDK.AWS.SQS;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.SSM;

namespace Microservice
{
    public class MicroserviceStack : Stack
    {
        const string QUEUE_SSM_PARAMETER_NAME = "/monolith/policyQueueUrl";

        internal MicroserviceStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
           var queue = new Queue(this, "queue", new  QueueProps {
                Fifo = true
           });

            var ssmParam = new StringParameter(this, "policyQueueUrl", new StringParameterProps {
                ParameterName = QUEUE_SSM_PARAMETER_NAME,
                Description = "URL of the queue with policies",
                StringValue = queue.QueueUrl
            });

           var table = new Table(this, "policies", new TableProps {
               PartitionKey = new Attribute
                {
                    Name = "PolicyId",
                    Type = AttributeType.STRING
                }
           });

            // see examples:
            // https://github.com/DiddyApp/diddy-backend/blob/e2d0d798cee47e5bd84811cc6e34d9f2dd4529cd/src/lambdas/Goals/AddGoalFunction.cs
            // https://github.com/DiddyApp/diddy-backend/blob/e2d0d798cee47e5bd84811cc6e34d9f2dd4529cd/src/infrastructure/Goals/LambdaResources.cs
            // https://github.com/xerris/CDKApp/blob/cefa0d06632c8634a416e109eef659e4a1d34170/src/CdkApp/CdkAppStack.cs
            // https://github.com/search?l=C%23&q=Runtime.DOTNET_CORE_3_1&type=Code
           
           var savePolicy = new Function(this, "savePolicy", new FunctionProps {
               Runtime = Runtime.DOTNET_CORE_3_1,
               FunctionName = "savePolicy",
               Handler = "SavePolicy::SavePolicy.Function::FunctionHandler",
               Code = Code.FromAsset(GetAssetPath("src/lambdas/SavePolicy/bin/Debug/netcoreapp3.1")),
               Timeout = Duration.Seconds(15),
               Environment = new Dictionary<string, string> {
                   ["TABLE"] = table.TableName
               },
               Events = new[] { new SqsEventSource(queue) }
           });
           table.GrantWriteData(savePolicy);
            table.Grant(savePolicy, "dynamodb:DescribeTable");

            /*
           var readPolicy = new Function(this, "readPolicy", new FunctionProps {
               Runtime = Runtime.DOTNET_CORE_3_1,
               FunctionName = "readPolicy",
               Handler = "ReadPolicy::ReadPolicy.Function::FunctionHandler",
               Code = Code.FromAsset("lambdas/ReadPolicy/publish"),
               Timeout = Duration.Seconds(15),
               Environment = new Dictionary<string, string> {
                   ["TABLE"] = table.TableName
               }
           });
           table.GrantReadData(readPolicy);

           var api = new LambdaRestApi(this, "PoliciesAPI", new LambdaRestApiProps {
                Handler = readPolicy,
            });
            */

            // TODO: save the queue in Parameter Store an the writer will read it.
            new CfnOutput(this, "Queue", new CfnOutputProps() { Value = queue.QueueUrl });
            //new CfnOutput(this, "Api", new CfnOutputProps() { Value = api.Url });
        }

        private string GetAssetPath(string path)
        {
            return Debugger.IsAttached
                ? "../../../../../" + path
                : path;
        }
    }
}
