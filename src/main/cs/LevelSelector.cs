using Godot;
using System;

public partial class LevelSelector : VBoxContainer
{
	private readonly PackedScene GameScene = 
		ResourceLoader.Load<PackedScene>("res://src/main/scenes/MultiPlayerGame.tscn");
	
	private readonly PackedScene ButtonScene = 
		ResourceLoader.Load<PackedScene>("res://src/main/scenes/button.tscn");

	public void _OnButtonPressed(int levelNum)
	{
		MultiPlayerGame game = (MultiPlayerGame) GameScene.Instantiate();
		game._Init(levelNum);
		GetNode("..").AddChild(game);
		// GetNode("..").AddChild(ButtonScene.Instantiate());
		this.QueueFree();
	}
}
