using System;
// Source: https://stackoverflow.com/questions/108819/best-way-to-randomize-an-array-with-net
// Schuffle in O(n)

static class ArrayExtensions
{
    public static T[] Shuffle<T>(this T[] array)
    {
        return array.Shuffle(new Random());
    }

    public static T[] Shuffle<T>(this T[] array, Random rng)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
        return array;
    }
}