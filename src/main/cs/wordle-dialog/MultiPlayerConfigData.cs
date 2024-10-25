using Godot;
using System;

public partial class MultiPlayerConfigData : Control
{

	public void _OnRestartPressed()
	{
		((MultiPlayerGame)GetNode("/root/App/WordleGame")).RestartGame();
		GetOwner<Dialog>()._OnCloseButtonPressed();
	}

	public void _OnResetPressed()
	{
		((MultiPlayerGame)GetNode("/root/App/WordleGame")).ResetStats();
		GetOwner<Dialog>()._OnCloseButtonPressed();

	}

	public void _OnQuitPressed()
	{
		GetTree().Root.PropagateNotification((int) NotificationWMCloseRequest);
		GetTree().Quit();
	}

	public void _OnMutliPlayerPressed()
	{
		// Node menuDialog = Constants.LevelSelectorScene.Instantiate();
        // GetTree().Root.GetNode("App").AddChild(menuDialog);
        // GetTree().Root.GetNode("App/WordleGame").QueueFree();
	}
}
