using System;

namespace NepConSharp
{
	public partial class SystemInfoDialog : Gtk.Dialog
	{
		public RemoteSystem Info;
		public SystemList SystemList;

		public SystemInfoDialog (RemoteSystem _info, SystemList _list): base ()		
		{
			Info = _info;
			SystemList = _list;
			this.Build ();
			this.LoadGateways ();
			this.LoadProtocols ();
			this.Populate ();
		}

		public void Populate ()
		{
			this.entryName.Text = Info.GetName ();
			this.userName.Text = Info.GetUser ();
			this.port.Text = Info.GetPort ();
			this.host.Text = Info.GetHost ();
			if (Info.GetIsGateway () == "true") this.isGateway.Active = true;
			if (Info.GetAuthType () == "password") {
				this.password.Text = Info.GetAuthKey ();
				this.authPassword.Active = true;
			} else {
				this.authPublicKey.Active = true;
			}
			SetSelectedItem (this.gateway, Info.GetGatewayName ());
			SetSelectedItem (this.protocol, Info.GetProtocol ());
		}

		public void LoadGateways ()
		{
			var gatewayInfos = SystemList.FindAllGateways ();
			foreach(RemoteSystem gateway in gatewayInfos) {
				this.gateway.AppendText (gateway.GetName ());
			}
		}

		public void LoadProtocols ()
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

		private void SetSelectedItem (Gtk.ComboBox box, string _value)
		{
			Gtk.TreeIter iter;
			box.Model.GetIterFirst (out iter);
			do {
				var row = (string) box.Model.GetValue (iter, 0);
				if (String.Equals (_value, row)) {
					box.SetActiveIter (iter);
					break;
				}
			} while (box.Model.IterNext (ref iter));
		}

		protected void OnApplyClickedEvent (object sender, EventArgs e)
		{
			Info.SetName (this.entryName.Text);
			Info.SetHost (this.host.Text);
			Info.SetIsGateway (this.isGateway.Active ? "true" : "false");
			Info.SetGatewayName (this.GetSelectedItem (this.gateway));
			Info.SetAuthType (this.authPublicKey.Active ? "key" : "password");
			Info.SetAuthKey (this.password.Text);
			Info.SetPort (this.port.Text);
			Info.SetProtocol (this.GetSelectedItem (this.protocol));
			Info.SetUser (this.userName.Text);
			SystemList.Add (Info);
			this.Destroy ();
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

