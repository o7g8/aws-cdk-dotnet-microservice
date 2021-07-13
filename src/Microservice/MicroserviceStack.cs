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
                    Name = "CprNo",
                    Type = AttributeType.STRING
                },
               SortKey = new Attribute
               {
                   Name = "PolicyOwner",
                   Type = AttributeType.STRING
               }
           });
           
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

           var readPolicy = new Function(this, "readPolicy", new FunctionProps {
               Runtime = Runtime.DOTNET_CORE_3_1,
               FunctionName = "readPolicy",
               Handler = "ReadPolicy::ReadPolicy.Function::FunctionHandler",
               Code = Code.FromAsset(GetAssetPath("src/lambdas/ReadPolicy/bin/Debug/netcoreapp3.1")),
               Timeout = Duration.Seconds(15),
               Environment = new Dictionary<string, string> {
                   ["TABLE"] = table.TableName
               }
           });
           table.GrantReadData(readPolicy);
            table.Grant(readPolicy, "dynamodb:DescribeTable");

            var api = new LambdaRestApi(this, "PoliciesAPI", new LambdaRestApiProps {
                Handler = readPolicy,
            });
            // see https://docs.aws.amazon.com/cdk/api/latest/docs/aws-apigateway-readme.html
            var apiPolicies = api.Root.AddResource("policies");
            var apiPolicy = apiPolicies.AddResource("{cprno}");
            apiPolicy.AddMethod("GET");

            new CfnOutput(this, "Queue", new CfnOutputProps() { Value = queue.QueueUrl });
            new CfnOutput(this, "Api", new CfnOutputProps() { Value = api.Url });
        }

        private string GetAssetPath(string path)
        {
            return Debugger.IsAttached
                ? "../../../../../" + path
                : path;
        }
    }
}
