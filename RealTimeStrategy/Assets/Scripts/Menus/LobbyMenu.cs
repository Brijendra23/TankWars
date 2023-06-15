using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;

    private void Start()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;

    }
    private void OnDestroy()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
    }
    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
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
