using System;

namespace NepConSharp
{
	public partial class SystemInfoDialog : Gtk.Dialog
	{
		public SystemInformation Info;

		public SystemInfoDialog (SystemInformation info)
		{
			this.Build ();
			this.LoadGateways();
			this.LoadProtocols();
			Info = info;
		}

		public void LoadGateways()
		{
			var gatewayInfos = SystemInformation.GetAllGW();
			foreach(SystemInformation gateway in gatewayInfos) {
				this.gateway.AppendText(gateway.name);
			}
		}

		public void LoadProtocols()
		{
			//TODO: Protocols should be dynamically loaded here instead
			//      of hardcoded. Each protocol should probably be it's
			//		own class.
		}

		private string GetSelectedItem (Gtk.ComboBox box)
		{
			Gtk.TreeIter iter;
			if (box.GetActiveIter (out iter))
				return ((string) box.Model.GetValue (iter, 0));
			return "";
		}

		protected void OnApplyClickedEvent (object sender, EventArgs e)
		{
			Console.WriteLine (this.entryName.Text);
			Console.WriteLine (this.userName.Text);
			Console.WriteLine (this.password.Text);

			Info.name = this.entryName.Text;
			Info.host = this.host.Text;
			Info.isgw = this.isGateway.Active ? "true" : "false";
			Info.hasgw = this.GetSelectedItem (this.gateway);
			Info.authtype = this.authPublicKey.Active ? "key" : "password";
			Info.authkey = this.password.Text;
			Info.port = this.port.Text;
			Info.protocol = this.GetSelectedItem (this.protocol);
			Info.user = this.userName.Text;
			Info.Save ();
			//TODO: Return attributes and/or save system info.
		}
				
		protected void OnCancelClickedEvent (object sender, EventArgs e)
		{
			this.Destroy();
		}

		protected void OnIsGatewayToggled (object sender, EventArgs e)
		{
			if (this.isGateway.Active) {
				this.protocol.Active = 1; //TODO: Remove magic number.
				this.gateway.Active = -1;
			}

			this.gateway.Sensitive = this.protocol.Sensitive = !this.isGateway.Active;
		}		


	}
}

