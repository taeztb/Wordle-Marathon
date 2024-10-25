using Godot;
using System;
using MultiPlayerWordleUI;

public partial class MultiPlayerGrid : GridContainer, IWordleUI
{
    private Vector2I GridDimensions;
    public MultiPlayerRow[] GridRows;
    public (string, Guess.Accuracy)[][] GridState;
    public bool Used;

    public void Init(int length, int height)
    {
        this.GridDimensions = new Vector2I(length, height);
        this.GridRows = new MultiPlayerRow[GridDimensions.Y];
        this.GridState = new (string, Guess.Accuracy)[GridDimensions.Y][];
        this.Used = false;
        for (int i = 0; i < GridRows.Length; i++)
        {
            GridRows[i] = (MultiPlayerRow)Constants.RowScene.Instantiate();
            GridRows[i].Init(GridDimensions.X, 1);
            GridState[i] = GridRows[i].GetRowState();
            this.AddChild(GridRows[i]);
        }
    }

    public bool IsUsed()
    {
        return Used;
    }

    public void SaveGame((string, Guess.Accuracy)[] rowState)
    {
        System.Text.StringBuilder save = new System.Text.StringBuilder();
        for (int i = 0; i < GridDimensions.Y; i++)
        {
            for (int j = 0; j < GridDimensions.X; j++)
            {
                save.Append(GridState[i][j].Item1);
            }
        }
        GetOwner<MultiPlayerGame>().SaveGame(save.ToString());
    }

    public void LoadGame(string save)
    {
        string guess = string.Empty;
        for (int i = 0; i < save.Length; i++)
        {
            guess += save[i];
            if (guess.Length == GridDimensions.X)
            {
                GridRows[i / GridDimensions.X].LoadGame(guess);
                _OnTextSubmitted(guess.ToLower());
                guess = string.Empty;
            }
        }
    }

    public void RestartGame()
    {
        foreach (MultiPlayerRow row in GridRows)
        {
            row.RestartGame();
        }
        this._OnFocusEntered();
    }

    public void DisplayResult(Guess.Result result)
    {
        for (int i = 0; i < GridRows.Length; i++)
        {
            if (!GridRows[i].IsUsed())
            {
                GridRows[i].DisplayResult(result);
                return;
            }
        }
        this._OnFocusEntered();
    }

    public void DisplayAccuracy(Guess.Accuracy[] accuracy, double duration = 0.0)
    {
        for (int i = 0; i < GridRows.Length; i++)
        {
            if (!GridRows[i].IsUsed())
            {
                GridRows[i].DisplayAccuracy(accuracy, 0.25);
                return;
            }
        }
    }

    public void _OnTextSubmitted(string text)
    {
        GetOwner<MultiPlayerGame>().MakeGuess(text);
        this._OnFocusEntered();
    }

    public void _OnTextChanged(string text)
    {
        throw new InvalidOperationException("error: attempted to call text changed on grid");
    }

    public void _OnFocusEntered()
    {
        foreach (MultiPlayerRow row in GridRows)
        {
            if (!row.IsUsed())
            {
                row._OnFocusEntered();
                return;
            }
        }
        // GridRows[GridRows.Length - 1]._OnFocusEntered();
    }
}
