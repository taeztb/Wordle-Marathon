using Godot;
using System;

public partial class ConfigData : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void _OnRestartPressed()
	{
		((Game)GetNode("/root/App/WordleGame")).RestartGame();
		GetOwner<Dialog>()._OnCloseButtonPressed();
	}

	public void _OnResetPressed()
	{
		((Game)GetNode("/root/App/WordleGame")).ResetStats();
		GetOwner<Dialog>()._OnCloseButtonPressed();

	}

	public void _OnQuitPressed()
	{
		GetTree().Root.PropagateNotification((int) NotificationWMCloseRequest);
		GetTree().Quit();
	}
}
