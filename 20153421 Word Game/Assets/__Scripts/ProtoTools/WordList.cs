using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordList : MonoBehaviour
{
    static WordList _Instance;

    public static WordList Instance
    {
        get
        {
            return _Instance;
        }
    }

    public TextAsset wordListText;

    public int numToParseBeforeYield = 10000;
    public int totalLines;
    public int longWordCount;
    public int wordCount;

    string[] lines;
    List<string> longWords;

    public List<string> LongWords
    {
        get
        {
            return longWords;
        }
    }

    List<string> words;

    public List<string> Words
    {
        get
        {
            return words;
        }
    }

    [Header("Set in inspector")]
    public int wordLengthMin = 3;
    public int wordLengthMax = 7;

    private void Awake()
    {
        _Instance = this;
    }

    public void Init()
    {
        //줄 바꿈으로 분리해서 단어 넣기
        lines = wordListText.text.Split('\n');
        totalLines = lines.Length;

        StartCoroutine("ParseList");
    }

    IEnumerator ParseList()
    {
        string word;
        longWords = new List<string>();
        words = new List<string>();

        for(int currLine = 0 ; currLine<totalLines; currLine++) 
        {
            word = lines[currLine];

            if(word.Length == wordLengthMax)
            {
                longWords.Add(word);
            }

            if(word.Length>=wordLengthMin && word.Length <= wordLengthMax)
            {
                words.Add(word);
            }

            if(currLine % numToParseBeforeYield == 0)
            {
                longWordCount = longWords.Count;
                wordCount = words.Count;

                yield return null;
            }
        }

        //this.gameobject의 모든 함수를 뒤져 찾는다
        gameObject.SendMessage("WordListParseComplete");
    }
}
