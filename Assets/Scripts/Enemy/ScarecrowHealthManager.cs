using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScarecrowHealthManager : EnemyHealthManager
{
    Coroutine leanCoroutine;
    Quaternion originalRotation;
    public override void Start()
    {
        HEALTH = 999999;
        damageJump = false;
    }
    public override void Update() { }
    public override void GetDamage(float damage)
    {
        Vector3 hitDir = (transform.position - FindAnyObjectByType<PlayerController>().transform.position).normalized;
        float strength = 0.42f; // регулируй здесь
        float angle = Vector3.Angle(transform.up, hitDir) * strength;
        Vector3 tiltAxis = Vector3.Cross(transform.up, hitDir);

        if (leanCoroutine != null)
        {
            StopCoroutine(leanCoroutine);
            transform.rotation = originalRotation;
        }
        originalRotation = transform.rotation;
        leanCoroutine = StartCoroutine(LeanScarecrow(tiltAxis, angle, GetComponent<Scarecrow>().groundCheck.position));
    }
    IEnumerator LeanScarecrow(Vector3 axis, float angle, Vector3 pivot)
    {
        float Hyper(float x, float k = 2f)
        {
            return x / (1f + k * (1f - x));
        }

        float forwardDuration = 0.08f;
        float t = 0f;
        float prev = 0f;

        while (t < forwardDuration)
        {
            float x = t / forwardDuration;
            float curve = Hyper(x);
            float currentAngle = Mathf.Lerp(0f, angle, curve);
            float delta = currentAngle - prev;

            transform.RotateAround(pivot, axis, delta);

            prev = currentAngle;
            t += Time.deltaTime;
            yield return null;
        }

        {
            float delta = angle - prev;
            transform.RotateAround(pivot, axis, delta);
            prev = angle;
        }

        float backDuration = 0.25f;
        t = 0f;

        while (t < backDuration)
        {
            float x = t / backDuration;
            float curve = x;
            float currentAngle = Mathf.Lerp(angle, 0f, curve);
            float delta = currentAngle - prev;

            transform.RotateAround(pivot, axis, delta);

            prev = currentAngle;
            t += Time.deltaTime;
            yield return null;
        }

        {
            float delta = 0f - prev;
            transform.RotateAround(pivot, axis, delta);
        }
        leanCoroutine = null;
    }
}
