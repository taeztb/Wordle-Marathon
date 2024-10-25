using Godot;
using System;

public partial class Guess : Node
{
    private readonly string GuessAnswer;
    private readonly string GuessedWord;
    private readonly Result GuessResult;
    private readonly Accuracy[] GuessAccuracy;

    public enum Result
    {
        None,
        Match,
        Valid,
        Invalid
    }

    public enum Accuracy
    {
        None,
        Correct,
        SemiCorrect,
        Incorrect
    }

    public Guess(string answer, string guess, bool safety = true)
    {
        this.GuessAnswer = answer;
        this.GuessedWord = guess;
        if (safety)
        {
            this.GuessResult = CheckGuessResult();
        }
        this.GuessAccuracy = CheckGuessAccuracy();
    }

    private Result CheckGuessResult()
    {
        if (GuessedWord == GuessAnswer)
            return Result.Match;
        string filePath =
            $"res://src/main/resources/words/all/{GuessAnswer.Length}/{GuessedWord[0]}.txt";
        using (FileAccess words = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
        {
            while (words.GetPosition() < words.GetLength())
            {
                if (GuessedWord == words.GetLine())
                {
                    return Result.Valid;
                }
            }
        }
        return Result.Invalid;
    }

    private Accuracy[] CheckGuessAccuracy()
    {
        Accuracy[] answerAccuracy = new Accuracy[GuessAnswer.Length];
        Accuracy[] guessAccuracy = new Accuracy[GuessAnswer.Length];
        for (int i = 0; i < GuessAnswer.Length; i++)
        {
            if (GuessedWord[i] == GuessAnswer[i])
            {
                answerAccuracy[i] = Accuracy.Correct;
                guessAccuracy[i] = Accuracy.Correct;
            }
            else
            {
                answerAccuracy[i] = Accuracy.Incorrect;
                guessAccuracy[i] = Accuracy.Incorrect;
            }
        }
        for (int i = 0; i < GuessAnswer.Length; i++)
        {
            if (guessAccuracy[i] == Accuracy.Incorrect)
            {
                for (
                    int j = GuessAnswer.IndexOf(GuessedWord[i]);
                    j > -1;
                    j = GuessAnswer.IndexOf(GuessedWord[i], j + 1)
                )
                {
                    if (answerAccuracy[j] == Accuracy.Incorrect)
                    {
                        answerAccuracy[j] = Accuracy.SemiCorrect;
                        guessAccuracy[i] = Accuracy.SemiCorrect;
                        break;
                    }
                }
            }
        }
        return guessAccuracy;
    }

    public Result GetGuessResult()
    {
        return this.GuessResult;
    }

    public Accuracy[] GetGuessAccuracy()
    {
        if (this.GuessResult == Result.Invalid)
        {
            throw new InvalidOperationException(
                "error: attempted to get accuracy of invalid guess"
            );
        }
        return this.GuessAccuracy;
    }
}
