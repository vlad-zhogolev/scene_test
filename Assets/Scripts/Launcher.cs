using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Client;
using Client.Model;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Com.Sberbank.VRHouse
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        public delegate void OnInitMessage(Init init);
        public event OnInitMessage OnInit;

        #region Private Fields

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";

        AssetBundle sceneAssetBundle;

        string sceneName;

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

        Init MockInit()
        {
            return new Init
            {
                scene = new Client.Model.Scene
                {
                    name = "test",
                    link = "https://ucb9d9b89f12e88ca86bac47b7a5.dl.dropboxusercontent.com/cd/0/get/AyvTKCtdO6T6XRO6_A_OYH08ZZfOrmimCQr66iHiyBe5jqxAhbWdGqHmKa0_3e_X11D-T9O_Ob7_nTt1b6b1fTijxmSajzgi45UcoKh5jgGDe_gZbkIHSCSEld2W7T31tCA/file?dl=1#"
                },
                objects = new Client.Model.Object[0],
                roomId = "test_room",
                maxPlayers = 2
            };
        }

        void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            DataProvider.client = new WebSocketClient("vrhouse.denmko.ru", 1998);
            //DataProvider.client.PostEcho().GetAwaiter().GetResult();

            //using (UnityWebRequest www = UnityWebRequest.Get("http://vrhouse.denmko.ru:1998/client/register"))
            //{
            //    www.SendWebRequest();
            //
            //    if (www.isNetworkError || www.isHttpError)
            //    {
            //        Debug.Log(www.error);
            //    }
            //    else
            //    {
            //        Debug.Log("Form upload complete!");
            //    }
            //}


            //DataProvider.client.OnInit += HandleInit;
            //this.OnInit += HandleInit;
            //OnInit(MockInit());
            HandleInit(MockInit());
        }

        //void Start()
        //{
        //    PhotonNetwork.AutomaticallySyncScene = true;
        //}

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
            //PhotonNetwork.JoinLobby();
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = (byte)DataProvider.init.maxPlayers };
            PhotonNetwork.JoinOrCreateRoom(DataProvider.init.roomId, roomOptions, TypedLobby.Default);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinedLobby()
        {
            //Debug.Log("Launcher: Joined lobby");
            //RoomOptions roomOptions = new RoomOptions() { MaxPlayers = maxPlayersPerRoom };
            //PhotonNetwork.JoinOrCreateRoom(room, roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Launcher: Instantiating NetworkingPlayer");
            PhotonNetwork.LoadLevel(sceneName);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Launcher: Failed to join room");
        }

        #endregion

        void HandleInit(Init init)
        {
            Debug.LogFormat("{0}: Handling Init", GetType().Name);
            DataProvider.init = init;
            
            sceneName = "_Mobile";
            Debug.LogFormat("{0}: Start connecting to scene", GetType().Name);
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
            
            //StartCoroutine(DownloadScene());
        }

        IEnumerator DownloadScene()
        {
            string url = DataProvider.init.scene.link;
            using (var request = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                Debug.LogFormat("{0}: Download scene asset bundle by url: {1}", GetType().Name, url);
                yield return request.SendWebRequest();
                if (request.isHttpError || request.isNetworkError)
                {
                    Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);
                    yield break;
                }
                sceneAssetBundle = DownloadHandlerAssetBundle.GetContent(request);
                Debug.LogFormat("{0}: Scene asset bundle downloaded", GetType().Name);

                sceneName = sceneAssetBundle.GetAllScenePaths()[0];

                Debug.LogFormat("{0}: Start connecting to scene", GetType().Name);
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
                //SceneManager.LoadScene(sceneName);
            }
        }
    }
}
