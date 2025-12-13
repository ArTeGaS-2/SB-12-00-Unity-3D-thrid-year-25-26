// GemVisual.cs
using UnityEngine;

public class GemVisual : MonoBehaviour
{
    [Header("Hover / Press")]
    [SerializeField] float hoverScaleFactor = 1.05f;   // при наведенні
    [SerializeField] float pressedScaleFactor = 0.9f;  // при натисканні
    [SerializeField] float scaleLerpSpeed = 10f;       // швидкість анімації масштабу

    [Header("Spawn / Destroy")]
    [SerializeField] float spawnStartFactor = 0.0f;     // з якого масштабу зростати (0 = з нуля)
    [SerializeField] float destroyDurationHint = 0.25f; // орієнтовна тривалість стискання

    public float DestroyDurationHint => destroyDurationHint;

    Vector3 baseScale;
    Vector3 targetScale;

    bool isHover;
    bool isPressed;
    bool isDestroying;  // якщо true – ігноруємо hover/press

    void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    void Update()
    {
        transform.localScale =
            Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);
    }

    void OnMouseEnter()
    {
        if (isDestroying) return;

        isHover = true;
        if (!isPressed)
            targetScale = baseScale * hoverScaleFactor;
    }

    void OnMouseExit()
    {
        if (isDestroying) return;

        isHover = false;
        if (!isPressed)
            targetScale = baseScale;
    }

    void OnMouseDown()
    {
        if (isDestroying) return;

        isPressed = true;
        targetScale = baseScale * pressedScaleFactor;
    }

    void OnMouseUp()
    {
        if (isDestroying) return;

        isPressed = false;

        if (isHover)
            targetScale = baseScale * hoverScaleFactor;
        else
            targetScale = baseScale;
    }

    // Викликається з BoardController при появі нового гема
    public void PlaySpawn()
    {
        transform.localScale = baseScale * spawnStartFactor;
        targetScale = baseScale;
    }

    // Викликається з BoardController перед видаленням гема
    public void PlayDestroy()
    {
        isDestroying = true;
        isHover = false;
        isPressed = false;
        targetScale = Vector3.zero;
    }
}
