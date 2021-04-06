/*
 * Copyright (C) 2015 Diladele B.V.
 *
 * Diladele Squid Installer software is distributed under GPL license.
 */

using System;
using System.ServiceProcess;

namespace Diladele.Squid.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var squidService = new SquidService();
            if (Environment.UserInteractive)
            {
                squidService.TestStartupAndStop(args);
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] { squidService });
            }
        }
    }
}
