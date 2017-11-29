using System.Linq;
using UnityEngine;

public class RingWeapon : SpecialWeapon {
    public GameObject ring;

    void Start() {
        Expand();
    }

    void Expand() {
        //expand a circle around the player

        Instantiate(ring, transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
    }
}
