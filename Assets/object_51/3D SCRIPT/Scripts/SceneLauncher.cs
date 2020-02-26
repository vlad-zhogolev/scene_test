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
            var name = bundle.AllAssetNames()[0];
            var loadAsset = bundle.LoadAssetAsync<GameObject>(name);
            yield return loadAsset;
            var newMesh = ((GameObject)loadAsset.asset).GetComponent<MeshFilter>().sharedMesh;

            var obj = ObjectsProvider.objects[swap.oldId];
            obj.GetComponent<MeshFilter>().mesh = newMesh;

            ObjectsProvider.objects.Remove(swap.oldId);
            ObjectsProvider.objects[swap.newId] = obj;
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
