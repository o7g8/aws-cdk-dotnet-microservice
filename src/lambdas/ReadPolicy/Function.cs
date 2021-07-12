using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ReadPolicy
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            try
            {
                /*
                var dynamoClient = new AmazonDynamoDBClient();
                var request = new ScanRequest {
                    TableName = "serverless-inventory-app-dynamodb-table"
                };
                var response = await dynamoClient.ScanAsync(request);
                var result = response.Items;
                */

                var result = apigProxyEvent.PathParameters.ContainsKey("owner")
                    ? "Searching for " + Uri.UnescapeDataString(apigProxyEvent.PathParameters["owner"])
                    : "hello";
                context.Logger.LogLine(apigProxyEvent.Body);
                context.Logger.LogLine(apigProxyEvent.Path);
                context.Logger.LogLine(JsonSerializer.Serialize(apigProxyEvent.PathParameters));

                return await Task.FromResult(
                 new APIGatewayProxyResponse
                 {
                     Body = JsonSerializer.Serialize(result),
                     StatusCode = 200
                 });
            }
            catch (Exception ex)
            {
                context.Logger.LogLine(ex.Message);
                return new APIGatewayProxyResponse
                {
                    Body = apigProxyEvent.Body,
                    StatusCode = 409
                };
            }
        }
    }
}
