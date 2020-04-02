using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveJob : ICharacterJob
{
    private readonly Character character;
    private readonly IList<Vector2Int> path;
    private int pathIdx = 1;
    private bool isDone;
    private static readonly float moveSpeed = .05f;
    private static readonly float diagSpeed = Mathf.Sqrt(Mathf.Pow(moveSpeed, 2)/2);
	public MoveJob(Character character, Vector2Int pos, bool walkTo)
	{
        this.character = character;
        Vector2Int[] fullPath = Utils.FindPath(character.GetCurrentTilePosition(), pos);
        path = walkTo ? fullPath : (IList<Vector2Int>) new ArraySegment<Vector2Int>(fullPath, 0, fullPath.Length - 1);
        if (path == null || path.Count <= 1)
        {
            isDone = true;
        }
	}

    public void DoProgress()
    {
        Vector2 curPos = character.GetCurrentPosition();
        float x = curPos.x;
        float y = curPos.y;
        int toX = path[pathIdx].x;
        int toY = path[pathIdx].y;
        float toXThisTick;
        float toYThisTick;
        bool diagMovement = (toX >= x + moveSpeed || toX <= x - moveSpeed)
            && (toY >= y + moveSpeed || toY <= y - moveSpeed);
        float moveThisTick = diagMovement ? diagSpeed : moveSpeed;
        if (toX >= x + moveSpeed)
        {
            toXThisTick = x + moveThisTick;
        }
        else if (toX <= x - moveSpeed)
        {
            toXThisTick = x - moveThisTick;
        }
        else
        {
            toXThisTick = toX;
        }
        if (toY >= y + moveSpeed)
        {
            toYThisTick = y + moveThisTick;
        }
        else if (toY <= y - moveSpeed)
        {
            toYThisTick = y - moveThisTick;
        }
        else
        {
            toYThisTick = toY;
        }
        character.Move(new Vector2(toXThisTick, toYThisTick));
        curPos = character.GetCurrentPosition();
        if (curPos.x == toX && curPos.y == toY)
        {
            pathIdx++;
            if(pathIdx == path.Count)
            {
                isDone = true;
            }
        }
    }

    public bool IsDone()
    {
        return isDone;
    }
}
