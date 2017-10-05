using UnityEngine;
using System.Collections;

public static class GizmoDraw {
    public static void Circle(Vector3 center, float radius, float d_th = 0.1f) {
        System.Func<float, Vector3> pos = delegate (float th) {
            return center + new Vector3(radius * Mathf.Cos(th), 0,
                                        radius * Mathf.Sin(th));
        };

        for (float th = 0; th < Mathf.PI * 2; th += d_th) {
            Gizmos.DrawLine(pos(th), pos(th + d_th));
        }
    }

    public static void Ray(Ray ray, float length, Color color) {
        Debug.DrawRay(ray.origin, ray.direction * length, color);
    }
}
