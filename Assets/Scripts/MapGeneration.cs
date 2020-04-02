using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
/// <summary>
/// Script for generating the map and updating its animations
/// </summary>
public class MapGeneration : MonoBehaviour
{
    private IList<AnimatedSprite> animatedSprites;
    private ISet<Vector2Int> filledWangTiles;
    private int frameCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        animatedSprites = new List<AnimatedSprite>();
        filledWangTiles = new HashSet<Vector2Int>();
        CreateInitialMap();
        AddSparkles();
    }
    /// <summary>
    /// Create the map data and the base tiles
    /// </summary>
    private void CreateInitialMap()
    {
        CreateBaseMapSprites(0, Map.OCEAN_LEVEL, Map.SEA_LEVEL, Constants.seaData, true, false, false);
        CreateBaseMapSprites(Map.OCEAN_LEVEL, Map.SEA_LEVEL, Map.SHORE_LEVEL, Constants.SHORE_DATA, true, false, false);
        CreateBaseMapSprites(Map.SEA_LEVEL, Map.SHORE_LEVEL, Map.GRASS_LEVEL, Constants.BEACH_DATA, true, true, false);
        CreateBaseMapSprites(Map.SHORE_LEVEL, Map.GRASS_LEVEL, Map.CLIFF_LEVEL, Constants.GRASS_DATA, false, true, true);
        CreateBaseMapSprites(Map.GRASS_LEVEL, Map.CLIFF_LEVEL, int.MaxValue, Constants.CLIFF_DATA, false, false, false);

    }

    /// <summary>
    /// Adds sparkles to shore tiles
    /// </summary>
    private void AddSparkles()
    {
        for (int x = 0; x < Map.MAP_SIZE; x++)
        {
            for (int y = 0; y < Map.MAP_SIZE; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!filledWangTiles.Contains(pos))
                {
                    if (Map.GetInstance().IsShoreTile(pos))
                    {
                        if (Utils.r.Next(100) < 2)
                        {   
                            AddSprite(pos, Utils.GetRandomListElement(Constants.sparkles), true, true);
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        frameCount = frameCount == 10 ? 0 : frameCount + 1;
        if (frameCount != 0)
        {
            return;
        }
        foreach (AnimatedSprite sprite in animatedSprites)
        {
            UpdateSprite(sprite);
        }
    }
    /// <summary>
    /// Update an animated sprite to the next sprite in sequence
    /// </summary>
    /// <param name="sprite"></param>
    private void UpdateSprite(AnimatedSprite sprite)
    {
        if (sprite.playingForward)
        {
            sprite.currSpriteIdx++;
            if (sprite.currSpriteIdx == 3)
            {
                sprite.playingForward = false;
            }
        }
        else
        {
            sprite.currSpriteIdx--;
            if (sprite.currSpriteIdx == 0)
            {
                sprite.playingForward = true;
            }
        }
        TileRegistry.GetInstance().SetTile(sprite.pos, sprite.sprites[sprite.currSpriteIdx], sprite.detail);

    }
    /// <summary>
    /// Adds a sprite to a tilemap at a position, and puts the sprite in a list to animate if it's animated
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="pos"></param>
    /// <param name="sprites"></param>
    /// <param name="animated"></param>
    private void AddSprite(Vector2Int pos, String[] sprites, bool animated, bool detail)
    {
        TileRegistry.GetInstance().SetTile(pos, sprites[0], detail);
        if (animated)
        {
            animatedSprites.Add(new AnimatedSprite(pos, sprites, detail));
        }
    }

    /// <summary>
    /// Get the dictionary of wang indices from which to choose the sprites
    /// </summary>
    /// <param name="lowHeight"></param>
    /// <param name="crossHeight"></param>
    /// <param name="highHeight"></param>
    /// <param name="upwards"></param>
    /// <param name="partial"></param>
    /// <returns> The dictionary of wang indices from which to choose the sprites</returns>
    private static IDictionary<Vector2Int, int> GetWangDic(int lowHeight, int crossHeight, int highHeight, bool upwards, bool partial)
    {
        IDictionary<Vector2Int, int> relevantTiles = new Dictionary<Vector2Int, int>();
        for (int x = 0; x < Map.MAP_SIZE; x++)
        {
            for (int y = 0; y < Map.MAP_SIZE; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                double? mainHeight = Map.GetInstance().GetHeight(pos);
                if (mainHeight != null && mainHeight > lowHeight && mainHeight <= highHeight)
                {
                    if ((upwards && Map.GetInstance().GetHeight(pos) > crossHeight) || (!upwards && Map.GetInstance().GetHeight(pos) <= crossHeight))
                    {
                        // Tiles to be fully filled
                        relevantTiles[pos] = 15;
                    }
                    else
                    {
                        // Tiles will be algorithmically filled later
                        relevantTiles[pos] = 0;
                    }
                }
            }
        }
        Utils.CalculateOtherTiles(relevantTiles, partial);
        return relevantTiles;
    }

    /// <summary>
    /// Assign the sprites for full 2 corner wang transitions
    /// </summary>
    /// <param name="lowHeight"></param>
    /// <param name="crossHeight"></param>
    /// <param name="highHeight"></param>
    /// <param name="wangData"></param>
    /// <param name="animated"></param>
    /// <param name="upwards"></param>
    private void CreateBaseMapSprites(int lowHeight, int crossHeight, int highHeight, String[][] wangData, bool animated, bool upwards, bool partial)
    {
        IDictionary<Vector2Int, int> relevantTiles = GetWangDic(lowHeight, crossHeight, highHeight, upwards, partial);
        FillTiles(relevantTiles, wangData, animated);
    }
    
    /// <summary>
    /// Fill assign tiles to tilemaps based on the data in the dictionary
    /// </summary>
    /// <param name="relevantTiles"></param>
    /// <param name="wangData"></param>
    /// <param name="animated"></param>
    private void FillTiles(IDictionary<Vector2Int, int> relevantTiles, String[][] wangData, bool animated)
    {
        foreach (KeyValuePair<Vector2Int, int> tile in relevantTiles)
        {
            Vector2Int pos = tile.Key;
            int wangIdx = tile.Value;
            bool isSolid = wangIdx == 0 || wangIdx == 15;
            // Don't overwrite tile if it has already been "Wanged"
            if (!filledWangTiles.Contains(pos))
            {
                if (isSolid)
                {
                    AddSprite(pos, new String[1] { Utils.GetRandomListElement(wangData[wangIdx]) }, false, false);
                }
                else if (wangIdx == -1)
                {
                    AddSprite(pos, new String[1] { Utils.GetRandomListElement(wangData[15]) }, false, false);
                }
                else
                {
                    AddSprite(pos, wangData[wangIdx], animated, false);
                }
                // We want to overwrite tiles considered "empty" next round of drawing if we need to, 
                // so we don't add those to the set.
                if (!isSolid)
                {
                    filledWangTiles.Add(pos);
                }
            }
        }
    }
}

