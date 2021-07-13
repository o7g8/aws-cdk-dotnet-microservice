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
        const string TABLE = "TABLE";
        private static AmazonDynamoDBClient dynamoClient = new AmazonDynamoDBClient();

        /// <summary>
        /// Search for a policy with a given CPR.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns>Policy JSON</returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            if (!apigProxyEvent.PathParameters.ContainsKey("cprno"))
            {
                return new APIGatewayProxyResponse
                {
                    Body = "Wrong request",
                    StatusCode = 400,
                };
            }
            var cprNo = Uri.UnescapeDataString(apigProxyEvent.PathParameters["cprno"]);
            context.Logger.LogLine($"Searching for {cprNo}");
            try
            {
                var response = await dynamoClient.QueryAsync(new QueryRequest() {
                    TableName = Environment.GetEnvironmentVariable(TABLE),
                    KeyConditionExpression = "CprNo = :v_Cpr",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {":v_Cpr", new AttributeValue { S = cprNo }}
                    },
                    Limit =1
                });
                var responseValue = response.Items.FirstOrDefault();
                if(responseValue == null)
                {
                    return new APIGatewayProxyResponse
                    {
                        Body = "Not found",
                        StatusCode = 404,
                    };
                }

                return new APIGatewayProxyResponse
                 {
                     Body = FormatPolicy(responseValue),
                     StatusCode = 200
                 };
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

        private string FormatPolicy(Dictionary<string, AttributeValue> responseValue)
        {
            var result = new Policy
            {
                CprNo = responseValue["CprNo"].S,
                PolicyOwner = responseValue["PolicyOwner"].S
            };
            return JsonSerializer.Serialize(result);
        }
    }

    // TODO: Find out how to get the table name in the runtime.
    //[DynamoDBTable("MicroserviceStack-policies6B0F0152-1HP5UY8WY9IZB")]
    public class Policy
    {
        //[DynamoDBHashKey] //Partition key
        public string CprNo { get; set; }

        //[DynamoDBProperty]
        public string PolicyOwner { get; set; }
    }
}
