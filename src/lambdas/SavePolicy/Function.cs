using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SavePolicy
{
    public class Function
    {
        // TODO: make it UT friendly https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.DownloadingAndRunning.html
        // https://stackoverflow.com/questions/66710616/how-to-mock-dynamodbcontext-batchwrite-for-unit-testing-in-net
        internal static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        internal static DynamoDBContext dbContext = new DynamoDBContext(client);
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {

        }


        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach(var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context);
            }
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            context.Logger.LogLine($"Processed message {message.Body}");

            try
            {
                /*
                // soft-typed approach with a table name which can be determined in run-time. can use automapper to build the document.
                var table = Table.LoadTable(client, "MicroserviceStack-policies6B0F0152-1HP5UY8WY9IZB");
                await table.PutItemAsync(new Document {
                    { "field", "value" }
                });
                */

                var policy = JsonConvert.DeserializeObject<Policy>(message.Body);
                await dbContext.SaveAsync(policy);
            } catch (Exception e)
            {
                context.Logger.LogLine($"ERROR: {e.Message}");
            }
            await Task.CompletedTask;
        }
    }

    // TODO: Find out how to get the table name in the runtime.
    [DynamoDBTable("MicroserviceStack-policies6B0F0152-1HP5UY8WY9IZB")]
    public class Policy
    {
        [DynamoDBHashKey] //Partition key
        public Guid PolicyId { get; set; }

        [DynamoDBProperty]
        public string PolicyOwner { get; set; }
    }
}
