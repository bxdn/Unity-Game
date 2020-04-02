using System;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    private readonly TileData[,] tiles;
    private readonly IDictionary<Vector2Int, Biome> biomes;
    private readonly IDictionary<Detail, ISet<Vector2Int>> detailTiles;
    private readonly ISet<ISet<Vector2Int>> regions;
    private readonly Vector2Int startPosition;
    private readonly ISet<Vector2Int> stockpile;

    public static readonly int SHORE_LEVEL = 140;
    public static readonly int SEA_LEVEL = 120;
    public static readonly int OCEAN_LEVEL = 90;
    public static readonly int GRASS_LEVEL = 150;
    public static readonly int CLIFF_LEVEL = 190;
    private static readonly int OCTAVES = 16;
    private static readonly double PERSISTANCE = .45;
    private static readonly double SCALE = .0075;
    private static readonly int NUM_BIOMES = 200;
    public static readonly int MAP_SIZE = 100;
    private static Map instance;
    private Map()
    {
        tiles = new TileData[MAP_SIZE, MAP_SIZE];
        biomes = new Dictionary<Vector2Int, Biome>();
        detailTiles = new Dictionary<Detail, ISet<Vector2Int>>();
        regions = new HashSet<ISet<Vector2Int>>();
        GenerateBiomes();
        CreateInitialMap();
        FixIncompatibleTiles();
        CreateDetails();
        CalculateRegions();
        startPosition = CalculateStartingPos();
        stockpile = new HashSet<Vector2Int>() { startPosition };
    }
    public enum Detail
    {
        DessertRock,
        Rock,
        Tree,
        Palm
    }
    public static Map GetInstance()
    {
        if (instance == null)
        {
            instance = new Map();
        }
        return instance;
    }

    private void CreateDetails()
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                IList<Vector2Int> immediateTiles = new List<Vector2Int>(Utils.GetAdjCoordsArr(pos)) { pos };
                bool areSandTiles = true;
                foreach (Vector2Int tile in immediateTiles)
                {
                    if (!IsSandTile(tile))
                    {
                        areSandTiles = false;
                        break;
                    }
                }
                if (areSandTiles)
                {
                    if (Utils.r.Next(100) < 10)
                    {
                        SetDetail(pos, Detail.Palm, false);
                        TileRegistry.GetInstance().SetPalmTile(pos);
                    }
                    else if (Utils.r.Next(100) < 1)
                    {
                        SetDetail(pos, Detail.DessertRock, false);
                        TileRegistry.GetInstance().SetRockBeachTile(pos);
                    }
                    continue;
                }
                bool areGrassyTiles = true;
                foreach (Vector2Int tile in immediateTiles)
                {
                    if (!IsGrassTile(tile))
                    {
                        areGrassyTiles = false;
                        break;
                    }
                }
                if (areGrassyTiles)
                {
                    if (Utils.r.Next(100) < 10)
                    {
                        SetDetail(pos, Detail.Tree, false);
                        TileRegistry.GetInstance().SetTreeTile(pos);
                    }
                    else if (Utils.r.Next(100) < 1)
                    {
                        SetDetail(pos, Detail.Rock, false);
                        TileRegistry.GetInstance().SetRockTile(pos);
                    }
                    continue;
                }
                bool areCliffTiles = true;
                foreach (Vector2Int tile in immediateTiles)
                {
                    if (!IsCliffTile(tile))
                    {
                        areCliffTiles = false;
                        break;
                    }
                }
                if (areCliffTiles)
                {
                    if (Utils.r.Next(100) < 10)
                    {
                        SetDetail(pos, Detail.Tree, false);
                        TileRegistry.GetInstance().SetTreeTile(pos);
                    }
                    else if (Utils.r.Next(100) < 1)
                    {
                        SetDetail(pos, Detail.Rock, false);
                        TileRegistry.GetInstance().SetRockTile(pos);
                    }
                }
            }
        }
    }
    private void CalculateRegions()
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if ((IsGrassTile(pos) || IsSandTile(pos))
                    && GetDetail(pos) == null)
                {
                    bool found = false;
                    foreach (ISet<Vector2Int> region in regions)
                    {
                        if (region.Contains(pos))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        ISet<Vector2Int> newRegion = new HashSet<Vector2Int>
                        {
                            pos
                        };
                        regions.Add(newRegion);
                        PopulateRegion(newRegion, pos);
                    }
                }
            }
        }
    }
    private void PopulateRegion(ISet<Vector2Int> region, Vector2Int startingPoint)
    {
        foreach (Vector2Int adjTile in Utils.GetAdjCoordsArr(startingPoint))
        {
            if ((IsGrassTile(adjTile) || IsSandTile(adjTile))
                    && GetDetail(adjTile) == null && region.Add(adjTile))
            {
                PopulateRegion(region, adjTile);
            }
        }
    }
    private void FixIncompatibleTiles()
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (IsShoreTile(pos))
                {
                    Fix_Tile_Rec(pos);
                }
            }
        }
    }

    private void Fix_Tile_Rec(Vector2Int pos)
    {
        bool sandFound = false;
        bool seaFound = false;
        Vector2Int[] adjTiles = Utils.GetAdjCoordsArr(pos);
        foreach (Vector2Int adjTile in adjTiles)
        {
            if (IsSandTile(adjTile))
            {
                sandFound = true;
            }
            else if (IsSeaTile(adjTile))
            {
                seaFound = true;
            }
            if (sandFound && seaFound)
            {
                GetGameTile(adjTile).height = SHORE_LEVEL;
                Fix_Tile_Rec(pos);
                Fix_Tile_Rec(adjTile);
                return;
            }
        }
    }

    private Biome CalculateBiome(Vector2Int pos)
    {
        int x = pos.x;
        int y = pos.y;
        double shortestDistanceSq = double.MaxValue;
        Biome currentBiome = Biome.Grass;
        foreach (KeyValuePair<Vector2Int, Biome> entry in biomes)
        {
            int compX = entry.Key.x;
            int compY = entry.Key.y;
            double distanceSq = Mathf.Pow(x - compX, 2) + Mathf.Pow(y - compY, 2);
            if (distanceSq < shortestDistanceSq)
            {
                shortestDistanceSq = distanceSq;
                currentBiome = entry.Value;
            }
        }
        return currentBiome;
    }

    private void CreateInitialMap()
    {
        SimplexNoise.Noise.Seed = Utils.r.Next(Int32.MaxValue);
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                double height = Utils.SumOcatave(OCTAVES, x, y, PERSISTANCE, SCALE);

                tiles[x, y] = new TileData(height, CalculateBiome(new Vector2Int(x, y)));
            }
        }
    }

    public bool IsSandTile(Vector2Int pos)
    {
        TileData tileData = GetGameTile(pos);
        if (tileData == null)
        {
            return false;
        }
        double height = GetGameTile(pos).height;
        return height <= GRASS_LEVEL && height > SHORE_LEVEL;
    }

    private void GenerateBiomes()
    {
        for (int i = 0; i < NUM_BIOMES; i++)
        {
            int x = Utils.r.Next(tiles.GetLength(0));
            int y = Utils.r.Next(tiles.GetLength(1));
            biomes[new Vector2Int(x, y)] = Utils.ChooseRandomBiome();
        }
    }

    public bool IsShoreTile(Vector2Int pos)
    {
        TileData tileData = GetGameTile(pos);
        if (tileData == null)
        {
            return false;
        }
        double height = GetGameTile(pos).height;
        return height <= SHORE_LEVEL && height > SEA_LEVEL;
    }

    public bool IsCliffTile(Vector2Int pos)
    {
        TileData tileData = GetGameTile(pos);
        if (tileData == null)
        {
            return false;
        }
        return GetGameTile(pos).height > CLIFF_LEVEL;
    }

    public bool IsGrassTile(Vector2Int pos)
    {
        TileData tileData = GetGameTile(pos);
        if (tileData == null)
        {
            return false;
        }
        double height = GetGameTile(pos).height;
        return height <= CLIFF_LEVEL && height > GRASS_LEVEL;
    }

    public bool IsSeaTile(Vector2Int pos)
    {
        TileData tileData = GetGameTile(pos);
        if (tileData == null)
        {
            return false;
        }
        double height = GetGameTile(pos).height;
        return height <= SEA_LEVEL && height > OCEAN_LEVEL;
    }

    private TileData GetGameTile(Vector2Int pos)
    {
        int x = pos.x;
        int y = pos.y;
        if (x < 0 || x >= tiles.GetLength(0) || y < 0 || y >= tiles.GetLength(1))
        {
            return null;
        }
        return tiles[x, y];
    }

    public Biome GetBiome(Vector2Int pos)
    {
        return GetGameTile(pos).biome;
    }

    public double? GetHeight(Vector2Int pos)
    {
        TileData tile = GetGameTile(pos);
        if (tile != null)
        {
            return tile.height;
        }
        return null;
    }

    public void SetDetail(Vector2Int pos, Detail? detail)
    {
        SetDetail(pos, detail, true);
    }

    private void SetDetail(Vector2Int pos, Detail? detail, bool adjustRegions)
    {
        GetGameTile(pos).detail = detail;
        if (detail != null)
        {
            Detail nonNullDetail = (Detail)detail;
            if (!detailTiles.ContainsKey(nonNullDetail))
            {
                detailTiles[nonNullDetail] = new HashSet<Vector2Int>();
            }
            detailTiles[nonNullDetail].Add(pos);

            ISet<Vector2Int> regionToRemove = null;
            foreach (ISet<Vector2Int> region in regions)
            {
                if (region.Contains(pos))
                {
                    regionToRemove = region;
                    break;
                }
            }
            regions.Remove(regionToRemove);
        }
        else
        {
            foreach (ISet<Vector2Int> tiles in detailTiles.Values)
            {
                if (tiles.Remove(pos))
                {
                    break;
                }
            }
            ISet<ISet<Vector2Int>> regionsToRemove = new HashSet<ISet<Vector2Int>>();
            foreach (Vector2Int adjTile in Utils.GetAdjCoordsArr(pos))
            {
                foreach (ISet<Vector2Int> region in regions)
                {
                    if (region.Contains(adjTile))
                    {
                        regionsToRemove.Add(region);
                        break;
                    }
                }
            }
            foreach (ISet<Vector2Int> region in regionsToRemove)
            {
                regions.Remove(region);
            }
        }
        if (adjustRegions)
        {
            CalculateRegions();
        }
    }

    public Detail? GetDetail(Vector2Int pos)
    {
        TileData tileData = GetGameTile(pos);
        if (tileData == null)
        {
            return null;
        }
        return tileData.detail;
    }
    private ISet<Vector2Int> GetDetailTiles(Detail detail)
    {
        if (detailTiles.ContainsKey(detail))
        {
            return detailTiles[detail];
        }
        return null;
    }

    public Vector2Int? FindClosestAccessibleDetail(Detail detailType, Vector2Int pos)
    {
        ISet<Vector2Int> correctRegion = null;
        foreach (ISet<Vector2Int> region in regions)
        {
            if (region.Contains(pos))
            {
                correctRegion = region;
                break;
            }
        }
        if (correctRegion != null)
        {
            Vector2Int? closestDetail = null;
            double minDistSq = int.MaxValue;
            foreach (Vector2Int detail in GetDetailTiles(detailType))
            {
                if (!GetGameTile(detail).locked)
                {
                    foreach (Vector2Int adjTile in Utils.GetAdjCoordsArr(detail))
                    {
                        if (correctRegion.Contains(adjTile))
                        {
                            double distSq = Math.Pow(detail.x - pos.x, 2) + Math.Pow(detail.y - pos.y, 2);
                            if (distSq < minDistSq)
                            {
                                minDistSq = distSq;
                                closestDetail = detail;
                            }
                            break;
                        }
                    }
                }
            }
            return closestDetail;
        }
        return null;
    }

    public void SetLocked(Vector2Int pos, bool locked)
    {
        TileData tileData = GetGameTile(pos);
        if (tileData == null)
        {
            return;
        }
        tileData.locked = locked;
    }

    public bool IsLocked(Vector2Int pos)
    {
        TileData tileData = GetGameTile(pos);
        if (tileData == null)
        {
            return false;
        }
        return tileData.locked;
    }

    private Vector2Int CalculateStartingPos()
    {
        ISet<Vector2Int> biggestRegion = null;
        foreach (ISet<Vector2Int> region in regions)
        {
            if (biggestRegion == null || region.Count > biggestRegion.Count)
            {
                biggestRegion = region;
            }
        }
        return Utils.GetRandomElement(biggestRegion);
    }

        public Vector2Int[] GetStartingPositions()
    {
        ISet<Vector2Int> activeRegion = null;
        foreach (ISet<Vector2Int> region in regions)
        {
            if (region.Contains(startPosition))
            {
                activeRegion = region;
            }
        }
        Vector2Int[] startingPositions = new Vector2Int[7];
        startingPositions[0] = startPosition;
        int count = 1;
        ISet<Vector2Int> visitedTiles = new HashSet<Vector2Int>() { startPosition };
        ISet<Vector2Int> toVisitTiles = new HashSet<Vector2Int>() { startPosition };
        while (count < 7)
        {
            ISet<Vector2Int> newToVisitTiles = new HashSet<Vector2Int>();
            foreach (Vector2Int tile in toVisitTiles)
            {
                foreach (Vector2Int adjTile in Utils.GetAdjCoordsArr(tile))
                {
                    newToVisitTiles.Add(adjTile);
                    if (activeRegion.Contains(adjTile) && visitedTiles.Add(adjTile))
                    {
                        startingPositions[count] = adjTile;
                        count++;
                        if (count == 7)
                        {
                            return startingPositions;
                        }
                    }
                }
            }
            toVisitTiles = newToVisitTiles;
        }
        return null;
    }
}
