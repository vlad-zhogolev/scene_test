using UnityEngine;
using System.Collections;

public class DownloadHelper {
	private static AndroidJavaObject downloadHelper = null;
	private static string ACTIVITY_NAME = "com.unity3d.player.UnityPlayer";
	private static string CONTEXT = "currentActivity";
	private static string DOWNLOAD_HELPER = "com.oculus.downloadhelper.DownloadHelper";
	private static string BASE_PATH = "";

	private static void StartIfNeeded() {
		if (downloadHelper == null) {
			BASE_PATH = "file://" + Application.persistentDataPath + "/";
			AndroidJavaObject context = new AndroidJavaClass (ACTIVITY_NAME).GetStatic<AndroidJavaObject> (CONTEXT);
			downloadHelper = new AndroidJavaObject (DOWNLOAD_HELPER, context);
		}
	}

	public static long Download(string url, string destination, string title) {
		StartIfNeeded ();
		if (downloadHelper != null) {
			return downloadHelper.Call<long> ("StartDownload", url, BASE_PATH + destination, title);
		}
		return -1;
	}

	public static int GetDownloadProgress(long downloadId) {
		StartIfNeeded ();
		if (downloadHelper != null) {
			return downloadHelper.Call<int>("GetDownloadProgress", downloadId);
		}
		return -1;
	}

	public static void CancelDownload(long downloadId) {
		StartIfNeeded ();
		if (downloadHelper != null) {
			downloadHelper.Call ("CancelDownload", downloadId);
		}
	}
}