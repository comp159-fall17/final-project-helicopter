using UnityEngine;
using System.Collections;

public static class GizmoDraw {
    public static void Circle(Vector3 center, float radius, Color color,
                              float d_th = 0.1f) {
        Arc(center, radius, 0, Mathf.PI * 2, color, d_th);
    }

    public static void Circle(Vector3 center, float radius, float d_th = 0.1f) {
        Circle(center, radius, Color.white, d_th); 
    }

    public static void Arc(Vector3 center, float radius, float begin,
                             float end, Color color, float d_th = 0.1f) {
        System.Func<float, Vector3> pos = delegate (float th) {
            return center + new Vector3(radius * Mathf.Cos(th), 0,
                                        radius * Mathf.Sin(th));
        };

        if (begin > end) {
            float temp = begin;
            begin = end;
            end = temp;
        }

        for (float th = begin; th < end; th += d_th) {
            Debug.DrawLine(pos(th), pos(Mathf.Min(th + d_th, end)), color);
        }
    }

    public static void Arc(Vector3 center, float radius, float begin,
                             float end, float d_th = 0.1f) {
        Arc(center, radius, begin, end, Color.white, d_th);
    }

    public static void Wedge(Vector3 center, float radius, float wedgeAngle,
                             float centerAngle, Color color,
                             float d_th = 0.1f) {
        System.Func<float, float> toUnitCircle = delegate (float deg) {
            return -deg * Mathf.Deg2Rad + Mathf.PI / 2;
        };

        float begin = centerAngle - wedgeAngle / 2;
        float end = centerAngle + wedgeAngle / 2;

        Arc(center, radius, toUnitCircle(begin), toUnitCircle(end), color, d_th);

        Vector3 beginPoint = Quaternion.AngleAxis(begin, Vector3.up) * Vector3.forward;
        Vector3 endPoint = Quaternion.AngleAxis(end, Vector3.up) * Vector3.forward;

        Debug.DrawRay(center, beginPoint * radius, color);
        Debug.DrawRay(center, endPoint * radius, color);
    }

    public static void Wedge(Vector3 center, float radius, float wedgeAngle,
                            float centerAngle, float d_th = 0.1f) {
        Wedge(center, radius, wedgeAngle, centerAngle, Color.white, d_th);
    }

    public static void Wedge(Vector3 center, float radius, float wedgeAngle,
                             Vector3 forwardVector, Color color,
                             float d_th = 0.1f) {
        float centerAngle = Vector3.Angle(Vector3.forward, forwardVector);
        centerAngle *= Mathf.Sign(forwardVector.x);

        Wedge(center, radius, wedgeAngle, centerAngle, color, d_th);
    }

    public static void Wedge(Vector3 center, float radius, float wedgeAngle,
                             Vector3 forwardVector, float d_th = 0.1f) {
        Wedge(center, radius, wedgeAngle, forwardVector, Color.white, d_th);
    }

    public static void Ray(Ray ray, float length, Color color) {
        Debug.DrawRay(ray.origin, ray.direction * length, color);
    }

    public static void Ray(Ray ray, float length) {
        Ray(ray, length, Color.white);
    }
}
