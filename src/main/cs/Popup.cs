using Godot;
using System;

public partial class Popup : LineEdit
{
	
	public string Message;
	public float Duration;

	private AnimationPlayer Animation;

	public void _Init(string message, float duration = 1.0f)
	{
		this.Message = message;
		this.Duration = duration;
		this.Animation = (AnimationPlayer) GetNode("Animation");
		this.Text = this.Message;
	}

	public override async void _EnterTree()
    {
		await ToSignal(GetTree().CreateTimer(Duration), "timeout");
		Animation.Play("FadePopup");
		await ToSignal(GetTree().CreateTimer(0.5), "timeout");
		this.QueueFree();
    }
}
