using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter target = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeSelected = null;
    [SerializeField] private Health health = null;

    public static int AuthorityOnSpawned { get; internal set; }

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;
    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;
    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }
    public Targeter GetTargeter()
    {
        return target;
    }

    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
        ServerOnUnitSpawned?.Invoke(this);
    }
    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        ServerOnUnitDespawned?.Invoke(this);
    }
    [Server]
    void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        
        AuthorityOnUnitSpawned?.Invoke(this);
    }
    public override void OnStopClient()
    {
        if (!isOwned) { return; }
        AuthorityOnUnitDespawned?.Invoke(this);
    }
    [Client]
    public void Select()
    {
        if(!isOwned) return;    
        onSelected?.Invoke();
    }
    [Client]
    public void Deselect()
    {
        if (!isOwned) { return; }
        onDeSelected?.Invoke();
    }

    #endregion
}
