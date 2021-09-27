using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject blob;
    float Animation;
    Vector3 blobPos, playerPos;
    bool moveBlob, popBlob;
    /* private void Start()
     {
         blobPos = blob.transform.position;
     }*/
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Cylinder"))
        {
            Animation = 0;
            blob.transform.GetComponent<BoxCollider>().enabled = false;
            blob.transform.localScale = new Vector3(1, 1, 1);
            other.transform.GetComponent<CapsuleCollider>().enabled = false;
            if ((transform.localScale.x == 1 && other.GetComponent<Pipe>().size == 0) ||
                (transform.localScale.x == 1.2f && other.GetComponent<Pipe>().size == 1) ||
                (transform.localScale.x == 1.4f && other.GetComponent<Pipe>().size == 2))
            {
                Debug.Log("BLOB");
                blob.SetActive(true);
                blob.transform.parent = transform;
                blob.transform.localPosition = new Vector3(0, 0, 2);
                Invoke(nameof(afterDelayBounce), .8f);
            }
            else
            {
                PlayerController.instance.player.SetBool("fall", true);
                transform.position = new Vector3(transform.position.x, other.transform.position.y + 1, transform.position.z);
                PlayerController.instance.gameOver = true;
            }
        }
        else if (other.CompareTag("Blob") && !popBlob)
        {
            PlayerController.instance.pathCreator.transform.position += new Vector3(0, .8f, 0);
            transform.localScale += new Vector3(0.2f, 0.2f, 0);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Obstacle"))
        {
            other.GetComponent<BoxCollider>().enabled = false;
            Animation = 0;
            blob = Instantiate(blob);
            blob.transform.GetComponent<BoxCollider>().enabled = false;
            blob.SetActive(true);
            blob.transform.parent = transform;
            blob.transform.localScale = new Vector3(1, 1, 1);
            blob.transform.localPosition = new Vector3(0, 0, 2);
            PlayerController.instance.pause = true;
            PlayerController.instance.startRunning = false;
            transform.position -= new Vector3(0, 0, 2.5f);
            blobPos = blob.transform.position;
            moveBlob = true;
            popBlob = true;
        }
    }
    void afterDelayBounce()
    {
        blobPos = blob.transform.position;
        moveBlob = true;
    }
    private void Update()
    {
        if (moveBlob)
        {
            blob.transform.GetComponent<BoxCollider>().enabled = true;
            blob.transform.localScale = new Vector3(0.5f, 0.5f, 0.6f);
            Animation += Time.deltaTime;
            Animation = Animation % 1.5f;
            blob.transform.parent = null;
            if (!popBlob)
                blob.transform.position = MathParabola.Parabola(blobPos, blobPos + Vector3.forward * 15f, 5f, Animation / 1.5f);
            else
                blob.transform.position = MathParabola.Parabola(blobPos, blobPos + Vector3.right * 2f, 5f, Animation / 1.5f);
            if (Animation >= 1.4f)
            {
                Debug.Log("Stop");
                moveBlob = false;
                popBlob = false;
                blob.transform.position = new Vector3(blob.transform.position.x, 8, blob.transform.position.z);
                blob.transform.GetComponent<BoxCollider>().enabled = true;
            }
        }

    }
}
