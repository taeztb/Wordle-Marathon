using Godot;
using System;

public partial class App : MarginContainer
{
    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("ui_cancel")) 
		{
			// GetTree().Root.PropagateNotification((int) NotificationWMCloseRequest);
			// GetTree().Quit();
			WordleAI GameAI = (WordleAI)GetNode("/root/App/WordleGame/WordleAI");
			Grid GameGrid = (Grid)GetNode("/root/App/WordleGame/Content/Grid");
			GD.Print("Next Guess:" + GameAI.GetBestGuess(GameGrid.GridState));
		}
    }
}
