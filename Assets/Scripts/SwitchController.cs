using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    [SerializeField]
    private Light[] lights;

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.root.gameObject.tag == NetworkingPlayer.PLAYER_TAG &&
            OVRInput.GetUp(OVRInput.Button.One))
        {
            foreach (var light in lights)
            {
                light.enabled = !light.enabled;
            }
        }
    }
}
