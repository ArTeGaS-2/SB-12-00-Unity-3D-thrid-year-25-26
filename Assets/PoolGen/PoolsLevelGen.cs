using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoolsLevelGen : MonoBehaviour
{
    public static PoolsLevelGen Instance;

    [Header("Посилання")]
    public GameObject levelPrefab; // Префаб одного рівня

    [Header("Налаштування")]
    public float levelHeightStep = 14f; // Висота між платформами
    [Range(2,500)]public int poolCapacity = 50; // Кількість інстансів в пулі
    public string playerTag = "Player"; // Тег гравця для тригерів

    [Header("Стан")]
    public int currentLevelNum = 0; // Поточний рівень, де зараз гравець
    public int topLevelNum = 0; // Найвищий індекс/рівень у вікні
    public int bottomLevelNum; // Найнижчий індекс/ рівень у вікні

    // --- Приватні структури
    readonly List<GameObject> pool = new List<GameObject>();
    readonly Stack<GameObject> free = new Stack<GameObject>();
    readonly Dictionary<int, GameObject> activeByLevel = 
        new Dictionary<int, GameObject>();

    private Vector3 basePosition; // Позиція першого згенерованого рівня
   
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        basePosition = new Vector3(0,7,0);

        BuildPool();

        currentLevelNum = 0;
        RecomputeWindow();
    }
    public void SetCurrentLevel(int number)
    {
        if (currentLevelNum == number) return;
        currentLevelNum = number;
        RecomputeWindow();
    }
    private void BuildPool()
    {
        for (int i = 0; i < poolCapacity; i++)
        {
            var go = Instantiate(levelPrefab, transform); // Створює об'єкт
            go.SetActive(false); // Відключає об'єкт
            pool.Add(go); // Додає до списку
            free.Push(go); // Додає до стеку(в кінець)
        }
    }
    // Головна логіка вікна рівнів
    private void RecomputeWindow()
    {
        int half = poolCapacity / 2; // Рахує половину пула рівнів

        // Верхня межа не вище 0
        topLevelNum = Mathf.Min(0 , currentLevelNum + half);
        // Низ так, щоб у сумі було рівно poolCapacity рівнів
        bottomLevelNum = topLevelNum - poolCapacity;

        // Виводимо з активних ті, що вийшли за межі
        var toRelease = new List<int>();
        foreach (var kv in activeByLevel) // перебір всих активних рівнів
        {
            int lvl = kv.Key; // пара ключ-значення, за кожним ключем - рівень
            if (lvl < bottomLevelNum || lvl > topLevelNum || lvl == 0)
                toRelease.Add(lvl);
        }
        foreach (int lvl in toRelease) ReleaseLevel(lvl); // Активація

        // Заповнюємо діапазон
        for (int lvl = bottomLevelNum; lvl <= topLevelNum; lvl++)
        {
            if (lvl == 0) continue; // Нульовий лишається не використаним
            if (activeByLevel.ContainsKey(lvl)) continue; // Перевіряє наявність

            if (free.Count == 0)
            {
                int far = FindFarthestActiveFrom(currentLevelNum);
                if (far != int.MaxValue) ReleaseLevel(far);
            }

            var go = free.Pop(); // Дістає елемент зі стеку
            PlaceAndInit(go, lvl);
            activeByLevel[lvl] = go;
        }
    }
    private void PlaceAndInit(GameObject go, int levelNumber)
    {
        go.transform.position = basePosition + Vector3.up * (
            levelNumber * levelHeightStep);
        go.transform.rotation = Quaternion.identity;
        go.SetActive(true);

        var lvl = go.GetComponentInChildren<Level>(true);
        if (lvl != null) lvl.Init(this, levelNumber, playerTag);

        UpdateTextIfAny(go, levelNumber);
    }
    private void ReleaseLevel(int levelNumber)
    {
        if (!activeByLevel.TryGetValue(levelNumber, out var go)) return;
        activeByLevel.Remove(levelNumber);
        go.SetActive(false);
        free.Push(go);
    }
    private int FindFarthestActiveFrom(int pivot)
    {
        int farKey = int.MaxValue; // Заглушка
        int farDist = -1; // Мінімально можливе
        foreach (var kv in activeByLevel)
        {
            int d = Mathf.Abs(kv.Key - pivot); // Відстань до гравця
            if (d > farDist) // якщо цей ще далі, ніж минулий максимум
            {
                farDist = d; 
                farKey = kv.Key; // Новий максимум
            }
        }
        return farKey; // повертаємо номер рівня, найбільш віддаленого від гравця
    }
    private void UpdateTextIfAny(GameObject root, int num)
    {
        // Якщо у префабі є TMP_Text - міняємо напис (полотно з текстом)
        var tmp = root.GetComponentInChildren<TMP_Text>(true);
        if (tmp != null) tmp.text = Mathf.Abs(num).ToString();
    }
}
