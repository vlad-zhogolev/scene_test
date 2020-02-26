using Client.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class SceneLauncher : MonoBehaviour
{
    private CancellationTokenSource token;

    [SerializeField]
    bool testChangeState = false;

    // Start is called before the first frame update
    void Start()
    {
        DataProvider.client.OnSwap += SwapObjects;
        DataProvider.client.OnInteractiveChange += ChangeState;
        this.token = new CancellationTokenSource();
        var token = this.token.Token;
        PeriodicTask.Run(() => { SendPlayerPosition(); }, TimeSpan.FromSeconds(1), token);
    }

    public void ChangeState(Client.Model.Interactive interactive)
    {
        var uid = interactive.objectId;
        var obj = ObjectsProvider.objects[uid].gameObject;
        var interactable = obj.GetComponent(typeof(IInteractable)) as IInteractable;
        interactable.ChangeState(interactive.state);
    }

    public void SendPlayerPosition()
    {
        if (DataProvider.player == null)
        {
            DataProvider.player = GameObject.FindWithTag("Player");
        }

        if (DataProvider.player == null)
        {
            return;
        }

        DataProvider.client.PostPosition(DataProvider.player.transform.position.x, DataProvider.player.transform.position.z);
    }

    public void SwapObjects(Swap swap)
    {
        StartCoroutine(GetAssetBundle(swap));
    }

    IEnumerator GetAssetBundle(Swap swap)
    {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(swap.link);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            var name = bundle.GetAllAssetNames()[0];
            var loadedAsset = bundle.LoadAssetAsync<GameObject>(name);
            yield return loadedAsset;
            var loadedObject = ((GameObject)loadedAsset.asset).gameObject;

            var currentObject = ObjectsProvider.objects[swap.oldId];
            for (var i = currentObject.transform.GetChildCount() - 1; i >= 0; --i)
            {
                var child = currentObject.transform.GetChild(i);
                Destroy(child.gameObject);
            }

            Instantiate(loadedObject.transform, currentObject.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (testChangeState)
        {
            testChangeState = false;
            ChangeState(new Interactive() { objectId = "q", state = 1});
        }
    }

    private void OnDestroy()
    {
        token?.Cancel();
    }
}
