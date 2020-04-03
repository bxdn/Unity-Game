using System;
using UnityEngine;

public class DropoffJob : ICharacterJob
{
    private bool isDone = false;
    private readonly Vector2Int pos;
    private readonly Map.Item item;
	public DropoffJob(Vector2Int pos, Map.Item item) 
	{
        this.pos = pos;
        this.item = item;
	}

    public void DoProgress()
    {
        Map.GetInstance().SetItem(pos, item);
        TileRegistry.GetInstance().SetItem(pos, item);
        isDone = true;
    }

    public bool IsDone()
    {
        return isDone;
    }
}
