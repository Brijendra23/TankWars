using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Minimap : MonoBehaviour,IPointerDownHandler,IDragHandler
{
    [SerializeField] private RectTransform minimapRect = null;
    [SerializeField] private float mapScale = 20f;//square map
    [SerializeField] private float offset = -6f;
    private Transform playerCameraTransform;



    private void Update()
    {
        if(playerCameraTransform != null) { return; }
        if(NetworkClient.connection.identity==null) { return; }
        playerCameraTransform=NetworkClient.connection.identity.
            GetComponent<RTSPlayerScript>().GetCameraTransform();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }
    private void MoveCamera()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        // detecting the location of cursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimapRect,
            mousePos,
            null,
            out Vector2 localPoint))
        {
            return;
        }
        Vector2 lerp = new Vector2(
            (localPoint.x-minimapRect.rect.x)/minimapRect.rect.width,
            (localPoint.y-minimapRect.rect.y)/minimapRect.rect.height);//converting the output of pixel into vectors so that when scaling different map the pixels variate so this method will stop it from breaking
        Vector3 newCameraPos = new Vector3(Mathf.Lerp(-mapScale,mapScale,lerp.x),
            playerCameraTransform.position.y,
            Mathf.Lerp(-mapScale, mapScale, lerp.y));//pos in map with real coordinates

        //tell the camera to move
        playerCameraTransform.position = newCameraPos+new Vector3(0f,0f,offset);//setting camera offset
    }

    
}
