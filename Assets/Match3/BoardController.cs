// BoardController.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardController : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] int width = 9;
    [SerializeField] int height = 9;
    [SerializeField] float gemStep = 1.1f;

    [Header("Gems")]
    [SerializeField] List<GameObject> gemPrefabs;

    [Header("Animation")]
    [SerializeField] float destroyDelay = 0.25f;       // пауза, поки геми стискаються до нуля
    [SerializeField] float afterCollapseDelay = 0.1f;  // пауза після заповнення

    [Header("Score")]
    [SerializeField] ScoreManager scoreManager;

    public int Width => width;
    public int Height => height;

    public GameObject[,] Grid { get; private set; }

    public bool IsBusy { get; private set; } // true – поки йде анімація/каскад

    private void Awake()
    {
        Grid = new GameObject[width, height];
    }

    private void Start()
    {
        if (scoreManager != null)
            scoreManager.ResetScore();

        // початкове заповнення зі spawn-анімацією
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SpawnGem(x, y, playSpawn: true);
            }
        }
    }

    GameObject SpawnGem(int x, int y, bool playSpawn = true)
    {
        int type = Random.Range(0, gemPrefabs.Count);
        Vector3 position = new Vector3(x * gemStep, y * gemStep, 0);

        GameObject obj = Instantiate(gemPrefabs[type], position, Quaternion.identity, transform);

        Gem gem = obj.GetComponent<Gem>();
        if (gem == null)
            gem = obj.AddComponent<Gem>();

        gem.X = x;
        gem.Y = y;
        gem.Type = type;

        GemVisual visual = obj.GetComponent<GemVisual>();
        if (visual != null && playSpawn)
        {
            visual.PlaySpawn();
        }

        Grid[x, y] = obj;

        return obj;
    }

    public void SwapGems(Gem a, Gem b)
    {
        // в масиві
        Grid[a.X, a.Y] = b.gameObject;
        Grid[b.X, b.Y] = a.gameObject;

        // координати
        int ax = a.X, ay = a.Y;
        a.X = b.X; a.Y = b.Y;
        b.X = ax; b.Y = ay;

        // позиції в сцені
        Vector3 posA = a.transform.position;
        a.transform.position = b.transform.position;
        b.transform.position = posA;
    }

    public IEnumerator ResolveSwap(Gem a, Gem b)
    {
        if (IsBusy) yield break;
        IsBusy = true;

        // пробний свап
        SwapGems(a, b);
        yield return null;

        // перший пошук матчів
        var matches = MatchFinder.FindMatches(Grid, width, height);
        if (matches.Count == 0)
        {
            // хід невдалий – відкат
            SwapGems(a, b);
            IsBusy = false;
            yield break;
        }

        // був хоча б один матч – запускаємо каскад
        do
        {
            yield return StartCoroutine(AnimateMatchesAndCollapse(matches));
            matches = MatchFinder.FindMatches(Grid, width, height);
        }
        while (matches.Count > 0);

        IsBusy = false;
    }

    IEnumerator AnimateMatchesAndCollapse(HashSet<Vector2Int> matches)
    {
        // 0. Рахуємо очки
        int removedCount = matches.Count;
        if (scoreManager != null && removedCount > 0)
        {
            scoreManager.AddGems(removedCount);

        }

        // 1. Візуальне стискання matched-гемів
        foreach (var pos in matches)
        {
            int x = pos.x;
            int y = pos.y;

            if (x < 0 || x >= width || y < 0 || y >= height)
                continue;

            GameObject go = Grid[x, y];
            if (go == null) continue;

            var visual = go.GetComponent<GemVisual>();
            if (visual != null)
            {
                visual.PlayDestroy();
            }
        }

        // даємо анімації зіграти
        yield return new WaitForSeconds(destroyDelay);

        // 2. Реально видаляємо геми
        foreach (var pos in matches)
        {
            int x = pos.x;
            int y = pos.y;

            if (x < 0 || x >= width || y < 0 || y >= height)
                continue;

            if (Grid[x, y] != null)
            {
                Destroy(Grid[x, y]);
                Grid[x, y] = null;
            }
        }

        // 3. Обвалюємо стовпчики й доспавнюємо нові
        CollapseAndRefill();

        // невелика пауза, щоб було видно зміни
        yield return new WaitForSeconds(afterCollapseDelay);
    }

    void CollapseAndRefill()
    {
        for (int x = 0; x < width; x++)
        {
            int empty = 0;

            for (int y = 0; y < height; y++)
            {
                if (Grid[x, y] == null)
                {
                    empty++;
                }
                else if (empty > 0)
                {
                    Grid[x, y - empty] = Grid[x, y];
                    Grid[x, y] = null;

                    Gem g = Grid[x, y - empty].GetComponent<Gem>();
                    g.Y -= empty;

                    Vector3 pos = g.transform.position;
                    pos.y -= gemStep * empty;
                    g.transform.position = pos;
                }
            }

            // добираємо нові зверху з spawn-анімацією
            for (int y = height - empty; y < height; y++)
            {
                SpawnGem(x, y, playSpawn: true);
            }
        }
    }
}
