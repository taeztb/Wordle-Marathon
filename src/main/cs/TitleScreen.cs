using Godot;
using System;

public partial class TitleScreen : VBoxContainer
{
	private readonly PackedScene LevelSelectorScene = 
		ResourceLoader.Load<PackedScene>("res://src/main/scenes/LevelSelector.tscn");
	public void _OnButtonPressed()
	{
		GetNode("..").AddChild(LevelSelectorScene.Instantiate());
		this.QueueFree();
	}
}
