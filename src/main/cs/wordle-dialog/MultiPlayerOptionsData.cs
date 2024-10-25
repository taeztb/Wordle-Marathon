using Godot;
using System;

public partial class MultiPlayerOptionsData : Control
{

	public void _OnReplayPressed()
	{
		((MultiPlayerGame)GetNode("/root/App/WordleGame")).RestartGame();
		GetOwner<Dialog>()._OnCloseButtonPressed();
	}

	public void _OnMenuPressed()
	{
		Node menuDialog = ResourceLoader.Load<PackedScene>("res://src/main/scenes/LevelSelector.tscn").Instantiate();
        GetTree().Root.GetNode("App").AddChild(menuDialog);
        GetTree().Root.GetNode("App/WordleGame").QueueFree();
	}

	public void _OnQuitPressed()
	{
		GetTree().Root.PropagateNotification((int) NotificationWMCloseRequest);
		GetTree().Quit();
	}
}
