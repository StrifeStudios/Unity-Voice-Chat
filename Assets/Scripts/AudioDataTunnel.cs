using System;
using UnityEngine;
using NSpeex;
using System.Collections.Generic;

/// <summary>
/// A component which handles relaying AudioClip frame data in a Unity-Networking-friendly way.
/// </summary>
public class AudioDataTunnel : MonoBehaviour, IAudioDataProvider
{
    private SpeexEncoder encoder = new SpeexEncoder(BandMode.Wide);
    private SpeexDecoder decoder = new SpeexDecoder(BandMode.Wide);

    [SerializeField]
    private List<NetworkPlayer> remoteTargets;
    public List<NetworkPlayer> RemoteTargets
    {
        get { return remoteTargets; }
        set { remoteTargets = value; }
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

    public void SendAudioDataToRemote(AudioFrameData audioFramedata)
    {
        foreach (NetworkPlayer remoteTarget in this.remoteTargets)
        {
            if (remoteTarget.ipAddress == "unassigned")
            {
                throw new InvalidOperationException("No remote target set for audio tunnel.");
            }
            int length;
            byte[] bytes = encodedBytes(audioFramedata, out length);

            networkView.RPC("ReadAudioData_Remote", remoteTarget, bytes, audioFramedata.NumChannels, length);
        }
    }

    private byte[] encodedBytes(AudioFrameData audioFramedata, out int length)
    {
        byte[] recordedBytes = Util.ToByteArrayBlockCopy(audioFramedata.AudioData);

        int numOfShortsToSend = (recordedBytes.Length / 2) - (recordedBytes.Length / 2) % encoder.FrameSize;

        short[] data = new short[numOfShortsToSend];
        Buffer.BlockCopy(recordedBytes, 0, data, 0, numOfShortsToSend * 2);
        byte[] encodedData = new byte[numOfShortsToSend * 2];

        // note: the number of samples per frame must be a multiple of encoder.FrameSize
        Debug.Log("recordedBytes: " + recordedBytes.Length +
                  " data: " + data.Length +
                  " encodedData: " + encodedData.Length +
                  " numOfBytesToSend: " + numOfShortsToSend * 2 +
                  " framesize: " + encoder.FrameSize);
        length = encoder.Encode(data, 0, data.Length, encodedData, 0, encodedData.Length);
        return encodedData;
    }
    
    [RPC]
    public void ReadAudioData_Remote(byte[] bytes, int numChannels, int length)
    {
        short[] decodedFrame = new short[bytes.Length / 2]; // should be the same number of samples as on the capturing side
        Debug.Log("bytes: " + bytes.Length + " Decoded frame: " + decodedFrame.Length + " Decoder: " + decoder.FrameSize);
        decoder.Decode(bytes, 0, length, decodedFrame, 0, false);
        float[] data = Util.ToFloatArray(decodedFrame);
        OnAudioDataReceived(data, numChannels);
    }
}