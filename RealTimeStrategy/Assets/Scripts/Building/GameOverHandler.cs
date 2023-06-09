using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameOver;
    private List<UnitBase> bases= new List<UnitBase>();
    public static event Action<string> ClientOnGameOver;
    
    #region Server
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }
    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }
    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }
    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);
        if(bases.Count!=1) { return; }
        //playerid that has less then 1 bases left there connection id is stored
        int playerId = bases[0].connectionToClient.connectionId;
        RpcGameOver($"Player {playerId}");
        ServerOnGameOver?.Invoke();

    }

    #endregion

    #region Client
    //server uses Rpc to call function on clients
    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        //invoking event to use UI for gameOver
        ClientOnGameOver?.Invoke(winner);
    }
    #endregion
}
