using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Runtime.Remoting.Channels;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField]    public Unit unitPrefab=null;
    [SerializeField] public Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQueue = 5;// maximum unit that can be queued
    [SerializeField] private float spawnMoveRange = 7f;// offset for spawn;
    [SerializeField] private float unitSpawnDuration = 5.0f;//one unit spawn time;
    
    [SyncVar(hook =nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;// tell the all clients 
    [SyncVar]
    private float unitTimer;// tell the all clients

    private float progressImageVelocity;

    private void Update()
    {
        if(isServer)
        {
            ProduceUnits();
        }
        if(isClient)
        {
            UpdateTimerDisplay();
        }
    }

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
       NetworkServer.Destroy(gameObject);
    }
    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits == 0) { return; }
        unitTimer += Time.deltaTime;
        if (unitTimer < unitSpawnDuration) { return; }//time to spawn a unit that is queued is greater then return else spawn the unit
        GameObject unitInstance = Instantiate(unitPrefab.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
        Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere * spawnMoveRange;// to stop stacking of the tank we take a unit distance any random value and spawn 
        spawnOffset.y = unitSpawnPoint.position.y;// soawning only in smae height the height not change
        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position+ spawnOffset);//moves unit to certain distance so that unit not stack over each other
        queuedUnits--;
        unitTimer = 0;
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if(queuedUnits==maxUnitQueue) { return; }
        RTSPlayerScript player=connectionToClient.identity.GetComponent<RTSPlayerScript>();
        if(player.GetResources()<unitPrefab.GetResourcesCost()) { return; }
        queuedUnits++;
        player.SetResources(player.GetResources()-unitPrefab.GetResourcesCost());
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
    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        remainingUnitsText.text=newUnits.ToString();
    }
    private void UpdateTimerDisplay()//changing timer
    {
        float newProgress = unitTimer / unitSpawnDuration;
        if (newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        }
        else//to smotth the filling
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(
                unitProgressImage.fillAmount, 
                newProgress, 
                ref progressImageVelocity,
                0.1f);
        }
    }
    #endregion
}
