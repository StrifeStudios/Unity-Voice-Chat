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

    void Update()
    {
        if (Input.GetKeyDown(hotkey))
        {
            StartRecording();
        }
        else if (Input.GetKeyUp(hotkey))
        {
            StopRecording();
        }
    }

    private void OnConnectedToServer()
    {
        Debug.Log("Connected.");
        this.tunnel.RemoteTarget = Network.connections[0];
    }

    public void StartRecording(string deviceName = null)
    {
        Debug.Log("Starting Recording.");
        this.recordingClip = Microphone.Start(deviceName, false, 100, recordingFrequency);
        this.microphoneDataCollector.RecordingClip = this.recordingClip;
    }

    public void StopRecording(string deviceName = null)
    {
        Debug.Log("Stopping recording.");
        Microphone.End(deviceName);
    }
}