using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField]
    private float xAngle;
    [SerializeField]
    private float yAngle;
    [SerializeField]
    private float zAngle;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(xAngle, yAngle, zAngle, Space.Self);
    }
}
