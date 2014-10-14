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
    private Queue<AudioFrameData> audioQueue = new Queue<AudioFrameData>();
    private bool isPlaying = false;
    private int numChannels = 1;
    private int writeBoundary;

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
        this.audio.clip = AudioClip.Create("Reconstructed", 100 * recordingFrequency, this.numChannels, this.recordingFrequency, false, false, null, OnAudioClipSetPosition);
        this.writeBoundary = 0;
    }

    public int RecordingFrequency
    {
        get { return recordingFrequency; }
        set { recordingFrequency = value; }
    }

    private void OnAudioDataReceived(AudioFrameData frameData)
    {
        audioQueue.Enqueue(frameData);
        this.audio.clip.SetData(frameData.AudioData, this.writeBoundary);
        this.writeBoundary += frameData.AudioData.Length;
        if (!this.audio.isPlaying)
        {
            this.audio.Play();
        }
    }

    private void OnAudioClipSetPosition(int position)
    {
        if (position >= this.writeBoundary)
        {
            Debug.Log("Read: " + position + ", Write: " + writeBoundary);
            audio.Stop();
            audio.timeSamples = this.writeBoundary;
        }
    }
}
