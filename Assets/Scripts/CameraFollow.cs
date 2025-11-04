using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speed;

    private Rigidbody2D playerRb;

    [HideInInspector] public float offset;
    public float maxOffset;
    public float minX, maxX;
    public float orthographicSizeAfterWin = 2.5f;


    private void Start ()
    {
        offset = 0;
        playerRb = player.GetComponent<Rigidbody2D>();

        gameObject.transform.position = new Vector3 (player.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);

    }

    private void Update ()
    {
        if(playerRb.linearVelocity.x > 0)
        {
            offset += Time.deltaTime * speed;
            if(offset > maxOffset)
            {
                offset = maxOffset;    
            }
            
        }
        else if(playerRb.linearVelocity.x < 0)
        {
            offset -= Time.deltaTime * speed;
            if(offset < -maxOffset)
            {
                offset = -maxOffset;
            }
        }

        float nextX = player.transform.position.x + offset;
        if(nextX < minX) nextX = minX;
        if(nextX > maxX) nextX = maxX;

        gameObject.transform.position = new Vector3 (nextX, gameObject.transform.position.y, gameObject.transform.position.z);
    }
    // called in game manager
    public void Win ()
    {
        maxOffset = 0;
        gameObject.transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, gameObject.transform.position.z);
        Camera cam = GetComponent<Camera>();
        cam.orthographicSize /= orthographicSizeAfterWin;
    }

}