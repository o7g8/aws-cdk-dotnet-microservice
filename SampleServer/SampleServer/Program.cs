using System;
using System.Threading;

using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Nancy;
using Nancy.Hosting.Self;
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
        static Random random = new Random();
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
                PolicyOwner = personGenerator.GenerateRandomFirstAndLastName(),
                CprNo = GenerateCprNo()
            };
            SendMessage(policy);
        }

        private static string GenerateCprNo()
        {
            var daysOld = random.Next(20 * 365, 100 * 365);
            var bday = DateTime.Today.AddDays(-daysOld);
            var seq = random.Next(1000, 9999);
            return bday.ToString("ddMMyy") + "-" + seq.ToString();
        }

        private static void SendMessage(Policy policy)
        {
            var body = JsonConvert.SerializeObject(policy);
            var resp = queue.SendMessage(new SendMessageRequest {
                QueueUrl = queueUrl,
                MessageGroupId = "policies",
                MessageDeduplicationId = policy.CprNo,
                MessageBody = body
            });
            Console.WriteLine($"Sent {body}, status: {resp.HttpStatusCode}");
        }
    }

    public class Policy {
        public string PolicyOwner { get; set; }
        public string CprNo { get; set; }
    }
}
