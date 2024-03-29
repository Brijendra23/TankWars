using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Resources : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;

    private RTSPlayerScript player;
    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayerScript>();
        
        ClientHandleResourcesUpdated(player.GetResources());
        player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
    }
 
    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }
    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text= $"Resources: {resources}";
    }
}
