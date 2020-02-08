using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkingPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject avatar;
    public Transform playerGlobal;
    public Transform playerLocal;
    public Transform leftHand;
    public Transform rightHand;

    public static readonly string PLAYER_TAG = "Player";

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            Debug.Log("NetworkingPlayer instantiated");
            playerGlobal = GameObject.Find("OVRPlayerController").transform;
            playerLocal = playerGlobal.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");
            leftHand = playerGlobal.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor");
            rightHand = playerGlobal.Find("OVRCameraRig/TrackingSpace/RightHandAnchor");
            leftHand.gameObject.tag = PLAYER_TAG;
            rightHand.gameObject.tag = PLAYER_TAG;
            this.transform.SetParent(playerLocal);
            this.transform.localPosition = Vector3.zero;
            avatar.SetActive(false);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerGlobal.position);
            stream.SendNext(playerGlobal.rotation);
            stream.SendNext(playerLocal.localPosition);
            stream.SendNext(playerLocal.localRotation);
        }
        else
        {
            this.transform.position = (Vector3)stream.ReceiveNext();
            this.transform.rotation = (Quaternion)stream.ReceiveNext();
            avatar.transform.localPosition = (Vector3)stream.ReceiveNext();
            avatar.transform.localRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
