using System;

namespace NepConSharp
{
	public static class Config
	{
		private static string UserHome = null;
		private static string SettingsFile = "systems.json.rij";
		private static string SettingsPath = ".nepcon";
		private static string ConnectedFile = "connected_xs.png";
		private static string DisconnectedFile = "disconnected_xs.png";

		public static string GetSettingsFile (bool fullPath = false)
		{
			var file = "settings.json";
			if (fullPath) {
				return GetAppSettingsDir(true) + "/" + file;
			} else {
				return file; 
			}
		}

		public static string GetSystemsFile (bool fullPath = false)
		{ 
			if (fullPath) {
				return GetAppSettingsDir(true) + "/" + SettingsFile;
			} else {
				return SettingsFile; 
			}
		}

		public static string GetAppSettingsDir (bool fullPath = false)
		{ 
			if (fullPath) {
				return GetUserHome () + "/" + SettingsPath;
			} else {
				return SettingsPath;
			}
		}

		public static string GetConnectedImageFile ()
		{
			return GetAppSettingsDir (true) + "/" + ConnectedFile;
		}

		public static string GetDisconnectedImageFile ()
		{
			return GetAppSettingsDir (true) + "/" + DisconnectedFile;
		}

		public static string GetUserHome ()
		{
			// return if set
			if (UserHome != null) return UserHome;
			// find home based on os type
			var platform = GetPlatformID ();
			if (platform == PlatformID.Unix || platform == PlatformID.MacOSX) {
				return UserHome = Environment.GetEnvironmentVariable ("HOME");
			} else if (platform == PlatformID.Win32NT) {
				return UserHome = Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%");	
			} else {
				return UserHome = Environment.CurrentDirectory;
			}
		}

		public static System.PlatformID GetPlatformID ()
		{
			return Environment.OSVersion.Platform;
		}
	}
}

