using System.Collections.Generic;
using UnityEngine;

public class UnityVoiceChatController_Server : MonoBehaviour
{
    private List<AudioDataTunnel> clientTunnels = new List<AudioDataTunnel>();

    private void Awake()
    {
        AudioDataTunnel tunnel = this.GetComponent<AudioDataTunnel>();
        tunnel.AudioDataReceived += OnAudioDataReceived;
    }

    private void OnAudioDataReceived(AudioFrameData frameData)
    {
        foreach (AudioDataTunnel tunnel in this.clientTunnels)
        {
            tunnel.SendAudioDataToRemote(frameData);
        }
    }

    private void OnPlayerConnected(NetworkPlayer player)
    {
        AudioDataTunnel newTunnel = this.gameObject.AddComponent<AudioDataTunnel>();
        newTunnel.name = string.Format("Player_{0}_Tunnel", player.externalIP);
        newTunnel.RemoteTarget = player;
        clientTunnels.Add(newTunnel);
    }
}