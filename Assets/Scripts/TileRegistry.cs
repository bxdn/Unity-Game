using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class TileRegistry
{
    private readonly IDictionary<String, Tile> allTiles;
    private readonly IList<Tile> treeTiles;
    private readonly IList<Tile> palmTiles;
    private readonly IList<Tile> rockTiles;
    private readonly IList<Tile> rockBeachTiles;
    private readonly IDictionary<String, Sprite> allSprites;
    private static readonly Tilemap baseMap = GameObject.Find("Tilemap-Base").GetComponent<Tilemap>();
    private static readonly Tilemap detailMap = GameObject.Find("Tilemap-Detail").GetComponent<Tilemap>();


    private static TileRegistry instance;
    private TileRegistry()
    {
        treeTiles = new List<Tile>();
        palmTiles = new List<Tile>();
        rockBeachTiles = new List<Tile>();
        rockTiles = new List<Tile>();
        allTiles = new Dictionary<String, Tile>();
        allSprites = new Dictionary<String, Sprite>();
        SpriteAtlas spriteAtlas = Resources.Load<SpriteAtlas>("SpriteAtlas");
        Sprite[] sprites = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(sprites);
        foreach (Sprite sprite in sprites)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            allTiles[sprite.name.Substring(0, sprite.name.Length - 7)] = tile;
            allSprites[sprite.name.Substring(0, sprite.name.Length - 7)] = sprite;
        }
        treeTiles.Add(allTiles["Overworld_Tileset_253"]);
        treeTiles.Add(allTiles["Overworld_Tileset_286"]);
        palmTiles.Add(allTiles["TropicalExtras_Tree_Beach_0"]);
        palmTiles.Add(allTiles["TropicalExtras_Tree_Beach_1"]);
        palmTiles.Add(allTiles["TropicalExtras_Tree_Beach_2"]);
        rockTiles.Add(allTiles["TropicalExtras_Rock_0"]);
        rockTiles.Add(allTiles["TropicalExtras_Rock_1"]);
        rockBeachTiles.Add(allTiles["TropicalExtras_Rock_Beach_0"]);
        rockBeachTiles.Add(allTiles["TropicalExtras_Rock_Beach_1"]);
    }

    public static TileRegistry GetInstance()
    {
        if (instance == null)
        {
            instance = new TileRegistry();
        }
        return instance;
    }

    public void SetTile(Vector2Int pos, String spriteName, bool detail)
    {
        try
        {
            if (detail)
            {
                detailMap.SetTile(new Vector3Int(pos.x, pos.y, 0), allTiles[spriteName]);
            }
            else
            {
                baseMap.SetTile(new Vector3Int(pos.x, pos.y, 0), allTiles[spriteName]);
            }
        }
        catch (Exception e)
        {
            Debug.Log(spriteName + "Does not exist!");
            Debug.Log(e.StackTrace);
        }
    }

    internal void SetItem(Vector2Int pos, Map.Item item)
    {
        switch (item)
        {
            case Map.Item.Wood:
                detailMap.SetTile(new Vector3Int(pos.x, pos.y, 0), allTiles[Constants.WOOD_SPRITE]);
                break;
        }
    }

    public void SetTreeTile(Vector2Int pos)
    {
        detailMap.SetTile(new Vector3Int(pos.x, pos.y, 0), Utils.GetRandomListElement(treeTiles));
    }

    public void RemoveTree(Vector2Int treeTile)
    {
        detailMap.SetTile(new Vector3Int(treeTile.x, treeTile.y, 0), null);
    }

    public void SetPalmTile(Vector2Int pos)
    {
        detailMap.SetTile(new Vector3Int(pos.x, pos.y, 0), Utils.GetRandomListElement(palmTiles));
    }
    public void SetRockTile(Vector2Int pos)
    {
        detailMap.SetTile(new Vector3Int(pos.x, pos.y, 0), Utils.GetRandomListElement(rockTiles));
    }
    public void SetRockBeachTile( Vector2Int pos)
    {
        detailMap.SetTile(new Vector3Int(pos.x, pos.y, 0), Utils.GetRandomListElement(rockBeachTiles));
    }
    public void SetCharSprite(SpriteRenderer renderer, Character.Sex sex, int spriteNum)
    {
        String spriteName = sex == Character.Sex.Male ? "Male 14-" + spriteNum + "_1" : "Female 17-" + spriteNum + "_1";
        renderer.sprite = allSprites[spriteName];
    }
}
