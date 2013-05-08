using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace NepConSharp
{

	public class SystemEventArgs : EventArgs
	{
		public SystemInformation Before;
		public SystemInformation After;
		public string Action;

		public SystemEventArgs ()
		{
			Before = new SystemInformation ();
			After = new SystemInformation ();
			Action = "";
		}

		public SystemEventArgs (SystemInformation before, SystemInformation after, string action)
		{
			Before = before;
			After = after;
			Action = action;
		}

		public SystemEventArgs (SystemInformation after, string action)
		{
			Before = new SystemInformation ();
			After = after;
			Action = action;
		}
	}

	public class ConnectionEventArgs : EventArgs
	{
	}

	public class SystemList : List<SystemInformation>
	{
		// Called when a new system is added to the list. 
		// This is not called when editing an existing system.
		public delegate void SystemCreatedHandler (object sender, SystemEventArgs data);
		public event SystemCreatedHandler SystemCreated;
		// Called when updating an existing system.
		public delegate void SystemUpdatedHandler (object sender, SystemEventArgs data);
		public event SystemUpdatedHandler SystemUpdated;
		// Called when deleting an existing system.
		public delegate void SystemDeletedHandler (object sender, SystemEventArgs data);
		public event SystemDeletedHandler SystemDeleted;
		// Called when a connection is terminated.
		public delegate void ConnectionClosedHandler (object sender, ConnectionEventArgs data);
		public event ConnectionClosedHandler ConnectionClosed;
		// Called when a connection is established.
		public delegate void ConnectionOpenedHandler (object sender, ConnectionEventArgs data);
		public event ConnectionClosedHandler ConnectionOpened;

		protected void OnSystemCreated (object sender, SystemEventArgs data)
		{
			if (data == null) data = new SystemEventArgs ();
			if (SystemCreated != null) SystemCreated(this, data);
		}

		protected void OnSystemUpdated (object sender, SystemEventArgs data)
		{
			if (data == null) data = new SystemEventArgs ();
			if (SystemUpdated != null) SystemUpdated(this, data);
		}

		protected void OnSystemDeleted (object sender, SystemEventArgs data)
		{
			if (data == null) data = new SystemEventArgs ();
			if (SystemDeleted != null) SystemDeleted (this, data);
		}

		protected void OnConnectionClosed (object sender, SystemEventArgs data)
		{
			if (data == null) data = new SystemEventArgs ();
			if (ConnectionClosed != null) ConnectionClosed (this, data);
		}

		protected void OnConnectionOpened (object sender, SystemEventArgs data)
		{
			if (data == null) data = new SystemEventArgs ();
			if (ConnectionOpened != null) ConnectionOpened (this, data);
		}

		public override bool Contains (SystemInformation _info)
		{
			if (this.Find (_info) == null) return false;
			return true;
		}

		public SystemInformation Find (SystemInformation _info)
		{
			if (_info == null) return null;
			foreach (SystemInformation info in this)
				if (info.Name == _info.Name) return info;
			return null;
		}

		public override bool Remove (SystemInformation _info)
		{
			if (!this.Contains (_info))
				return false;
			foreach (SystemInformation info in this)
				if (info.Name == _info.Name) {
					
					return true;
				}
			return false;
		}

		public override void Add (SystemInformation info, bool update = true)
		{
			if (info == null) return;
			if (this.Contains (info) && update) {
				this.Remove(info);
				base.Add (info);
				OnSystemUpdated(this, new SystemEventArgs (info, "update"));
			} else {
				base.Add (info);
				OnSystemCreated (this, new SystemEventArgs (info, "create"));
			}
		}

		public override void AddRange (IEnumerable<SystemInformation> items, bool update = true)
		{
			foreach (SystemInformation info in items) this.Add (info, update);
		}

		public SystemInformation NewSystemInformation ()
		{

		}

		public static bool LoadAll (bool reload = false)
		{
			if (reload || this.Count == 0) {
				var jsonParser = new DataContractJsonSerializer (typeof(List<SystemInformation>));
				this.AddRange(
					(List<SystemInformation>)jsonParser.ReadObject (File.OpenText (GetSystemsFile (true)).BaseStream));
				return true;
			} else {
				return true;
			}
		}

		public static List<SystemInformation> FindAllGateways ()
		{
			if (this.Count == 0) LoadAll ();
			var allGW = new List<SystemInformation> ();
			foreach (SystemInformation info in this) 
				if (info.IsGateway.ToLower() == "true") allGW.Add(info);
			return allGW;
		}

		public static string GetSystemsFile (bool fullPath = false)
		{
			return Config.GetSystemsFile (fullPath);
		}

	}

	[DataContract]
	public class SystemInformation
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

		#reigon PUBLIC_PROPERTIES

		public string GetHost () { return this.Host; }
		public string GetName() { return this.Name; }
		public string GetGatewayName () { return this.GatewayName; }
		public string GetIsGateway () { return this.IsGateway; }
		public string GetAuthType () { return this.AuthType; }
		public string GetAuthKey () { return this.AuthKey; }
		public string GetProtocol () { return this.Protocol; }
		public string GetUser () { return this.User; }
		public string GetPort () { return this.Port; }
		public string IsSaved () { return this.Saved; }

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
			this.Gateway = gateway;
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

		#endregion

		#region EVENTS

		public delegate void SystemSavedHandler (object sender, SystemEventArgs data);
		public event SystemSavedHandler SystemSaved;
		public delegate void SystemChangedHandler (object sender, SystemEventArgs data);
		public event SystemChangedHandler SystemChanged;

		#endregion

		public SystemInformation ()
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

		public SystemInformation (SystemInformation info)
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
			this.Saved = true;
			OnSystemSaved (this, new SystemEventArgs(this, "saved"))
		}

		public SystemInformation Clone ()
		{
				return new SystemInformation (this);
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
	}
}

