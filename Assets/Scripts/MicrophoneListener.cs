using UnityEngine;
using System.Collections;

public class MicrophoneListener : MonoBehaviour
{
    private string selectedDevice = null;
    [SerializeField]
    private int recordingFrequency = 10000;
    private AudioClip audioClip;
    void OnGUI()
    {
        GUILayout.BeginVertical();
        foreach (var device in Microphone.devices)
        {
            if (GUILayout.Button(device))
            {
                this.selectedDevice = device;
            }
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
