using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Threading;

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
        static void Main(string[] args)
        {
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
            Console.WriteLine("Saving policy");
        }
    }
}
