using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using UnityEngine;

public class RTSPlayerScript : NetworkBehaviour
{
    [SerializeField] private Building[] buildings= new Building[0];

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;//that buildings generate
    public int GetResources()//get resources to update the ui in the starting 
    {
        return resources;
    }
    [Server]//so that no cheating is done and the process happens in the server
    public void SetResources(int newResources)
    {
        resources= newResources;// everytime the method is called the resources are updated 
    }

    public event Action<int> ClientOnResourcesUpdated;//to tell the UI and let he client knw about the resources and change
    

    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();   

    public List<Unit> GetMyUnits() { return myUnits; }
    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }
    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)// asking server to spawn in every client
    {
        Building buildingToPlace = null;
        foreach(Building building in buildings)//checking for particular id building to network
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }
        if(buildingToPlace == null) { return; } //invalid index or building
        GameObject buildingInstance =
           Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);//generating the gameobject that is to be networked
        NetworkServer.Spawn(buildingInstance, connectionToClient);//networking the gameobject to spawning server and giving authority to spawning client


    }
    private void ServerHandleUnitSpawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId!=connectionToClient.connectionId) { return; }
        myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myUnits.Remove(unit);
    }
    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myBuildings.Remove(building);
    }
    #endregion


    #region Client
    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return; }
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;

    }
    public override void OnStopClient()
    {
     
        if (!isClientOnly||!isOwned) { return; }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }
    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);//lets the UI know that resources are updated
    }
    private void AuthorityHandleUnitSpawned(Unit unit)
    {
       myUnits.Add(unit);
    }
    private void AuthorityHandleUnitDespawned(Unit unit)
    {
       myUnits.Remove(unit);
    }
    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }
    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }



    #endregion



}
