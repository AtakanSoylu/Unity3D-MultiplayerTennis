using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerCharacterColider : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }
    
    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            gameObject.GetComponentInParent<Player>().RpcHitBall(other.gameObject);
        }
    }
}
