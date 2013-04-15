using System;

namespace NepConSharp
{
	public partial class SystemInfoDialog : Gtk.Dialog
	{
		public SystemInfoDialog ()
		{
			this.Build ();
			this.LoadGateways();
			this.LoadProtocols();
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

		protected void OnApplyClickedEvent (object sender, EventArgs e)
		{
			Console.WriteLine (this.entryName.Text);
			Console.WriteLine (this.userName.Text);
			Console.WriteLine (this.password.Text);
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

