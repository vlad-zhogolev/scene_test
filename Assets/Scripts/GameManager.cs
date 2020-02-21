using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject[] lightSwitches;

    #region MonoBehaviour Callbacks

    void Start()
    {
        PhotonNetwork.Instantiate("NetworkingPlayer", Vector3.zero, Quaternion.identity, 0);
    }

    #endregion

    #region Photon Callbacks

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("GameManager: OnPlayerEnteredRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var lightSwitch in lightSwitches)
            {
                lightSwitch.GetPhotonView().RPC("SwitchLights", other, lightSwitch.GetComponent<SwitchController>().isTurnedOn);
            }
        }
    }

    #endregion
}
