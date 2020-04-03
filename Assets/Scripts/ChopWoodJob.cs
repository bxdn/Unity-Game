using System;
using UnityEngine;

public class ChopWoodJob : ICharacterJob
{
    private readonly int treeToughness;
    private int chopProgress = 0;
    private bool isDone = false;
    private readonly Vector2Int treeTile;
    public ChopWoodJob(Vector2Int tree)
	{
        treeToughness = Utils.r.Next(200, 250);
        treeTile = tree;
    }

    public void DoProgress()
    {
        chopProgress++;
        if (chopProgress == treeToughness)
        {
            Map.GetInstance().SetDetail(treeTile, null);
            TileRegistry.GetInstance().RemoveTree(treeTile);
            isDone = true;
        }
    }

    public bool IsDone()
    {
        return isDone;
    }
}
