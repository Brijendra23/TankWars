using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField addressInput = null;
    [SerializeField] private Button joinButton=null;


    public void Join()
    {
        string address= addressInput.text;
        NetworkManager.singleton.networkAddress = address;//using mirror connecting to give address
        NetworkManager.singleton.StartClient();//starts the player as a client

        joinButton.interactable = false;//disables join button
    }
    private void OnEnable()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }
    private void OnDisable()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;

    }
    private void HandleClientConnected()// when player connects successfully to the server
    {
        joinButton.interactable = true;//goes to the homepage and wants to join someone else
        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()// if the player cannot connect to the server due to some reason
    {
        joinButton.interactable = true;
    }
}
