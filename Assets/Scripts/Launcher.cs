using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;

namespace RGSK
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        // The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        #endregion

        #region Public Fields
        public GameObject roomPanel;
        public GameObject playerEntry;
        public int arena;
        public int time = 10;
        public int timeBot = 6;
        public GameObject timePanel;

        public bool isGuest=false;
        #endregion

        #region Private Fields


        // This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        string gameVersion = "1";

        bool isConnecting;

        #endregion


        #region MonoBehaviour CallBacks


        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            Connect();
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            global.onlineBots = 0;
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }
        #endregion

        #region MonoBehaviourPunCallbacks Callbacks


        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            // we don't want to do anything if we are not attempting to join a room.
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (isConnecting)
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            roomPanel.SetActive(true);
            /* player properties
            Hashtable hashCar = new Hashtable();
            hashCar.Add("Car", global.playerCar);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashCar);
            */
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.

            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");

                Hashtable hashTime = new Hashtable();
                hashTime.Add("Time", 10);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashTime);
                //StartCoroutine(timeBots());
            }
            else
            {
                isGuest = true;
                timePanel.SetActive(true);
            }

            GameObject[] tags = GameObject.FindGameObjectsWithTag("GamerTag");
            for (int i = 0; i < tags.Length; i++)
            {
                Destroy(tags[i]);
            }

            for (int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                //pass info to tag
                GameObject entry = Instantiate(playerEntry);
                entry.transform.parent = playerEntry.transform.parent;
                entry.transform.GetChild(1).GetComponent<Text>().text = (string)PhotonNetwork.CurrentRoom.GetPlayer(int.Parse("0" + i)).NickName;
                entry.SetActive(true);
            }
        }

        #endregion
        private IEnumerator timeBots()
        {
            yield return new WaitForSeconds(1);
            timeBot--;
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && timeBot>0)
            {
                StartCoroutine(timeBots());
            }
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && timeBot == 0)
            {
                global.onlineBots = 1;
                GameObject.Find("Room Canvas").GetComponent<OnlineGameManager>().OnBotEnteredRoom();
            }
        }
        private void Update()
        {
            if (isGuest)
            {
                time = (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"];
                if (time == 0) return;
                timePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Starts In :" + time;
            }
        }

    }
}