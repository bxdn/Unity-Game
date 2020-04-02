using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AnimatedSprite
{
    public readonly String[] sprites;
    public int currSpriteIdx;
    public readonly Vector2Int pos;
    public bool playingForward;
    public readonly bool detail;
    public AnimatedSprite(Vector2Int pos, String[] sprites, bool detail)
    {
        this.sprites = sprites;
        this.pos = pos;
        currSpriteIdx = 0;
        playingForward = true;
        this.detail = detail;
    }
}