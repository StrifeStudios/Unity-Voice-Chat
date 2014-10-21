using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VoiceChatEndpoint : MonoBehaviour
{
    private NetworkPlayer owner;

    public NetworkPlayer Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    void Awake()
    {
        ProxyInitialize();
    }

    void ProxyInitialize()
    {
        ByteDataTunnel tunnel = this.GetComponent<ByteDataTunnel>();
        NSpeexAudioProcessor nSpeexProcessor = this.GetComponent<NSpeexAudioProcessor>();
        AudioDataReconstructor audioReconstructor = this.GetComponent<AudioDataReconstructor>();
        tunnel.DataReceived += nSpeexProcessor.DecodeData;
        audioReconstructor.DataSource = nSpeexProcessor;
    }

    [RPC]
    public void NotifyOwnership()
    {
        this.owner = Network.player;

        ByteDataTunnel tunnel = this.GetComponent<ByteDataTunnel>();
        NSpeexAudioProcessor nSpeexProcessor = this.GetComponent<NSpeexAudioProcessor>();
        tunnel.RemoteTargets = new System.Collections.Generic.List<NetworkPlayer>() { Network.connections[0] };

        Debug.Log("I am the owner of " + this.gameObject.name + ", enabling the microphone collection for that audio tunnel.");
        UnityMicrophoneDataCollector microphone = this.gameObject.AddComponent<UnityMicrophoneDataCollector>();
        microphone.AudioDataReceived += nSpeexProcessor.EncodeData;
        nSpeexProcessor.AudioFrameEncoded += tunnel.SendDataToRemote;
    }

    public void ServerInitialize(NetworkPlayer owner)
    {
        this.owner = owner;
        AudioDataReconstructor audioReconstructor = this.GetComponent<AudioDataReconstructor>();
        audioReconstructor.DataSource = null;
        ByteDataTunnel tunnel = this.GetComponent<ByteDataTunnel>();
        List<NetworkPlayer> targets = new List<NetworkPlayer>(Network.connections.Length - 1);
        foreach (NetworkPlayer player in Network.connections)
        {
            if (player != this.owner)
            {
                targets.Add(player);
            }
        }

        tunnel.RemoteTargets = targets;
        tunnel.DataReceived += tunnel.SendDataToRemote;
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        if (player == this.owner)
        {
            networkView.RPC("SelfDestruct", RPCMode.AllBuffered);
        }
    }

    [RPC]
    private void SelfDestruct()
    {
        Object.Destroy(this.gameObject);
    }
}