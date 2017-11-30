﻿using System.Collections;
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
    public GameObject door;
    public List<GameObject> nodes;
    public GameObject nodeParent;

    private List<GameObject> rooms = new List<GameObject>();
    private GameObject temp;
    private GameObject room;
    private float randNum;
    private int roomCount = 0;
    private int loopCount = 0;
    private int chestRoomCount = 0;
    private int shopRoomCount = 0;

    // Use this for initialization
    void Start () {
        newFloor();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void newFloor() //Generates the new floor by createing rooms next to the spawn room by chance
    {
        Instantiate(spawnRoom, nodes[0].transform.position, Quaternion.identity);
        while (roomCount < roomsPerFloor)
        {
            loopCount++;
            Debug.Log(nodes[3].activeInHierarchy);
            if (Random.Range(0f, 1f) > 0.5f && nodes[loopCount].activeInHierarchy == true) //50% chance to pass
            {
                room = randomRoom(); //Sets room to be a random room to be instantiated
                temp = Instantiate(room, nodes[loopCount].transform.position, Quaternion.identity) as GameObject;
                rooms.Add(temp);
                roomCount++;
                Debug.Log("spawn");
                nodes[loopCount].SetActive(false);
            }
            if (roomCount == 0 && loopCount == 4)
                loopCount = 0;
            if (loopCount == 4) //Every 4 iterations in the while loop
                moveNodes();
        }
        //Make boss room spawn as last room
    }

    private void moveNodes() //Moves nodes to be centered on the next room in the list
    {
        for (int i = 0; i <= 4; i++)
        {
            nodes[i].SetActive(true); //This loop and seting active above are not working because happens too fast for colliders
        }
        loopCount = 0;
        nodeParent.transform.position = rooms[0].transform.position; //Moves nodes to center on next room
        rooms.RemoveAt(0); //Removes that room from list
        Debug.Log("now");
    }

    private GameObject randomRoom() //Returns a random room with higher chance of enemy room
    {
        randNum = Random.Range(0f, 1f);
        if (randNum >= 0 && randNum <= .09 && chestRoomCount == 0) //10% chance and once per floor
        {
            chestRoomCount++;
            return chestRoom;
        }
        else if (randNum >= .01 && randNum <= .24 && shopRoomCount == 0) //15% chance and once per floor
        {
            shopRoomCount++;
            return shopRoom;
        }
        else //75% chance
        {
            randNum = Random.Range(0f, 1f);
            if (randNum >= 0 && randNum <= .33) //33% chance
            {
                return enemyRoom1;
            }
            else if (randNum >= .34 && randNum <= .66) //33% chance
            {
                return enemyRoom2;
            }
            else //33% chance
            {
                return enemyRoom3;
            }
        }
    }
}
