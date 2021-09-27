using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject blob;
    float Animation;
    Vector3 blobPos, playerPos;
    bool moveBlob, popBlob,hitPlayer;
    public int size= 0;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Cylinder"))
        {
            Animation = 0;
            blob.transform.GetComponent<BoxCollider>().enabled = false;
            blob.transform.localScale = new Vector3(1, 1, 1);
            other.transform.GetComponent<CapsuleCollider>().enabled = false;
            if ((size ==0 && other.GetComponent<Pipe>().size == 0) ||
                (size == 1 && other.GetComponent<Pipe>().size == 1) ||
                (size == 2 && other.GetComponent<Pipe>().size == 2))
            {
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
            PlayerController.instance.pathCreator.transform.position += new Vector3(0, .5f, 0);
            transform.localScale += new Vector3(0.2f, 0.2f, 0);
            size++;
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
            blob.transform.localPosition = Vector3.zero;
            PlayerController.instance.pause = true;
            playerPos = transform.position;
            transform.position -= new Vector3(0, 0, 8f);
            blobPos = blob.transform.position;
            moveBlob = true;
            popBlob = true;
            if(size !=0)
            transform.localScale -= new Vector3(0.2f, 0.2f, 0);
            Invoke(nameof(afterDelayPlay), .3f);
        }
    }
    void afterDelayPlay()
    {
        PlayerController.instance.distanceTravelled -= 8f;
        PlayerController.instance.pause = false;
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
            {
                blob.transform.localScale = new Vector3(0.5f, 0.5f, 0.3f);
                blob.transform.position = MathParabola.Parabola(blobPos, blobPos + Vector3.right * 3f, 3f, Animation / 1.5f);
            }
            if (Animation >= 1.4f)
            {
                Debug.Log("Stop");
                if(popBlob)
                    blob.transform.position = new Vector3(blob.transform.position.x, transform.position.y-2.5f, blob.transform.position.z);
                else blob.transform.position = new Vector3(blob.transform.position.x, transform.position.y, blob.transform.position.z);
                moveBlob = false;
                popBlob = false;
                blob.transform.GetComponent<BoxCollider>().enabled = true;
            }
        }
        else if(hitPlayer)
        {
            transform.position = Vector3.Lerp(playerPos, playerPos - new Vector3(0, 0, 4), 2f * Time.deltaTime);
        }

    }
}
