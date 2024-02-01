using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Four_Point_Movement : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private Transform pointC;
    [SerializeField] private Transform pointD;
    [SerializeField] private float progress;

    public Transform move_object;
    public float step = 0;
    public float max_step = 4;
    public float lowerLimit = 0.05f;
    public float upperLimit = 3.95f;
    public float time = 1f;
    private float timer = 0.0f;
    private float direction = 1.0f;
    public float speed = 1.0f;

    public float Move(float dt)
    {
        
        if (progress >= max_step && direction > 0)
        {
            progress = max_step;
        }
        else if (progress <= lowerLimit && direction < 0)
        {
            progress = 0;
        }
        else
        {
            timer += (direction * speed * dt);
            progress = Math3Rule(max_step, 100, timer) * 0.01f;
        }
        move_object.position = CubicLerp(pointA.position, pointB.position, pointC.position, pointD.position, progress);
        return progress;
    }
    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, progress * step);
    }
    private Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab_bc = QuadraticLerp(a, b, c, t);
        Vector3 bc_cd = QuadraticLerp(b, c, d, t);

        return Vector3.Lerp(ab_bc, bc_cd, progress * step);
    }
    private float Math3Rule(float a, float b, float A)
    {
        return ((A * b) / a);
    }
    public void SetDirection(bool forward)
    {
        direction = 1;
        if(!forward)
        {
            direction = -1;
        }
    }
    public Vector3 GetFinalPointHeight()
    {
        return pointD.position;
    }
}
