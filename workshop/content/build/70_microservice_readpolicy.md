---
title: "Microservice: create ReadPolicy Lambda"
date: 2021-08-10T20:48:55+02:00
draft: false
weight: 70
---

In this section we will create a `ReadPolicy` Lambda which will be invoked by API Gateway and which read the insurance policy from the DynamoDB table.

Open VS2019 with the `Microservice` solution.

Add a new Lambda project `ReadPolicy` similarly to `SavePolicy` but use _Empty Lambda_ blueprint instead. The project path should be in `<path>\src\lambdas`.

Add the following NuGet packages as a dependencies to the `ReadPolicy` project:

* `AWSSDK.DynamoDBv2`;

* `AWSSDK.Lambda`;

* `Amazon.Lambda.APIGatewayEvents`;

Build the whole `Microservice` solution and ensure there are no errors.

Open the `Function.cs` in `ReadPolicy` project, and paste following code into the file:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ReadPolicy
{
  public class Function
  {
    const string TABLE = "TABLE";
    private static AmazonDynamoDBClient dynamoClient = new AmazonDynamoDBClient();
    private static AmazonLambdaClient lambdaClient;

    static Function()
    {
      initialize();
    }

    static async void initialize()
    {
      lambdaClient = new AmazonLambdaClient();
      await callLambda();
    }

    public static async Task<GetAccountSettingsResponse> callLambda()
    {
      var request = new GetAccountSettingsRequest();
      var response = await lambdaClient.GetAccountSettingsAsync(request);
      return response;
    }

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
        var response = await dynamoClient.QueryAsync(new QueryRequest()
        {
          TableName = System.Environment.GetEnvironmentVariable(TABLE),
          KeyConditionExpression = "CprNo = :v_Cpr",
          ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {":v_Cpr", new AttributeValue { S = cprNo }}
                    },
          Limit = 1
        });
        var responseValue = response.Items.FirstOrDefault();
        if (responseValue == null)
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

  public class Policy
  {
    public string CprNo { get; set; }
    public string PolicyOwner { get; set; }
  }
}
```

Build the project and ensure there are no errors.
