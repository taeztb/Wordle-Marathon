using Godot;
using System;
using System.Linq;

public partial class WordleAI : Control
{
    string[] Vocabulary;
    string[] AnswerOptions;

    int WordLength;

    public void Init(int wordLength)
    {
        this.WordLength = wordLength;
        Vocabulary = System.IO.File.ReadAllLines(
            $"src/main/resources/words/selectable/{WordLength}.txt"
        );
    }

    public string GetBestGuess((string, Guess.Accuracy)[][] guesses)
    {
		string[] previousGuesses = new string[guesses.Length];
		void setAnswerOptions()
		{
			AnswerOptions = Vocabulary;
			for (int i = 0; i < 6; i++)
			{
				System.Text.StringBuilder guess = new System.Text.StringBuilder();
				Guess.Accuracy[] accuracy = new Guess.Accuracy[WordLength];
				for (int j = 0; j < WordLength; j++)
				{
					if(string.IsNullOrEmpty(guesses[i][j].Item1))
					{
						return;
					}
					guess.Append(guesses[i][j].Item1.ToLower());
					accuracy[j] = guesses[i][j].Item2;
				}
				AnswerOptions = NarrowAnswerOptions(guess.ToString(), accuracy);
				previousGuesses[i] = guess.ToString();
			}
		}
		setAnswerOptions();
		if (AnswerOptions.Length == 1)
		{
			return AnswerOptions[0];
		}
		string bestGuess = AnswerOptions[0];
        Guess.Accuracy[] guessValue;
        int minValue = int.MaxValue;
        foreach (string word in Vocabulary)
        {
            if (previousGuesses.Contains(word)) 
			{
				continue;
			}
			int totalValue = 0;
            foreach (string answer in AnswerOptions)
            {
				guessValue = (new Guess(answer, word, safety: false)).GetGuessAccuracy();
				int correctCount = 0;
				foreach (Guess.Accuracy grade in guessValue)
				{
					if (grade == Guess.Accuracy.Correct)
					{
						correctCount++;
					}
				}
				if (correctCount == WordLength)
				{
					totalValue += 0;
				}
				else
				{
					totalValue += SimulateGuess(word, guessValue);
				}
                if (totalValue > minValue)
                    break;
            }
            if (totalValue < minValue)
            {
                minValue = totalValue;
                bestGuess = word;
            }
        }
        GD.Print(bestGuess);
        return bestGuess;
    }

    public int SimulateGuess(string guess, Guess.Accuracy[] accuracy)
    {
        bool IsPossibleOption(string word)
        {
            for (int i = 0; i < WordLength; i++)
            {
                switch (accuracy[i])
                {
                    case Guess.Accuracy.Correct:
                        if (!guess[i].Equals(word[i]))
                            return false;
                        break;
                    case Guess.Accuracy.SemiCorrect:
                        if (guess[i].Equals(word[i]) || !word.Contains(guess[i]))
                            return false;
                        break;
                    case Guess.Accuracy.Incorrect:
                        if (word.Contains(guess[i]))
						{
							bool found = false;
							for (int j = 0; j < WordLength; j++)
							{
								if (j == i) continue;
								if (guess[j].Equals(guess[i]))
								{
									if (accuracy[j] == Guess.Accuracy.Correct || accuracy[j] == Guess.Accuracy.SemiCorrect)
									{
										found = true;
										break;
									}
								} 
							}
							if (!found) return false;
						}
                        break;
                }
            }
            return true;
        }

        int count = 0;
        foreach (string word in AnswerOptions)
        {
            if (IsPossibleOption(word))
            {
                count++;
            }
        }
        return count;
    }

	public string[] NarrowAnswerOptions(string guess, Guess.Accuracy[] accuracy)
    {
        bool IsPossibleOption(string word)
        {
            for (int i = 0; i < WordLength; i++)
            {
                switch (accuracy[i])
                {
                    case Guess.Accuracy.Correct:
                        if (!guess[i].Equals(word[i]))
                        {
							return false;
						}
                        break;
                    case Guess.Accuracy.SemiCorrect:
                        if (guess[i].Equals(word[i]) || !word.Contains(guess[i]))
                        {
							return false;
						}
                        break;
                    case Guess.Accuracy.Incorrect:
                        if (word.Contains(guess[i]))
                        {
							bool found = false;
							for (int j = 0; j < WordLength; j++)
							{
								if (j == i) continue;
								if (guess[j].Equals(guess[i]))
								{
									if (accuracy[j] == Guess.Accuracy.Correct || accuracy[j] == Guess.Accuracy.SemiCorrect)
									{
										found = true;
										break;
									}
								} 
							}
							if (!found) return false;
							
						}
                        break;
                }
            }
            return true;
        }

		System.Collections.Generic.HashSet<string> answerOptions =
            new System.Collections.Generic.HashSet<string>();        
		foreach (string word in AnswerOptions)
        {
            if (IsPossibleOption(word))
            {
                answerOptions.Add(word);
            }
        }
        String[] stringArray = new String[answerOptions.Count];
        answerOptions.CopyTo(stringArray);
        return stringArray;
    }

    public string[] GetAnswerOptions((string, Guess.Accuracy)[][] guesses)
    {
        System.Collections.Generic.HashSet<(string, int)> includedLetters =
            new System.Collections.Generic.HashSet<(string, int)>();
        System.Collections.Generic.HashSet<(string, int)> presentLetters =
            new System.Collections.Generic.HashSet<(string, int)>();
        System.Collections.Generic.HashSet<string> excludedLetters =
            new System.Collections.Generic.HashSet<string>();

		foreach ((string, Guess.Accuracy)[] guess in guesses)
        {
            for (int i = 0; i < WordLength; i++)
            {
                if (string.IsNullOrEmpty(guess[i].Item1))
				{
					break;
				}
				switch (guess[i].Item2)
                {
                    case Guess.Accuracy.Correct:
                        includedLetters.Add((guess[i].Item1.ToLower(), i));
                        break;
                    case Guess.Accuracy.SemiCorrect:
						presentLetters.Add((guess[i].Item1.ToLower(), i));
                        break;
                    case Guess.Accuracy.Incorrect:
                        excludedLetters.Add(guess[i].Item1.ToLower());
                        break;
					case Guess.Accuracy.None:
						break;
                }
            }
        }

        bool CheckIncludedLetters(string word)
        {
            foreach ((string, int) letter in includedLetters)
            {
                if (word.IndexOf(letter.Item1) != letter.Item2)
                {
                    int index = word.IndexOf(letter.Item1);
                    while (index != -1 && index < word.Length - 1)
                    {
                        index = word.IndexOf(letter.Item1, index + 1);
                        if (index == letter.Item2)
                        {
                            index = -2;
							break;
                        }
                    }
                    if (index != -2) return false;
                }
            }
            return true;
        }

        bool CheckPresentLetters(string word)
        {
            foreach ((string, int) letter in presentLetters)
            {
                if (!word.Contains(letter.Item1))
                {
					return false;
                }
                if (word.IndexOf(letter.Item1) == letter.Item2)
                {
					return false;
                }
                int index = word.IndexOf(letter.Item1);
                while (index != -1)
                {
                    index = word.IndexOf(letter.Item1, index + 1);
                    if (index == letter.Item2)
                    {
						return false;
                    }
                }
            }
            return true;
        }

        bool CheckExcludedLetters(string word)
        {
            foreach (string letter in excludedLetters)
            {
                if (word.Contains(letter))
                {
                    return false;
                }
            }
            return true;
        }

        System.Collections.Generic.HashSet<string> answerOptions =
            new System.Collections.Generic.HashSet<string>();

        foreach (string word in Vocabulary)
        {
            if (CheckExcludedLetters(word))
				if (CheckIncludedLetters(word))
				{
					if (CheckPresentLetters(word))
					{
						answerOptions.Add(word);

					}
				}
        }
        String[] stringArray = new String[answerOptions.Count];
        answerOptions.CopyTo(stringArray);
        return stringArray;
    }
}
