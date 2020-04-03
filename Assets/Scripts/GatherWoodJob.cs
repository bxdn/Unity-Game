using UnityEngine;

public class GatherWoodJob : ACharacterRecJob
{
    private bool initialized = false;
    private bool isDone = false;
    private bool chopDone = false;
    private Vector2Int lockTile;
    private readonly Character character;
	public GatherWoodJob(Character character)
	{
        this.character = character;
	}

    override public void DoProgress()
    {
        if(!initialized)
        {
            initialized = true;
            Vector2Int? closestTreeTile = Map.GetInstance().FindClosestAccessibleDetail(Map.Detail.Tree, character.GetCurrentTilePosition());
            if (closestTreeTile != null)
            {
                Vector2Int closestTreeTileNN = (Vector2Int)closestTreeTile;
                lockTile = closestTreeTileNN;
                Map.GetInstance().SetLocked(lockTile, true);
                subJobs.Enqueue(new MoveJob(character, closestTreeTileNN, false));
                subJobs.Enqueue(new ChopWoodJob(closestTreeTileNN));
            }
            else
            {
                isDone = true;
                return;
            }
        }
        base.DoProgress();
        if(subJobs.Count == 0)
        {
            if (!chopDone)
            {
                chopDone = true;
                Map.GetInstance().SetLocked(lockTile, false);
                Vector2Int? stockpileTile = Map.GetInstance().GetAvailableStockpileTile();
                if (stockpileTile != null)
                {
                    Vector2Int stockpileTileNN = (Vector2Int) stockpileTile;
                    lockTile = stockpileTileNN;
                    Map.GetInstance().SetLocked(lockTile, true);
                    subJobs.Enqueue(new MoveJob(character, stockpileTileNN, true));
                    subJobs.Enqueue(new DropoffJob(stockpileTileNN, Map.Item.Wood));
                }
            }
            else
            {
                Map.GetInstance().SetLocked(lockTile, false);
                isDone = true;
            }
        }
    }

    override public bool IsDone()
    {
        return isDone;
    }
}
