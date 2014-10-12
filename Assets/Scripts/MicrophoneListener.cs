using UnityEngine;
using System.Collections;

public class MicrophoneListener : MonoBehaviour
{
    private string selectedDevice = null;
    [SerializeField]
    private int recordingFrequency = 10000;
    private AudioClip audioClip;

    private string frequencyInputString = "10000";
    void OnGUI()
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
        string newFreqString = GUILayout.TextField(frequencyInputString);
        if (int.TryParse(newFreqString, out recordingFrequency))
        {
            this.frequencyInputString = newFreqString;
        }

        GUILayout.Space(50);
        if (selectedDevice != null)
        {
            if (GUILayout.Button("Record"))
            {
                RecordButtonPressed();
            }
            if (GUILayout.Button("Stop"))
            {
                StopButtonPressed();
            }
        }
        GUILayout.EndVertical();
    }

    private void RecordButtonPressed()
    {
        this.audioClip = Microphone.Start(selectedDevice, false, 10, recordingFrequency);
    }

    private void StopButtonPressed()
    {
        Microphone.End(selectedDevice);
        this.audio.PlayOneShot(audioClip);
    }
}
