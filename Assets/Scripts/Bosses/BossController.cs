using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : Shooter {
    protected override bool ShouldShoot { get { return false; } } 
    protected override bool ShouldShootSpecial { get { return false; } }
    protected override bool WallInWay { get { return false; } }
    protected override Vector3 Target { get { return transform.forward; } }

    protected override void Die() {
        GameManager.Instance.PlayDeathSound();
        Destroy(gameObject);
    }
}
