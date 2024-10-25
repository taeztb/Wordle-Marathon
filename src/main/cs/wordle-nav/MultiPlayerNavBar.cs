using Godot;
using System;
using MultiPlayerWordleNav;

public partial class MultiPlayerNavBar : VBoxContainer, IWordleNav
{
    public void _OnMenuPressed()
    {
        Node menuDialog = Constants.LevelSelectorScene.Instantiate();
        GetTree().Root.GetNode("App").AddChild(menuDialog);
        GetTree().Root.GetNode("App/WordleGame").QueueFree();
    }

    public void _OnHelpPressed()
    {
        Node helpDialog = Constants.HelpDialogScene.Instantiate();
        GetOwner<Node>().AddChild(helpDialog);
    }

    public void _OnStatsPressed()
    {
        Node statsDialog = Constants.StatsDialogScene.Instantiate();
        GetOwner<Node>().AddChild(statsDialog);
    }

    public void _OnConfigPressed()
    {
        Node configDialog = Constants.ConfigDialogScene.Instantiate();
        GetOwner<Node>().AddChild(configDialog);
    }
}
