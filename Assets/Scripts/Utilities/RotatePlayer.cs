using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayer : MonoBehaviour {
    PlayerControls PController;

    void Start() {
        GameObject player = GameManager.Instance.Player;
        if (player != null) {
            PController = player.GetComponent<PlayerControls>();
        }
    }

    void Update() {
        float rotation = PController != null ? PController.AbsoluteTargetAngle : 0;
        transform.rotation = Quaternion.Euler(new Vector3(0, rotation - 90, 0));
    }
}
