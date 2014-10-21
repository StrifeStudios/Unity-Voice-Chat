using NSpeex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NSpeexAudioProcessor : MonoBehaviour, IAudioDataProvider
{
    private bool printedToFile = false;



    private SpeexEncoder encoder;
    private SpeexDecoder decoder;
    [SerializeField]
    private BandMode bandMode = BandMode.Wide;
    private int numChannels = 1;

    void Awake()
    {
        this.encoder = new SpeexEncoder(bandMode);
        this.decoder = new SpeexDecoder(bandMode);
    }

    // IAudioDataProvider implementation
    public event AudioDataReceivedEventHandler AudioDataReceived;
    public void OnAudioDataReceived(float[] data, int numChannels)
    {
        if (AudioDataReceived != null)
        {
            AudioFrameData frameData = new AudioFrameData(data, numChannels);
            AudioDataReceived(frameData);
        }
    }

    public event Action<Byte[], int> AudioFrameEncoded;
    public void EncodeData(AudioFrameData frameData)
    {
        short[] shortData = new short[frameData.AudioData.Length];
        Util.ConvertToShortArray(frameData.AudioData, shortData);
        Byte[] encodedBytes = new Byte[shortData.Length * 2];
        int numEncodedBytes = encoder.Encode(shortData, 0, encoder.FrameSize, encodedBytes, 0, encodedBytes.Length);
        int difference = encodedBytes.Length - numEncodedBytes;
        Debug.Log("Wasted bytes: " + difference);
        byte[] trimmedBytes = new byte[numEncodedBytes];
        Buffer.BlockCopy(encodedBytes, 0, trimmedBytes, 0, numEncodedBytes);
        OnAudioFrameEncoded(trimmedBytes, numEncodedBytes);
    }

    private void OnAudioFrameEncoded(byte[] encodedBytes, int numEncodedBytes)
    {
        if (this.AudioFrameEncoded != null)
        {
            AudioFrameEncoded(encodedBytes, numEncodedBytes);
        }
    }

    public void DecodeData(byte[] inputData, int numBytes)
    {
        short[] shortData = new short[encoder.FrameSize];
        decoder.Decode(inputData, 0, numBytes, shortData, 0, false);
        float[] floatData = new float[shortData.Length];
        Util.ConvertToFloatArray(shortData, floatData);
        if (!printedToFile)
        {
            Util.PrintToFile(floatData, "decoded" + Util.CurrentTimeStamp);
            printedToFile = true;
        }
        OnAudioDataReceived(floatData, this.numChannels);
    }
}