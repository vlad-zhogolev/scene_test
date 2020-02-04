using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.Sberbank.VRHouse
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";

        #endregion

        #region Private Serializable Fields

        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 2;

        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;

        [Tooltip("Name of room to enter")]
        [SerializeField]
        string room = "Room";

        [Tooltip("Name of scene to load")]
        [SerializeField]
        string scene = "Sample";

        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Launcher: Start connecting to scene");
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("Launcher: OnConnectedToMaster() was called by PUN");
            PhotonNetwork.JoinLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Launcher: Joined lobby");
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = maxPlayersPerRoom };
            PhotonNetwork.JoinOrCreateRoom(room, roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Launcher: Instantiating NetworkingPlayer");
            PhotonNetwork.LoadLevel(scene);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Launcher: Failed to join room");
        }

        #endregion
    }
}
