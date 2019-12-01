using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI; 

public enum GameMode
{
    preGame,
    loading,
    makeLevel,
    levelPrep,
    inLevel
}

public class WordGame : MonoBehaviour
{
    static WordGame _Instance;

    public static WordGame Instance
    {
        get
        {
            return _Instance;
        }
    }

    public GameMode mode = GameMode.preGame;
    public WordLevel currLevel;
    public GameObject prefabLetter;
    public Rect wordArea = new Rect(-24, 19, 48, 28);
    public float letterSize = 1.5f;
    public float bigLetterSize = 4f;
    public Color bigColorDim = new Color(0.8f, 0.8f, 0.8f);
    public Color bigColorSelected = Color.white;
    public Vector3 bigLetterCenter = new Vector3(0, -16, 0);

    public bool showAllWyrds = true;

    public List<Wyrd> wyrds;
    public List<Letter> bigLetters;
    public List<Letter> bigLettersActive;

    public List<float> scoreFontSizes = new List<float> { 36, 64, 64, 1 };
    public Vector3 scoreMidPoint = new Vector3(1, 1, 0);
    public float scoreComboDelay = 0.5f;

    public Color[] wyrdPalette;

    private void Awake()
    {
        _Instance = this;
    }

    private void Start()
    {
        mode = GameMode.loading;
        WordList.Instance.Init();
    }

    public string testWord;
    string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private void Update()
    {
        Letter lett;
        char c;

        switch (mode)
        {
            case GameMode.inLevel:
                foreach (var s in Input.inputString)
                {
                    c = System.Char.ToUpperInvariant(s);

                    if (upperCase.Contains(c))
                    {
                        lett = FindNextLetterByChar(c);

                        if(lett != null)
                        {
                            testWord += c.ToString();
                            bigLettersActive.Add(lett);
                            bigLetters.Remove(lett);
                            lett.color = bigColorSelected;
                            ArrangeBigLetters();
                        }
                    }

                    if (c == '\b')
                    {
                        if (bigLettersActive.Count == 0) return;
                        if (testWord.Length > 1)
                        {
                            testWord = testWord.Substring(0, testWord.Length - 1);
                        }

                        else
                        {
                            testWord = "";
                        }

                        lett = bigLettersActive[bigLettersActive.Count - 1];
                        bigLettersActive.Remove(lett);
                        bigLetters.Add(lett);
                        lett.color = bigColorDim;
                        ArrangeBigLetters();
                    }

                    if (c == '\n' || c == '\r')
                    {
                        StartCoroutine(CheckWord());
                    }

                    if(c==' ')
                    {
                        bigLetters = ShuffleLetters(bigLetters);
                        ArrangeBigLetters();
                    }
                }

                break;
        }
    }

    Letter FindNextLetterByChar(char c)
    {
        foreach (var letter in bigLetters)
        {
            if(letter.c == c)
            {
                return letter;
            }
        }

        return null;
    }

    public IEnumerator CheckWord()
    {
        string subWord;
        bool foundTestWord = false;

        List<int> containedWords = new List<int>();

        for(int i = 0; i < currLevel.subWords.Count; i++)
        {
            if (wyrds[i].found)
            {
                continue;
            }

            subWord = currLevel.subWords[i];

            if (string.Equals(testWord, subWord))
            {
                HighlightWyrd(i);
                Score(wyrds[i], 1);
                foundTestWord = true;
            }

            else if (testWord.Contains(subWord))
            {
                containedWords.Add(i);
            }
        }

        if (foundTestWord)
        {
            int numContained = containedWords.Count;
            int idx;

            for(int i = 0; i < containedWords.Count; i++)
            {
                yield return new WaitForSeconds(scoreComboDelay);

                idx = numContained - i - 1;
                HighlightWyrd(containedWords[idx]);
                Score(wyrds[containedWords[idx]], i+2);
            }
        }

        ClearBigLettersActive();
    }

    void Score(Wyrd wyrd,int combo)
    {
        Vector3 pt = wyrd.letters[0].transform.position;
        List<Vector2> pts = new List<Vector2>();

        pt = Camera.main.WorldToViewportPoint(pt);

        pts.Add(pt);
        pts.Add(scoreMidPoint);
        pts.Add(Scoreboard.S.transform.position);

        int value = wyrd.letters.Count * combo;
        FloatingScore fs = Scoreboard.S.CreateFloatingScore(value, pts);

        fs.timeDuration = 2f;
        fs.fontSizes = scoreFontSizes;

        fs.easingCurve = Easing.InOut + Easing.InOut;

        string txt = wyrd.letters.Count.ToString();

        if(combo > 1)
        {
            txt += " x " + combo;
        }

        fs.GetComponent<Text>().text = txt;
    }

    void HighlightWyrd(int idx)
    {
        wyrds[idx].found = true;
        wyrds[idx].color = (wyrds[idx].color + Color.white) / 2f;
        wyrds[idx].visible = true;
    }

    void ClearBigLettersActive()
    {
        testWord = "";
        foreach (var letter in bigLettersActive)
        {
            bigLetters.Add(letter);
            letter.color = bigColorDim;
        }

        bigLettersActive.Clear();
        ArrangeBigLetters();
    }

    public void WordListParseComplete()
    {
        mode = GameMode.makeLevel;
        currLevel = MakeWordLevel();
    }

    WordLevel MakeWordLevel(int levelNum = -1)
    {
        WordLevel level = new WordLevel();

        if (levelNum == -1) level.longWordIndex = Random.Range(0, WordList.Instance.longWordCount);
        
        else
        {
            //you can add level 
        }

        level.levelNum = levelNum;
        level.word = WordList.Instance.LongWords[level.longWordIndex];
        level.charDict = WordLevel.MakeCharDict(level.word);

        StartCoroutine("FindSubWordsCoroutine", level);

        return level;
    }

    IEnumerator FindSubWordsCoroutine(WordLevel level)
    {
        level.subWords = new List<string>();
        string str;

        List<string> words = WordList.Instance.Words;

        for(int i = 0; i < words.Count ; i++)
        {
            str = words[i];

            if (WordLevel.CheckWordInLevel(str, level))
            {
                level.subWords.Add(str);
            }

            if(i%WordList.Instance.numToParseBeforeYield == 0)
            {
                yield return null;
            }
        }

        level.subWords.Sort();
        level.subWords = SortWordsByLength(level.subWords).ToList();

        SubWordSearchComplete();
    }

    public static IEnumerable<string> SortWordsByLength(IEnumerable<string> e)
    {
        //linq
        var sorted = from s in e
                     orderby s.Length ascending
                     select s;

        return sorted;
    }

    void SubWordSearchComplete()
    {
        mode = GameMode.levelPrep;
        Layout();
    }

    void Layout()
    {
        wyrds = new List<Wyrd>();

        GameObject go;
        Letter lett;
        string word;
        Vector3 pos;
        float left = 0;
        float columnWidth = 3;
        char c;
        Color col;
        Wyrd wyrd;
        int numRows = Mathf.RoundToInt(wordArea.height / letterSize);

        for(int i = 0; i < currLevel.subWords.Count; i++)
        {
            wyrd = new Wyrd();
            word = currLevel.subWords[i];

            columnWidth = Mathf.Max(columnWidth, word.Length);

            for(int j = 0; j < word.Length; j++)
            {
                c = word[j];
                go = Instantiate<GameObject>(prefabLetter);
                lett = go.GetComponent<Letter>();
                lett.c = c;
                pos = new Vector3(wordArea.x + left
                    + j * letterSize, wordArea.y, 0);
                pos.y -= (i % numRows) * letterSize;

                lett.position = pos + Vector3.up * (20 + i % numRows);
                lett.pos = pos;
                lett.timeStart = Time.time + i * 0.05f;

                lett.pos = pos;
                go.transform.localScale = Vector3.one * letterSize;
                wyrd.Add(lett);
            }

            if (showAllWyrds) wyrd.visible = true;
            wyrd.color = wyrdPalette[word.Length - WordList.Instance.wordLengthMin];
            wyrds.Add(wyrd);
            if((i%numRows) == numRows - 1)
            {
                left += (columnWidth + 0.5f) * letterSize;
            }
        }

        bigLetters = new List<Letter>();
        bigLettersActive = new List<Letter>();

        for(int i = 0; i < currLevel.word.Length; i++)
        {
            c = currLevel.word[i];
            go = Instantiate<GameObject>(prefabLetter);
            lett = go.GetComponent<Letter>();
            lett.c = c;
            go.transform.localScale = Vector3.one * bigLetterSize;

            pos = new Vector3(0, -100, 0);
            lett.position = pos;

            lett.timeStart = Time.time + currLevel.subWords.Count * 0.05f;
            lett.easingCurve = Easing.Sin + "-0.18";

            col = bigColorDim;
            lett.color = col;
            lett.visible = true;
            bigLetters.Add(lett);
        }

        bigLetters = ShuffleLetters(bigLetters);
        ArrangeBigLetters();

        mode = GameMode.inLevel;
            
    }

    List<Letter> ShuffleLetters(List<Letter> letters)
    {
        List<Letter> newL = new List<Letter>();

        int idx;

        while(letters.Count > 0)
        {
            idx = Random.Range(0, letters.Count);
            newL.Add(letters[idx]);
            letters.RemoveAt(idx);
        }

        return newL;
    }

    void ArrangeBigLetters()
    {
        float halfWidth = ((float)bigLetters.Count) / 2f - 0.5f;
        Vector3 pos;

        for (int i = 0; i < bigLetters.Count; i++)
        {
            pos = bigLetterCenter;
            pos.x += (i - halfWidth) * bigLetterSize;
            bigLetters[i].pos = pos;
        }

        halfWidth = ((float)bigLettersActive.Count) / 2f - 0.5f;

        for (int i = 0; i < bigLettersActive.Count; i++)
        {
            pos = bigLetterCenter;
            pos.x += (i - halfWidth) * bigLetterSize;
            pos.y += bigLetterSize * 1.25f;
            bigLettersActive[i].pos = pos;
        }
    }
}
