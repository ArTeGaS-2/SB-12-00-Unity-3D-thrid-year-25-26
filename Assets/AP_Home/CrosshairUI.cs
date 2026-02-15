using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public sealed class CrosshairUI : MonoBehaviour
{
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float segmentLength = 9f;
    [SerializeField] private float thickness = 2f;
    [SerializeField] private float gap = 6f;

    private void Awake()
    {
        var canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        if (!TryGetComponent<CanvasScaler>(out _))
        {
            gameObject.AddComponent<CanvasScaler>();
        }

        if (!TryGetComponent<GraphicRaycaster>(out _))
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }

        Rebuild();
    }

    private void Rebuild()
    {
        var old = transform.Find("CrosshairRoot");
        if (old != null)
        {
            Destroy(old.gameObject);
        }

        var rootObject = new GameObject("CrosshairRoot", typeof(RectTransform));
        rootObject.transform.SetParent(transform, false);

        var rootRect = rootObject.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = Vector2.zero;
        rootRect.sizeDelta = Vector2.zero;

        CreateSegment(rootRect, "Top", new Vector2(0f, gap + segmentLength * 0.5f), new Vector2(thickness, segmentLength));
        CreateSegment(rootRect, "Bottom", new Vector2(0f, -gap - segmentLength * 0.5f), new Vector2(thickness, segmentLength));
        CreateSegment(rootRect, "Left", new Vector2(-gap - segmentLength * 0.5f, 0f), new Vector2(segmentLength, thickness));
        CreateSegment(rootRect, "Right", new Vector2(gap + segmentLength * 0.5f, 0f), new Vector2(segmentLength, thickness));
    }

    private void CreateSegment(RectTransform parent, string segmentName, Vector2 anchoredPosition, Vector2 size)
    {
        var segment = new GameObject(segmentName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        segment.transform.SetParent(parent, false);

        var rect = segment.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        var image = segment.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
    }
}
