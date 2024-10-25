using Godot;
using System;

public partial class MultiPlayerStatsData : Control
{
    public override void _Ready()
    {
        string[] stats = ((MultiPlayerGame)GetNode("/root/App/WordleGame")).GetStats();

        BoxContainer statsSection = (BoxContainer)GetParent().GetNode("Margin/Content/Body/StatsSection");
        ((Label)statsSection.GetNode("GamesStat/Stat")).Text = stats[0];
        ((Label)statsSection.GetNode("WinStat/Stat")).Text = Math.Round(
                (Double.Parse(stats[1]) / Double.Parse(stats[0])) * 100
            )
            .ToString();
        ((Label)statsSection.GetNode("StreakStat/Stat")).Text = stats[2];
        ((Label)statsSection.GetNode("MaxStat/Stat")).Text = stats[3];

		BoxContainer guessSection = (BoxContainer)GetParent().GetNode("Margin/Content/Body/GuessSection");
		float maxLength = 588;
		int maxValue = -1;
		for (int i = 1; i <= 6; i++)
		{
			if (Int32.Parse(stats[3 + i]) > maxValue)
			{
				maxValue = Int32.Parse(stats[3 + i]);
			}
		}
		for (int i = 1; i <= 6; i++)
		{
			float length = maxLength * (float.Parse(stats[3 + i]) / (float)maxValue);
			((PanelContainer)guessSection.GetNode($"Guess{i}/Bar")).CustomMinimumSize = new Vector2(length, 48);
			((Label)guessSection.GetNode($"Guess{i}/Bar/Label")).Text = stats[3 + i];

		}


    }
}
