using UnityEngine;
using System;
using System.Collections;

public static class Utility
{
	public static string TimeString( TimeSpan time )
	{
		if (time.Days <= 0 && time.Hours <= 0 && time.Minutes <= 0)
			return string.Format("{0:D2}s", time.Seconds);
		else if (time.Days <= 0 && time.Hours <= 0)
			return string.Format("{0:D2}m:{1:D2}s", time.Minutes, time.Seconds);
		else if (time.Days <= 0)
			return string.Format("{0:D2}h:{1:D2}m:{2:D2}s", time.Hours, time.Minutes, time.Seconds);
		else
			return string.Format("{0}d:{1:D2}h:{2:D2}m:{3:D2}s", time.Days, time.Hours, time.Minutes, time.Seconds);
	}
}