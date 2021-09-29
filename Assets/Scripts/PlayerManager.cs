using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public GameObject blob,losePanel,winPanel;
    GameObject blobOriginal;
    float Animation;
    Vector3 blobPos, playerPos;
    bool moveBlob, popBlob,hitPlayer;
    public int size= 0;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Cylinder"))
        {
            Quaternion currentRotation = transform.GetChild(0).rotation;
            Quaternion wantedRotation = Quaternion.Euler(0, 0, 0);
            transform.GetChild(0).rotation = wantedRotation;
            PlayerController.instance.speed = 4f;
            
            if(!other.transform.parent.CompareTag("Bonus"))
            Invoke(nameof(afterDelayStopGlide), 1.5f);
            Animation = 0;
            if (blobOriginal != null)
                blob = blobOriginal;
            blob.transform.GetComponent<BoxCollider>().enabled = false;
            blob.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.x);
            if (other.transform.GetComponent<CapsuleCollider>() != null)
            {
                other.transform.GetComponent<CapsuleCollider>().enabled = false;
            }
            if (size==other.GetComponent<Pipe>().size && !PlayerController.instance.bonus)
            {
                PlayerController.instance.player.SetBool("glide", true);
                blob.SetActive(true);
                blob.transform.parent = transform;
                blob.transform.localPosition = new Vector3(0, 0, 2);
                Invoke(nameof(afterDelayBounce), .8f);
            }
            else if(size< other.GetComponent<Pipe>().size)
            {
                if (!PlayerController.instance.bonus)
                {
                    PlayerController.instance.player.SetBool("fall", true);
                    //transform.position = new Vector3(transform.position.x, other.transform.position.y + 1, transform.position.z);
                    PlayerController.instance.gameOver = true;
                    Invoke(nameof(lose),2.5f);
                }
                else
                {
                    PlayerController.instance.bonus = false;
                    PlayerController.instance.startRunning = false;
                    PlayerController.instance.player.SetBool("glide", false);
                    PlayerController.instance.player.SetBool("run", false);
                    Invoke(nameof(win), 1f);
                }
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
            blobOriginal = blob;
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
            if (size != 0)
            {
                size--;
                transform.localScale -= new Vector3(0.2f, 0.2f, 0);
                PlayerController.instance.pathCreator.transform.position -= new Vector3(0, .5f, 0); ;
            }
            else
            {
                Invoke(nameof(lose),1.5f);
            }
            Invoke(nameof(afterDelayPlay), .3f);
        }
        else if(other.CompareTag("Bonus"))
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
            PlayerController.instance.pathCreator.transform.position =
                new Vector3(PlayerController.instance.pathCreator.transform.position.x, 8.65f, PlayerController.instance.pathCreator.transform.position.z);
            PlayerController.instance.bonus = true;
            PlayerController.instance.startRunning = true;
            PlayerController.instance.player.SetBool("glide", true);
            PlayerController.instance.player.SetBool("run", false);
        }
    }
    void afterDelayStopGlide()
    {
        PlayerController.instance.speed = 5f;
        PlayerController.instance.player.SetBool("glide", false);
        PlayerController.instance.player.SetBool("run", true);
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
                blob.transform.GetComponent<BoxCollider>().enabled = true;
                if (popBlob)
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
    void lose()
    {
        losePanel.SetActive(true);
    }
    void win()
    {
        winPanel.SetActive(true);
    }
    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
