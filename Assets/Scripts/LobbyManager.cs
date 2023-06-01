using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _logText;
    [SerializeField] private InputField _createField;
    [SerializeField] private InputField _joinField;
    [SerializeField] private Button _createButton;
    [SerializeField] private Button _joinButton;
    [SerializeField] private Button _leaveRoomButton;
    private RoomOptions roomOptions;
    private void Start()
    {
        PhotonNetwork.NickName = "Player_" + Random.Range(1, 1000);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
        roomOptions = new RoomOptions { MaxPlayers = 2 };
        _leaveRoomButton.interactable = false;
    }
    public override void OnConnectedToMaster()
    {
        Log("Connected to Master");
    }
    public override void OnJoinedRoom()
    {
        Log("You Join Room");
        _leaveRoomButton.interactable = true;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Log(newPlayer.NickName + " Enter room"); 
        if(PhotonNetwork.CurrentRoom.PlayerCount == roomOptions.MaxPlayers)
        {
            LevelManager.Instance.StartScene("Game");
        }
    }
    public override void OnLeftRoom()
    {
        Log("You Leave Room");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Log(message);
        SwitchButtons(true);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Log(message);
        SwitchButtons(true);
    }
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(_createField.text, roomOptions);
        SwitchButtons(false);
    }
    public void JoinRoom()
    {
        if (_joinField.text != null)
        {
            PhotonNetwork.JoinRoom(_joinField.text);
            SwitchButtons(false);
        }
        else { Log("Please enter room name!"); }
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SwitchButtons(true);
    }
    public void Exit()
    {
        Application.Quit();
    }
    private void Log(string message)
    {
        Debug.Log(message);
        _logText.text += "\n";
        _logText.text += message;
    }
    private void SwitchButtons(bool value)
    {
        _joinButton.interactable = value;
        _createButton.interactable = value;
        _createField.interactable = value;
        _joinField.interactable = value;
    }
}
