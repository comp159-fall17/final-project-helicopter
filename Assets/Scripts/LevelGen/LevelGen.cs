using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour {

    public int roomsPerFloor;
    public List<GameObject> spawnRoom;
    public List<GameObject> chestRoom;
    public List<GameObject> shopRoom;
    public List<GameObject> enemyRoom1;
    public List<GameObject> enemyRoom2;
    public List<GameObject> enemyRoom3;
    public GameObject door;
    public List<GameObject> bossRoom;
    public List<GameObject> nodes;
    public GameObject nodeParent;
    public float roomRate;
    public int listLen;

    private GameObject[] rooms;
    private List<GameObject> tempRooms = new List<GameObject>();
    private GameObject temp;
    private GameObject room;
    private float randNum;
    private int totalRoomCount = 0;
    private int roomCount = 0;
    private int loopCount = 0;
    private int chestRoomCount = 0;
    private int shopRoomCount = 0;
    private int roomType = 0;

    void Start () {
        newFloor();
        //Invoke("removeFloor", 3);
        //Invoke("newFloor", 6);
	}
	
	void Update () { 

	}

    private void resetFloor()
    {
        nodeParent.transform.position = new Vector3(0, 0, 0);
        tempRooms = new List<GameObject>();
        totalRoomCount = 0;
        roomCount = 0;
        loopCount = 0;
        chestRoomCount = 0;
        shopRoomCount = 0;
    }

    private void newFloor() //Generates the new floor by createing rooms next to the spawn room by chance
    {
        resetFloor();
        roomType = Random.Range(0, listLen);
        Instantiate(spawnRoom[roomType], nodes[0].transform.position, Quaternion.identity);
        while (totalRoomCount < roomsPerFloor)
        {
            loopCount++;
            if (Random.Range(0f, 1f) > roomRate && checkPos(loopCount) == true)
            {
                if (roomsPerFloor - totalRoomCount == 1)
                    temp = Instantiate(bossRoom[roomType], nodes[loopCount].transform.position, Quaternion.identity) as GameObject;
                else
                {
                    room = randomRoom(roomType); //Sets room to be a random room to be instantiated
                    temp = Instantiate(room, nodes[loopCount].transform.position, Quaternion.identity) as GameObject;
                }
                tempRooms.Add(temp);
                roomCount++;
                totalRoomCount++;
                nodes[loopCount].SetActive(false);
            }
            if (roomCount == 0 && loopCount == 4)
                loopCount = 0;
            if (loopCount == 4) { //Every 4 iterations in the while loop
                loopCount = 0;
                roomCount = 0;
                moveNodes();
            }
        }
        //Make boss room spawn as last room
    }

    private bool checkPos(int i)
    {
        rooms = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject room in rooms)
        {
            if (i == 1 && room.transform.position == nodes[1].transform.position)
            {
                return false;
            }
            else if (i == 2 && room.transform.position == nodes[2].transform.position)
            {
                return false;
            }
            else if (i == 3 && room.transform.position == nodes[3].transform.position)
            {
                return false;
            }
            else if (i == 4 && room.transform.position == nodes[4].transform.position)
            {
                return false;
            }
        }
        return true;
    }

    private void moveNodes() //Moves nodes to be centered on the next room in the list
    {
        for (int i = 0; i <= 4; i++)
        {
            nodes[i].SetActive(true); //This loop and seting active above are not working because happens too fast for colliders
        }
        nodeParent.transform.position = tempRooms[0].transform.position; //Moves nodes to center on next room
        tempRooms.RemoveAt(0); //Removes that room from list
    }

    private GameObject randomRoom(int roomType) //Returns a random room with higher chance of enemy room
    {
        randNum = Random.Range(0f, 1f);
        if (randNum >= 0 && randNum <= .09 && chestRoomCount == 0) //10% chance and once per floor
        {
            chestRoomCount++;
            return chestRoom[roomType];
        }
        else if (randNum >= .01 && randNum <= .24 && shopRoomCount == 0) //15% chance and once per floor
        {
            shopRoomCount++;
            return shopRoom[roomType];
        }
        else //75% chance
        {
            randNum = Random.Range(0f, 1f);
            if (randNum >= 0 && randNum <= .33) //33% chance
            {
                return enemyRoom1[roomType];
            }
            else if (randNum >= .34 && randNum <= .66) //33% chance
            {
                return enemyRoom2[roomType];
            }
            else //33% chance
            {
                return enemyRoom3[roomType];
            }
        }
    }

    private GameObject[] allRooms;

    private void removeFloor()
    {
        allRooms = GameObject.FindGameObjectsWithTag("Room");
        foreach (var item in allRooms)
        {
            Destroy(item);
        }
    }
}
