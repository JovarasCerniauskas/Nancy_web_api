using Nancy.Hosting.Self;
using System;

namespace Nancy_web_api
{
    class Program
    {

        static void Main(string[] args)
        {

            HostConfiguration hostConfigs = new HostConfiguration();
            hostConfigs.UrlReservations.CreateAutomatically = true;
            using (NancyHost host = new NancyHost(new Uri("http://localhost:8000")))
            {
                host.Start();

                Console.WriteLine("Web api with nancy has been started");
                Console.WriteLine("Press enter to exit the application");
                Console.ReadLine();
            }
        }

    }
}
