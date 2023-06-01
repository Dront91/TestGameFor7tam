using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] private Text _player1;
    [SerializeField] private Text _player2;
    [SerializeField] private Text _player1Score;
    [SerializeField] private Text _player2Score;
    
    public void UpdateScore(PlayerController[] playerList, int[] result)
    {
        _player1.text = playerList[0].Nickname.text;
        _player2.text = playerList[1].Nickname.text;
        _player1Score.text = result[0].ToString();
        _player2Score.text = result[1].ToString();
    }
}
