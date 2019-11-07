using UnityEngine;
using System.Collections;

public class Random_Trial_Generator
{
    /// <summary>
    /// Generate saccade trials in relative rotation of the saccade
    /// </summary>
    /// <param name="items">    Items to be generated</param>
    /// <param name="weights">  Weights of each item</param>
    /// <param name="min">      minimum range of rotatoin</param>
    /// <param name="max">      maximum range of rotation</param>
    /// <returns>Generated stimulus rotation in array</returns>
    public static float[] generate(float[] items, int[] weights, float min, float max)
    {
        return Generate_Trial(Random_Weight_Items(items, weights), 0, min, max);
    }
    static float[] Random_Weight_Items(float[] items, int[] weights)
    {
        if (items.Length != weights.Length)
        {
            Debug.LogError("items and weights does not have same length");
        }
        int output_size = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            output_size += weights[i];
        }
        float[] output = new float[output_size];
        int idx = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i]; j++)
            {
                output[idx++] = items[i];
            }
        }
        Shuffle(output);
        return output;
    }
    static void Shuffle(float[] arr)
    {
        int n = arr.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            float value = arr[k];
            arr[k] = arr[n];
            arr[n] = value;
        }
    }
    static float[] Generate_Trial(float[] raw_direction, float start, float min, float max)
    {
        int s = 0;
        float[] res = new float[raw_direction.Length];
        try
        {
            while (s < raw_direction.Length)
            {
                if (IsBetween(start + raw_direction[s], min, max))
                {
                    res[s] = raw_direction[s];
                    start += raw_direction[s];
                    s++;
                }
                else
                {
                    int f = s+1;
                    while (!IsBetween(start + raw_direction[f], min, max))
                    {
                        f++;
                    }
                    float temp = raw_direction[f];
                    raw_direction[f] = raw_direction[s];
                    raw_direction[s] = temp;
                }
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            Debug.LogError("input is not balanced\n" + e.Message + "\n" +e.InnerException);
        }
        return res;
    }
    static bool IsBetween(float target, float min, float max)
    {
        return (target >= min && target <= max);
    }
}
