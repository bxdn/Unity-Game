using System;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    private static readonly String BASE_NAME = "Overworld_Tileset_";
    private static readonly String BASE_NAME2 = "OceanNoTransparency_Tileset_";
    private static readonly String[] GRASS_TILES = new string[3] { "Overworld_Tileset_0", "Overworld_Tileset_1", "Overworld_Tileset_2" };
    private static readonly String[] OCEAN_TILES = new string[1] { "Overworld_Tileset_772" };
    private static readonly String[] SEA_TILES = new string[1] { "Overworld_Tileset_677" };
    private static readonly String[] SAND_TILES = new string[1] { "Overworld_Tileset_585" };
    private static readonly String[] SHORE_TILES = new string[1] { "Overworld_Tileset_697" };
    public static readonly String[][] BEACH_DATA = Utils.GetAnimatedWangSet(BASE_NAME2, new int[3] { 216, 236, 256 }, SHORE_TILES, SAND_TILES);
    public static readonly String[][] seaData = Utils.GetAnimatedWangSet(BASE_NAME, new int[3] { 636, 676, 716 }, SEA_TILES, OCEAN_TILES);
    public static readonly String[][] SHORE_DATA = Utils.GetAnimatedWangSet(BASE_NAME, new int[3] { 656, 696, 736 }, SHORE_TILES, SEA_TILES);
    public static readonly String[][] CLIFF_DATA = Utils.GetAnimatedWangSet(BASE_NAME, new int[3] { 349, 389, 424 }, GRASS_TILES, GRASS_TILES);
    public static readonly String[][] hillData = Utils.GetAnimatedWangSet(BASE_NAME, new int[3] { 151, 187, 220 }, GRASS_TILES, GRASS_TILES);
    public static readonly String[][] GRASS_DATA = Utils.GetAnimatedWangSet(BASE_NAME, new int[3] { 557, 584, 609 }, SAND_TILES, GRASS_TILES);
    public static readonly String[] sparkle1 = new string[4] { "Overworld_Tileset_313", "Overworld_Tileset_314", 
        "Overworld_Tileset_315", "Overworld_Tileset_316" };
    public static readonly String[] sparkle2 = new string[4] { "Overworld_Tileset_345", "Overworld_Tileset_346",
        "Overworld_Tileset_347", "Overworld_Tileset_348" };
    public static readonly IList<String[]> sparkles = new List<String[]>(new String[2][] { sparkle1, sparkle2 });
    public static readonly GameObject GRID = GameObject.Find("Grid");
    public static readonly Canvas CANVAS = GameObject.Find("Canvas").GetComponent<Canvas>();
    public static readonly String WOOD_SPRITE = "Wood";
    public static readonly float DIAG_SCORE = Mathf.Sqrt(2);
}
