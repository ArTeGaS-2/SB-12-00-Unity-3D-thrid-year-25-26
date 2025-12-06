using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    public static HashSet<Vector2Int> FindMatches(
        GameObject[,] grid, int width, int height)
    {
        var result = new HashSet<Vector2Int>();

        // Горизонтальні метчі
        for (int y = 0; y < height; y++)
        {
            int streak = 1; // довжина поточної серії однакових гемів

            for (int x = 1; x < width; x++)
            {
                if (SameType(grid[x, y], grid[x - 1, y]))
                {
                    streak++;
                }
                else
                {
                    if (streak >= 3)
                    {
                        // додаємо всі позиції серії в результат
                        for (int k = 1; k <= streak; k++)
                            result.Add(new Vector2Int(x - k, y));
                    }
                    streak = 1;
                }
            }
            // кінець ряду: якщо серія закінчилась на правому краю
            if (streak >= 3)
            {
                for (int k = 0; k < streak; k++)
                    result.Add(new Vector2Int(width - 1 - k, y));
            }
        }
        // Вертикальні метчі
        for (int x = 0; x < width; x++)
        {
            int streak = 1;

            for (int y = 1; y < height; y++)
            {
                if (SameType(grid[x, y], grid[x, y - 1]))
                {
                    streak++;
                }
                else
                {
                    if (streak >= 3)
                    {
                        for (int k = 1; k <= streak; k++)
                            result.Add(new Vector2Int(x, y - k));
                    }
                    streak = 1;
                }
            }
            // кінець колонки
            if (streak >= 3)
            {
                if (streak >= 3)
                {
                    for (int k = 1; k <= streak; k++)
                        result.Add(new Vector2Int(x, height - 1 - k));
                }
            }
        }
        return result;
    }
    private static bool SameType(GameObject a, GameObject b)
    {
        if (a == null || b == null) return false;

        var ga = a.GetComponent<Gem>();
        var gb = b.GetComponent<Gem>();

        if (ga == null || gb == null) return false;

        return ga.Type == gb.Type;
    }
}
