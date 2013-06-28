using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using Tamir.SharpSsh.jsch;
using System.Security.Cryptography;
using System.Text;

namespace NepConSharp
{
	public class ListEventArgs : EventArgs
	{
		public enum ListEvent {
			SAVE, LOAD, SAVED, LOADED
		}

		public SystemList list;
		public ListEvent action;
		public delegate void CompleteHandler (bool success, ListEventArgs data);
		public event CompleteHandler OnComplete;

		public ListEventArgs (SystemList _list, ListEvent _action)
		{
			this.list = _list;
			this.action = _action;
		}

		public void Complete (bool _success, ListEventArgs _data)
		{
			if (OnComplete != null) OnComplete (_success, _data);
		}

	}

	public class SystemList : List<RemoteSystem>
	{

		private string Password = null;
		private string Username = null;

		public SystemList (string _username, string _password)
		{
			this.SetCredentials (_username, _password);
		}

		//public SystemList (string _username, string _password)
		//{
		//	this.SetCredentials (_username, _password);
		//}


		// Called when a new system is added to the list. 
		// This is not called when editing an existing system.
		public delegate void SystemCreatedHandler (object sender, SystemEventArgs data);
		public event SystemCreatedHandler SystemCreated;
		// called when we need to save the list
		public delegate void ListSaveHandler (object sender, ListEventArgs data);
		public event ListSaveHandler ListSave;
		// called when we need to save the list
		public delegate void ListLoadHandler (object sender, ListEventArgs data);
		public event ListLoadHandler ListLoad;
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

		protected void OnConnectionClosed (object sender, ConnectionEventArgs data)
		{
			if (data == null) data = new ConnectionEventArgs ();
			if (ConnectionClosed != null) ConnectionClosed (this, data);
		}

		protected void OnConnectionOpened (object sender, ConnectionEventArgs data)
		{
			if (data == null) data = new ConnectionEventArgs ();
			if (ConnectionOpened != null) ConnectionOpened (this, data);
		}

		new public bool Contains (RemoteSystem _info)
		{
			if (this.Find (_info) == null) return false;
			return true;
		}

		public RemoteSystem Find (string _name)
		{
			if (_name == null) return null;
			foreach (RemoteSystem info in this)
				if (info.GetName() == _name) return info;
			return null;
		}

		public RemoteSystem Find (RemoteSystem _info)
		{
			if (_info == null) return null;
			return Find (_info.GetName ());
		}

		new public bool Remove (RemoteSystem _info)
		{
			if (!this.Contains (_info))
				return false;
			foreach (RemoteSystem info in this)
				if (info.GetName () == _info.GetName ()) {
					if (info.IsConnected ()) info.CloseConnection ();
					base.Remove (info);
					OnSystemDeleted (this, new SystemEventArgs (info, "delete"));
					return true;
				}
			return false;
		}

		public void Add (RemoteSystem info, bool update = true)
		{
			if (info == null) return;
			if (this.Contains (info) && update) {
				RemoteSystem remote = null;
				this.Remove (info);
				if (!String.IsNullOrEmpty (info.GetGatewayName ()) && (remote = this.Find (info.GetGatewayName ())) != null) {
					info.SetGateway (remote);
					info.ConnectionClosed += OnConnectionClosed;
				}
				base.Add (info);
				OnSystemUpdated (this, new SystemEventArgs (info, "update"));
			} else {
				info.ConnectionClosed += OnConnectionClosed;
				base.Add (info);
				OnSystemCreated (this, new SystemEventArgs (info, "create"));
			}
		}

		public void AddRange (IEnumerable<RemoteSystem> items, bool update = true)
		{
			foreach (RemoteSystem info in items) this.Add (info, update);
		}

		public void SetCredentials (string _username, string _password)
		{
			if (String.IsNullOrEmpty (_username) || String.IsNullOrEmpty (_password)) {
				_username = " ";
				_password = " ";
			}
			this.Password = _password.PadRight (32, Char.Parse ("*")).Substring (0, 32);
			this.Username = _username.PadRight (16, Char.Parse ("*")).Substring (0, 16);
		}

		public bool SaveAll ()
		{
			if (this.Count > 0) {
				var jsonParser = new DataContractJsonSerializer (typeof(RemoteSystem[]));
				var fStream = new FileStream(SystemList.GetSystemsFile (true), FileMode.OpenOrCreate, FileAccess.Write);
				var crypter = Rijndael.Create();
				crypter.Key = ASCIIEncoding.ASCII.GetBytes (this.Password);
				crypter.IV = ASCIIEncoding.ASCII.GetBytes (this.Username);
				crypter.Mode = CipherMode.CBC;
				var cStream = new CryptoStream (fStream, crypter.CreateEncryptor (), CryptoStreamMode.Write);
				jsonParser.WriteObject (cStream, this.ToArray ());
				cStream.FlushFinalBlock ();
				fStream.Flush (true);
				cStream.Close ();
				fStream.Close ();
				return true;
			} else {
				return false;
			}
		}

		public bool LoadAll (bool reload = false)
		{
			var result = false;
			if (reload || this.Count == 0) {
				FileStream fStream = null;
				CryptoStream cStream = null;
				try {
					var jsonParser = new DataContractJsonSerializer (typeof(List<RemoteSystem>));
					fStream = new FileStream (SystemList.GetSystemsFile (true), FileMode.Open, FileAccess.Read);
					var crypter = Rijndael.Create ();
					crypter.Key = ASCIIEncoding.ASCII.GetBytes (this.Password);
					crypter.IV = ASCIIEncoding.ASCII.GetBytes (this.Username);
					crypter.Mode = CipherMode.CBC;
					cStream = new CryptoStream (fStream, crypter.CreateDecryptor (), CryptoStreamMode.Read);
					this.AddRange ((List<RemoteSystem>)jsonParser.ReadObject (cStream));
					cStream.Close ();
					result = true;
				} catch (System.IO.FileNotFoundException ex) {
					Console.WriteLine (ex.Message);
				} catch (System.Runtime.Serialization.SerializationException ex) {
					var msg = "It looks like there was a problem opening the settings file. Check your password.";
	            	var md = new Gtk.MessageDialog (null, Gtk.DialogFlags.Modal, Gtk.MessageType.Info, Gtk.ButtonsType.Ok, msg);
	            	md.Run ();
	            	md.Destroy();
					Console.WriteLine (ex.Message);
				} finally {
					if (fStream != null) fStream.Close ();
				}
			}
			return result;
		}

		public List<RemoteSystem> FindAllGateways ()
		{
			if (this.Count == 0) LoadAll ();
			var allGW = new List<RemoteSystem> ();
			foreach (RemoteSystem info in this) 
				if (info.GetIsGateway().ToLower() == "true") allGW.Add(info);
			return allGW;
		}

		public static string GetSystemsFile (bool fullPath = false)
		{
			return Config.GetSystemsFile (fullPath);
		}

	}

}
