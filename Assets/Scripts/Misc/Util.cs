using UnityEngine;
using System.Collections;

public class Util : Singleton<Util>
{	
	// Use this for initialization
	void Start()
	{		

	}
	
	// Update is called once per frame
	void Update()
	{
		
	}
	
	public static void Print(string message)
	{
		Debug.Log(message);
	}
	
	
	public static float GetScale()
	{
		return Screen.height / 768f;
	}
}