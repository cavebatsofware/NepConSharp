using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using Tamir.SharpSsh.jsch;

namespace NepConSharp
{

	public class ConnectionEventArgs : EventArgs
	{
		public RemoteSystem System;
		public string EventName;

		public ConnectionEventArgs () 
		{
			return;
		}

		public ConnectionEventArgs (RemoteSystem _system, string _eventName)
		{
			System = _system;
			EventName = _eventName;
		}
	}

}
