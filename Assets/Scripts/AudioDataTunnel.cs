using System;
using UnityEngine;

/// <summary>
/// A component which handles relaying AudioClip frame data in a Unity-Networking-friendly way.
/// </summary>
public class AudioDataTunnel : MonoBehaviour, IAudioDataProvider
{
    private NetworkPlayer remoteTarget;
    public NetworkPlayer RemoteTarget
    {
        get { return remoteTarget; }
        set { remoteTarget = value; }
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
        if (remoteTarget.ipAddress == "unassigned")
        {
            throw new InvalidOperationException("No remote target set for audio tunnel.");
        }
        byte[] bytes = Util.ToByteArrayBlockCopy(audioFramedata.AudioData);
        networkView.RPC("ReadAudioData_Remote", remoteTarget, bytes, audioFramedata.NumChannels);
    }

    [RPC]
    public void ReadAudioData_Remote(byte[] bytes, int numChannels)
    {
        float[] data = Util.ToFloatArray(bytes);
        OnAudioDataReceived(data, numChannels);
    }
}