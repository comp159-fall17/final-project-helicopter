﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : Shooter {
    protected override bool StartShooting {
        get {
            return Input.GetMouseButtonDown(0);
        }
    }

    protected override bool StopShooting {
        get {
            return !Input.GetMouseButton(0);
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

        updateInputAxes();
        TrackCamera();
    }

    void updateInputAxes() {
        inputAxes.x = Input.GetAxis("Horizontal");
        //inputAxes.y;
        inputAxes.z = Input.GetAxis("Vertical");

        inputAxes *= walkSpeed;
    }

    void TrackCamera() {
        follow.transform.position = CopyY(transform.position,
                                         follow.transform.position);
    }

    void FixedUpdate() {
        body.velocity = CopyY(inputAxes, body.velocity);
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

    protected override Vector3 GetTarget() {
        // Raycast to corresponding point on screen.
        Ray viewRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(viewRay, out hit);

        return hit.point;
    }
}
