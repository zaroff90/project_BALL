using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonCustomFields : MonoBehaviourPunCallbacks
{

    public string OT;

    private void Start()
    {
        if (this.photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            //turn off player control
        }
        //get game controllers
        StartCoroutine(OnRefresh());
    }
    public IEnumerator OnRefresh()
    {
        if (PhotonNetwork.IsConnected == false)
        {
            yield break;
        }
        yield return new WaitForSeconds(.5f);
        /*this.gameObject.tag = (string)PhotonNetwork.CurrentRoom.GetPlayer(this.GetComponent<PhotonView>().OwnerActorNr).CustomProperties["Role"];*/
        //assign names and whatnot
        if (this.photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            //target game controllers
        }
        OT = this.GetComponent<PhotonView>().OwnerActorNr.ToString();
    }
    public override void OnPlayerLeftRoom(Player other)
    {

        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

        }
        if (OT == other.ActorNumber.ToString())
        {
            Debug.Log("we quit");
            this.gameObject.GetComponent<PlayerScripts>().isAi = true;
        }
    }
}