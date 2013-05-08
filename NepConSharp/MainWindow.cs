using System;
using Gtk;
using NepConSharp;

public partial class MainWindow: Gtk.Window
{	
	private Gtk.ListStore systemListStore;
	private SystemInformation selectedSystem;
	private static SystemInfoDialog systemInfoDialog;

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		Init ();
	}

	private void Init ()
	{
		InitSystemList ();
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

	private void InitSystemList ()
	{
		var nameCol = SetupTextColumn ("Name", 0, true);
		var hostCol = SetupTextColumn ("Host", 1, true);
		lstSystem.AppendColumn (nameCol);
		lstSystem.AppendColumn (hostCol);
		systemListStore = new Gtk.ListStore (typeof(string), typeof(string));
		lstSystem.Model = systemListStore;
		lstSystem.Selection.Changed += OnSelectionChanged;
		SystemInformation.ItemSaved += OnItemSaved;
		foreach (SystemInformation system in SystemInformation.GetAll())
			systemListStore.AppendValues (system.name, system.host);
	}

	protected void OnItemSaved (object sender, EventArgs e)
	{
		var system = (SystemInformation) sender;
		systemListStore.AppendValues (system.name, system.host);
	}

	protected void OnConnectClicked (object sender, EventArgs e)
	{
		Console.WriteLine ("CONNECT: ");
	}

	protected void OnNewClicked (object sender, EventArgs e)
	{
		var system = selectedSystem == null ? new SystemInformation () : selectedSystem;
		systemInfoDialog = new SystemInfoDialog(system);
		systemInfoDialog.Show();
	}

    protected void OnSelectionChanged (object o, EventArgs args)
    {
        TreeIter iter;
        TreeModel model;
        if (((TreeSelection)o).GetSelected (out model, out iter))
        {
            var value = (string) model.GetValue (iter, 0);
			selectedSystem = SystemInformation.FindByName (value);
        }
    }
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

}
