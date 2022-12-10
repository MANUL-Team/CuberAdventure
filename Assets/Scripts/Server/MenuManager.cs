using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviourPunCallbacks
{


    [SerializeField] private InputField nickname;
    [SerializeField] private InputField createInput;
    [SerializeField] private InputField joinInput;


    public void CreateRoom(){
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(createInput.text, roomOptions);
    }

    public void JoinRoom(){
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom(){
        PhotonNetwork.LoadLevel("ServLevel");
    }

    public void SaveName(){
        PlayerPrefs.SetString("Name", nickname.text);
        PhotonNetwork.NickName = nickname.text;
    }

    public void QuickMatch(){
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message){
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }


    void Start()
    {
        nickname.text = PlayerPrefs.GetString("Name");
        PhotonNetwork.NickName = nickname.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
