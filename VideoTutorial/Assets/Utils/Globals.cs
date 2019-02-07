using UnityEngine;

public sealed class Globals
{
	public static int Tempo
	{
		set { PlayerPrefs.SetInt("Tempo", Mathf.Clamp(value, 0, 2)); }
		get { return PlayerPrefs.GetInt("Tempo", 0); }
	}

	public static float InactivityTime
	{
		set { PlayerPrefs.SetFloat("INACTIVITY_TIME", value); }
		get { return PlayerPrefs.GetFloat("INACTIVITY_TIME", 180); }
	}

	public static string RecordFilePath
	{
		set { PlayerPrefs.SetString("MAX_FILE_PATH", value); }
		get { return PlayerPrefs.GetString("MAX_FILE_PATH", string.Empty); }
	}

	public static float RecordTime = 10;

	public static float StepValue = 8;

	public static bool UseLogger;
}