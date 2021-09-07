using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayDieCutscene : MonoBehaviour
{
    public GameObject videoPlayer;
    [SerializeField] int stopTime;


    float videoStartTime;
    bool collided;


    void Start()
    {
        videoPlayer.SetActive(false);
    }

    private void Update()
    {
        if (collided && Time.time - videoStartTime >= stopTime)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "PlayerCar")
        {
            //idsable player so no more interactions are possible and play cutscene video 
            player.enabled = false;
            videoPlayer.SetActive(true);
            //stop video after provided time and load next scene
            Destroy(videoPlayer, stopTime);
            videoStartTime = Time.time;
            collided = true;
        }
    }
}
