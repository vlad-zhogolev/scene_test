using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class DownloadAndLoadScene : MonoBehaviour
{
    string url = "https://uc03e634e3dea4ef42a001e84722.dl.dropboxusercontent.com/cd/0/get/AytQ8kw_3jRkzLA4q-E-pAtpVDq1JtzTe_KrwBugO5yiafptIlWMHd_963EkcLWZaHEo9l2hWaZiUJQSv9G7RqBdVOrX20tRrorB8th0UMdegJKvLKwsB8fB3O6ox5wrds4/file?dl=1#";

    AssetBundle assetBundle;

    // Start is called before the first frame update
    void  Start()
    {
        StartCoroutine(DownloadScene());
    }

    IEnumerator DownloadScene()
    {
        Debug.Log("DownloadAndLoadScene: start");
        using (var request = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);
                yield break;
            }
            assetBundle = DownloadHandlerAssetBundle.GetContent(request);
        }
        Debug.Log("DownloadAndLoadScene: end");
        string[] scenePaths = assetBundle.GetAllScenePaths();

        SceneManager.LoadScene(scenePaths[0]);
        Debug.Log(scenePaths[0]);
    }
}
