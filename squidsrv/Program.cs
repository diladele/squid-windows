using System.ServiceProcess;

namespace Diladele.Squid.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            { 
                new SquidService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
