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
			return true;
		}

		public static bool InitConnection (string info)
		{
//			var dbInfo = new DBConnectionInformation ();
//			var jsonParser = new DataContractJsonSerializer (typeof (DBConnectionInformation));
//			var stream = new MemoryStream (Encoding.UTF8.GetBytes (info));
//			dbInfo = (DBConnectionInformation)jsonParser.ReadObject (stream);
//			Console.WriteLine(dbInfo.host);
//			Console.WriteLine(dbInfo.port);
//			Console.WriteLine(dbInfo.pooling);
//			Console.WriteLine(dbInfo.database);
//			Console.WriteLine(dbInfo.timeout);
//			Console.WriteLine(dbInfo.user);
//			Console.WriteLine(dbInfo.password);
			return true;
		}

		public static void BeginLogin (string name, string password)
		{
			SystemInformation.LoadAll ();
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
			return true;
		}
	}
}
