using System;

namespace NepConSharp
{
	public static class Config
	{
		private static string UserHome = null; 

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
			var file = "systems.json";
			if (fullPath) {
				return GetAppSettingsDir(true) + "/" + file;
			} else {
				return file; 
			}
		}

		public static string GetAppSettingsDir (bool fullPath = false)
		{ 
			var path = ".nepcon";
			if (fullPath) {
				return GetUserHome () + "/" + path;
			} else {
				return path;
			}
		}

		public static string GetDBName ()
		{
			return "nepcon";
		}

		public static string GetUserHome ()
		{
			// return if set
			if (UserHome != null) return UserHome;
			// find home based on os type
			var platform = Environment.OSVersion.Platform;
			if (platform == PlatformID.Unix || platform == PlatformID.MacOSX) {
				return UserHome = Environment.GetEnvironmentVariable ("HOME");
			} else if (platform == PlatformID.Win32NT) {
				return UserHome = Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%");	
			} else {
				return UserHome = Environment.CurrentDirectory;
			}
		}
	}
}

