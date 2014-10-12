using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MicrophoneListener : MonoBehaviour
{
    private string selectedDevice = null;
    [SerializeField]
    private int recordingFrequency = 10000;
    private AudioClip currentRecordingClip;
    private bool isRecording = false;

    private List<AudioClip> recordings = new List<AudioClip>();

	void Start()
	{
		NetworkManager.OnGameStart += OnGameStart;
	}

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

    void OnGUI()
    {
		if (NetworkManager.gameStarted)
		{
			GUILayout.BeginVertical ();
			foreach (var device in Microphone.devices) {
					if (this.selectedDevice == device) {
							GUI.contentColor = Color.cyan;
					} else {
							GUI.contentColor = Color.white;
					}
					if (GUILayout.Button (device)) {
							this.selectedDevice = device;
					}
			}
			GUI.contentColor = Color.white;

			GUILayout.Label ("Recording frequency:");
			string newFreqString = GUILayout.TextField (recordingFrequency.ToString ());
			int.TryParse (newFreqString, out recordingFrequency);

			GUILayout.Space (50);
			if (selectedDevice != null) {
					if (GUILayout.Button (isRecording ? "Stop" : "Record")) {
							isRecording = !isRecording;
							if (isRecording) {
									RecordButtonPressed ();
							} else {
									StopButtonPressed ();
							}
					}
					if (GUILayout.Button ("Play")) {
							PlaySoundClip ();
					}
			}
			GUILayout.Space (25);
			foreach (var recording in this.recordings) {           
					if (this.currentRecordingClip == recording) {
							GUI.contentColor = Color.cyan;
					} else {
							GUI.contentColor = Color.white;
					}

					if (GUILayout.Button (recording.name)) {
							this.currentRecordingClip = recording;
							PlaySoundClip ();
					}
			}
			GUILayout.EndVertical ();
		}
    }

    private void RecordButtonPressed()
    {
        this.currentRecordingClip = Microphone.Start(selectedDevice, false, 10, recordingFrequency);
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

	private void OnGameStart()
	{
		//Do something related to the game starting
	}
}