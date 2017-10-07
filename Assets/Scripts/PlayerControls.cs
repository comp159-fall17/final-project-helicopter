using UnityEngine;

/// <summary>
/// Controller for players. Should only have as many instantiated as there are players.
/// </summary>
public class PlayerControls : Shooter {
    protected override bool ShouldShoot {
        get {
            bool wallInWay = !Physics.Raycast(transform.position,
                                              BulletSpawnPoint - transform.position,
                                              2, ~(1 << LayerMask.NameToLayer("Player")));
            return Input.GetMouseButton(0) && wallInWay;
        }
    }

    protected override Vector3 Target {
        get {
            // Raycast to corresponding point on screen.
            Ray viewRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(viewRay, out hit);

            return CopyY(hit.point, Body.position);
        }
    }

    Vector3 inputAxes;

    Camera follow;

    protected override void Start() {
        base.Start();

        follow = Camera.main;
        inputAxes = new Vector3(0, 0, 0);
    }

    protected override void Update() {
        base.Update();

        UpdateInputAxes();
        TrackCamera();
    }

    void UpdateInputAxes() {
        inputAxes.x = Input.GetAxis("Horizontal");
        //inputAxes.y;
        inputAxes.z = Input.GetAxis("Vertical");

        inputAxes *= Speed;
    }

    void TrackCamera() {
        follow.transform.position = CopyY(transform.position,
                                          follow.transform.position);
    }

    void FixedUpdate() {
        Body.velocity = CopyY(inputAxes, Body.velocity);
    }

    /// <summary>
    /// Creates a copy of v with w.y substituted.
    /// </summary>
    /// <returns>"Copied" vector.</returns>
    /// <param name="to">Destination.</param>
    /// <param name="from">Source.</param>
    Vector3 CopyY(Vector3 to, Vector3 from) {
        to.y = from.y;
        return to;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Contains("Pickup")) {
            Debug.Log("Picked up " + other.gameObject.tag); //for now, just testing the collecting of a pickup
            Destroy(other.gameObject);
        }
    }
}
