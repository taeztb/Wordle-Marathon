using Godot;
using System;

public partial class Reel : VBoxContainer
{
	private readonly PackedScene PopupScene = 
		ResourceLoader.Load<PackedScene>("res://src/main/scenes/gui/Popup.tscn");
	
	public override void _Ready()
	{
	}

	public void createPopup(string Message, float duration = 1.0f)
	{
		if (this.GetChildCount() > 8) return;
		Popup popup = (Popup) PopupScene.Instantiate();
		popup._Init(Message, duration: duration);
		this.AddChild(popup);
		this.MoveChild(popup, 0);
	}
}
