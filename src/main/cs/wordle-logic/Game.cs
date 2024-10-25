using Godot;
using System;

public partial class Game : Node
{
    string Answer;
    bool Win;

    int WordLength;
    int GuessCount;

    Grid GameGrid;
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
        this.GameGrid = (Grid)GetNode("Content/Grid");
        this.PopupReel = (Reel)GetNode("Margin/Reel");
        this.GameAI = (WordleAI)GetNode("WordleAI");
        GameGrid.Init(this.WordLength, 6);
        GameGrid.GrabFocus();
        GameAI.Init(this.WordLength);
        LoadGame();
    }

    public string GetAnswer()
    {
        return Answer;
    }

    public void SaveGame(string save)
    {
        if (Win || (GuessCount >= 6))
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
        Win = false;
        GuessCount = 0;
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
            AddChild(ResourceLoader.Load<PackedScene>("res://src/main/cs/wordle-dialog/GameDialog.tscn").Instantiate());

        }
        // for (int i  = 0; i < GameGrid.GridState.Length; i++)
        // {
        //     for (int j = 0; j < GameGrid.GridState[i].Length; j++)
        //     {
        //         GD.Print(GameGrid.GridState[i][j]);
        //     }
        // }
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
