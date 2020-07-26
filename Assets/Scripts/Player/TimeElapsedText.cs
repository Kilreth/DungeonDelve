using System;

public static class TimeElapsedText
{
	public static string FormatTimeElapsed(float seconds)
	{
		TimeSpan time = TimeSpan.FromSeconds(seconds);

		string text = string.Format("{0} second{1}",
			time.Seconds, time.Seconds == 1 ? "" : "s");
		if (time.Minutes > 0)
			text = string.Format("{0} minute{1}{2} and {3}",
				time.Minutes, time.Minutes == 1 ? "" : "s", time.Hours > 0 ? "," : "", text);
		if (time.Hours > 0)
			text = string.Format("{0} hour{1}, {2}",
				time.Hours, time.Hours == 1 ? "" : "s", text);
		if (time.Days > 0)
			text = string.Format("{0} day{1}, {2}",
				time.Days, time.Days == 1 ? "" : "s", text);

		return text;
	}
}
