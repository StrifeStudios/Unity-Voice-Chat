using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Util : Singleton<Util>
{
    public static float GetScale()
    {
        return Screen.height / 768f;
    }

    public static byte[] ToByteArray(float[] floatArray)
    {
        int len = floatArray.Length * 4;
        byte[] byteArray = new byte[len];
        int pos = 0;
        foreach (float f in floatArray)
        {
            byte[] data = System.BitConverter.GetBytes(f);
            System.Array.Copy(data, 0, byteArray, pos, 4);
            pos += 4;
        }
        return byteArray;
    }

    public static float[] ToFloatArray(byte[] byteArray)
    {
        int len = byteArray.Length / 4;
        float[] floatArray = new float[len];
        for (int i = 0; i < byteArray.Length; i += 4)
        {
            floatArray[i / 4] = System.BitConverter.ToSingle(byteArray, i);
        }
        return floatArray;
    }

    public static float[] ToFloatArray(short[] shortArray)
    {
        byte[] byteArray = new byte[shortArray.Length * 2];
        Buffer.BlockCopy(shortArray, 0, byteArray, 0, shortArray.Length);
        return ToFloatArray(byteArray);
    }

    public static byte[] ToByteArrayBlockCopy(float[] floatArray)
    {
        int byteCount = floatArray.Length * 4;
        byte[] byteArray = new byte[byteCount];
        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteCount);
        return byteArray;
    }

    public static float[] ToFloatArrayBlockCopy(byte[] byteArray)
    {
        int floatCount = byteArray.Length / 4;
        float[] floatArray = new float[floatCount];
        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
        return floatArray;
    }

    public static void ConvertToShortArray(float[] input, short[] output)
    {
        if (output.Length < input.Length)
        {
            throw new System.ArgumentException("in: " + input.Length + ", out: " + output.Length);
        }
        for (int i = 0; i < input.Length; ++i)
        {
            output[i] = (short)Mathf.Clamp((int)(input[i] * 32767.0f), short.MinValue, short.MaxValue);
        }
    }

    public static void ConvertToFloatArray(short[] input, float[] output)
    {
        if (output.Length < input.Length)
        {
            throw new System.ArgumentException("in: " + input.Length + ", out: " + output.Length);
        }
        for (int i = 0; i < input.Length; ++i)
        {
            output[i] = (float)Mathf.Clamp(((float)input[i] / 32767.0f), -1.0f, 1.0f);
        }
    }

    public static void PrintToFile(float[] data, string fileName)
    {
        using (var writer = File.CreateText(fileName + "_floats.txt"))
        {
            for (int g = 0; g < data.Length; g++)
            {
                writer.WriteLine(data[g]);
            }
        }
    }

    public static void PrintToFile(byte[] data, string fileName)
    {
        using (var writer = File.CreateText(fileName + "_bytes.txt"))
        {
            for (int g = 0; g < data.Length; g++)
            {
                writer.WriteLine(data[g]);
            }
        }
    }

    public static string CurrentTimeStamp
    {
        get
        {
            DateTime now = DateTime.Now;
            return string.Format("{0}-{1}-{2}-{3}", now.Hour, now.Minute, now.Second, now.Millisecond);
        }
    }
}