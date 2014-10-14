using UnityEngine;

/// <summary>
/// Controller responsible for relaying a single client's recorded microphone data to the server.
/// </summary>
public class UnityVoiceChatController_Client : MonoBehaviour
{
    [SerializeField]
    private int recordingFrequency = 10000;
    private AudioClip recordingClip;
    private UnityMicrophoneDataCollector microphoneDataCollector;
    private AudioDataTunnel tunnel;

    private KeyCode hotkey = KeyCode.BackQuote;

    private void Awake()
    {
        this.tunnel = this.GetComponent<AudioDataTunnel>();
        this.microphoneDataCollector = this.GetComponent<UnityMicrophoneDataCollector>();
        microphoneDataCollector.AudioDataReceived += tunnel.SendAudioDataToRemote;
        AudioDataReconstructor audioReconstructor = this.GetComponent<AudioDataReconstructor>();
        audioReconstructor.DataSource = this.tunnel;
    }

    private void OnConnectedToServer()
    {
        Debug.Log("Connected.");
        this.tunnel.RemoteTargets = new System.Collections.Generic.List<NetworkPlayer>() { Network.connections[0] };
    }
}