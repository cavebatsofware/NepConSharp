using System;
using Gtk;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace NepConSharp
{
	class MainClass
	{
		private static LoginWindow loginWindow;
		private static MainWindow mainWindow;
		private static SystemInfoDialog systemInfoDialog;
		//private static NpgsqlConnection dbConn;

		public static void Main (string[] args)
		{
			Application.Init ();
			loginWindow = new LoginWindow ();
			loginWindow.Show ();
			Application.Run ();
		}

		public static bool LoadConfig (string file)
		{
			SystemInformation.LoadAll ();
			return true;
		}

		public static void BeginLogin (string name, string password)
		{
			if (AuthenticateUser (name, password)) {
				System.Console.WriteLine ("AUTHENTICATED!");
				systemInfoDialog = new SystemInfoDialog();
				systemInfoDialog.Show();
				mainWindow = new MainWindow ();
				loginWindow.Hide ();
				mainWindow.Show ();
			} else {
				System.Console.WriteLine ("AUTHENTICATION FAILED!");
			}
		}

		public static bool AuthenticateUser (string name, string password)
		{
			// TODO: actually authenticate
			return true;
		}
	}
}
