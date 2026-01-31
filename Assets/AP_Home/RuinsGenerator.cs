using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public sealed class RuinsGenerator : MonoBehaviour
{
    [Header("Area (centered on this transform)")]
    [SerializeField] private Vector2 areaSize = new Vector2(120f, 120f);
    [SerializeField] private float edgeMargin = 6f;

    [Header("Random")]
    [SerializeField] private int seed = 12345;

    [Header("Generation")]
    [SerializeField] private bool clearBeforeGenerate = true;
    [SerializeField] private int wallSegments = 35;
    [SerializeField] private int rubbleClusters = 55;
    [SerializeField] private int columns = 12;

    [Header("Walls")]
    [SerializeField] private float wallThickness = 0.8f;
    [SerializeField] private float wallMinLength = 4f;
    [SerializeField] private float wallMaxLength = 18f;
    [SerializeField] private float wallMinHeight = 0.8f;
    [SerializeField] private float wallMaxHeight = 3.4f;

    [Header("Rubble")]
    [SerializeField] private int rubbleMinPieces = 3;
    [SerializeField] private int rubbleMaxPieces = 8;
    [SerializeField] private float rubbleMinSize = 0.6f;
    [SerializeField] private float rubbleMaxSize = 3.8f;
    [SerializeField] private float rubbleMaxStackHeight = 2.2f;

    [Header("Columns")]
    [SerializeField] private float columnRadius = 0.8f;
    [SerializeField] private float columnMinHeight = 2.2f;
    [SerializeField] private float columnMaxHeight = 5.5f;
    [SerializeField] private float fallenColumnChance = 0.35f;

    [ContextMenu("Generate Ruins")]
    public void Generate()
    {
        var rng = new System.Random(seed);

        if (clearBeforeGenerate)
        {
            ClearGenerated();
        }

        CreateRoomLikeLayout(rng);
        CreateWallSegments(rng);
        CreateRubble(rng);
        CreateColumns(rng);
    }

    [ContextMenu("Clear Generated")]
    public void ClearGenerated()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
            DestroySmart(child);
        }
    }

    private void CreateRoomLikeLayout(System.Random rng)
    {
        // A few "room" outlines (broken rectangles) to make it feel like a place, not a polygon.
        CreateBrokenRectangle(rng, centerLocal: new Vector3(-18f, 0f, 10f), size: new Vector2(22f, 16f));
        CreateBrokenRectangle(rng, centerLocal: new Vector3(20f, 0f, -14f), size: new Vector2(18f, 20f));
        CreateBrokenRectangle(rng, centerLocal: new Vector3(-4f, 0f, -24f), size: new Vector2(26f, 14f));
    }

    private void CreateBrokenRectangle(System.Random rng, Vector3 centerLocal, Vector2 size)
    {
        var halfX = size.x * 0.5f;
        var halfZ = size.y * 0.5f;
        var wallHeight = RandomRange(rng, 1.2f, 3.2f);

        // Each side is split into 2 segments with a gap.
        var gapX = RandomRange(rng, 2.8f, 5.5f);
        var gapZ = RandomRange(rng, 2.8f, 5.5f);

        // North (positive Z)
        CreateWallPiece(centerLocal + new Vector3(-gapX, wallHeight * 0.5f, halfZ), new Vector3(halfX - gapX, wallHeight, wallThickness), RandomYaw(rng));
        CreateWallPiece(centerLocal + new Vector3(gapX, wallHeight * 0.5f, halfZ), new Vector3(halfX - gapX, wallHeight * 0.9f, wallThickness), RandomYaw(rng));

        // South (negative Z)
        CreateWallPiece(centerLocal + new Vector3(-gapX, wallHeight * 0.5f, -halfZ), new Vector3(halfX - gapX, wallHeight * 0.85f, wallThickness), RandomYaw(rng));
        CreateWallPiece(centerLocal + new Vector3(gapX, wallHeight * 0.5f, -halfZ), new Vector3(halfX - gapX, wallHeight, wallThickness), RandomYaw(rng));

        // East (positive X)
        CreateWallPiece(centerLocal + new Vector3(halfX, wallHeight * 0.5f, -gapZ), new Vector3(wallThickness, wallHeight, halfZ - gapZ), 90f + RandomYaw(rng));
        CreateWallPiece(centerLocal + new Vector3(halfX, wallHeight * 0.5f, gapZ), new Vector3(wallThickness, wallHeight * 0.8f, halfZ - gapZ), 90f + RandomYaw(rng));

        // West (negative X)
        CreateWallPiece(centerLocal + new Vector3(-halfX, wallHeight * 0.5f, -gapZ), new Vector3(wallThickness, wallHeight * 0.9f, halfZ - gapZ), 90f + RandomYaw(rng));
        CreateWallPiece(centerLocal + new Vector3(-halfX, wallHeight * 0.5f, gapZ), new Vector3(wallThickness, wallHeight, halfZ - gapZ), 90f + RandomYaw(rng));
    }

    private void CreateWallSegments(System.Random rng)
    {
        for (var i = 0; i < wallSegments; i++)
        {
            var length = RandomRange(rng, wallMinLength, wallMaxLength);
            var height = RandomRange(rng, wallMinHeight, wallMaxHeight);

            var position = RandomPointInside(rng);
            position.y = height * 0.5f;

            var yaw = RandomRange(rng, 0f, 360f);
            var scale = new Vector3(length, height, wallThickness);

            CreateWallPiece(position, scale, yaw);

            // Occasionally add a broken "top" segment to make uneven height.
            if (rng.NextDouble() < 0.45)
            {
                var topLength = length * RandomRange(rng, 0.35f, 0.75f);
                var topHeight = height * RandomRange(rng, 0.25f, 0.55f);
                var topScale = new Vector3(topLength, topHeight, wallThickness * RandomRange(rng, 0.8f, 1.2f));
                var offset = new Vector3(RandomRange(rng, -length * 0.2f, length * 0.2f), height * 0.5f + topHeight * 0.5f, RandomRange(rng, -0.3f, 0.3f));
                CreateWallPiece(position + Quaternion.Euler(0f, yaw, 0f) * offset, topScale, yaw + RandomYaw(rng));
            }
        }
    }

    private void CreateRubble(System.Random rng)
    {
        for (var i = 0; i < rubbleClusters; i++)
        {
            var basePosition = RandomPointInside(rng);
            var count = rng.Next(rubbleMinPieces, rubbleMaxPieces + 1);

            for (var j = 0; j < count; j++)
            {
                var size = new Vector3(
                    RandomRange(rng, rubbleMinSize, rubbleMaxSize),
                    RandomRange(rng, rubbleMinSize * 0.6f, Mathf.Min(rubbleMaxSize, rubbleMaxStackHeight)),
                    RandomRange(rng, rubbleMinSize, rubbleMaxSize));

                var localOffset = new Vector3(
                    RandomRange(rng, -2.4f, 2.4f),
                    size.y * 0.5f,
                    RandomRange(rng, -2.4f, 2.4f));

                // Some pieces stacked on top of others.
                if (rng.NextDouble() < 0.25)
                {
                    localOffset.y += RandomRange(rng, 0.4f, 1.3f);
                }

                var position = basePosition + localOffset;
                var rotation = Quaternion.Euler(
                    RandomRange(rng, 0f, 18f),
                    RandomRange(rng, 0f, 360f),
                    RandomRange(rng, 0f, 18f));

                var cube = CreatePrimitive(PrimitiveType.Cube, "Rubble");
                cube.transform.SetParent(transform, false);
                cube.transform.localPosition = position;
                cube.transform.localRotation = rotation;
                cube.transform.localScale = size;
            }
        }
    }

    private void CreateColumns(System.Random rng)
    {
        for (var i = 0; i < columns; i++)
        {
            var height = RandomRange(rng, columnMinHeight, columnMaxHeight);
            var position = RandomPointInside(rng);

            var cylinder = CreatePrimitive(PrimitiveType.Cylinder, "Column");
            cylinder.transform.SetParent(transform, false);

            // Unity cylinder is 2 units tall at y-scale 1.
            var yScale = height / 2f;
            cylinder.transform.localScale = new Vector3(columnRadius, yScale, columnRadius);

            if (rng.NextDouble() < fallenColumnChance)
            {
                cylinder.transform.localPosition = position + new Vector3(0f, columnRadius, 0f);
                cylinder.transform.localRotation = Quaternion.Euler(90f, RandomRange(rng, 0f, 360f), 0f);
            }
            else
            {
                cylinder.transform.localPosition = position + new Vector3(0f, height * 0.5f, 0f);
                cylinder.transform.localRotation = Quaternion.Euler(0f, RandomRange(rng, 0f, 360f), RandomRange(rng, -6f, 6f));
            }
        }
    }

    private void CreateWallPiece(Vector3 localPosition, Vector3 localScale, float yaw)
    {
        var cube = CreatePrimitive(PrimitiveType.Cube, "WallPiece");
        cube.transform.SetParent(transform, false);
        cube.transform.localPosition = localPosition;
        cube.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        cube.transform.localScale = localScale;
    }

    private Vector3 RandomPointInside(System.Random rng)
    {
        var halfX = areaSize.x * 0.5f - edgeMargin;
        var halfZ = areaSize.y * 0.5f - edgeMargin;

        var x = RandomRange(rng, -halfX, halfX);
        var z = RandomRange(rng, -halfZ, halfZ);
        return new Vector3(x, 0f, z);
    }

    private static float RandomYaw(System.Random rng) => RandomRange(rng, -18f, 18f);

    private static float RandomRange(System.Random rng, float minInclusive, float maxInclusive)
    {
        var t = (float)rng.NextDouble();
        return Mathf.Lerp(minInclusive, maxInclusive, t);
    }

    private static GameObject CreatePrimitive(PrimitiveType type, string baseName)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = baseName;
        return go;
    }

    private static void DestroySmart(GameObject go)
    {
        if (go == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Undo.DestroyObjectImmediate(go);
            return;
        }
#endif
        Object.Destroy(go);
    }
}

