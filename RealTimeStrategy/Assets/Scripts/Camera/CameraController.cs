using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;// to move the camera using mouse when the mouse goes to corners of the screen
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;//camera movement limits


    private Vector2 previousInput;

    private Controls controls;//new input system control reference
    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);

        controls = new Controls();//new control/ input system that we made
        controls.Player.MoveCamera.performed += SetPreviousInput;//when the action or inputis pressed and kepton pressed
        controls.Player.MoveCamera.canceled += SetPreviousInput; //when the action/input in released

        controls.Enable();
    }//these events need not to be unsbscribed since they handled by unity

    [ClientCallback]

    private void Update()
    {
        if(!isOwned||!Application.isFocused)//checks the authority and whether the application is focused or in background
        {
            return;
        }
        UpdateCameraPosition();

    }
    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCameraTransform.position;
        if(previousInput==Vector2.zero)// if no input is given
        {
            Vector3 cursorMovement = Vector3.zero;
            Vector2 cursorPosition = Mouse.current.position.ReadValue();//getting the mouse current posiiton ot check whether they out of borders to move the camera
            if(cursorPosition.y>=Screen.height-screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            if(cursorPosition.y<=screenBorderThickness)
            {
                cursorMovement.z-= 1;
            }
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }
            pos += cursorMovement.normalized * speed * Time.deltaTime;//modifying cam pos according to cursor 
        }
        else
        {
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * Time.deltaTime * speed;//movement with input

        }
        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenZLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);
        playerCameraTransform.position = pos;
    }
    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();//this ctx not know what value is coming but we set it as in input sytem the vector2 we know vector 2 is coming
                                                 //and the values needed to be set according to input
    }

}
