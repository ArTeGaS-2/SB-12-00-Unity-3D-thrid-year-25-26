using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Кидаєш цей скрипт на Plane.
/// URP-варіант: використовує Universal Render Pipeline/Lit.
/// </summary>
[ExecuteInEditMode]
public class RuinsGeneratorURP : MonoBehaviour
{
    [Header("Основні налаштування")]
    public int objectCount = 150;
    public Vector2 uniformScaleRange = new Vector2(0.3f, 2.5f);
    public float minDistanceBetweenObjects = 0.5f;
    public bool clearPrevious = true;

    [Header("Рандом")]
    public bool useSeed = false;
    public int seed = 0;

    [Header("Візуальні налаштування")]
    public bool randomizeColor = true;
    public Color baseColor = new Color(0.6f, 0.6f, 0.6f);
    [Range(0f, 0.5f)]
    public float colorVariation = 0.2f;

    // Типи примітивів
    private static readonly PrimitiveType[] primitiveTypes =
    {
        PrimitiveType.Cube

    };

    private readonly System.Collections.Generic.List<Transform> spawnedObjects =
        new System.Collections.Generic.List<Transform>();

    // Кеш URP-шейдера
    private static Shader urpLitShader;

    private void Awake()
    {
        // Знаходимо URP Lit шейдер один раз
        if (urpLitShader == null)
        {
            urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLitShader == null)
                Debug.LogError("Shader 'Universal Render Pipeline/Lit' не знайдено. Перевір, що URP активний у проєкті.");
        }
    }

    private void Start()
    {
        Generate();
    }

#if UNITY_EDITOR
    [ContextMenu("Generate Ruins")]
    private void ContextGenerate()
    {
        Generate();
    }
#endif

    public void Generate()
    {
        if (useSeed)
            Random.InitState(seed);

        var rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("На об'єкті з RuinsGeneratorURP немає Renderer. Кинь скрипт на Plane або інший Mesh з Renderer.");
            return;
        }

        Bounds bounds = rend.bounds;

        if (clearPrevious)
            ClearSpawned();

        spawnedObjects.Clear();

        int safety = 0;
        for (int i = 0; i < objectCount && safety < objectCount * 10; i++)
        {
            safety++;

            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            float y = bounds.max.y;

            Vector3 pos = new Vector3(x, y, z);

            if (minDistanceBetweenObjects > 0f)
            {
                bool tooClose = false;
                foreach (Transform t in spawnedObjects)
                {
                    if (Vector3.Distance(t.position, pos) < minDistanceBetweenObjects)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose)
                {
                    i--;
                    continue;
                }
            }

            PrimitiveType type = primitiveTypes[Random.Range(0, primitiveTypes.Length)];
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = "RuinPiece_" + i;

            go.transform.SetParent(transform);
            go.transform.position = pos;
            go.transform.rotation = Random.rotation;

            float uScale = Random.Range(uniformScaleRange.x, uniformScaleRange.y);
            Vector3 scale = new Vector3(
                uScale * Random.Range(0.4f, 1.2f),
                uScale * Random.Range(0.4f, 1.5f),
                uScale * Random.Range(0.4f, 1.2f)
            );
            go.transform.localScale = scale;

            go.transform.position += Vector3.down * (scale.y * 0.1f);

            if (randomizeColor && urpLitShader != null)
            {
                var mr = go.GetComponent<Renderer>();
                if (mr != null)
                {
                    var mat = new Material(urpLitShader);
                    Color randomOffset = new Color(
                        Random.Range(-colorVariation, colorVariation),
                        Random.Range(-colorVariation, colorVariation),
                        Random.Range(-colorVariation, colorVariation)
                    );
                    mat.color = baseColor + randomOffset;
                    mr.material = mat;
                }
            }

            spawnedObjects.Add(go.transform);
        }
    }

    public void ClearSpawned()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("RuinPiece_"))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(child.gameObject);
                else
                    Destroy(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }
        }
        spawnedObjects.Clear();
    }
}
