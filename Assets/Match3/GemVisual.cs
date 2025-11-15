using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemVisual : MonoBehaviour
{
    [Header("Натиск")]
    [SerializeField] float hoverScaleFactor = 1.1f; //наведення
    [SerializeField] float pressedScaleFactor = 0.9f; //натиск
    [SerializeField] float scaleLerpSpeed = 10f; // швидк. анім.

    Vector3 baseScale;
    Vector3 targetScale;

    bool isHover;
    bool isPressed;

    void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }
    void Update()
    {
        transform.localScale =
            Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * scaleLerpSpeed);
    }
    void OnMouseEnter()
    {
        isHover = true;
        if (!isPressed)
        {
            targetScale = baseScale * hoverScaleFactor;
        }   
    }
    void OnMouseExit()
    {
        isHover = false;
        if (!isPressed)
        {
            targetScale = baseScale;
        }
    }
    void OnMouseDown()
    {
        isPressed = true;
        targetScale = baseScale * pressedScaleFactor;
    }
    void OnMouseUp()
    {
        isPressed = false;
        if (isHover) targetScale = baseScale * hoverScaleFactor;
        else targetScale = baseScale;
    }
}
