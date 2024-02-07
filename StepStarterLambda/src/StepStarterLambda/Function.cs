using System.Text.Json;
using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace StepStarterLambda;

public class Function
{
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        // Execute Step Function
        var _region = Environment.GetEnvironmentVariable("Region");

        var _stateMachineArn = Environment.GetEnvironmentVariable("StateMachineArn");

        bool _synchronous = bool.Parse(Environment.GetEnvironmentVariable("Synchronous"));

        string jsonContent = JsonSerializer.Serialize(request);

        return StartExecutionAsync(_region, jsonContent, _stateMachineArn, _synchronous).Result;
    }

    /// <summary>
    /// Start execution of an AWS Step Functions state machine.
    /// </summary>
    /// <param name="executionName">The name to use for the execution.</param>
    /// <param name="executionJson">The JSON string to pass for execution.</param>
    /// <param name="stateMachineArn">The Amazon Resource Name (ARN) of the
    /// Step Functions state machine.</param>
    /// <returns>The Amazon Resource Name (ARN) of the AWS Step Functions
    /// execution.</returns>
    public async Task<string> StartExecutionAsync(string Region, string executionJson, string stateMachineArn, bool synchronous)
    {
        // Set the AWS region where your S3 bucket is located
        var _region = RegionEndpoint.GetBySystemName(Region);

        var stepFunctionsClient = new AmazonStepFunctionsClient(_region);

        if (synchronous)
        {
            var executionRequest = new StartSyncExecutionRequest
            {
                Input = executionJson,
                StateMachineArn = stateMachineArn
            };

            var response = await stepFunctionsClient.StartSyncExecutionAsync(executionRequest);

            return response.Output;
        }
        else
        {
            var executionRequest = new StartExecutionRequest
            {
                Input = executionJson,
                StateMachineArn = stateMachineArn
            };

            var response = await stepFunctionsClient.StartExecutionAsync(executionRequest);

            return response.ExecutionArn;
        }
    }
}
