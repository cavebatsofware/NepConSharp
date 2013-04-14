using System;
using Gtk;

namespace NepConSharp
{
	public partial class LoginWindow : Gtk.Window
	{
		public LoginWindow () : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}

		protected void OnLoginClickedEvent (object sender, EventArgs a)
		{
			string name = tbxName.Text;
			string password = tbxPassword.Text;
			MainClass.BeginLogin (name, password);
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}
	}
}

