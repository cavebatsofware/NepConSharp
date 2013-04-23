using System;
using Gtk;
using NepConSharp;

public partial class MainWindow: Gtk.Window
{	
	private Gtk.ListStore systemListStore;

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
		foreach (SystemInformation system in SystemInformation.GetAll())
			systemListStore.AppendValues (system.name, system.host);
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
