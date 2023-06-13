using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        RTSPlayerScript player=conn.identity.GetComponent<RTSPlayerScript>();
        player.SetTeamColor(new Color(Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)));//sending random color to Rts player script when connection joins

        GameObject unitSpawnerInstance= Instantiate(unitSpawnerPrefab,
            conn.identity.transform.position,
            conn.identity.transform.rotation);
        NetworkServer.Spawn(unitSpawnerInstance,conn);
    }
    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

        }
    }
}
