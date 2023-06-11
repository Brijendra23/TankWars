using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;// when the building dies what happens?
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private RTSPlayerScript player;
    public override void OnStartServer()
    {
        timer=interval; 
        player=connectionToClient.identity.GetComponent<RTSPlayerScript>();

        health.ServerOnDie += ServerHandleDie;//what should be done when building die- subscribing
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;// what happens when server got disconnected- subscribing

    }
    public override void OnStopServer()
    {
        health.ServerOnDie += ServerHandleDie;//what should be done when building die- unsubscribing
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;// what happens when server got disconnected- unsubscribing

    }
    [ServerCallback]
    private void Update()
    {
        timer -=Time.deltaTime;
        if(timer<=0)
        {
            player.SetResources(player.GetResources()+resourcesPerInterval);//update the resources by adding the resource genenrated along with the present resources
            timer += interval;
        }
    }
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);// destroys the resources generator buildings
    }
    private void ServerHandleGameOver()
    {
        enabled = false;// resources generator component is disabled hence no resource genenration
    }

}
