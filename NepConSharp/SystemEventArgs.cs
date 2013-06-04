using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using Tamir.SharpSsh.jsch;

namespace NepConSharp
{

	public class SystemEventArgs : EventArgs
	{
		public RemoteSystem Before;
		public RemoteSystem After;
		public string Action;

		public SystemEventArgs ()
		{
			Before = new RemoteSystem ();
			After = new RemoteSystem ();
			Action = "";
		}

		public SystemEventArgs (RemoteSystem before, RemoteSystem after, string action)
		{
			Before = before;
			After = after;
			Action = action;
		}

		public SystemEventArgs (RemoteSystem after, string action)
		{
			Before = new RemoteSystem ();
			After = after;
			Action = action;
		}
	}

}
