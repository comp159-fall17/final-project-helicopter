using UnityEngine;

public class SpawnDoors : MonoBehaviour {
    private GameObject door;
    private bool spawn = true;

    void OnTriggerEnter() {
        door = LevelGen.Instance.Door;
        if (spawn) {
            Instantiate(door, transform.position, transform.rotation, transform);
            spawn = false;
        }
    }
}
