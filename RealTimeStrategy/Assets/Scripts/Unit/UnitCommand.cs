using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommand : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler=null;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    private Camera mainCamera;
    void Start()
    {
        mainCamera= Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }
        Ray ray= mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if(!Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity,layerMask)) { return; }
        if(hit.collider.TryGetComponent<Targetable>(out Targetable targetable))
        {
            if(targetable.isOwned)
            {
                TryMove(hit.point);
                return;
            }
            TryTarget(targetable);
            return;
        }
        TryMove(hit.point);
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);

        }
    }

    private void TryMove(Vector3 point)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
           unit.GetUnitMovement().CmdMove(point);

        }
    }
}
