
using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayerScript : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private Building[] buildings= new Building[0];
    
    [SerializeField] private float buildingRangeLimit = 5.0f;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;//that buildings generate

    public event Action<int> ClientOnResourcesUpdated;//to tell the UI and let he client knw about the resources and change
    private Color teamColor = new Color();
    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();
    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }
    public int GetResources()//get resources to update the ui in the starting 
    {
        return resources;
    }
    

    public Color GetTeamColor() //getting team color
    { 
        return teamColor; 
    }   
    

       

    public List<Unit> GetMyUnits() { return myUnits; }
    public List<Building> GetMyBuildings() {  return myBuildings; }
    

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buildingCollider.center,
            buildingCollider.size / 2,
            Quaternion.identity,
            buildingBlockLayer))//checking whether thebuilding we are going to place collides with any building near by
        {
            return false;
        }
        
        foreach (Building building in myBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)//checking the range with the building
            {
                return true;
            }
        }
        return false;
    }

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
    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
       teamColor= newTeamColor;
    }
    [Server]//so that no cheating is done and the process happens in the server
    public void SetResources(int newResources)
    {
        resources = newResources;// everytime the method is called the resources are updated 
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

        if(resources< buildingToPlace.GetPrice()) { return; }//checking for the building price
        BoxCollider buildingCollider= buildingToPlace.GetComponent<BoxCollider>();
       
        if (!CanPlaceBuilding(buildingCollider,point)) { return; }
        GameObject buildingInstance =
           Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);//generating the gameobject that is to be networked
        NetworkServer.Spawn(buildingInstance, connectionToClient);//networking the gameobject to spawning server and giving authority to spawning client
        SetResources(resources- buildingToPlace.GetPrice() );

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
