using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomPortal : MonoBehaviour {
    public Vector3 offset;

    private int floor;

    void start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            floor = LevelGen.Instance.CurrentFloor;
            // Enables teirs of boss depending on floor
            if (floor == 0){
                GameObject.FindGameObjectWithTag("Boss").transform.Find("Teir1").gameObject.SetActive(true);
            }
            else if (floor == 1)
            {
                GameObject.FindGameObjectWithTag("Boss").transform.Find("Teir1").gameObject.SetActive(true);
                GameObject.FindGameObjectWithTag("Boss").transform.Find("Teir2").gameObject.SetActive(true);
            }
            else if (floor == 2)
            {
                GameObject.FindGameObjectWithTag("Boss").transform.Find("Teir1").gameObject.SetActive(true);
                GameObject.FindGameObjectWithTag("Boss").transform.Find("Teir2").gameObject.SetActive(true);
                GameObject.FindGameObjectWithTag("Boss").transform.Find("Teir3").gameObject.SetActive(true);
            }
            else if (floor == 3)
            {
                GameObject.FindGameObjectWithTag("Boss").transform.Find("Teir1").gameObject.SetActive(true);
                GameObject.FindGameObjectWithTag("Boss").transform.Find("Teir2").gameObject.SetActive(true);
                GameObject.FindGameObjectWithTag("Boss").transform.Find("Teir3").gameObject.SetActive(true);
                GameObject.FindGameObjectWithTag("Boss").transform.Find("Teir4").gameObject.SetActive(true);
            }
            else
            {
                GameObject.FindGameObjectWithTag("Boss").transform.Find("Boss").gameObject.SetActive(true);
                GameObject.Find("Music").GetComponent<AudioSource>().clip = GameObject.Find("Music").GetComponent<Music>().bossMusic;
            }

            // teleport to Far Away where the boss room is
            Vector3 somewhere = LevelGen.ReallyFarAway;
            somewhere.y = 1;

            // add offset to the whole thing
            somewhere.x += offset.x;
            somewhere.z += offset.z;

            other.transform.position = somewhere;
        }
    }
}
