using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class TestFileWrite : MonoBehaviour
{
    //static readonly string manifestUrlStatic = "https://uc422fe434e18932b32b564780c1.dl.dropboxusercontent.com/cd/0/get/Ayx3mU1m3O0Zv_VHRxy2b1VnBNtEMb0qXeUJwyBQzEwU6HPY-ycPZ6vUVz9T8rikpu4Bvzi1fkkfeBLUB8vEADxpMG6lvcWVEIPnM3LR9YZkQMOdiyGEHdXaOKmFt5AA1JA/file?dl=1#";
    static readonly string manifestUrlStatic = "file:///D:/UnityProjects/Launcher/VRHouse/Assets/AssetBundles/downloadtest.manifest";
    static readonly string assetBundleUrlStatic = "https://uc4203002e8e8b3ee0483d663faa.dl.dropboxusercontent.com/cd/0/get/Ayy8zW58_v9SbqayP4xxCHQfKOnS7esVqig0THTMXJf2yJtYbArR6kD-AIM5xnRenHV-OUqha3xm4D-CRZRe1dreTIAei3Q_Ij31PHF09GpPpe8PMP7nTubXqq1fPXeGCoY/file?dl=1#";

    void Start()
    {
        StartCoroutine(DownloadOrGetCachedAssetBundle(manifestUrlStatic, assetBundleUrlStatic));
    }

    IEnumerator DownloadOrGetCachedAssetBundle(string manifestUrl, string assetBundleUrl)
    {
        //Load the manifest hash        
        Debug.LogFormat("{0}: downloading manifest by url: {1}", GetType().Name, manifestUrl);
        Hash128 hash = default;
        using (var manifestRequest = UnityWebRequest.Get(manifestUrl))
        {
            yield return manifestRequest.SendWebRequest();
            if (manifestRequest.isHttpError || manifestRequest.isNetworkError)
            {
                Debug.LogErrorFormat("{0}: error request [{1}, {2}]", GetType().Name, manifestUrl, manifestRequest.error);
                yield break;
            }
            // taken from here: https://habr.com/ru/post/433366/
            // Unity sucks
            var hashRow = manifestRequest.downloadHandler.text.ToString().Split("\n".ToCharArray())[5];
            hash = Hash128.Parse(hashRow.Split(':')[1].Trim());
        }

        Debug.LogFormat("{0}: hash.isValid = {1}", GetType().Name, hash.isValid);
        if (hash.isValid)
        {
            bool isCached = Caching.IsVersionCached(assetBundleUrl, hash);
            Debug.LogFormat("{0}: downloading asset bundle, is cached: {1}, url: {2}", GetType().Name, isCached, manifestUrl);
            AssetBundle assetBundle;
            using (var request = UnityWebRequestAssetBundle.GetAssetBundle(assetBundleUrl, hash, 0))
            {
                yield return request.SendWebRequest();
                if (request.isHttpError || request.isNetworkError)
                {
                    Debug.LogErrorFormat("{0}: error request [{1}, {2}]", GetType().Name, manifestUrl, request.error);
                    yield break;
                }
                assetBundle = DownloadHandlerAssetBundle.GetContent(request);
            }
            Debug.LogFormat("{0}: downloading asset bundle finished by url: {1}", GetType().Name, manifestUrl);
            string[] scenePaths = assetBundle.GetAllScenePaths();

            SceneManager.LoadScene(scenePaths[0]);
        }
    }
}
