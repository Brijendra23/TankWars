using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    //completely client side so no need to be the networked hence monobehaviour 
    [SerializeField] public GameObject gameOverParent=null;
    [SerializeField] private TMP_Text winnerNameText=null;

    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }
    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }
    public void LeaveGame()
    {
        if(NetworkServer.active&& NetworkClient.isConnected) 
        { //if we the winner is server 
            NetworkManager.singleton.StopHost();
        }
        else
        { //if winner is client
            NetworkManager.singleton.StopClient();
        }
    }

    private void ClientHandleGameOver(string winner)
    {
        winnerNameText.text=$"{winner} Has Won";
        gameOverParent.SetActive(true);
    }
}
