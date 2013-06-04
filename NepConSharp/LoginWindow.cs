using System;
using Gtk;

namespace NepConSharp
{
	public class LoginEventArgs : EventArgs
	{
		public string Username = null;
		public string Password = null;
		public LoginEventArgs (string _username, string _password)
		{
			this.Username = _username;
			this.Password = _password;
		}
	}

	public partial class LoginWindow : Gtk.Window
	{
		public delegate void LoginClickedHandler (object sender, LoginEventArgs data);
		public event LoginClickedHandler LoginClicked;

		public LoginWindow () : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}

		protected void OnLoginClickedEvent (object sender, EventArgs a)
		{
			string name = tbxName.Text;
			string password = tbxPassword.Text;
			if (this.LoginClicked != null) {
				var args = new LoginEventArgs (name, password);
				LoginClicked (this, args);
			}
			this.Hide ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}
	}
}

