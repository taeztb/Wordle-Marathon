using Godot;
using System;

namespace MultiPlayerWordleUI
{
    public interface IWordleUI
    {
        public void Init(int length, int height) { }

        public bool IsUsed()
        {
            return false;
        }

        public void SaveGame((string, Guess.Accuracy) state) { }

        public void LoadGame(string save) { }

        public void RestartGame() { }

        public void DisplayResult(Guess.Result result) { }

        public void DisplayAccuracy(Guess.Accuracy[] accuracy, double duration = 0.0) { }

        public void _OnTextSubmitted() { }

        public void _OnTextChanged() { }

        public void _OnFocusEntered() { }
    }

    public static class Constants
    {
        private static readonly string ScenePath = "res://src/main/cs/wordle-ui/{0}.tscn";
        public static readonly PackedScene GridScene = ResourceLoader.Load<PackedScene>(
            string.Format(ScenePath, "MultiPlayerGrid")
        );
        public static readonly PackedScene RowScene = ResourceLoader.Load<PackedScene>(
            string.Format(ScenePath, "MultiPlayerRow")
        );
        public static readonly PackedScene CellScene = ResourceLoader.Load<PackedScene>(
            string.Format(ScenePath, "MultiPlayerCell")
        );

        private static readonly string StylePath = "res://src/main/cs/wordle-ui/styles/{0}.tres";
        public static readonly StyleBox BlankCell = ResourceLoader.Load<StyleBox>(
            string.Format(StylePath, "BlankCell")
        );
        public static readonly StyleBox FullCell = ResourceLoader.Load<StyleBox>(
            string.Format(StylePath, "FullCell")
        );
        public static readonly StyleBox CorrectCell = ResourceLoader.Load<StyleBox>(
            string.Format(StylePath, "CorrectCell")
        );
        public static readonly StyleBox SemiCorrectCell = ResourceLoader.Load<StyleBox>(
            string.Format(StylePath, "SemiCorrectCell")
        );
        public static readonly StyleBox IncorrectCell = ResourceLoader.Load<StyleBox>(
            string.Format(StylePath, "IncorrectCell")
        );
    }
}
