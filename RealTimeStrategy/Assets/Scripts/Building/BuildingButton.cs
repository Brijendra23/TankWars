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
    private RTSPlayerScript player;//price 
    private GameObject buildingPreviewInstance;//preview for building
    private Renderer buildingRendererInstance;// red or green whether spawnable or not


    private void Start()
    {
        mainCamera = Camera.main;
        iconImage.sprite = building.GetIcon();
        priceText.text= building.GetPrice().ToString();
    }

    public void Update()
    {
        if (player== null)
        {
            player=NetworkClient.connection.identity.GetComponent<RTSPlayerScript>();
        }
        if(buildingPreviewInstance == null) { return; }
        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) { return; }
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
            //placebuilding
        }
        Destroy(buildingPreviewInstance);
    }
    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) { return; }
        buildingPreviewInstance.transform.position= hit.point;
        if (!buildingPreviewInstance.activeSelf)//we make itt active at desired position with first change in drag
        {
            buildingPreviewInstance.SetActive(true);    
        }
    }
}
