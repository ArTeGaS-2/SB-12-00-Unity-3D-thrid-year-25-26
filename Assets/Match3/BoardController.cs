using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [Header("Сітка")]
    public int width = 9; // ширина сітки
    public int lenght = 9; // довжина сітки
    public float gemStep = 1.1f; // відстань між гемами

    [Header("Геми")]
    public List<GameObject> gemPrefabs; // префаби гемів

    [Header("Анімації")]
    [SerializeField] float destroyDelay = 0.25f;
    [SerializeField] float afterCollapseDelay = 0.1f;

    [Header("Рахунок")]
    [SerializeField] ScoreManager scoreManager;

    public int Width => width;
    public int Lenght => lenght;

    public GameObject[,] Grid;
    public bool IsBusy { get; private set; }

    private void Awake()
    {
        Grid = new GameObject[width, lenght];
    }
    private void Start()
    {
        if (scoreManager != null) scoreManager.ResetScore();

        for (int y = 0; y < lenght; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SpawnGem(x,y, playSpawn: true);
            }
        }
    }
    private GameObject SpawnGem(int x, int y, bool playSpawn = true)
    {
        int type = Random.Range(0, gemPrefabs.Count);
        Vector3 position = new Vector3(
            x * gemStep, y * gemStep, 0);
        GameObject obj = Instantiate(gemPrefabs[type], position,
            Quaternion.identity, transform);

        // заповнюємо дані про гем
        Gem gem = obj.GetComponent<Gem>();
        if (gem == null)
            gem = obj.GetComponent<Gem>();

        gem.X = x;
        gem.Y = y;
        gem.Type = type;

        GemVisual visual = obj.GetComponent<GemVisual>();
        if (visual != null && playSpawn) visual.PlaySpawn();

        Grid[x, y] = obj;
        return obj;
    }
    public void SwapGems(Gem a, Gem b)
    {
        Grid[a.X, a.Y] = b.gameObject;
        Grid[b.X, b.Y] = a.gameObject;

        int ax = a.X, ay = a.Y;
        a.X = b.X;
        a.Y = b.Y;
        b.X = ax;
        b.Y = ay;

        Vector3 posA = a.transform.position;
        a.transform.position = b.transform.position;
        b.transform.position = posA;
    }
    public System.Collections.IEnumerator ResolveSwap(Gem a, Gem b)
    {
        if (IsBusy) yield break;
        IsBusy = true;

        SwapGems(a, b);
        yield return null;

        // перший пошук метчів
        var matches = MatchFinder.FindMatches(Grid, width, lenght);
        if (matches.Count == 0)
        {
            SwapGems(a, b);
            IsBusy = false;
            yield break;
        }
        do
        {
            yield return StartCoroutine(
                AnimateMatchesAndCollapse(matches));
            matches = MatchFinder.FindMatches(Grid, width, lenght);
        }
        while (matches.Count > 0);

        IsBusy = false;
    }
}
