using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building=null;//price and preview
    [SerializeField] private Image iconImage=null;//UI
    [SerializeField] private TMP_Text priceText=null;//TEXT
    [SerializeField] private LayerMask floorMask = new LayerMask();//Interaction between prefab and layer

    private Camera mainCamera;// raycasting
    private BoxCollider buildingCollider;
    private RTSPlayerScript player;//price 
    private GameObject buildingPreviewInstance;//preview for building
    private Renderer buildingRendererInstance;// red or green whether spawnable or not
   


    private void Start()
    {
        mainCamera = Camera.main;// for rycasting
        iconImage.sprite = building.GetIcon();
        priceText.text= building.GetPrice().ToString();// setting the price text
        buildingCollider = building.GetComponent<BoxCollider>();//to check if can place building
    }

    private void Update()
    {
        if (player== null)
        {
            player=NetworkClient.connection.identity.GetComponent<RTSPlayerScript>();// if to determine the player 
        }
        if(buildingPreviewInstance == null) { return; }//no prevview prefab then return
        UpdateBuildingPreview();// update preview alon with location of hit point method
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) { return; }

        if(player.GetResources() < building.GetPrice()) { return; }

        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());//instantiates the prefab at 0,0,0
        buildingRendererInstance= buildingPreviewInstance.GetComponentInChildren<Renderer>();//childeren of building component
        buildingPreviewInstance.SetActive(false);//since it spawns at 0,0,0
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(buildingPreviewInstance==null) { return; }
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());// raycasting from manicamer using mouse and converting the poin in screen to ray such that can be used for detection if it hits something
        if(Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity,floorMask)) //checks if the ray hits something at infinite distance from main camera on the floor mask layer
        {
            player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }
        Destroy(buildingPreviewInstance);
    }
    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) { return; }
        buildingPreviewInstance.transform.position= hit.point;
        if (!buildingPreviewInstance.activeSelf)//we make it active at desired position with first change in drag
        {
            buildingPreviewInstance.SetActive(true);    
        }
        
        Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;//check if can place building true then green else red
        buildingRendererInstance.material.SetColor("_BaseColor", color);
    }
}
