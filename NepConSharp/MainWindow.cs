using System;
using Gtk;
using NepConSharp;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{	
	private Gtk.ListStore systemListStore;
	private SystemList systemList = null;
	private RemoteSystem selectedSystem;
	private Dictionary<int, bool> localPorts = new Dictionary<int, bool>();
	Gdk.Pixbuf Connected = new Gdk.Pixbuf (Config.GetConnectedImageFile (), 32, 32);
	Gdk.Pixbuf Disconnected = new Gdk.Pixbuf (Config.GetDisconnectedImageFile (), 32, 32);
	private int[] portBlock = { 9190, 9191, 9192, 9193, 9194, 9195, 9196, 9197, 9198, 9199 };

	public MainWindow (string username, string password): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		Init (username, password);
	}

	private void Init (string username, string password)
	{
		systemList = new SystemList(username, password);
		InitSystemList ();
		foreach (int port in portBlock) localPorts.Add (port, false);
	}

	public static Gtk.TreeViewColumn SetupTextColumn(string name, int pos, bool expand)
	{
		var col = new Gtk.TreeViewColumn ();
		var cell = new Gtk.CellRendererText ();
		col.Title = name;
		col.PackStart (cell, true);
		col.AddAttribute (cell, "text", pos);
		return col;
	}

	private int GetOpenPort ()
	{
		foreach (KeyValuePair<int, bool> item in localPorts)
			if (item.Value == false) {
				return item.Key;
			}
		return -1;
	}

	private void ReleasePort (int _lport)
	{
		this.localPorts[_lport] = false;
	}

	private void ReservePort (int _lport)
	{
		this.localPorts[_lport] = true;
	}

	private void InsertSystem (RemoteSystem _system)
	{
		var connState = (_system.IsConnected () ? Connected : Disconnected);
		systemListStore.AppendValues (_system.GetName (), _system.GetHost (), connState);
	}

	private void InitSystemList ()
	{
		var nameCol = SetupTextColumn ("Name", 0, true);
		var hostCol = SetupTextColumn ("Host", 1, true);
		var connCol = new Gtk.TreeViewColumn ();
		var pbCell = new Gtk.CellRendererPixbuf ();
		connCol.Title = "Connected";
		connCol.PackStart (pbCell, true);
		connCol.AddAttribute (pbCell, "pixbuf", 2);
		systemList.SystemCreated += OnSystemCreated;
		systemList.SystemUpdated += OnSystemUpdated;
		systemList.SystemDeleted += OnSystemDeleted;
		systemList.ConnectionClosed += OnConnectionClosed;
		lstSystem.AppendColumn (nameCol);
		lstSystem.AppendColumn (hostCol);
		lstSystem.AppendColumn (connCol);
		systemListStore = new Gtk.ListStore (typeof(string), typeof(string), typeof(Gdk.Pixbuf));
		lstSystem.Model = systemListStore;
		lstSystem.Selection.Changed += OnSelectionChanged;
		systemList.LoadAll ();
	}

	protected void InitListStore (RemoteSystem except = null)
	{
		foreach (RemoteSystem system in systemList) {
			if (except != null && except.GetName () == system.GetName ())
				continue;
			InsertSystem (system);
		}
	}

	protected void OnSystemCreated (object sender, SystemEventArgs e)
	{
		var system = e.After;
		this.InsertSystem (system);
	}

	protected void OnSystemUpdated (object sender, SystemEventArgs e)
	{
		var system = e.After;
		systemListStore.Clear ();
		this.SystemChanged (system);
	}

	protected void OnSystemDeleted (object sender, SystemEventArgs e)
	{
		systemListStore.Clear ();
		this.InitListStore ();
	}

	protected void OnConnectionClosed (object sender, ConnectionEventArgs e)
	{
		var system = e.System ;
		this.SystemChanged (system);
		this.ReleasePort (system.GetLPort ());
	}

	private void SystemChanged (RemoteSystem system)
	{
		systemListStore.Clear ();
		this.InitListStore (system);
		this.InsertSystem (system);
	}

	protected void OnConnectClicked (object sender, EventArgs e)
	{
		if (selectedSystem != null) {
			var lport = GetOpenPort ();
			if (lport == -1) return;
			if (selectedSystem.OpenConnection (systemList.Find (selectedSystem.GetGatewayName ()), lport))
			{
				this.ReservePort (lport);
				this.SystemChanged (selectedSystem);
			}
		}
	}

	protected void OnChangePasswordClicked (object sender, EventArgs e)
	{
		var loginWindow = new LoginWindow ();
		loginWindow.LoginClicked += (s, data) => systemList.SetCredentials (data.Username, data.Password);
		loginWindow.Show ();
	}

	protected void OnDisconnectClicked (object sender, EventArgs e)
	{
		if (selectedSystem != null) selectedSystem.CloseConnection ();

	}

	protected void OnNewClicked (object sender, EventArgs e)
	{
		var system = new RemoteSystem ();
		new SystemInfoDialog(system, systemList).Show();
	}

	protected void OnEditClicked (object sender, EventArgs e)
	{
		if (selectedSystem == null) return;
		new SystemInfoDialog(selectedSystem, systemList).Show();
	}

	protected void OnDeleteClicked (object sender, EventArgs e)
	{
		if (selectedSystem == null) return;
		systemList.Remove (selectedSystem);
	}

	protected void OnSaveSettingsClicked (object sender, EventArgs e)
	{
		systemList.SaveAll ();
	}

    protected void OnSelectionChanged (object o, EventArgs args)
    {
        TreeIter iter;
        TreeModel model;
        if (((TreeSelection)o).GetSelected (out model, out iter))
        {
            var value = (string) model.GetValue (iter, 0);
			selectedSystem = systemList.Find (value);
        }
    }
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		systemList.SaveAll ();
		Application.Quit ();
		a.RetVal = true;
	}

}
