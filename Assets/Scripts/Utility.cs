using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public static class Utility
{

    public static T Log<T>(T param, string message = "")
    {
        try
        {
            Debug.Log(message + param.ToString());
        }
        catch
        {
            Debug.Log(message + "(null)");
        }
        return param;
    }

    public static IEnumerable<T> Generate<T>(T seed, Func<T, T> mutate)
    {
        var accum = seed;
        while (true)
        {
            yield return accum;
            accum = mutate(accum);
        }
    }

    public static IEnumerable<Transform> RecursiveWalker(Transform parent)
    {
        foreach (Transform child in parent)
        {
            foreach (Transform grandchild in RecursiveWalker(child))
                yield return grandchild;
            yield return child;
        }
    }

    public static IEnumerable<Tuple<int, int, T>> LazyMatrix<T>(T[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
            for (int j = 0; j < matrix.GetLength(1); j++)
                yield return Tuple.Create(i, j, matrix[i, j]);
    }

    public static Vector3 Truncate(Vector3 vec, float maxMagnitude)
    {
        var magnitude = vec.magnitude;
        //1. return (vec.normalized) * Mathf.Clamp(magnitude, 0f, maxMagnitude);
        //2. return (vec/magnitude)  * Mathf.Min(magnitude, maxMagnitude);	//Dado que una distancia nunca es menor a 0
        //3. return (vec/magnitude)  * Mathf.Min(magnitude, maxMagnitude);
        //4. vvvv
        return vec * Mathf.Min(1f, maxMagnitude / magnitude);
    }

    public static void KnuthShuffle<T>(List<T> array)
    {
        for (int i = 0; i < array.Count - 1; i++)
        {
            var j = Random.Range(i, array.Count);
            if (i != j)
            {
                var temp = array[j];
                array[j] = array[i];
                array[i] = temp;
            }
        }
    }

    public static Vector3 RandomDirection()
    {
        var theta = Random.Range(0f, 2f * Mathf.PI);
        var phi = Random.Range(0f, Mathf.PI);
        var u = Mathf.Cos(phi);
        return new Vector3(Mathf.Sqrt(1 - u * u) * Mathf.Cos(theta), Mathf.Sqrt(1 - u * u) * Mathf.Sin(theta), u);
    }

    public static Vector3 RemoveY (this Vector3 current)
    {
        return new Vector3(current.x, 0, current.z);
    }

}
