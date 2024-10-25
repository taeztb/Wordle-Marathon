using Godot;
using System;

public partial class Button : Godot.Button
{
	private readonly PackedScene LevelSelectorScene = 
		ResourceLoader.Load<PackedScene>("res://src/main/scenes/LevelSelector.tscn");
	public void _OnButtonPressed()
	{
		GetNode("..").AddChild(LevelSelectorScene.Instantiate());
		GetNode("..").GetChild(0).QueueFree();
		this.QueueFree();
	}
}
