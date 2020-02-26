using UnityEngine;
using Photon.Pun;
using System.Linq;

public class SwitchController : MonoBehaviourPun, IInteractable
{
    [SerializeField]
    private Light[] lights;

    [SerializeField]
    public bool isTurnedOn;

    bool isPlayerInteracts = false;

    [SerializeField]
    bool switchLights = false;
    
    [PunRPC]
    void SwitchLights(bool isTurnedOn)
    {
        this.isTurnedOn = isTurnedOn;
        foreach (var light in lights)
        {
            light.enabled = isTurnedOn;
        }
    }

    public void ChangeState(int state)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            bool isLightTurnedOn = state == 0 ? false : true;
            this.photonView.RPC("SwitchLights", RpcTarget.All, isLightTurnedOn);
        }
    }

    private void SendState()
    {
        var state = isTurnedOn ? 1 : 0;
        var uid = ObjectsProvider.objects.First(x => x.Value == gameObject).Key;
        DataProvider.client.PostChangeInterativeState(new Client.Model.Interactive()
        {
            objectId = uid,
            state = state
        });
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        Debug.Log("SwitchController: send light state");
    //        stream.SendNext(isTurnedOn);
    //    }
    //    else
    //    {
    //        Debug.Log("SwitchController: received light state");
    //        isTurnedOn = (bool)stream.ReceiveNext();
    //        foreach (var light in lights)
    //        {
    //            light.enabled = isTurnedOn;
    //        }
    //    }
    //}

    void Update()
    {
        bool isRIndexTriggerPressed = OVRInput.Get(OVRInput.RawButton.RIndexTrigger);
        bool isButtonReleased = OVRInput.GetUp(OVRInput.Button.One);
        if (switchLights || (isPlayerInteracts && (isRIndexTriggerPressed || isButtonReleased)))
        {
            switchLights = false;
            this.photonView.RPC("SwitchLights", RpcTarget.All, !isTurnedOn);
            SendState();

            //Debug.LogFormat("SwitchController: Switch lights, isRIndexTriggerPressed = {0}, isButtonReleased = {1}", isRIndexTriggerPressed, isButtonReleased);
            //base.photonView.RequestOwnership();
            //isTurnedOn = !isTurnedOn;
            //foreach (var light in lights)
            //{
            //    light.enabled = isTurnedOn;
            //}
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
