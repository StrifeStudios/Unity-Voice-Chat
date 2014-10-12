using UnityEngine;
using System.Collections.Generic;

public class AudioMixer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public float mix(List<float> audioSamples)
	{
		float output = 0f;

		//TODO: Make this math magical
		foreach (float sample in audioSamples)
		{
			output += sample;
		}
	}
}
