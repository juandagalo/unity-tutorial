using UnityEngine;

public class DreamLogs
{
#if UNITY_EDITOR
	public static bool HasReporter = true;
#else
	public static bool HasReporter = true;
#endif

	public static void Log(object log, params object[] replacements)
	{
		if (!HasReporter)
		{
			return;
		}

		if (replacements != null && replacements.Length > 0)
		{
			Debug.Log(string.Format(log.ToString(), replacements));
		}
		else
		{
			Debug.Log(log);
		}
	}

	public static void LogWarning(object log, params object[] replacements)
	{
		if (!HasReporter)
		{
			return;
		}

		if (replacements != null && replacements.Length > 0)
		{
			Debug.LogWarning(string.Format(log.ToString(), replacements));
		}
		else
		{
			Debug.LogWarning(log);
		}
	}

	public static void LogError(object log, params object[] replacements)
	{
		if (!HasReporter)
		{
			return;
		}

		if (replacements != null && replacements.Length > 0)
		{
			Debug.LogError(string.Format(log.ToString(), replacements));
		}
		else
		{
			Debug.LogError(log);
		}
	}
}