using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using Tamir.SharpSsh.jsch;
using System.Threading;

namespace NepConSharp
{
	public class RemoteConnection
	{
		private System.Diagnostics.Process RemoteProc = null;
		private RemoteSystem RemoteSys = null;
		public delegate void ConnectionClosedHandler (object sender, ConnectionEventArgs data);
		public event ConnectionClosedHandler ConnectionClosed;

		public RemoteConnection (RemoteSystem _remote)
		{
			RemoteSys = _remote;
		}

		public void Open ()
		{
			if (this.RemoteProc == null || this.RemoteProc.HasExited) this.Connect ();
		}

		public void Close ()
		{
			if (this.RemoteProc != null && !this.RemoteProc.HasExited) RemoteProc.Kill ();
		}

		private System.Diagnostics.ProcessStartInfo SetupCommand (RemoteSystem _remote)
		{
			var command = new System.Diagnostics.ProcessStartInfo ();
			var platform = Config.GetPlatformID ();
			switch (_remote.GetProtocol ())
			{
				case "SSH":
					if (platform == System.PlatformID.Unix)
					{
						command.FileName = "/usr/bin/xterm";
						command.Arguments = "-fa lucidatypewriter-14 " +
							"-fg midnightblue -bg palegoldenrod -geometry 120x40 " +
							"-e ssh -o UserKnownHostsFile=/dev/null -o StrictHostKeyChecking=no " + 
							_remote.GetUser () + "@127.0.0.1 -p" + _remote.GetLPort ().ToString ();
					} else if (platform == System.PlatformID.MacOSX) {
						// TODO: COMPLETE
					} else if (platform == System.PlatformID.Win32NT) {
						// TODO: COMPLETE
					}
					break;
				case "RDP":
					if (platform == System.PlatformID.Unix)
					{
						command.FileName = "/usr/bin/rdesktop";
						command.Arguments = "-u " + _remote.GetUser () + " -g 1280x1024 127.0.0.1:" + _remote.GetLPort ().ToString ();
					} else if (platform == System.PlatformID.MacOSX) {
						// TODO: COMPLETE
					} else if (platform == System.PlatformID.Win32NT) {
						command.FileName = "mstsc.exe";
						command.Arguments = "/v 127.0.0.1:" + _remote.GetLPort ();
					}
					break;
				case "VNC":
					if (platform == System.PlatformID.Unix)
					{
						command.FileName = "/usr/bin/vncviewer";
						command.Arguments = "-encodings tight -quality 6 -compresslevel 6 127.0.0.1::" + _remote.GetLPort ().ToString ();
					} else if (platform == System.PlatformID.MacOSX) {
						// TODO: COMPLETE
					} else if (platform == System.PlatformID.Win32NT) {
						command.FileName = "C:\\Program Files\\TightVNC\\tvnviewer.exe";
						command.Arguments = "127.0.0.1::" + _remote.GetLPort ().ToString ();
					}
					break;
				default:
					command.FileName = "/bin/true";
					command.Arguments = "";
					break;
			}
			return command;
		}

		private void Connect ()
		{
			if (this.RemoteProc != null && this.RemoteProc.HasExited) this.RemoteProc.Dispose ();
			this.RemoteProc = new System.Diagnostics.Process();
			this.RemoteProc.EnableRaisingEvents = true; 
			this.RemoteProc.StartInfo = SetupCommand (this.RemoteSys);
			this.RemoteProc.Start ();
			this.RemoteProc.Exited += (object sender, System.EventArgs e) => { 
				OnConnectionClosed (this, new ConnectionEventArgs(this.RemoteSys, "closed")); 
			};
		}

		protected void OnConnectionClosed (object sender, ConnectionEventArgs data)
		{
			if (data == null) data = new ConnectionEventArgs ();
			if (ConnectionClosed != null) ConnectionClosed (this, data);
		}
	}

}
