﻿using UnityEngine;
using Photon.Pun;

public class SwitchController : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private Light[] lights;

    [SerializeField]
    private bool isTurnedOn;

    bool isPlayerInteracts = false;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Debug.Log("SwitchController: send light state");
            stream.SendNext(isTurnedOn);
        }
        else
        {
            Debug.Log("SwitchController: received light state");
            isTurnedOn = (bool)stream.ReceiveNext();
            foreach (var light in lights)
            {
                light.enabled = isTurnedOn;
            }
        }
    }

    void Update()
    {
        bool isRIndexTriggerPressed = OVRInput.Get(OVRInput.RawButton.RIndexTrigger);
        bool isButtonReleased = OVRInput.GetUp(OVRInput.Button.One);
        if (isPlayerInteracts && (isRIndexTriggerPressed || isButtonReleased))
        {
            Debug.LogFormat("SwitchController: Switch lights, isRIndexTriggerPressed = {0}, isButtonReleased = {1}", isRIndexTriggerPressed, isButtonReleased);
            isTurnedOn = !isTurnedOn;
            foreach (var light in lights)
            {
                light.enabled = isTurnedOn;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isPlayerInteracts = (other.transform.root.gameObject.CompareTag(NetworkingPlayer.PLAYER_TAG));
        if (isPlayerInteracts)
        {
            Debug.Log("SwitchController: Player interacts with switch");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerInteracts = !(other.transform.root.gameObject.CompareTag(NetworkingPlayer.PLAYER_TAG));
        if (!isPlayerInteracts)
        {
            Debug.Log("SwitchController: Player ended interaction with switch");
        }
    }
}
