using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsProvider : MonoBehaviour
{
    [SerializeField]
    private List<ObjectWithUid> objectWithUidList = new List<ObjectWithUid>();

    public static Dictionary<string, GameObject> objects;

    void Start()
    {
        objects = new Dictionary<string, GameObject>();
        foreach (var item in objectWithUidList)
        {
            objects.Add(item.uid, item.obj);
        }
    }
}

[Serializable]
public class ObjectWithUid
{
    public GameObject obj;
    public string uid;
}
