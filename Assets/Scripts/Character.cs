using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Character
{
    public readonly String name;
    public readonly Sex sex;
    private Vector2 curPos;
    private GameObject panel;
    private GameObject text;
    private GameObject sprite;
    private ICharacterJob curJob;
    private readonly Queue<ICharacterJob> jobs;
    private readonly IDictionary<Map.Detail, int> inventory;
    private static readonly TextAsset namesCsv = Resources.Load<TextAsset>("names");
    private static readonly int[] spriteNums = new int[3] { 1, 2, 4 };
    private static readonly Font ARIAL = Resources.GetBuiltinResource<Font>("Arial.ttf");
    public Character(Vector2 initialPos)
	{
        String[] allNames = namesCsv.text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        String[] nameRow = allNames[Utils.r.Next(allNames.Length)].Split(',');
        name = nameRow[0];
        sex = nameRow[1] == "M" ? Sex.Male : Sex.Female;
        jobs = new Queue<ICharacterJob>();
        inventory = new Dictionary<Map.Detail, int>();
        int spriteNum = Utils.GetRandomListElement(spriteNums);
        curPos = initialPos;
        CreateSprite(spriteNum);
        CreatePanel(CreateText());
        RenderParts(initialPos);
    }

    private void RenderParts(Vector2 pos)
    {
        sprite.transform.position = new Vector3(pos.x + .5f, pos.y + .5f, 0);
        text.transform.position = new Vector3(pos.x + .5f, pos.y + 1.5f, 0);
        panel.transform.position = new Vector3(pos.x + .5f, pos.y + 1.5f, 0);
    }

    private void CreateSprite(int spriteNum)
    {
        sprite = new GameObject("Sprite");
        TileRegistry.GetInstance().SetCharSprite(sprite.AddComponent<SpriteRenderer>(), sex, spriteNum);
        sprite.transform.SetParent(Constants.GRID.transform);
    }


    private void CreatePanel(Vector2 textWidth)
    {
        panel = new GameObject("Panel", typeof(RectTransform));
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, .5f);
        image.type = Image.Type.Sliced;
        RectTransform transform = (RectTransform) panel.transform;
        transform.localScale = new Vector3(.01f, .01f, 1);
        transform.sizeDelta = new Vector2(textWidth.x + 25, textWidth.y + 10);
        transform.SetParent(Constants.CANVAS.transform);
        transform.SetSiblingIndex(0);
    }

    private Vector2 CreateText()
    {
        text = new GameObject("Text", typeof(RectTransform));
        Text textComponent = text.AddComponent<Text>();
        textComponent.text = name;
        textComponent.fontSize = 50;
        textComponent.font = ARIAL;
        textComponent.lineSpacing = 0;
        textComponent.alignment = TextAnchor.MiddleCenter;
        RectTransform transform = (RectTransform) text.transform;
        transform.localScale = new Vector3(.01f, .01f, 1);
        Vector2 newSize = new Vector2(LayoutUtility.GetPreferredWidth((RectTransform)transform),
           LayoutUtility.GetPreferredHeight((RectTransform)transform));
        transform.sizeDelta = newSize;
        transform.SetParent(Constants.CANVAS.transform);
        return newSize;
    }

    public enum Sex
    {
        Male,
        Female
    }

    public void Move(Vector2 pos)
    {
        curPos = pos;
        RenderParts(pos);
    }

    public Vector2Int GetCurrentTilePosition()
    {
        return new Vector2Int((int)curPos.x, (int)curPos.y);
    }

    public Vector2 GetCurrentPosition()
    {
        return new Vector2(curPos.x, curPos.y);
    }

    public void DoJobProgress()
    {
        if(IsIdle() || curJob.IsDone())
        {
            if (jobs.Count > 0)
            {
                curJob = jobs.Dequeue();
                DoJobProgress();
            }
            else 
            {
                curJob = null;
            }
            return;
        }
        curJob.DoProgress();
    }

    public bool IsIdle()
    {
        return curJob == null;
    }

    public void MoveTo(Vector2Int pos)
    {
        jobs.Enqueue(new MoveJob(this, pos, true));
    }

    public void ChopTree(Vector2Int tree, Vector2Int dropOff)
    {
        jobs.Enqueue(new MoveJob(this, tree, false));
        jobs.Enqueue(new ChopJob(tree));
        jobs.Enqueue(new MoveJob(this, dropOff, true));
        jobs.Enqueue(new DropoffJob(dropOff, Map.Item.Wood));
    }

    public void Sleep(int sleepTime)
    {
        jobs.Enqueue(new WaitJob(sleepTime));
    }
}

