using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Shooter {
    public bool showRange;

    float range = 10;
    float visionCone = 45;
    float facingDistanceScale = 1.5f;

    GameObject rangeMarker;

    /// <summary>
    /// Range of action.
    /// </summary>
    public float Range { get { return range; } }

    /// <summary>
    /// Cone of vision (theta = visionCone * 2, centered on transform.forward).
    /// </summary>
    public float VisionCone { get { return visionCone; } }

    /// <summary>
    /// How much farther the enemy can see if they're facing the target.
    /// </summary>
    public float FacingDistanceScale { get { return facingDistanceScale; } }

    protected override bool ShouldShoot {
        get {
            Vector3 target = GetTarget();
            Vector3 direction = target - transform.position;
            float targetDistance = direction.magnitude;

            float angle = Vector3.Angle(direction, transform.forward);

            if (Mathf.Abs(angle) < VisionCone) {
                targetDistance /= FacingDistanceScale;
            }

            bool inRange = targetDistance < Range;

            if (inRange) {
                transform.LookAt(target);
            }

            return inRange;
        }
    }

    protected override void Start() {
        base.Start();

        CreateRangeMarker();
    }

    protected override void Update() {
        base.Update();

        UpdateRangeMarker();
    }

    void CreateRangeMarker() {
        rangeMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        rangeMarker.transform.position = transform.position;
        rangeMarker.GetComponent<Collider>().isTrigger = true;

        Material rangeMaterial = rangeMarker.GetComponent<Renderer>().material;

        ActivateTransparency(rangeMaterial);

        // set color to translucent
        Color newColor = rangeMaterial.color;
        newColor.a = 0.3f;
        rangeMaterial.color = newColor;
    }

    /// <summary>
    /// Activates shader transparency for a Material.
    /// 
    /// From StandardShaderGUI.SetupMaterialWithBlendMode.
    /// </summary>
    /// <param name="mat">Material.</param>
    void ActivateTransparency(Material mat) {
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend",
                   (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    // Update is called once per frame
    void UpdateRangeMarker() {
        rangeMarker.transform.position = transform.position;

        rangeMarker.SetActive(showRange);

        Vector3 newScale = rangeMarker.transform.localScale;
        newScale = new Vector3(1, 0, 1) * Range * 2;
        newScale.y = 0.1f;

        rangeMarker.transform.localScale = newScale;
    }

    protected override Vector3 GetTarget() {
        return GameObject.FindGameObjectWithTag("Player")
                         .transform.position;
    }
}
