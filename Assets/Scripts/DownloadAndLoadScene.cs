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
    string url = "https://uc91a76e971c0b1f22607af8c75c.dl.dropboxusercontent.com/cd/0/get/AyotK5mpnqLdDXlfulZAFNL6Dfk6SM4KxTzblDvn-UXbiJEqMQazxEqBUYUuMf7oJrl-IJPqutiqht3nfGba3wLmtXk1k3YWTht_gNU_1B_FL-7mnwYD11vRkvL2n4RdKK4/file?dl=1#";

    AssetBundle assetBundle;

    // Start is called before the first frame update
    IEnumerator Start()
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
