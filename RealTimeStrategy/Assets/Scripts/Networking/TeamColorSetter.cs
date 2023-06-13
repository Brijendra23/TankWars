using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];//list of renderers that needs to be changed in a prefab according to team color

    [SyncVar(hook =nameof(HandleTeamColorUpdated))]
    private Color teamColor = new Color();// server gives different team color this variable syncs tjhe color of the team according to server
    #region Server
    public override void OnStartServer()
    {
        RTSPlayerScript player=connectionToClient.identity.GetComponent<RTSPlayerScript>();
        teamColor=player.GetTeamColor();// getting the players team color when it joins the server
    }
    #endregion

    #region Client
    private void HandleTeamColorUpdated(Color oldColor,Color newColor)// updating the color of the prefab renderers according to team color
    {
        foreach(Renderer renderer in colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }



    #endregion

}
