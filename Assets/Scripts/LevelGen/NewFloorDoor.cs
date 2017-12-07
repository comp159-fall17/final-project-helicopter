using UnityEngine;

public class NewFloorDoor : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            LevelGen.Instance.ReloadFloor(true);
        }
    }
}
