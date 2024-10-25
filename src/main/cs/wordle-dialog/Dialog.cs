using Godot;
using System;
using WordleDialog;

public partial class Dialog : PanelContainer, IWordleDialog
{
    ColorRect BackgroundBlur = new ColorRect();

    public override void _Ready()
    {
        async void FadeInDialog()
        {
            Tween tween = GetTree().CreateTween();

            tween.TweenProperty(this, "modulate", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.25);
            tween
                .Parallel()
                .TweenProperty(BackgroundBlur, "color", new Color(0.0f, 0.0f, 0.0f, 0.5f), 0.25);
            await ToSignal(tween, "finished");
        }

        this.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        BackgroundBlur.Color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        BackgroundBlur.SetAnchorsPreset(LayoutPreset.FullRect);
        GetParent().AddChild(BackgroundBlur);
        FadeInDialog();
    }

    public void _OnCloseButtonPressed()
    {
        async void FadeOutDialog()
        {
            Tween tween = GetTree().CreateTween();

            tween.TweenProperty(
                this,
                "position",
                new Vector2(this.Position.X, this.Position.Y + 64),
                0.25
            );
            tween
                .Parallel()
                .TweenProperty(this, "modulate", new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.25);
            tween
                .Parallel()
                .TweenProperty(BackgroundBlur, "color", new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.25);
            await ToSignal(tween, "finished");
            this.QueueFree();
            BackgroundBlur.QueueFree();
        }

        FadeOutDialog();
    }
}
