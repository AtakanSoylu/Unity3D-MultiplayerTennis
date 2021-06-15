using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotScript : MonoBehaviour
{
    GameObject MainPlayer;
    Animator anim;
    public GameObject ball;
    public Transform ballExitPositon;
    float playerSpeed = 5f;
    float height = 2f;
    void Start()
    {
        anim = GetComponent<Animator>();
        MainPlayer = GameObject.FindGameObjectWithTag("PlayerMain");
    }

    void Update()
    {
        if (transform.position.z>ball.transform.position.z+0.2f)
        {
            transform.Translate(new Vector3(0, 0, 1) * playerSpeed * Time.deltaTime);
        }else if (transform.position.z+0.2f < ball.transform.position.z)
        {
            transform.Translate(new Vector3(0, 0, -1) * playerSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            
            HitBall();

        }
    }
    Vector3 LaunchVelocity()
    {
        Vector3 target = new Vector3(-7.15f, 0.094f, Random.Range(9.5f, 17.7f));
        float displacementY = target.y - ball.transform.position.y;
        Vector3 displacementXZ = new Vector3(target.x - ball.transform.position.x, 0, target.z - ball.transform.position.z);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * GameController.gravity * (height - ballExitPositon.position.y));
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * (height - ballExitPositon.position.y) / GameController.gravity) + Mathf.Sqrt(-2 * height / GameController.gravity));
        return velocityXZ + velocityY;
    }
    void HitBall()
    {
        GameController.lastHitPlayerOne = false;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Transform>().position = ballExitPositon.GetComponent<Transform>().position;

        if (ball.GetComponent<Rigidbody>().useGravity == false)
            ball.GetComponent<Rigidbody>().useGravity = true;

        if (transform.position.z < ball.transform.position.z)
            anim.Play("ShootRightAnim");
        else
            anim.Play("ShootLeftAnim");

        Physics.gravity = Vector3.up * GameController.gravity;
        ball.GetComponent<Rigidbody>().velocity = LaunchVelocity();
        GameController.gravity = GameController.gravity - 0.5f;
    }
}
