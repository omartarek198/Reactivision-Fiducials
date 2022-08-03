using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TUIO_TEST
{
    static class Program
    {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(String[] argv)
		{

			int port = 0;
			switch (argv.Length)
			{
				case 1:
					port = int.Parse(argv[0], null);
					if (port == 0) goto default;
					break;
				case 0:
					port = 3333;
					break;
				default:
					Console.WriteLine("usage: java TuioDemo [port]");
					System.Environment.Exit(0);
					break;
			}

			TuioDemo app = new TuioDemo(port);
			Application.Run(app);
		}
	}
}
 
