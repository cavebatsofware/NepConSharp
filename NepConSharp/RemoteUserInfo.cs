using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using Tamir.SharpSsh.jsch;

namespace NepConSharp
{

	public class RemoteUserInfo : UserInfo
	{
		private RemoteSystem mSystem;
		public RemoteUserInfo (RemoteSystem _system) { mSystem = _system; }

			public String getPassword () { return mSystem.GetAuthKey(); }

			public bool promptYesNo (String str) { return true; }
			
			public String getPassphrase () { return mSystem.GetAuthKey (); }

			public bool promptPassphrase (String message) { return true; }

			public bool promptPassword(String message)
			{
				mSystem.GetAuthKey ();
				return true;
			}

			public void showMessage(String message)
			{ 
				Console.WriteLine (message);
			}
	}

}
