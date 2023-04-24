using System;
using System.Collections.Generic;
using static PKHeX.Core.Ball;

namespace PKHeX.Core;

/// <summary>
/// Contains logic to apply a new <see cref="Ball"/> value to a <see cref="PKM"/>.
/// </summary>
public static class BallApplicator
{
    /// <summary>
    /// Gets all balls that are legal for the input <see cref="PKM"/>.
    /// </summary>
    /// <remarks>
    /// Requires checking the <see cref="LegalityAnalysis"/> for every <see cref="Ball"/> that is tried.
    /// </remarks>
    /// <param name="pk">Pokémon to retrieve a list of valid balls for.</param>
    /// <returns>Enumerable list of <see cref="Ball"/> values that the <see cref="PKM"/> is legal with.</returns>
    public static IEnumerable<Ball> GetLegalBalls(PKM pk)
    {
        var clone = pk.Clone();
        foreach (var b in BallList)
        {
            var ball = (int)b;
            clone.Ball = ball;
            if (clone.Ball != ball)
                continue; // Some setters guard against out of bounds values.
            if (new LegalityAnalysis(clone).Valid)
                yield return b;
        }
    }

    /// <summary>
    /// Applies a random legal ball value if any exist.
    /// </summary>
    /// <remarks>
    /// Requires checking the <see cref="LegalityAnalysis"/> for every <see cref="Ball"/> that is tried.
    /// </remarks>
    /// <param name="pk">Pokémon to modify.</param>
    public static int ApplyBallLegalRandom(PKM pk)
    {
        Span<Ball> balls = stackalloc Ball[MaxBallSpanAlloc];
        var count = GetBallListFromColor(pk, balls);
        balls = balls[..count];
        Util.Rand.Shuffle(balls);
        return ApplyFirstLegalBall(pk, balls);
    }

    /// <summary>
    /// Applies a legal ball value if any exist, ordered by color.
    /// </summary>
    /// <remarks>
    /// Requires checking the <see cref="LegalityAnalysis"/> for every <see cref="Ball"/> that is tried.
    /// </remarks>
    /// <param name="pk">Pokémon to modify.</param>
    public static int ApplyBallLegalByColor(PKM pk)
    {
        Span<Ball> balls = stackalloc Ball[MaxBallSpanAlloc];
        GetBallListFromColor(pk, balls);
        return ApplyFirstLegalBall(pk, balls);
    }

    /// <summary>
    /// Applies a random ball value in a cyclical manner.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    public static int ApplyBallNext(PKM pk)
    {
        Span<Ball> balls = stackalloc Ball[MaxBallSpanAlloc];
        GetBallList(pk.Ball, balls);
        var next = balls[0];
        return pk.Ball = (int)next;
    }

    private static int ApplyFirstLegalBall(PKM pk, Span<Ball> balls)
    {
        foreach (var b in balls)
        {
            pk.Ball = (int)b;
            if (new LegalityAnalysis(pk).Valid)
                break;
        }
        return pk.Ball;
    }

    private static int GetBallList(int ball, Span<Ball> result)
    {
        var balls = BallList;
        var currentBall = (Ball)ball;
        return GetCircularOnce(balls, currentBall, result);
    }

    private static int GetBallListFromColor(PKM pk, Span<Ball> result)
    {
        // Gen1/2 don't store color in personal info
        var pi = pk.Format >= 3 ? pk.PersonalInfo : PersonalTable.USUM.GetFormEntry(pk.Species, 0);
        var color = (PersonalColor)pi.Color;
        var balls = BallColors[color];
        var currentBall = (Ball)pk.Ball;
        return GetCircularOnce(balls, currentBall, result);
    }

    private static int GetCircularOnce<T>(T[] items, T current, Span<T> result)
    {
        var currentIndex = Array.IndexOf(items, current);
        if (currentIndex < 0)
            currentIndex = items.Length - 2;

        int ctr = 0;
        for (int i = currentIndex + 1; i < items.Length; i++)
            result[ctr++] = items[i];
        for (int i = 0; i <= currentIndex; i++)
            result[ctr++] = items[i];
        return ctr;
    }

    private static readonly Ball[] BallList = (Ball[])Enum.GetValues(typeof(Ball));
    private static int MaxBallSpanAlloc => BallList.Length;

    static BallApplicator()
    {
        Span<Ball> exclude = stackalloc Ball[] {None, Poke};
        Span<Ball> end = stackalloc Ball[] {Poke};
        Span<Ball> all = stackalloc Ball[BallList.Length - exclude.Length];
        all = all[..FillExcept(all, exclude, BallList)];

        var colors = (PersonalColor[])Enum.GetValues(typeof(PersonalColor));
        foreach (var c in colors)
        {
            // Replace the array reference with a new array that appends non-matching values, followed by the end values.
            var defined = BallColors[c];
            Span<Ball> match = (BallColors[c] = new Ball[all.Length + end.Length]);
            defined.CopyTo(match);
            FillExcept(match[defined.Length..], defined, all);
            end.CopyTo(match[^end.Length..]);
        }

        static int FillExcept(Span<Ball> result, ReadOnlySpan<Ball> exclude, ReadOnlySpan<Ball> all)
        {
            int ctr = 0;
            foreach (var b in all)
            {
                if (Contains(exclude, b))
                    continue;
                result[ctr++] = b;
            }
            return ctr;

            static bool Contains(ReadOnlySpan<Ball> arr, Ball b)
            {
                foreach (var a in arr)
                {
                    if (a == b)
                        return true;
                }
                return false;
            }
        }
    }

    /// <summary>
    /// Priority Match ball IDs that match the color ID in descending order
    /// </summary>
    private static readonly Dictionary<PersonalColor, Ball[]> BallColors = new()
    {
        [PersonalColor.Red] =    new[] { Cherish, Repeat, Fast, Heal, Great, Dream, Lure },
        [PersonalColor.Blue] =   new[] { Dive, Net, Great, Beast, Lure },
        [PersonalColor.Yellow] = new[] { Level, Ultra, Repeat, Quick, Moon },
        [PersonalColor.Green] =  new[] { Safari, Friend, Nest, Dusk },
        [PersonalColor.Black] =  new[] { Luxury, Heavy, Ultra, Moon, Net, Beast },

        [PersonalColor.Brown] =  new[] { Level, Heavy },
        [PersonalColor.Purple] = new[] { Master, Love, Dream, Heal },
        [PersonalColor.Gray] =   new[] { Heavy, Premier, Luxury },
        [PersonalColor.White] =  new[] { Premier, Timer, Luxury, Ultra },
        [PersonalColor.Pink] =   new[] { Love, Dream, Heal },
    };

    /// <summary>
    /// Personal Data color IDs
    /// </summary>
    private enum PersonalColor : byte
    {
        Red,
        Blue,
        Yellow,
        Green,
        Black,

        Brown,
        Purple,
        Gray,
        White,
        Pink,
    }
}
