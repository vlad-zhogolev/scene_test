using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using Photon.Pun;
using Photon.Realtime;

public class InstantiationTest : MonoBehaviourPun
{
    static readonly string url = "https://uc16c5b55f8479e40e6d20b57eb2.dl.dropboxusercontent.com/cd/0/get/AypqWbYN7y2FkV_J2t_hoS6sIGDMvxsr-cr7SNBaI6cclRHm4B0zc122-Ge6cLHoktNc835wsUIkAOB6RipbJ4R3xdCBiTHqvQOEfb5iuy3qvbe1OP6kzfA0iB1uOiGEd-M/file?dl=1#";
    static readonly string manifestUrl = "https://uc16c5b55f8479e40e6d20b57eb2.dl.dropboxusercontent.com/cd/0/get/AypqWbYN7y2FkV_J2t_hoS6sIGDMvxsr-cr7SNBaI6cclRHm4B0zc122-Ge6cLHoktNc835wsUIkAOB6RipbJ4R3xdCBiTHqvQOEfb5iuy3qvbe1OP6kzfA0iB1uOiGEd-M/file?dl=1#";

    //static readonly string url = "";

    AssetBundle assetBundle;

    //GameObject prefab;

    IEnumerator Start()
    {
        //using (var manifestRequest = UnityWebRequestAssetBundle.GetAssetBundle(manifestUrl))
        //{
        //    Debug.LogFormat("{0}: Download asset bundle by url: {1}", this.GetType().Name, manifestUrl);
        //    yield return manifestRequest.SendWebRequest();
        //    if (manifestRequest.isHttpError || manifestRequest.isNetworkError)
        //    {
        //        Debug.LogErrorFormat("error request [{0}, {1}]", manifestUrl, manifestRequest.error);
        //        yield break;
        //    }
        //    var manifestBundle = DownloadHandlerAssetBundle.GetContent(manifestRequest);
        //    Debug.LogFormat("{0}: Manifest bundle name: {1}", GetType().Name, manifestBundle.name);
        //    var manifest = manifestBundle.LoadAsset<AssetBundleManifest>(manifestBundle.name);

        //    string today = DateTime.Today.ToLongDateString();
        //    string cachePath = "Objects/" + today;
        //    Directory.CreateDirectory(cachePath);
        //    Cache newCache = Caching.AddCache(cachePath);

        //    if (newCache.valid)
        //    {
        //        Caching.currentCacheForWriting = newCache;
        //    }

        //    Hash128 hash = manifest.GetAssetBundleHash(manifestBundle.name);
        //    using (var request = UnityWebRequestAssetBundle.GetAssetBundle(url, hash, 0))
        //    {
        //        Debug.LogFormat("{0}: Download asset bundle by url: {1}", this.GetType().Name, url);
        //        yield return request.SendWebRequest();
        //        if (request.isHttpError || request.isNetworkError)
        //        {
        //            Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);
        //            yield break;
        //        }
        //        assetBundle = DownloadHandlerAssetBundle.GetContent(request);
        //        Debug.LogFormat("{0}: Asset bundle downloaded", GetType().Name);

        //        List<Hash128> listOfCachedVersions = new List<Hash128>();
        //        Caching.GetCachedVersions(assetBundle.name, listOfCachedVersions);

        //        string[] assetNames = assetBundle.GetAllAssetNames();
        //        var prefab = assetBundle.LoadAsset<GameObject>(assetNames[0]);

        //        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        //        pool.ResourceCache.Add(prefab.name, prefab);
        //        if (pool != null && prefab != null)
        //        {
        //            photonView.RPC("SpawnOnNetwork", RpcTarget.MasterClient, prefab.name);
        //        }
        //    }
        //}

        using (var request = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            Debug.LogFormat("{0}: Download asset bundle by url: {1}", this.GetType().Name, url);
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);
                yield break;
            }
            assetBundle = DownloadHandlerAssetBundle.GetContent(request);
            Debug.LogFormat("{0}: Asset bundle downloaded", GetType().Name);

            string[] assetNames = assetBundle.GetAllAssetNames();
            var prefab = assetBundle.LoadAsset<GameObject>(assetNames[0]);

            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            pool.ResourceCache.Add(prefab.name, prefab);
            if (pool != null && prefab != null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    SpawnOnNetwork(prefab.name);
                    //photonView.RPC("SpawnOnNetwork", RpcTarget.MasterClient, prefab.name);
                }
            }
        }
    }

    [PunRPC]
    void SpawnOnNetwork(string name)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateSceneObject(name, new Vector3(0, 5, 0), Quaternion.identity);
        }
    }
}
