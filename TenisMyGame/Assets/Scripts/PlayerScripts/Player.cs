using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : NetworkBehaviour
{
    Animator anim;

    Rigidbody playerPhysics;
    float playerSpeed = 3f;

    private GameObject target;
    public GameObject ballPrefab;
    private GameObject player;
    public Transform ballExitPosition;
    [SerializeField]
    public Transform serveExitPosition;
    private Camera mainCam;
    float horizontal, vertical;
    float height= 2f;
    float hitSpeed = 6f;
    Vector3 coliderDistance;

    
    [SyncVar]
    public bool canServe;
    [SyncVar]
    public int thisPlayer;
    [SyncVar]
    public int score;
    [SyncVar]
    public bool lastHit;
 
    void Awake()
    {
        playerPhysics = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        target = transform.Find("Target").gameObject;

        mainCam = transform.Find("Camera").GetComponent<Camera>();
        mainCam.gameObject.SetActive(false);

        player = transform.Find("Character").gameObject;

    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (!mainCam.gameObject.activeInHierarchy)
            {
                mainCam.gameObject.SetActive(true);
            }
        }
        if (!isLocalPlayer)
        {

            return;
        }
        TargetMove();
        if (canServe)
        {
            if (Input.GetKeyDown(KeyCode.Space)) { 
                CmdServe();
            }
        }
        
        //        if (GameController.isGameReady)
        
    }
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            CharMove();
        }
    }

    void CharMove()
    {
        horizontal = Input.GetAxisRaw("Horizontal"); 
        if(horizontal == 1)
        {
            player.transform.Translate(new Vector3(0, 0, horizontal) * playerSpeed * Time.deltaTime);
            anim.SetBool("isLeftMove", true);
            anim.SetBool("isRightMove", false);
        }
        else if (horizontal == -1)
        {
            player.transform.Translate(new Vector3(0, 0, horizontal) * playerSpeed*Time.deltaTime);
            anim.SetBool("isLeftMove", false);
            anim.SetBool("isRightMove", true);
        }
        else
        {
            anim.SetBool("isLeftMove", false);
            anim.SetBool("isRightMove", false);
        }
       
    }

    void TargetMove()
    {
        if (Input.GetKey(KeyCode.N))
        {
            target.transform.Translate(new Vector3(0, 0, 1) * playerSpeed * Time.deltaTime);
        }else if (Input.GetKey(KeyCode.M)){
            target.transform.Translate(new Vector3(0, 0, -1) * playerSpeed * Time.deltaTime);
        }
    }
    

    
    [Command]
    void CmdServe()
    {
        canServe = false;
        RpcServeHit(); 
        
    }
    [ClientRpc]
    public void RpcServeHit()
    {
        lastHit = true;
        anim.SetTrigger("serve");
        anim.SetFloat("HitSpeed", 6f);
        hitSpeed = 6f;
        Physics.gravity = Vector3.up * GameController.gravity;
        StartCoroutine(ServeEnumrator());
    }
   
    IEnumerator ServeEnumrator()
    {
        yield return new WaitForSeconds(1);
        if (isServer)
        {
            GameObject ball = Instantiate(ballPrefab, serveExitPosition.position, player.transform.rotation);
            NetworkServer.Spawn(ball);
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;//Hizi sifirlar
            ball.GetComponent<Ball>().lastPlayer = gameObject;//Topa vuran oyuncuyu belirler
            ball.transform.position = ballExitPosition.position;
            ball.GetComponent<Rigidbody>().velocity = LaunchVelocity(ball);
            GameController.gravity = -9f;
        }
    }
    //Topun gidecegi dogrultu ve hizi bulur.
    Vector3 LaunchVelocity(GameObject ball)
    {
        //float displacementY = target.transform.position.y - ball.transform.position.y;
        Vector3 displacementXZ = new Vector3(target.transform.position.x - ball.transform.position.x, 0, target.transform.position.z - ball.transform.position.z);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * GameController.gravity * (height - ballExitPosition.position.y));
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * (height - ballExitPosition.position.y) / GameController.gravity) + Mathf.Sqrt(-2 * height / GameController.gravity));
        return velocityXZ + velocityY;
    }
    //Client tabanli topa vurus fonksiyonu
    [ClientRpc]
    public void RpcHitBall(GameObject ball)
    {
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;//Hizi sifirlar
        ball.GetComponent<Ball>().lastPlayer = gameObject;//Topa vuran oyuncuyu belirler
        lastHit = true;
        anim.SetTrigger("hit");
        anim.SetFloat("HitSpeed", hitSpeed);
        hitSpeed++;
        /*if (thisPlayer == 1)//Animasyonlari baslatir
        {
            if (player.transform.position.z > ball.transform.position.z)
                anim.Play("ShootRightAnim");
            else
                anim.Play("ShootLeftAnim");
        }else
        {
            if (transform.position.z < ball.transform.position.z)
                anim.Play("ShootRightAnim");
            else
                anim.Play("ShootLeftAnim");
        }*/
        ball.transform.position = ballExitPosition.position;
        Physics.gravity = Vector3.up * GameController.gravity;
        ball.GetComponent<Rigidbody>().velocity = LaunchVelocity(ball);
        GameController.gravity = GameController.gravity - 0.5f;

    }

}
