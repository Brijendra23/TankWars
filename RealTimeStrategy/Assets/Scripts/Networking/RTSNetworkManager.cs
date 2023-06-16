using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitBasePrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;
  
    public static event Action ClientOnConnected;//is invoked by the server to handle the event when player in connected in lobby
    public static event Action ClientOnDisconnected;// when cannot connected

    private bool isGameInProgress = false;//to check whether the server has started the game so no entry of player


    public List<RTSPlayerScript> Player { get; }= new List<RTSPlayerScript>();
    #region Server
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(!isGameInProgress) { return; }
        conn.Disconnect();
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if(conn.identity!=null)
        {
            RTSPlayerScript player = conn.identity.GetComponent<RTSPlayerScript>();
            Player.Remove(player);
        }
        
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    { 
       
        Player.Clear(); 
        isGameInProgress = false;
       

    }
    public void StartGame()
    {
        if(Player.Count <2) { return; }
        isGameInProgress=true;
        ServerChangeScene("Scene_Map_01");
    }
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);


        RTSPlayerScript player = conn.identity.GetComponent<RTSPlayerScript>();

        Player.Add(player);

        player.SetDisplayName($"Player {Player.Count}");

        player.SetTeamColor(new Color(UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)));//sending random color to Rts player script when connection joins


        player.SetIsPartyOwner(Player.Count==1);//since the first player that joins is the host
    }
    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
            foreach(RTSPlayerScript player in Player)//instantiating the players unit bases
            {
               GameObject baseInstance= Instantiate(unitBasePrefab,
                   GetStartPosition().position,
                   Quaternion.identity);
                NetworkServer.Spawn(baseInstance,player.connectionToClient);//auhtority

                if (player.connectionToClient.owned.Contains(baseInstance.GetComponent<NetworkIdentity>()))
                {
                    player.transform.position = baseInstance.transform.position;
                }
            }

        }
    }
    #endregion

    #region Client

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        ClientOnConnected?.Invoke();

    }
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        ClientOnDisconnected?.Invoke();
    }
    public override void OnStopClient()
    {
        Player.Clear();
    }

    #endregion


}
