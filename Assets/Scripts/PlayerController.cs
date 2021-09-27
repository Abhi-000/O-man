using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PlayerController : MonoBehaviour
{
    public Animator player;
    public static PlayerController instance;
    // Start is called before the first frame update
    public CharacterController cc;
    public PathCreator pathCreator;
    public float speed =5;
    [HideInInspector]public float distanceTravelled,xPos;
    [HideInInspector]public bool startRunning,pause;
    [HideInInspector]public bool gameOver = false;
    Vector3 desiredPos,pos;
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            startRunning = false;
            player.SetBool("run", false);
        }
        if (Input.GetMouseButtonDown(0))
        {
            startRunning = true;
            xPos = Input.mousePosition.x;
            pos = transform.position;
        }
        if (Input.GetMouseButton(0) && !pause)
        {
            float xPosDiff = (xPos - Input.mousePosition.x) / Screen.width;
            xPosDiff *= -15;
            desiredPos = pos + new Vector3(xPosDiff, 0, 0);
            desiredPos.x = Mathf.Clamp(desiredPos.x, -4f, 4f);
            distanceTravelled += speed * Time.deltaTime;
            desiredPos.y =  pathCreator.path.GetPointAtDistance(distanceTravelled).y;
            desiredPos.z = pathCreator.path.GetPointAtDistance(distanceTravelled).z;
        }
        if (startRunning && !gameOver && !pause)
        {
            player.SetBool("run", true);
            cc.Move(new Vector3(desiredPos.x-transform.position.x,0, 8 * Time.deltaTime));
            transform.position = new Vector3(transform.position.x, desiredPos.y, desiredPos.z);
        }
    }
}
