using System;
/// <summary>
/// Collects microphone data via the Unity API and broadcasts that AudioClip data as it is becomes available.
/// </summary>
using UnityEngine;
public class UnityMicrophoneDataCollector : MonoBehaviour, IAudioDataProvider
{

    private bool printedToFile;


    [SerializeField]
    private int recordingFrequency = 16000;
    private AudioClip __recordingClip;
    private int lastSample = 0;
    private KeyCode hotkey = KeyCode.BackQuote;
    private string currentLabel = "Stopped";
    private DataAccumulationBuffer<float> accumulator;
    private int numChannels;
    private int frameSize = 320;
    private float[] localFrameStorage = new float[320];


    private void Awake()
    {
        accumulator = new DataAccumulationBuffer<float>(frameSize);
        accumulator.DataChunkFilled += OnDataChunkFilled;
    }

    private void OnDataChunkFilled(float[] buffer, int startingIndex)
    {
        if (buffer.Length != frameSize)
        {
            Buffer.BlockCopy(buffer, startingIndex, this.localFrameStorage, 0, localFrameStorage.Length * 4);
            buffer = this.localFrameStorage;
        }

        OnAudioDataReceived(buffer, this.numChannels);
    }

    /// <summary>
    /// The AudioClip to monitor for recording and whose data will be broadcasted.
    /// </summary>
    private AudioClip RecordingClip
    {
        get { return __recordingClip; }
        set
        {
            __recordingClip = value;
            if (__recordingClip == null)
            {
                this.enabled = false;
            }
            else
            {
                this.enabled = true;
                this.numChannels = __recordingClip.channels;
            }

            lastSample = 0;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(hotkey))
        {
            StartRecording();
        }
        else if (Input.GetKeyUp(hotkey))
        {
            StopRecording();
        }

        int recordingPosition = Microphone.GetPosition(null);

        int diff = recordingPosition - lastSample;
        if (diff > 0)
        {
            float[] samples = new float[diff * __recordingClip.channels];
            __recordingClip.GetData(samples, lastSample);
            AccumulateData(samples);
        }
        lastSample = recordingPosition;
    }

    private void AccumulateData(float[] samples)
    {
        accumulator.AccumulateData(samples);
    }

    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(Screen.width * .5f, Screen.height * .5f, 300, 300), currentLabel);
    }

    public void StartRecording(string deviceName = null)
    {
        currentLabel = "Recording";
        this.RecordingClip = Microphone.Start(deviceName, false, 100, this.recordingFrequency);
    }

    public void StopRecording(string deviceName = null)
    {
        currentLabel = "Stopped";
        Microphone.End(deviceName);
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
        if (!printedToFile)
        {
            Util.PrintToFile(data, "received" + Util.CurrentTimeStamp);
            printedToFile = true;
        }
    }
}