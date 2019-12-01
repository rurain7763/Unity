using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wyrd
{
    public string str;
    public List<Letter> letters = new List<Letter>();
    public bool found = false;

    public bool visible
    {
        get
        {
            if (letters.Count == 0) return false;
            return letters[0].visible;
        }

        set
        {
            foreach (var letter in letters)
            {
                letter.visible = value;
            }
        }
    }

    public Color color
    {
        get
        {
            if (letters.Count == 0) return Color.black;
            return letters[0].color;
        }

        set
        {
            foreach (var letter in letters)
            {
                letter.color = value;
            }
        }
    }

    public void Add(Letter letter)
    {
        letters.Add(letter);
        str += letter.c.ToString();
    }
}
