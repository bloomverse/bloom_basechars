//using Photon.Pun;
//using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaLobbyController : MonoBehaviour
{
 /*   private int Roomsize= 4;

    public override void OnConnectedToMaster(){
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode,string message){
        Debug.Log("Failed to join room");
        CreateRoom();
    }

    public void CreateRoom(){
        int randomNumber = 100; //Random.Range(0,10000);
        RoomOptions roomOps = new RoomOptions(){ IsVisible=true,IsOpen=true, MaxPlayers = (byte)Roomsize};
        PhotonNetwork.CreateRoom("ArenaRoom" + randomNumber,roomOps);
        Debug.Log(randomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode,string message){
       Debug.Log("Failed creating room");
       CreateRoom();
    }

    public void QuickCancel(){
       PhotonNetwork.LeaveRoom();
    }*/
}
