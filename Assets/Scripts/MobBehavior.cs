﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class MobBehavior : MonoBehaviour
{
    private IList<Character> characters;
    // Start is called before the first frame update
    void Start()
    {
        characters = new List<Character>();
        Create_Sprites();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Character character in characters)
        {
            if (character.IsIdle())
            {
                character.ChopTree();
            }
            character.DoJobProgress();
        }
    }

    private void Create_Sprites()
    {
        foreach(Vector2Int pos in Map.GetInstance().GetStartingPositions())
        {
            characters.Add(new Character(pos));
        }
    }
}
