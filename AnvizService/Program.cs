using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using System.IO;
using System.Security.Policy;

namespace AnvizProject
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			String[] myargs = Environment.GetCommandLineArgs();
			if (myargs.Length > 1)
			{
				String arg = myargs[1].ToLower();
				if ((arg == "-install") || (arg == "-i"))
				{
					String[] args = new String[1];
					args[0] = Assembly.GetExecutingAssembly().Location;
					ExecuteInstallUtil(args);
				}
				if ((arg == "-uninstall") || (arg == "-u"))
				{
					String[] args = new String[2];
					args[0] = Assembly.GetExecutingAssembly().Location;
					args[1] = "-u";
					ExecuteInstallUtil(args);
				}
				return;
			} 
			
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new AnvizService() 
			};
			ServiceBase.Run(ServicesToRun);
		}
		static void ExecuteInstallUtil(String[] args)
		{
			AppDomain dom = AppDomain.CreateDomain("execDom");
			String objAssemblyPath = Path.GetDirectoryName((new Object()).GetType().Assembly.Location);
			String iuPath = Path.Combine(objAssemblyPath, "InstallUtil.exe");
			Evidence evidence = new Evidence();
			dom.ExecuteAssembly(iuPath, evidence, args);
		}
	}
}
