using Godot;
using System;
using MultiPlayerWordleUI;

public partial class MultiPlayerRow : HBoxContainer, IWordleUI
{
    private Vector2I RowDimensions;
    public MultiPlayerCell[] RowCells;
    private (string, Guess.Accuracy)[] RowState;
    public bool Used;

    public void Init(int length, int height)
    {
        this.RowDimensions = new Vector2I(length, height);
        this.RowCells = new MultiPlayerCell[RowDimensions.X];
        this.RowState = new (string, Guess.Accuracy)[RowDimensions.X];
        this.Used = false;
        for (int i = 0; i < RowCells.Length; i++)
        {
            RowCells[i] = (MultiPlayerCell)Constants.CellScene.Instantiate();
            RowCells[i].Init(1, 1);
            this.AddChild(RowCells[i]);
        }
    }

    public bool IsUsed()
    {
        return Used;
    }

    public (string, Guess.Accuracy)[] GetRowState()
    {
        return RowState;
    }

    public void SaveGame((string, Guess.Accuracy) cellState)
    {
        if (!this.IsUsed() && !string.IsNullOrEmpty(cellState.Item1))
        {
            for (int i = 0; i < RowDimensions.X; i++)
            {
                if (RowState[i].Item1 == null || RowState[i].Item1 == string.Empty)
                {
                    RowState[i] = cellState;
                    break;
                }
            }
        }
        else if (!this.IsUsed() && string.IsNullOrEmpty(cellState.Item1))
        {
            bool found = false;
            for (int i = 1; i < RowDimensions.X; i++)
            {
                if (RowState[i].Item1 == null || RowState[i].Item1 == string.Empty)
                {
                    RowState[i - 1].Item1 = string.Empty;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                RowState[RowDimensions.X - 1].Item1 = string.Empty;
            }
        }
        else if (this.IsUsed())
        {
            for (int i = 0; i < RowDimensions.X; i++)
            {
                if (RowState[i].Item2 == Guess.Accuracy.None)
                {
                    RowState[i] = cellState;
                    break;
                }
            }
        }
        MultiPlayerGrid parentGrid = (MultiPlayerGrid)GetParent();
        parentGrid.SaveGame(RowState);
    }

    public void LoadGame(string save)
    {
        for (int i = 0; i < save.Length; i++)
        {
            RowCells[i].LoadGame(save[i].ToString());
        }
    }

    public void RestartGame()
    {
        this.Used = false;
        foreach (MultiPlayerCell cell in RowCells)
        {
            cell.RestartGame();
        }
    }

    public void DisplayResult(Guess.Result result)
    {
        async void BounceRow()
        {
            await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
            foreach (MultiPlayerCell cell in RowCells)
            {
                await ToSignal(GetTree().CreateTimer(0.15f), "timeout");
                cell.DisplayResult(result);
            }
        }

        void ShakeRow()
        {
            Tween tween = GetTree().CreateTween();
            for (int i = 0; i < 3; i++)
            {
                tween.TweenProperty(this, "position", new Vector2(8.0f, this.Position.Y), 0.025);
                tween.TweenProperty(this, "position", new Vector2(0.0f, this.Position.Y), 0.025);
                tween.TweenProperty(this, "position", new Vector2(-8.0f, this.Position.Y), 0.025);
                tween.TweenProperty(this, "position", new Vector2(0.0f, this.Position.Y), 0.025);
            }
        }

        switch (result)
        {
            case Guess.Result.Match:
                this.Used = true;
                BounceRow();
                break;
            case Guess.Result.Valid:
                this.Used = true;
                break;
            case Guess.Result.Invalid:
                this.Used = false;
                ShakeRow();
                break;
        }
    }

    public void DisplayAccuracy(Guess.Accuracy[] accuracy, double duration = 0.0)
    {
        async void FlipRow()
        {
            double percentDecay = 1.0;
            for (int i = 0; i < RowDimensions.X; i++)
            {
                await ToSignal(GetTree().CreateTimer(duration * percentDecay), "timeout");
                RowCells[i].DisplayAccuracy(
                    new Guess.Accuracy[] { accuracy[i] },
                    0.15 * percentDecay
                );
                percentDecay -= i * (0.01 - (0.0005 * RowCells.Length));
            }
        }

        FlipRow();
    }

    public void _OnTextSubmitted(string text)
    {
        MultiPlayerGrid parentGrid = (MultiPlayerGrid)GetParent();
        foreach ((string, Guess.Accuracy) cellState in RowState)
        {
            text += cellState.Item1.ToLower();
        }
        parentGrid._OnTextSubmitted(text);
    }

    public void _OnTextChanged(string text)
    {
        this._OnFocusEntered();
    }

    public void _OnFocusEntered()
    {
        foreach (MultiPlayerCell cell in RowCells)
        {
            if (!cell.IsUsed())
            {
                cell._OnFocusEntered();
                return;
            }
        }
        RowCells[RowCells.Length - 1]._OnFocusEntered();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!this.IsUsed())
        {
            if (@event.IsActionReleased("ui_text_backspace"))
            {
                for (int i = 0; i < RowCells.Length; i++)
                {
                    if (!RowCells[i].HasFocus())
                        continue;

                    if (i == 0)
                    {
                        RowCells[i]._OnTextChanged(string.Empty);
                    }
                    else if (!RowCells[i].IsUsed())
                    {
                        RowCells[i - 1]._OnTextChanged(string.Empty);
                    }
                    else if (i == RowCells.Length - 1)
                    {
                        RowCells[RowCells.Length - 1]._OnTextChanged(string.Empty);
                    }
                }
            }
        }
    }
}
