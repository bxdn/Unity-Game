using System;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static readonly System.Random r = new System.Random();

    private Utils()
    {
    }

    public static T GetRandomListElement<T>(IList<T> list)
    {
        int idx = r.Next(list.Count);
        return list[idx];
    }

    public static T GetRandomElement<T>(ICollection<T> collection)
    {
        int target = r.Next(collection.Count);
        int idx = 0;
        foreach(T instance in collection)
        {
            if(idx == target)
            {
                return instance;
            }
            idx++;
        }
        return default;
    }

    public static double SumOcatave(int num_iterations, int x, int y, double persistence, double scale)
    {
        double maxAmp = 0.0;
        double amp = 1.0;
        float freq = (float)scale;
        double noise = 0.0;
        for (int i = 0; i < num_iterations; i++)
        {
            noise += SimplexNoise.Noise.CalcPixel2D(x, y, freq) * amp;
            maxAmp += amp;
            amp *= persistence;
            freq *= 2;
        }
        noise /= maxAmp;
        return noise;
    }

    public static Biome ChooseRandomBiome()
    {
        Array values = Enum.GetValues(typeof(Biome));
        return (Biome)values.GetValue(r.Next(values.Length));
    }

    public static AdjTiles GetAdjCoordsStruct(Vector2Int pos)
    {
        int x = pos.x;
        int y = pos.y;
        AdjTiles toRet = new AdjTiles
        {
            l = new Vector2Int(x - 1, y),
            tl = new Vector2Int(x - 1, y + 1),
            t = new Vector2Int(x, y + 1),
            tr = new Vector2Int(x + 1, y + 1),
            r = new Vector2Int(x + 1, y),
            br = new Vector2Int(x + 1, y - 1),
            b = new Vector2Int(x, y - 1),
            bl = new Vector2Int(x - 1, y - 1)
        };
        return toRet;
    }

    public static Vector2Int[] GetAdjCoordsArr(Vector2Int pos)
    {
        int x = pos.x;
        int y = pos.y;
        Vector2Int[] toRet = new Vector2Int[8];
        toRet[0] = new Vector2Int(x - 1, y);
        toRet[1] = new Vector2Int(x, y + 1);
        toRet[2] = new Vector2Int(x + 1, y);
        toRet[3] = new Vector2Int(x, y - 1);
        toRet[4] = new Vector2Int(x - 1, y + 1);
        toRet[5] = new Vector2Int(x + 1, y + 1);
        toRet[6] = new Vector2Int(x + 1, y - 1);
        toRet[7] = new Vector2Int(x - 1, y - 1);
        return toRet;
    }

    public static String[][] GetAnimatedWangSet(String baseName, int[] startingIdxs, String[] firstSpriteNames, String[] finalSpriteNames)
    {
        String[][] wangSet = new string[16][];
        wangSet[0] = firstSpriteNames;
        wangSet[1] = GenerateAnimatedTileArray(baseName, startingIdxs[0] + 4);
        wangSet[2] = GenerateAnimatedTileArray(baseName, startingIdxs[1] + 4);
        wangSet[3] = GenerateAnimatedTileArray(baseName, startingIdxs[1] + 2);
        wangSet[4] = GenerateAnimatedTileArray(baseName, startingIdxs[1] + 3);
        wangSet[5] = GenerateAnimatedTileArray(baseName, startingIdxs[2] + 4);
        wangSet[6] = GenerateAnimatedTileArray(baseName, startingIdxs[2] + 1);
        wangSet[7] = GenerateAnimatedTileArray(baseName, startingIdxs[2] + 2);
        wangSet[8] = GenerateAnimatedTileArray(baseName, startingIdxs[0] + 3);
        wangSet[9] = GenerateAnimatedTileArray(baseName, startingIdxs[0] + 1);
        wangSet[10] = GenerateAnimatedTileArray(baseName, startingIdxs[2] + 3);
        wangSet[11] = GenerateAnimatedTileArray(baseName, startingIdxs[0] + 2);
        wangSet[12] = GenerateAnimatedTileArray(baseName, startingIdxs[1]);
        wangSet[13] = GenerateAnimatedTileArray(baseName, startingIdxs[0]);
        wangSet[14] = GenerateAnimatedTileArray(baseName, startingIdxs[2]);
        wangSet[15] = finalSpriteNames;
        return wangSet;
    }

    private static String[] GenerateAnimatedTileArray(String baseName, int startIdx)
    {
        int tileSeparation = 5;
        String[] spriteList = new string[4];
        spriteList[0] = baseName + Convert.ToString(startIdx);
        spriteList[1] = baseName + Convert.ToString(startIdx + tileSeparation);
        spriteList[2] = baseName + Convert.ToString(startIdx + tileSeparation * 2);
        spriteList[3] = baseName + Convert.ToString(startIdx + tileSeparation * 3);
        return spriteList;
    }

    public static V TryGetValue<K, V>(IDictionary<K, V> dic, K idx, V defaultValue)
    {
        return dic.ContainsKey(idx) ? dic[idx] : defaultValue;
    }

    /// <summary>
    /// Fills the wang dictionary with data cooresponding to which corners to fill
    /// </summary>
    /// <param name="relevantTiles"></param>
    /// <param name="partial"></param>
    public static void CalculateOtherTiles(IDictionary<Vector2Int, int> relevantTiles, bool partial)
    {
        foreach (Vector2Int pos in new Dictionary<Vector2Int, int>(relevantTiles).Keys)
        {
            // If the idx is 15, we know that this tile is completely filled per GetWangDic(...)
            if (relevantTiles[pos] == 0)
            {
                AdjTiles adjTiles = GetAdjCoordsStruct(pos);
                int tSpriteIdx = TryGetValue(relevantTiles, adjTiles.t, 0);
                int bSpriteIdx = TryGetValue(relevantTiles, adjTiles.b, 0);
                int lSpriteIdx = TryGetValue(relevantTiles, adjTiles.l, 0);
                int rSpriteIdx = TryGetValue(relevantTiles, adjTiles.r, 0);
                int tlSpriteIdx = TryGetValue(relevantTiles, adjTiles.tl, 0);
                int trSpriteIdx = TryGetValue(relevantTiles, adjTiles.tr, 0);
                int blSpriteIdx = TryGetValue(relevantTiles, adjTiles.bl, 0);
                int brSpriteIdx = TryGetValue(relevantTiles, adjTiles.br, 0);
                // If all adjacent tiles are empty, this one should be too, continue to next tile
                if ((tSpriteIdx | bSpriteIdx | lSpriteIdx | rSpriteIdx
                    | tlSpriteIdx | trSpriteIdx | blSpriteIdx | brSpriteIdx) == 0)
                {
                    continue;
                }
                // If upper right corner should be filled
                if (((tSpriteIdx & (1 << 1)) == 1 << 1)
                    || ((trSpriteIdx & (1 << 2)) == 1 << 2)
                    || ((rSpriteIdx & 1 << 3) == 1 << 3))
                {
                    relevantTiles[pos] += 1;
                }
                // If lower right corner should be filled
                if (((bSpriteIdx & 1) == 1)
                    || ((rSpriteIdx & (1 << 2)) == 1 << 2)
                    || ((brSpriteIdx & 1 << 3) == 1 << 3))
                {
                    relevantTiles[pos] += 2;
                }
                // If lower left corner should be filled
                if (((lSpriteIdx & (1 << 1)) == 1 << 1)
                    || ((blSpriteIdx & 1) == 1)
                    || ((bSpriteIdx & 1 << 3) == 1 << 3))
                {
                    relevantTiles[pos] += 4;
                }
                // If upper left corner should be filled
                if (((tSpriteIdx & (1 << 2)) == 1 << 2)
                    || ((tlSpriteIdx & (1 << 1)) == 1 << 1)
                    || ((lSpriteIdx & 1) == 1))
                {
                    relevantTiles[pos] += 8;
                }
                int wangIdx = relevantTiles[pos];

                if (partial)
                {
                    if (wangIdx != 0 && (wangIdx == 1 || wangIdx == 2
                    || wangIdx == 4 || wangIdx == 5
                    || wangIdx == 8 || wangIdx == 10))
                    {
                        relevantTiles[pos] = 0;
                    }
                }
                // Special case where the tile sprite completely differs from height, 
                // so we don't treat it as solid
                if (relevantTiles[pos] == 15)
                {
                    relevantTiles[pos] = -1;
                }
            }
        }
    }

    public static float FindHScore(Vector2Int start, Vector2Int end)
    {
        int deltaX = Mathf.Abs(start.x - end.x);
        int deltaY = Mathf.Abs(start.y - end.y);
        int diagMoves = Mathf.Min(deltaX, deltaY);
        int remainder = Mathf.Max(deltaX, deltaY) - diagMoves;
        return diagMoves * Constants.DIAG_SCORE + remainder;
    }

    public static float FindGScore(Vector2Int prevTile, float prevScore, Vector2Int end, bool diag)
    {
        float additionalScore = diag ? Constants.DIAG_SCORE : 1;
        return prevScore - FindHScore(prevTile, end) + additionalScore;
    }

    public static Vector2Int[] FindPath(Vector2Int start, Vector2Int end)
    {
        if (start.Equals(end))
        {
            return new Vector2Int[1] { start };
        }
        IDictionary<Vector2Int, AStarNode> openSet = new Dictionary<Vector2Int, AStarNode>();
        IDictionary<Vector2Int, AStarNode> closedSet = new Dictionary<Vector2Int, AStarNode>();
        openSet[start] = new AStarNode { score = 0, prevTile = null };
        bool pathFound = false;
        while (!pathFound)
        {
            if (openSet.Count == 0)
            {
                return null;
            }
            float minScore = int.MaxValue;
            Vector2Int? minScoreTile = null;
            foreach (KeyValuePair<Vector2Int, AStarNode> pair in openSet)
            {
                float score = pair.Value.score;
                if (score < minScore)
                {
                    minScore = score;
                    minScoreTile = pair.Key;
                }
            }
            if (minScoreTile != null)
            {
                Vector2Int activeTile = (Vector2Int)minScoreTile;
                AStarNode activeInfo = openSet[activeTile];
                openSet.Remove(activeTile);
                closedSet[activeTile] = activeInfo;
                foreach (Vector2Int adjTile in Utils.GetAdjCoordsArr(activeTile))
                {
                    if (adjTile.Equals(end))
                    {
                        openSet.Clear();
                        pathFound = true;
                        closedSet[end] = new AStarNode { score = 0, prevTile = activeTile };
                        break;
                    }
                    bool diag = adjTile.x != activeTile.x && adjTile.y != activeTile.y;
                    float gScore = FindGScore(activeTile, minScore, end, diag);
                    PutIfWalkable(openSet, closedSet, adjTile, gScore + FindHScore(adjTile, end), activeTile);
                }
            }
        }
        IList<Vector2Int> reversePath = new List<Vector2Int>();
        Vector2Int? curTile = end;
        while (curTile != null)
        {
            reversePath.Add((Vector2Int)curTile);
            curTile = closedSet[(Vector2Int)curTile].prevTile;
        }
        int tiles = reversePath.Count;
        Vector2Int[] path = new Vector2Int[tiles];
        for (int i = 0; i < tiles; i++)
        {
            path[i] = reversePath[tiles - i - 1];
        }
        return path;
    }

    private static void PutIfWalkable(IDictionary<Vector2Int, AStarNode> openDic, IDictionary<Vector2Int, AStarNode> closedDic, Vector2Int coord, float score, Vector2Int prevCoord)
    {
        if ((Map.GetInstance().IsGrassTile(coord) || Map.GetInstance().IsSandTile(coord))
            && Map.GetInstance().GetDetail(coord) == null
            && !openDic.ContainsKey(coord)
            && !closedDic.ContainsKey(coord))
        {
            openDic[coord] = new AStarNode { score = score, prevTile = prevCoord };
        }
    }

    private struct AStarNode
    {
        public float score;
        public Vector2Int? prevTile;
    }
}
