//using Photon.Pun;
using UnityEngine;

public class ArenaRoomController : MonoBehaviour {
    
    [SerializeField] private int multiplayerSceneIndex;

  /*  public override void OnEnable(){
        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void  OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom(){
        Debug.Log("Joined room");
        StartGame();
    }
    private void StartGame(){
        if(PhotonNetwork.IsMasterClient){
            Debug.Log("Starting Game");
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }*/

}
