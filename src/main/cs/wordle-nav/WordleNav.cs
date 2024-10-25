using Godot;
using System;

namespace WordleNav
{
    public interface IWordleNav { }

    public static class Constants
    {
        private static readonly string scenePath = "res://src/main/cs/wordle-dialog/{0}.tscn";
        public static readonly PackedScene MenuDialogScene = ResourceLoader.Load<PackedScene>(
            string.Format(scenePath, "HelpDialog")
        );
        public static readonly PackedScene HelpDialogScene = ResourceLoader.Load<PackedScene>(
            string.Format(scenePath, "HelpDialog")
        );
        public static readonly PackedScene StatsDialogScene = ResourceLoader.Load<PackedScene>(
            string.Format(scenePath, "StatsDialog")
        );
        public static readonly PackedScene ConfigDialogScene = ResourceLoader.Load<PackedScene>(
            string.Format(scenePath, "ConfigDialog")
        );
        public static readonly PackedScene LevelSelectorScene = ResourceLoader.Load<PackedScene>(
            "res://src/main/scenes/LevelSelector.tscn"
        );
    }
}
