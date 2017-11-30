using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour {

    public int roomsPerFloor;
    public GameObject spawnRoom;
    public GameObject chestRoom;
    public GameObject shopRoom;
    public GameObject enemyRoom1;
    public GameObject enemyRoom2;
    public GameObject enemyRoom3;
    public List<GameObject> nodes;

    private int roomCount = 0;
    private GameObject room;
    private float randNum;

    // Use this for initialization
    void Start () {
        newRoom();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void newRoom()
    {
        Instantiate(spawnRoom, nodes[0].transform.position, Quaternion.identity);
        while (roomCount < roomsPerFloor)
        {
            if (Random.Range(0f, 1f) > 0.5f && nodes[1].activeInHierarchy == true) //50% chance to pass
            {
                room = randomRoom(); //Sets room to be a random room to be instantiated
                Instantiate(room, nodes[1].transform.position, Quaternion.identity);
                roomCount++;
            }
            else if (Random.Range(0f, 1f) > 0.5f && nodes[2].activeInHierarchy == true) //50% chance to pass
            {
                room = randomRoom();
                Instantiate(room, nodes[2].transform.position, Quaternion.identity);
                roomCount++;
            }
            else if (Random.Range(0f, 1f) > 0.5f && nodes[3].activeInHierarchy == true) //50% chance to pass
            {
                room = randomRoom();
                Instantiate(room, nodes[3].transform.position, Quaternion.identity);
                roomCount++;
            }
            else if (Random.Range(0f, 1f) > 0.5f && nodes[4].activeInHierarchy == true) //50% chance to pass
            {
                room = randomRoom(); 
                Instantiate(room, nodes[4].transform.position, Quaternion.identity);
                roomCount++;
            }
        }
    }

    private GameObject randomRoom() //Returns a random room with higher chance of enemy room
    {
        randNum = Random.Range(0f, 1f);
        if (randNum >= 0 && randNum <= .09) //10% chance
        {
            return chestRoom;
        }
        else if (randNum >= .01 && randNum <= .24) //15% chance
        {
            return shopRoom;
        }
        else if (randNum >= .25 && randNum <= .49) //25% chance
        {
            return enemyRoom1;
        }
        else if (randNum >= .5 && randNum <= .74) //25% chance
        {
            return enemyRoom2;
        }
        else //25% chance
        {
            return enemyRoom3;
        }
    }
}
