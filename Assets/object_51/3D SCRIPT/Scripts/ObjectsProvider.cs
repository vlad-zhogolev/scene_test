using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsProvider : MonoBehaviour
{
    [SerializeField]
    List<string> uids;

    [SerializeField]
    List<GameObject> gameObjects;

    public static Dictionary<string, GameObject> objects;

    void Start()
    {
        objects = new Dictionary<string, GameObject>();
        for (var i = 0; i < Math.Min(uids.Count, gameObjects.Count); ++i)
        {
            objects.Add(uids[i], gameObjects[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
