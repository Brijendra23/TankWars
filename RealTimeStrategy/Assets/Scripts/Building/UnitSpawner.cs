using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Runtime.Remoting.Channels;
using UnityEngine.EventSystems;
using System;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField]    public GameObject unitPrefab=null;
    [SerializeField] public Transform unitSpawnPoint = null;
    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }
    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }
    [Server]
    private void ServerHandleDie()
    {
       // NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance= Instantiate(unitPrefab,unitSpawnPoint.position,unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    

    #endregion



    #region Client
    [ClientCallback]

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button !=PointerEventData.InputButton.Left ) { return; }
        if (!isOwned)
        {return;}
        CmdSpawnUnit();
    }


    #endregion
}
