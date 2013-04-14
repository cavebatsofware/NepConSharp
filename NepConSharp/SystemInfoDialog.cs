using System;

namespace NepConSharp
{
	public partial class SystemInfoDialog : Gtk.Dialog
	{
		public SystemInfoDialog ()
		{
			this.Build ();
			this.LoadGateways();
		}

		public void LoadGateways()
		{
			this.gateway.AppendText("Gateway 1");
		}

		protected void OnApplyClickedEvent (object sender, EventArgs e)
		{
			Console.WriteLine (this.entryName.Text);
			Console.WriteLine(this.userName.Text);
			Console.WriteLine (this.password.Text);
		}		

		protected void OnIsGatewayToggled (object sender, EventArgs e)
		{
			this.gateway.Sensitive = !this.isGateway.Active;
		}


	}
}

