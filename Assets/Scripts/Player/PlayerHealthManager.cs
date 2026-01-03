using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    PlayerController pc;

    public float HEALTH;
    public float MAXHEALTH;
    public Slider healthSlider;

    private float targetValue; // Целевое значение слайдера
    public float smoothSpeed = 10f; // Скорость плавности

    public float campfireHeal = 2;
    public float campfireDistance = 10;
    public LayerMask campfireMask;

    public GameObject plusPrefab;
    Transform plusParent = null;
    float plusTimer = 0;
    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }
    void Start()
    {
        HEALTH = MAXHEALTH;
        targetValue = 1f;
        healthSlider.value = 1f;
    }

    void Update()
    {
        if (pc.isSeat && HEALTH < MAXHEALTH && Physics.CheckSphere(transform.position, campfireDistance, campfireMask))
        {
            GetHeal(campfireHeal * Time.deltaTime);
            if (plusParent == null)
            {
                plusParent = Instantiate(new GameObject(), transform).transform;
            }
            plusTimer += Time.deltaTime;
            if (plusTimer > 0.5f)
            {
                GameObject newPlus = Instantiate(plusPrefab, plusParent);
                Vector3 dir = (new Vector3(Random.value * 2 - 1, 0, Random.value * 2 - 1).normalized) * 0.5f;
                newPlus.transform.localPosition = dir;
                newPlus.AddComponent<Plus>();
                plusTimer = 0;
            }
        }

        // Плавное приближение текущего значения слайдера к целевому
        float currentValue = healthSlider.value;
        targetValue = HEALTH / MAXHEALTH;
        healthSlider.value = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * smoothSpeed);
    }

    public void GetDamage(float damage)
    {
        HEALTH -= damage;
        HEALTH = Mathf.Clamp(HEALTH, 0, MAXHEALTH);
    }
    public void GetHeal(float heal)
    {
        HEALTH += heal;
        HEALTH = Mathf.Clamp(HEALTH, 0, MAXHEALTH);
    }
}

class Plus : MonoBehaviour
{
    Renderer renderer;
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }
    void Start()
    {
        StartCoroutine(Animation());
    }
    IEnumerator Animation()
    {
        float duration = 3;
        float elapsed = 0;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, 2, 0);

        Color startColor = renderer.material.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float ct = t >= 0.5f ? (t - 0.5f) * 2 : 0;

            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            transform.Rotate(new Vector3(0, 180 * Time.deltaTime, 0));
            renderer.material.color = Color.Lerp(startColor, endColor, ct);

            yield return null;
        }

        if (transform.parent.childCount == 1) Destroy(transform.parent.gameObject);
        Destroy(gameObject);
    }
}