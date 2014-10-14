using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Reconstructs audio data from an IAudioDataProvider into an audio clip.
/// </summary>
public class AudioDataReconstructor : MonoBehaviour
{
    private IAudioDataProvider dataSource;
    [SerializeField]
    private int recordingFrequency = 10000;
    private int numChannels = 1;
    private int writeBoundary = 0;
    private int readBoundary = 0;
    private int minBufferLength = 2500;

    public IAudioDataProvider DataSource
    {
        get { return dataSource; }
        set
        {
            if (this.dataSource != null)
            {
                this.dataSource.AudioDataReceived -= OnAudioDataReceived;
            }
            if (value != null)
            {
                this.dataSource = value;
                this.dataSource.AudioDataReceived += OnAudioDataReceived;
            }
        }
    }

    void Awake()
    {
        this.audio.clip = AudioClip.Create("Reconstructed", 100 * recordingFrequency, this.numChannels, this.recordingFrequency, false, false);
    }

    void Update()
    {
        if (audio.isPlaying)
        {
            int currentPosition = audio.timeSamples;
            OnAudioClipPositionUpdated(currentPosition);
        }
    }

    public int RecordingFrequency
    {
        get { return recordingFrequency; }
        set { recordingFrequency = value; }
    }

    private void OnAudioDataReceived(AudioFrameData frameData)
    {
        this.audio.clip.SetData(frameData.AudioData, this.writeBoundary);
        this.writeBoundary += frameData.AudioData.Length;
        if (readBoundary + minBufferLength <= writeBoundary && !audio.isPlaying)
        {
            this.audio.Play();
        }
    }

    private void OnAudioClipPositionUpdated(int position)
    {
        Debug.Log("OnAudioRead: " + position);
        readBoundary = position;
        if (readBoundary >= writeBoundary)
        {
            Debug.Log("Read boundary reached write boundary.");
            audio.Pause();
            this.audio.timeSamples = 0;
            this.writeBoundary = 0;
            this.readBoundary = 0;
        }
    }
}
