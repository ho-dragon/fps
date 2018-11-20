using UnityEngine;

public static class Logger {
	public enum LogLevel {
		debug,
		verbose,
		warning,
		error
	}

	public static bool isMuted = false;

	public static int logLevel = (int)LogLevel.debug;

	public static void SetLogLevel(LogLevel logLevel) {
        Logger.logLevel = (int)logLevel;
	}

	public static void DebugHighlight(string msg) {
		if (logLevel > (int)LogLevel.debug) {
			return;
		}

		UnityEngine.Debug.Log("<color=#FF0000>" + msg + "</color>");
	}


	public static void DebugRed(string fmt, params object[] param) {
		DebugHighlight(fmt, param);
	}

	public static void DebugHighlight(string fmt, params object[] param) {
		if (logLevel > (int)LogLevel.debug) {
			return;
		}

		DebugHighlight(string.Format(fmt, param));
	}

	public static void Debug(string msg) {
		if (logLevel > (int)LogLevel.debug || isMuted) {
			return;
		}

		UnityEngine.Debug.Log(msg);
	}

	public static void Debug(string fmt, params object[] param) {
		if (logLevel > (int)LogLevel.debug || isMuted) {
			return;
		}

		Log(string.Format(fmt, param));
	}


	public static void Log(string msg) {
		if (logLevel > (int)LogLevel.verbose || isMuted) {
			return;
		}

		UnityEngine.Debug.Log(msg);
	}

	public static void Log(string fmt, params object[] param) {
		if (logLevel > (int)LogLevel.verbose || isMuted) {
			return;
		}

		Log(string.Format(fmt, param));
	}

	public static void Warning(string msg) {
		if (logLevel > (int)LogLevel.warning || isMuted) {
			return;
		}

		UnityEngine.Debug.LogWarning(msg);
	}

	public static void Warning(string fmt, params object[] param) {
		if (logLevel > (int)LogLevel.warning || isMuted) {
			return;
		}

		Warning(string.Format(fmt, param));
	}


	public static void Error(string msg) {
		if (logLevel > (int)LogLevel.error || isMuted) {
			return;
		}

		UnityEngine.Debug.LogError(msg);
	}

	public static void Error(string fmt, params object[] param) {
		if (logLevel > (int)LogLevel.error || isMuted) {
			return;
		}

		Error(string.Format(fmt, param));
	}

	public static void Exception(System.Exception ex) {
		Error(ex.ToString());
	}

	public static void LogTrace(string obj, string fn, string msg) {
#if UNITY_EDITOR
		Debug(string.Format("[<color=#FF8040>{0}</color>.<color=#86E57F>{1}</color>] {2}", obj, fn, msg));
#else
		Debug(string.Format("[{0}.{1}] {2}", obj, fn, msg));
#endif
	}

	public static void LogTrace(string obj, string fn, string fmt, params object[] param) {
		if (logLevel > (int)LogLevel.debug) {
			return;
		}

		LogTrace(obj, fn, string.Format(fmt, param));
	}

}
