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

		public static void Main (string[] args)
		{
			Application.Init ();
			loginWindow = new LoginWindow ();
			loginWindow.LoginClicked += (sender, data) => BeginLogin (data.Username, data.Password);
			loginWindow.Show ();
			Application.Run ();
		}

		public static void BeginLogin (string name, string password)
		{
			if (AuthenticateUser (name, password)) {
				System.Console.WriteLine ("AUTHENTICATED!");
				mainWindow = new MainWindow (name, password);
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
