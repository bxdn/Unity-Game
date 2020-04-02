using System;
using UnityEngine;

public class ChopJob : ICharacterJob
{
    private int jobProgress = 0;
    private bool isDone = false;
    private readonly Vector2Int treeTile;
    private readonly int treeToughness;
	public ChopJob(Vector2Int treeTile)
	{
        if (Map.GetInstance().IsLocked(treeTile))
        {
            isDone = true;
        }
        Map.GetInstance().SetLocked(treeTile, true);
        this.treeTile = treeTile;
        treeToughness = Utils.r.Next(200, 250);
	}

    public void DoProgress()
    {
        jobProgress++;
        if(jobProgress == treeToughness)
        {
            Map.GetInstance().SetLocked(treeTile, false);
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
