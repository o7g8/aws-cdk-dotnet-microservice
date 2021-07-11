using System;
using System.Threading;

using Nancy;
using Nancy.Hosting.Self;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Newtonsoft.Json;
using RandomNameGeneratorLibrary;

namespace SampleServer
{

    public class VersionModule : NancyModule
    {
        public VersionModule()
        {
            Get("/", parameters => "Version 0.1");
        }
    }
    class Program
    {
        const string QUEUE_SSM_PARAMETER_NAME = "/monolith/policyQueueUrl";
        static PersonNameGenerator personGenerator = new PersonNameGenerator();
        static string queueUrl;
        static AmazonSQSClient queue;

        static void Main(string[] args)
        {
            var queueUrlParam = new AmazonSimpleSystemsManagementClient()
                .GetParameter(new GetParameterRequest {
                    Name = QUEUE_SSM_PARAMETER_NAME
                });
            queueUrl = queueUrlParam.Parameter.Value;
            Console.WriteLine($"Using queue URL: {queueUrlParam.Parameter.Value}");
            queue = new AmazonSQSClient();

            using (var nancyHost = new NancyHost(new Uri("http://localhost:8888/")))
            {
                nancyHost.Start();

                Console.WriteLine("Nancy now listening - navigating to http://localhost:8888/. Press enter to stop");

                var timer = new Timer(savePolicy, null, 0, 3000);
                Console.ReadKey();
                timer.Dispose();
            }

            Console.WriteLine("Stopped. Good bye!");
        }

        private static void savePolicy(object state)
        {
            var policy = new Policy {
                PolicyId = Guid.NewGuid(),
                PolicyOwner = personGenerator.GenerateRandomFirstAndLastName(),
            };
            SendMessage(policy);
        }

        private static void SendMessage(Policy policy)
        {
            var body = JsonConvert.SerializeObject(policy);
            var resp = queue.SendMessage(new SendMessageRequest {
                QueueUrl = queueUrl,
                MessageGroupId = "policies",
                MessageDeduplicationId = policy.PolicyId.ToString(),
                MessageBody = body
            });
            Console.WriteLine($"Sent {body}, status: {resp.HttpStatusCode}");
        }
    }

    public class Policy {
        public Guid PolicyId { get; set; }
        public string PolicyOwner { get; set; }
    }
}
