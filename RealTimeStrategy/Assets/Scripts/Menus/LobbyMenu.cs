using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private TMP_Text[] playerName=new TMP_Text[4];

    private void Start()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSPlayerScript.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayerScript.ClientOnInfoUpdated += ClientHandleInfoUpdated;

    }
    private void OnDestroy()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSPlayerScript.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayerScript.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    private void ClientHandleInfoUpdated()
    {
        List<RTSPlayerScript> players = ((RTSNetworkManager)NetworkManager.singleton).Player;
        for(int i=0;i<players.Count; i++)
        {
            playerName[i].text = players[i].GetDisplayName();//to set the name
        }
        for (int i = players.Count; i < playerName.Length; i++)//when a player leaves the lobby to set it back to waiting for players
        {
            playerName[i].text = "Waiting For Player..";
        }
        startGameButton.interactable = players.Count >=2;
    }

    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }
    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
       startGameButton.gameObject.SetActive(state);
    }
    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<RTSPlayerScript>().CmdStartGame();
    }

    public void LeaveLobby()
    {
        if(NetworkServer.active&&NetworkClient.active)//if you are the host
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene(0);
        }
    }
}
