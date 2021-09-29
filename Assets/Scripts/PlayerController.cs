using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PathCreation;

public class PlayerController : MonoBehaviour
{
    public Slider levelProgress;
    public Animator player;
    public static PlayerController instance;
    // Start is called before the first frame update
    public CharacterController cc;
    public PathCreator pathCreator;
    public float speed =5;
    public GameObject swipeToMove;
    [HideInInspector]public float distanceTravelled,xPos;
    [HideInInspector]public bool startRunning,pause;
    [HideInInspector] public bool gameOver = false, bonus = false;
    Vector3 desiredPos,pos;
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0) && !bonus)
        {
            startRunning = false;
            player.SetBool("run", false);
        }
        if (Input.GetMouseButtonDown(0))
        {
            levelProgress.gameObject.SetActive(true);
            swipeToMove.SetActive(false);
            startRunning = true;
            xPos = Input.mousePosition.x;
            pos = transform.position;
        }
        if (Input.GetMouseButton(0) && !pause || bonus)
        {
            if (!bonus)
            {
                float xPosDiff = (xPos - Input.mousePosition.x) / Screen.width;
                //for rotation
               /* newXPos = Mathf.Lerp(xPos, Input.mousePosition.x, 10*Time.deltaTime);
                Debug.Log(xPos -newXPos);
                if (xPosDiff < 0)
                {
                    Quaternion currentRotation = transform.GetChild(0).rotation;
                    Quaternion wantedRotation = Quaternion.Euler(0, 20, 0);
                    transform.GetChild(0).rotation = Quaternion.RotateTowards(currentRotation, wantedRotation, 10);
                }
                else if (xPosDiff > 0)
                {
                    Quaternion currentRotation = transform.GetChild(0).rotation;
                    Quaternion wantedRotation = Quaternion.Euler(0, -20, 0);
                    transform.GetChild(0).rotation = Quaternion.RotateTowards(currentRotation, wantedRotation, 10);
                }
                else
                {
                    Quaternion currentRotation = transform.GetChild(0).rotation;
                    Quaternion wantedRotation = Quaternion.Euler(0, 0, 0);
                    transform.GetChild(0).rotation = Quaternion.RotateTowards(currentRotation, wantedRotation, 10);
                }*/
                xPosDiff *= -15;
                desiredPos = pos + new Vector3(xPosDiff, 0, 0);
                
            }
            desiredPos.x = Mathf.Clamp(desiredPos.x, -4f, 4f);
            distanceTravelled += speed * Time.deltaTime;
            desiredPos.y =  pathCreator.path.GetPointAtDistance(distanceTravelled).y;
            desiredPos.z = pathCreator.path.GetPointAtDistance(distanceTravelled).z;
        }
        if (startRunning && !gameOver && !pause)
        {
            levelProgress.value = transform.position.z * 0.0055f;
            player.SetBool("run", true);
            cc.Move(new Vector3(desiredPos.x-transform.position.x,0, 8 * Time.deltaTime));
            transform.position = new Vector3(transform.position.x, desiredPos.y, desiredPos.z);
        }
    }
}
