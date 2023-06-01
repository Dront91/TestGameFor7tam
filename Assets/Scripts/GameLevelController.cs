using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameLevelController : MonoBehaviourPunCallbacks
{
    public static GameLevelController Instance;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private int _maxCoinCount;
    public int MaxCoinCount => _maxCoinCount;
    [SerializeField] private List<GameObject> _spawnPoints;
    [SerializeField] private ScorePanel _scorePanel;
    private List<PlayerController> _playerList = new List<PlayerController>();
    public List<PlayerController> PlayerList => _playerList;
    private PlayerController _player;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        var Rand = Random.Range(0, _spawnPoints.Count);
        var p = PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPoints[Rand].transform.position, Quaternion.identity);
        _player = p.GetComponent<PlayerController>();
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < _maxCoinCount; i++)
            {
                PhotonNetwork.Instantiate(_coinPrefab.name, new Vector3(Random.Range(-7f, 7f), Random.Range(-4.5f, 4.5f), 0), Quaternion.identity);
            }
        }
    }
    private void OnDestroy()
    {
        foreach (var player in _playerList)
        {
            player.OnDeath -= OnPlayerDeath;
        }
    }
    public void AddPlayerToList(PlayerController player)
    {
        _playerList.Add(player);
        player.OnDeath += OnPlayerDeath;
    }
    private void OnPlayerDeath()
    {
        StopGame();
    }
    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        LevelManager.Instance.StartScene("Lobby");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        StopGame();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _player.SwitchControl(true);
        _scorePanel.gameObject.SetActive(false);
    }

    private void StopGame()
    {
        _player.SwitchControl(false);
        var playerList = _playerList.OrderBy(p => p._photonView.Owner.ActorNumber).ToArray();
        var result = _playerList.OrderBy(p => p._photonView.Owner.ActorNumber).Select(p => p.MoneyCount).ToArray();
        _scorePanel.UpdateScore(playerList, result);
        _scorePanel.gameObject.SetActive(true);
    }
}
