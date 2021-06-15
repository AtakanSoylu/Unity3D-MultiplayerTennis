using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class AutoHostScript : MonoBehaviour
{
    [SerializeField] NetworkManagerTennis networkManager;

    
    public void JoinLocal()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }
}
