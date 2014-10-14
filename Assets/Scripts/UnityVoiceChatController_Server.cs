using System.Collections.Generic;
using UnityEngine;

public class UnityVoiceChatController_Server : MonoBehaviour
{
    [SerializeField]
    private GameObject audioEndpointPrefab;
    private List<AudioDataTunnel> clientTunnels = new List<AudioDataTunnel>();

    private void Awake()
    {
        this.audioEndpointPrefab = Resources.Load<GameObject>("AudioEndpointPrefab");
    }

    private void OnPlayerConnected(NetworkPlayer player)
    {
        foreach (var tunnel in this.clientTunnels)
        {
            tunnel.RemoteTargets.Add(player);
        }

        GameObject newAudioEndpoint = (GameObject)Network.Instantiate(audioEndpointPrefab, Vector3.zero, Quaternion.identity, 0);
        newAudioEndpoint.networkView.RPC("NotifyOwnership", player);
        newAudioEndpoint.GetComponent<VoiceChatEndpoint>().ServerInitialize(player);
        clientTunnels.Add(newAudioEndpoint.GetComponent<AudioDataTunnel>());
    }
}