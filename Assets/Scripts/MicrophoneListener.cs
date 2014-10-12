using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class MicrophoneListener : MonoBehaviour
{
    private string selectedDevice = null;
    [SerializeField]
    private int recordingFrequency = 10000;
    private AudioClip currentRecordingClip;
    private bool isRecording = false;

    private List<AudioClip> recordings = new List<AudioClip>();

    int lastSample;

    void Awake()
    {
        var options = Microphone.devices;
        if (options.Length == 0)
        {
            throw new InvalidOperationException("There is no recording device detected.");
        }
        else
        {
            this.selectedDevice = options[0];
        }
    }

    void Update()
    {
        if (NetworkManager.gameStarted)
        {
            int pos = Microphone.GetPosition(null);
            int diff = pos - lastSample;
            if (diff > 0)
            {
                float[] samples = new float[diff * currentRecordingClip.channels];
                currentRecordingClip.GetData(samples, lastSample);
                byte[] ba = Util.ToByteArray(samples);
                if (Network.isClient)
                {
                    networkView.RPC("RouteThroughServer", RPCMode.Server, ba, currentRecordingClip.channels);
                }
                else
                {
                    networkView.RPC("Send", RPCMode.Others, ba, currentRecordingClip.channels);
                }
            }
            lastSample = pos;
        }
    }

    void OnGUI()
    {
        if (NetworkManager.gameStarted)
        {
            GUILayout.BeginVertical();
            foreach (var device in Microphone.devices)
            {
                if (this.selectedDevice == device)
                {
                    GUI.contentColor = Color.cyan;
                }
                else
                {
                    GUI.contentColor = Color.white;
                }
                if (GUILayout.Button(device))
                {
                    this.selectedDevice = device;
                }
            }
            GUI.contentColor = Color.white;

            GUILayout.Label("Recording frequency:");
            string newFreqString = GUILayout.TextField(recordingFrequency.ToString());
            int.TryParse(newFreqString, out recordingFrequency);

            GUILayout.Space(50);
            if (selectedDevice != null)
            {
                if (GUILayout.Button(isRecording ? "Stop" : "Record"))
                {
                    isRecording = !isRecording;
                    if (isRecording)
                    {
                        RecordButtonPressed();
                    }
                    else
                    {
                        StopButtonPressed();
                    }
                }
                if (GUILayout.Button("Play"))
                {
                    PlaySoundClip();
                }
            }
            GUILayout.Space(25);
            foreach (var recording in this.recordings)
            {
                if (this.currentRecordingClip == recording)
                {
                    GUI.contentColor = Color.cyan;
                }
                else
                {
                    GUI.contentColor = Color.white;
                }

                if (GUILayout.Button(recording.name))
                {
                    this.currentRecordingClip = recording;
                    PlaySoundClip();
                }
            }
            GUILayout.EndVertical();
        }
    }

    private void RecordButtonPressed()
    {
        this.currentRecordingClip = Microphone.Start(selectedDevice, false, 100, recordingFrequency);
    }

    private void StopButtonPressed()
    {
        Microphone.End(selectedDevice);
        if (this.currentRecordingClip != null)
        {
            recordings.Add(currentRecordingClip);
        }
    }

    private void PlaySoundClip()
    {
        this.audio.PlayOneShot(currentRecordingClip);
    }

    [RPC]
    public void Send(byte[] ba, int chan)
    {
        float[] f = Util.ToFloatArray(ba);
        audio.clip = AudioClip.Create("test", f.Length, chan, recordingFrequency, true, false);
        audio.clip.SetData(f, 0);
        if (!audio.isPlaying)
            audio.Play();
    }

    [RPC]
    public void RouteThroughServer(byte[] ba, int chan)
    {
        Debug.Log("Received client thing");
        networkView.RPC("Send", RPCMode.Others, ba, chan);
        Send(ba, chan);
    }
}