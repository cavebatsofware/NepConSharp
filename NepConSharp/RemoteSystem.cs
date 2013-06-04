using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using Tamir.SharpSsh.jsch;
using System.Threading;

namespace NepConSharp
{
	[DataContract]
	public class RemoteSystem
	{

		#region DATA_MEMBERS

		[DataMember]
		private string Host;

		[DataMember]
		protected string Port;

		[DataMember]
		protected string User;

		[DataMember]
		protected string AuthType;

		[DataMember]
		private string AuthKey;

		[DataMember]
		private string IsGateway;

		[DataMember]
		protected string GatewayName;

		[DataMember]
		protected string Protocol;

		[DataMember]
		protected string Name;

		#endregion

		#region PUBLIC_PROPERTIES

		public string GetHost () { return this.Host; }
		public string GetName() { return this.Name; }
		public string GetGatewayName () { return this.GatewayName; }
		public string GetIsGateway () { return this.IsGateway; }
		public RemoteSystem GetGateway () { return this.Gateway; }
		public string GetAuthType () { return this.AuthType; }
		public string GetAuthKey () { return this.AuthKey; }
		public string GetProtocol () { return this.Protocol; }
		public string GetUser () { return this.User; }
		public string GetPort () { return this.Port; }
		public int GetLPort () { return this.LPort; }
		public bool IsSaved () { return this.Saved; }
		public bool IsConnected () { return this.SSHSession == null ? false : this.SSHSession.isConnected (); }

		public int GetPortNumber ()
		{
			int port = -1;
			return int.TryParse (this.GetPort (), out port) ? port : -1;
		}

		public void SetHost (string host)
		{
			var data = new SystemEventArgs();
			data.Before = this.Clone ();
			this.Host = host;
			data.After = this;
			data.Action = "change";
			OnSystemChanged (this, data);
		}

		public void SetName (string name)
		{
			var data = new SystemEventArgs();
			data.Before = this.Clone ();
			this.Name = name;
			data.After = this;
			data.Action = "change";
			OnSystemChanged (this, data);
		}

		public void SetGatewayName (string gateway)
		{
			var data = new SystemEventArgs();
			data.Before = this.Clone ();
			this.GatewayName = gateway;
			data.After = this;
			data.Action = "change";
			OnSystemChanged (this, data);
		}

		public void SetIsGateway (string isGateway)
		{
			var data = new SystemEventArgs();
			data.Before = this.Clone ();
			this.IsGateway = isGateway;
			data.After = this;
			data.Action = "change";
			OnSystemChanged (this, data);
		}

		public void SetPort (string port)
		{
			var data = new SystemEventArgs();
			data.Before = this.Clone ();
			this.Port = port;
			data.After = this;
			data.Action = "change";
			OnSystemChanged (this, data);
		}

		public void SetPortNumber (uint port)
		{
			this.SetPort (port.ToString ());
		}

		public void SetUser (string user)
		{
			var data = new SystemEventArgs();
			data.Before = this.Clone ();
			this.User = user;
			data.After = this;
			data.Action = "change";
			OnSystemChanged (this, data);
		}

		public void SetAuthType (string authType)
		{
			var data = new SystemEventArgs();
			data.Before = this.Clone ();
			this.AuthType = authType;
			data.After = this;
			data.Action = "change";
			OnSystemChanged (this, data);
		}

		public void SetGateway (RemoteSystem _gateway)
		{
			Gateway = _gateway;
		}

		public void SetAuthKey (string authKey)
		{
			var data = new SystemEventArgs();
			data.Before = this.Clone ();
			this.AuthKey = authKey;
			data.After = this;
			data.Action = "change";
			OnSystemChanged (this, data);
		}

		public void SetProtocol (string protocol)
		{
			var data = new SystemEventArgs();
			data.Before = this.Clone ();
			this.Protocol = protocol;
			data.After = this;
			data.Action = "change";
			OnSystemChanged (this, data);
		}

		#endregion

		#region PRIVATE_FIELDS

		private bool Saved = false;
		private bool Changed = false;
		private int LPort = -1;
		private Session SSHSession = null;
		private RemoteConnection RemoteSession = null;
		private RemoteSystem Gateway = null;

		#endregion

		#region EVENTS

		public delegate void SystemSavedHandler (object sender, SystemEventArgs data);
		public event SystemSavedHandler SystemSaved;
		public delegate void SystemChangedHandler (object sender, SystemEventArgs data);
		public event SystemChangedHandler SystemChanged;
		public delegate void ConnectionOpenedHandler (object sender, ConnectionEventArgs data);
		public event ConnectionOpenedHandler ConnectionOpened;
		public delegate void ConnectionClosedHandler (object sender, ConnectionEventArgs data);
		public event ConnectionClosedHandler ConnectionClosed;

		#endregion

		public RemoteSystem ()
		{
			this.Name = "";
			this.Host = "";
			this.Port = "";
			this.User = "";
			this.AuthType = "";
			this.AuthKey = "";
			this.IsGateway = "";
			this.GatewayName = "";
			this.Protocol = "";
		}

		public RemoteSystem (RemoteSystem info)
		{
			this.Name = info.GetName ();
			this.Host = info.GetHost ();
			this.GatewayName = info.GetGatewayName ();
			this.IsGateway = info.GetIsGateway ();
			this.User = info.GetUser ();
			this.Port = info.GetPort ();
			this.Protocol = info.GetProtocol ();
			this.AuthType = info.GetAuthType ();
			this.AuthKey = info.GetAuthKey ();
		}

		public bool Save ()
		{
			OnSystemSaved (this, new SystemEventArgs(this, "saved"));
			return true;
		}

		public RemoteSystem Clone ()
		{
			return new RemoteSystem (this);
		}

		public bool OpenConnection (RemoteSystem _defaultGateway, int _localPort)
		{
			if (!this.IsConnected ()) {
				var jsch = new JSch ();
				var gw = _defaultGateway;
				this.LPort = _localPort;
				this.SSHSession = jsch.getSession (gw.GetUser (), gw.GetHost (), gw.GetPortNumber ());
				this.SSHSession.setUserInfo (new RemoteUserInfo (gw));
				try {
					this.SSHSession.connect ();
				} catch (Tamir.SharpSsh.jsch.JSchException ex) {
					Console.WriteLine ("Could not establish connection to remote gateway.");
					return false;
				}
				if (this.IsConnected ()) {
					this.SSHSession.setPortForwardingL (this.LPort, this.GetHost (), this.GetPortNumber ());
					this.RemoteSession = new RemoteConnection (this);
					this.RemoteSession.ConnectionClosed += (object sender, ConnectionEventArgs args) => { this.CloseConnection (); };
					this.RemoteSession.Open ();
				}
			}
			return this.IsConnected ();
		}

		public bool CloseConnection ()
		{
			if (this.IsConnected ()) {
				this.RemoteSession.Close ();
				this.RemoteSession = null;
				this.SSHSession.delPortForwardingL (this.LPort);
				this.SSHSession.disconnect ();
				this.OnConnectionClosed (this, new ConnectionEventArgs (this, "closed"));
				this.SSHSession = null;
			} 
			return !this.IsConnected ();
		}

		public override string ToString ()
		{
			return this.Name;
		}

		protected void OnSystemSaved (object sender, SystemEventArgs data)
		{
			if (data == null) data = new SystemEventArgs ();
			if (SystemSaved != null) SystemSaved (this, data);
		}

		protected void OnSystemChanged (object sender, SystemEventArgs data)
		{
			if (data == null) data = new SystemEventArgs ();
			if (SystemChanged != null) SystemChanged (this, data);
		}

		protected void OnConnectionOpened (object sender, ConnectionEventArgs data)
		{
			if (data == null) data = new ConnectionEventArgs ();
			if (ConnectionOpened != null) ConnectionOpened (this, data);
		}

		protected void OnConnectionClosed (object sender, ConnectionEventArgs data)
		{
			if (data == null) data = new ConnectionEventArgs ();
			if (ConnectionClosed != null) ConnectionClosed (this, data);
		}
	}


}

