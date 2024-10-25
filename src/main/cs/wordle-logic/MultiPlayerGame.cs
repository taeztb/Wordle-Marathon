using Godot;
using System;

public partial class MultiPlayerGame : Game
{
    string Answer;
    bool Win;
    bool AIWin;

    string[] AIPreviousGuesses = new string[6];

    int WordLength;
    int GuessCount;

    MultiPlayerGrid GameGrid;
    MultiPlayerGrid AIGrid;
    WordleAI GameAI;
    Reel PopupReel;

    public void _Init(int wordLength)
    {
        this.WordLength = wordLength;
    }

    public override void _Ready()
    {
        this.Answer = SelectWord();
        this.Win = false;
        this.AIWin = false;
        this.GameGrid = (MultiPlayerGrid)GetNode("Content/Scroll/Container/Panel1/Grid");
        this.AIGrid = (MultiPlayerGrid)GetNode("Content/Scroll/Container/Panel2/AI");
        this.PopupReel = (Reel)GetNode("Margin/Reel");
        this.GameAI = (WordleAI)GetNode("WordleAI");
        GameGrid.Init(this.WordLength, 6);
        AIGrid.Init(this.WordLength, 6);
        GameGrid.GrabFocus();
        GameAI.Init(this.WordLength);
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < WordLength; j++)
            {
                AIGrid.GridRows[i].RowCells[j].Editable = false;
            }
        }
        LoadGame();
    }

    public string GetAnswer()
    {
        return Answer;
    }

    public void SaveGame(string save)
    {
        
        if (AIWin || Win || (GuessCount >= 6))
        {
            System.IO.File.WriteAllText($"src/data/saves/{WordLength}.txt", string.Empty);
            return;
        }
        if (save.Length / WordLength == GuessCount)
        {
            System.IO.File.WriteAllText($"src/data/saves/{WordLength}.txt", (Answer + save));
        }
    }

    public void LoadGame()
    {
        string save = System.IO.File.ReadAllText($"src/data/saves/{WordLength}.txt");
        if (string.IsNullOrEmpty(save))
        {
            return;
        }
        Answer = save.Substring(0, WordLength);
        GameGrid.LoadGame(save.Substring(WordLength));
    }

    public void RestartGame()
    {
        this.Answer = SelectWord();
        if (!Win && GuessCount < 6)
        {
            PopupReel.createPopup("Round Restarted!", duration: 3.0f);
        }
        else
        {
            PopupReel.createPopup("Play Again!", duration: 3.0f);
        }
        AIWin = false;
        Win = false;
        GuessCount = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < WordLength; j++)
            {
                MultiPlayerCell cell = AIGrid.GridRows[i].RowCells[j];
                cell.Text = string.Empty;
                cell.Editable = true;
                AIGrid.GridRows[i].Used = false;
                cell.Used = false;
                cell.AddThemeStyleboxOverride("read-only", MultiPlayerWordleUI.Constants.BlankCell);
                cell.AddThemeStyleboxOverride("normal", MultiPlayerWordleUI.Constants.BlankCell);
                cell.AddThemeStyleboxOverride("focus", MultiPlayerWordleUI.Constants.BlankCell);
            }
        }
        SaveGame(string.Empty);
        GameGrid.RestartGame();
    }

    public string SelectWord()
    {
        string selectedWord;
        Random randomize = new Random();
        int bytesPerWord = WordLength + 2;
        string filePath = $"res://src/main/resources/words/selectable/{WordLength}.txt";
        using (FileAccess selectableWords = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
        {
            int totalWords = ((int)selectableWords.GetLength()) / bytesPerWord;
            int randomWord = randomize.Next(0, totalWords) * bytesPerWord;
            selectableWords.Seek((ulong)randomWord);
            selectedWord = selectableWords.GetLine();
        }
        return selectedWord;
    }

    public void MakeGuess(string word)
    {
        async void DisplayGameOver(bool win)
        {
            if (win)
            {
                PopupReel.createPopup("Genius");
                await ToSignal(GetTree().CreateTimer(3.0f), "timeout");
            }
            else
            {
                PopupReel.createPopup(this.Answer, duration: 3.0f);
                await ToSignal(GetTree().CreateTimer(3.0f), "timeout");

            }
            AddChild(ResourceLoader.Load<PackedScene>("res://src/main/cs/wordle-dialog/MultiPlayerGameDialog.tscn").Instantiate());

        }
        Guess guess = new Guess(Answer, word);
        switch (guess.GetGuessResult())
        {
            case Guess.Result.Match:
                GameGrid.DisplayAccuracy(guess.GetGuessAccuracy());
                GameGrid.DisplayResult(guess.GetGuessResult());
                Win = true;
                GuessCount++;
                SetStats();
                DisplayGameOver(Win);
                break;
            case Guess.Result.Valid:
                GameGrid.DisplayAccuracy(guess.GetGuessAccuracy());
                GameGrid.DisplayResult(guess.GetGuessResult());
                GuessCount++;
                
                string aiGuessString;
                if (GuessCount == 1)
                {
                    aiGuessString = word;
                }
                else
                {
                    for (int i = 0; i < GuessCount - 1; i++)
                    {
                        for (int j = 0; j < WordLength; j++)
                        {
                            AIGrid.GridState[i][j] = (AIPreviousGuesses[i][j].ToString(), AIGrid.GridState[i][j].Item2);
                            GD.Print(AIGrid.GridState[i][j]);
                        }
                    }
                    aiGuessString = GameAI.GetBestGuess(AIGrid.GridState);
                }
                AIPreviousGuesses[GuessCount - 1] = aiGuessString;
                Guess aiGuess = new Guess(Answer, aiGuessString);
                switch (aiGuess.GetGuessResult())
                {
                    case Guess.Result.Match:
                        AIGrid.DisplayAccuracy(aiGuess.GetGuessAccuracy());
                        AIGrid.DisplayResult(aiGuess.GetGuessResult());
                        AIWin = true;
                        SetStats();
                        DisplayGameOver(Win);
                        SaveGame(string.Empty);
                        break;
                    case Guess.Result.Valid:
                        AIGrid.DisplayAccuracy(aiGuess.GetGuessAccuracy());
                        AIGrid.DisplayResult(aiGuess.GetGuessResult());
                        break;
                }

                if (GuessCount >= 6)
                {
                    Win = false;
                    SetStats();
                    DisplayGameOver(Win);
                }
                break;
            case Guess.Result.Invalid:
                GameGrid.DisplayResult(guess.GetGuessResult());
                PopupReel.createPopup("Not in word list");
                break;
        }
    }

    public void SetStats()
    {
        string[] stats = System.IO.File.ReadAllLines($"src/data/stats/{WordLength}.txt");
        stats[0] = (Int32.Parse(stats[0]) + 1).ToString(); // gamesPlayed
        if (Win)
        {
            stats[1] = (Int32.Parse(stats[1]) + 1).ToString(); // gamesWon
            stats[2] = (Int32.Parse(stats[2]) + 1).ToString(); // currentStreak
            stats[3 + GuessCount] = (Int32.Parse(stats[3 + GuessCount]) + 1).ToString(); // guessCount;

        }
        else
        {
            stats[2] = (0).ToString(); // currentStreak
        }
        stats[3] = Math.Max(Int32.Parse(stats[2]), Int32.Parse(stats[3])).ToString(); // maxStreak
        System.IO.File.WriteAllText($"src/data/stats/{WordLength}.txt", string.Join("\n", stats));
    }

    public string[] GetStats()
    {
        return System.IO.File.ReadAllLines($"src/data/stats/{WordLength}.txt");
    }

    public void ResetStats()
    {
        string[] stats = {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0",};
        System.IO.File.WriteAllText($"src/data/stats/{WordLength}.txt", string.Join("\n", stats));
        PopupReel.createPopup("Stats Reset!", duration: 3.0f);
    }
}
