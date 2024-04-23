using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using Unity.Netcode;
using Unity.Collections;

public class OnlineQuestions : NetworkBehaviour
{
    public List<string> subjects = new List<string>();

    public List<List<string>> loadedQuestionsUnder12;
    public List<List<string>> loadedQuestionsOver12;

    public NetworkVariable<FixedString128Bytes> nextSubject = new NetworkVariable<FixedString128Bytes>("");

    public int randomQuestion = 0;

    [Header("Over Age of 12 Lists")]
    public List<QuestionCard> generalQuestions = new List<QuestionCard>();
    public List<QuestionCard> geographyQuestions = new List<QuestionCard>();
    public List<QuestionCard> scienceQuestions = new List<QuestionCard>();
    public List<QuestionCard> historyQuestions = new List<QuestionCard>();
    public List<QuestionCard> mathQuestions = new List<QuestionCard>();
    public List<QuestionCard> englishQuestions = new List<QuestionCard>();

    [Header("Under Age of 12 Lists")]
    public List<QuestionCard> under12GeneralQuestions = new List<QuestionCard>();
    public List<QuestionCard> under12GeographyQuestions = new List<QuestionCard>();
    public List<QuestionCard> under12ScienceQuestions = new List<QuestionCard>();
    public List<QuestionCard> under12HistoryQuestions = new List<QuestionCard>();
    public List<QuestionCard> under12MathQuestions = new List<QuestionCard>();
    public List<QuestionCard> under12EnglishQuestions = new List<QuestionCard>();


    public struct QuestionCard
    {
        public string question;
        public string answer;
        public string wrongOne;
        public string wrongTwo;
    }

    // Start is called before the first frame update
    void Start()
    {
        subjects.Add("General");
        subjects.Add("Geography");
        subjects.Add("Science");
        subjects.Add("History");
        subjects.Add("Maths");
        subjects.Add("English");

        /////If a perma-link to raw online text for questions is established
        //string url = "https://raw.githubusercontent.com/itcOnlineGaming/TopOfTheClass/main/Resources/12OverEdit.csv?token=GHSAT0AAAAAACGDH72J2B2MPLIPDHLARFZYZHOOGRA";
        //string urlUnder = "https://raw.githubusercontent.com/itcOnlineGaming/TopOfTheClass/main/Resources/Under12.csv?token=GHSAT0AAAAAACGDH72JUEPL6WMPNPXMKVMYZHOOGLA";
        //WWW www = new WWW(url);
        //WWW wwwTwo = new WWW(urlUnder);
        //StartCoroutine(WaitForRequestOver(www));
        //StartCoroutine(WaitForRequestUnder(wwwTwo));
        /////

        /////If questions are not loaded from an up to date perma-link online source
        
        var loadedOne = Resources.Load<TextAsset>("Under12");
        var loadedTwo = Resources.Load<TextAsset>("12OverEdit");

        loadedQuestionsUnder12 = yutokun.CSVParser.LoadFromString(loadedOne.text);
        loadedQuestionsOver12 = yutokun.CSVParser.LoadFromString(loadedTwo.text);

        SplitQuestionsOver12();
        SplitQuestionsUnder12();
        StartCoroutine("RemoveQuestions");
        /////
    }

    IEnumerator WaitForRequestOver(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            Debug.Log("Content : " + www.text);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }

        loadedQuestionsOver12 = yutokun.CSVParser.LoadFromString(www.text);
        SplitQuestionsOver12();
    }

    IEnumerator WaitForRequestUnder(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            Debug.Log("Content : " + www.text);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }

        loadedQuestionsUnder12 = yutokun.CSVParser.LoadFromString(www.text);
        SplitQuestionsUnder12();
        StartCoroutine("TimeRemoveQuestions");
    }

    IEnumerator RemoveQuestions()
    {
        DiscardQuestions(ref generalQuestions);
        DiscardQuestions(ref geographyQuestions);
        DiscardQuestions(ref scienceQuestions);
        DiscardQuestions(ref historyQuestions);
        DiscardQuestions(ref mathQuestions);
        DiscardQuestions(ref englishQuestions);

        DiscardQuestions(ref under12GeneralQuestions);
        DiscardQuestions(ref under12GeographyQuestions);
        DiscardQuestions(ref under12ScienceQuestions);
        DiscardQuestions(ref under12HistoryQuestions);
        DiscardQuestions(ref under12MathQuestions);
        DiscardQuestions(ref under12EnglishQuestions);
        yield break;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SelectQuestionOver12ServerRpc()
    {
        if (nextSubject.Value != "")
        {
            GetComponent<OnlineGameManager>().subjectTextNetString.Value = nextSubject.Value;
            switch (nextSubject.Value.ToString())
            {
                case "General":
                    randomQuestion = Random.Range(1, generalQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = generalQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();
                    GetComponent<OnlineGameManager>().answerTextOne.Value = generalQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = generalQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = generalQuestions[randomQuestion].wrongTwo;
                    generalQuestions.RemoveAt(randomQuestion);//Remove from pile after question is asked
                    break;
                case "Geography":
                    randomQuestion = Random.Range(1, geographyQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = geographyQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();
                    GetComponent<OnlineGameManager>().answerTextOne.Value = geographyQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = geographyQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = geographyQuestions[randomQuestion].wrongTwo;
                    geographyQuestions.RemoveAt(randomQuestion);//Remove from pile after question is asked
                    break;
                case "Science":
                    randomQuestion = Random.Range(1, scienceQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = scienceQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();
                    GetComponent<OnlineGameManager>().answerTextOne.Value = scienceQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = scienceQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = scienceQuestions[randomQuestion].wrongTwo;
                    scienceQuestions.RemoveAt(randomQuestion);//Remove from pile after question is asked
                    break;
                case "History":
                    randomQuestion = Random.Range(1, historyQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = historyQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();

                    GetComponent<OnlineGameManager>().answerTextOne.Value = historyQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = historyQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = historyQuestions[randomQuestion].wrongTwo;
                    historyQuestions.RemoveAt(randomQuestion);//Remove from pile after question is asked
                    break;
                case "Maths":
                    randomQuestion = Random.Range(1, mathQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = mathQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();
                    GetComponent<OnlineGameManager>().answerTextOne.Value = mathQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = mathQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = mathQuestions[randomQuestion].wrongTwo;
                    mathQuestions.RemoveAt(randomQuestion);//Remove from pile after question is asked
                    break;
                case "English":
                    randomQuestion = Random.Range(1, englishQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = englishQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();
                    GetComponent<OnlineGameManager>().answerTextOne.Value = englishQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = englishQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = englishQuestions[randomQuestion].wrongTwo;
                    englishQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                default:
                    break;
            }
        }
        for (int i = 0; i < GetComponent<OnlineGameManager>().answerButtons.Length; i++)
        {
            if (GetComponent<OnlineGameManager>().answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text == "X")
            {
                GetComponent<OnlineGameManager>().answerButtons[i].SetActive(false);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SelectQuestionUnder12ServerRpc()
    {
        if (nextSubject.Value != "")
        {
            GetComponent<OnlineGameManager>().subjectTextNetString.Value = nextSubject.Value;
            switch (nextSubject.Value.ToString())
            {
                case "General":
                    randomQuestion = Random.Range(1, under12GeneralQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = under12GeneralQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();
                    GetComponent<OnlineGameManager>().answerTextOne.Value = under12GeneralQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = under12GeneralQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = under12GeneralQuestions[randomQuestion].wrongTwo;
                    under12GeneralQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                case "Geography":
                    randomQuestion = Random.Range(1, under12GeographyQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = under12GeographyQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();
                    GetComponent<OnlineGameManager>().answerTextOne.Value = under12GeographyQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = under12GeographyQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = under12GeographyQuestions[randomQuestion].wrongTwo;
                    under12GeographyQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                case "Science":
                    randomQuestion = Random.Range(1, under12ScienceQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = under12ScienceQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();
                    GetComponent<OnlineGameManager>().answerTextOne.Value = under12ScienceQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = under12ScienceQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = under12ScienceQuestions[randomQuestion].wrongTwo;
                    under12ScienceQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                case "History":
                    randomQuestion = Random.Range(1, under12HistoryQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = under12HistoryQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();
                    GetComponent<OnlineGameManager>().answerTextOne.Value = under12HistoryQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = under12HistoryQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = under12HistoryQuestions[randomQuestion].wrongTwo;
                    under12HistoryQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                case "Maths":
                    randomQuestion = Random.Range(1, under12MathQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = under12MathQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();
                    GetComponent<OnlineGameManager>().answerTextOne.Value = under12MathQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = under12MathQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = under12MathQuestions[randomQuestion].wrongTwo;
                    under12MathQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                case "English":
                    randomQuestion = Random.Range(1, under12EnglishQuestions.Count);
                    GetComponent<OnlineGameManager>().questionTextNetString.Value = under12EnglishQuestions[randomQuestion].question;
                    GetComponent<OnlineGameManager>().questionText.text = GetComponent<OnlineGameManager>().questionTextNetString.Value.ToString();
                    GetComponent<OnlineGameManager>().answerTextOne.Value = under12EnglishQuestions[randomQuestion].answer;
                    GetComponent<OnlineGameManager>().answerTextTwo.Value = under12EnglishQuestions[randomQuestion].wrongOne;
                    GetComponent<OnlineGameManager>().answerTextThree.Value = under12EnglishQuestions[randomQuestion].wrongTwo;
                    under12EnglishQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                default:
                    break;
            }
        }
        for (int i = 0; i < GetComponent<OnlineGameManager>().answerButtons.Length; i++)
        {
            if (GetComponent<OnlineGameManager>().answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text == "X")
            {
                GetComponent<OnlineGameManager>().answerButtons[i].SetActive(false);
            }
        }
    }

    public List<string> LoadFromOnline(string path)
    {
        //Load Text File
        int length = 0;
        List<string> text = new List<string>();

        foreach (var line in path)
        {
            text.Add(line.ToString());
            length++;
        }

        return text;
    }

    public void SplitQuestionsOver12()
    {
        QuestionCard card = new QuestionCard();
        card.question = "";
        card.answer = "";
        card.wrongOne = "";
        card.wrongTwo = "";
        int counter = 0;

        for (int i = 0; i < loadedQuestionsOver12.Count; i++)
        {
            for (int j = 1; j < 25; j++)
            {
                if (j == 1 || j == 5 || j == 9 || j == 13 || j == 17 || j == 21)
                {
                    card.question = loadedQuestionsOver12[i][j];
                }
                else if (!loadedQuestionsOver12[i][j].Contains("["))
                {
                    counter++;
                    if (counter < 2)
                    {
                        card.wrongOne = loadedQuestionsOver12[i][j];
                    }
                    else
                    {
                        card.wrongTwo = loadedQuestionsOver12[i][j];
                    }
                }
                else if (loadedQuestionsOver12[i][j].Contains("["))
                {
                    card.answer = loadedQuestionsOver12[i][j];
                    card.answer = card.answer.Replace("[", "");
                    card.answer = card.answer.Replace("]", "");
                }
                if (counter == 2)
                {
                    counter = 0;
                }

                switch (j)
                {
                    case 4:
                        //General
                        generalQuestions.Add(card);
                        break;
                    case 8:
                        //Geography
                        geographyQuestions.Add(card);
                        break;
                    case 12:
                        //Science
                        scienceQuestions.Add(card);
                        break;
                    case 16:
                        //History
                        historyQuestions.Add(card);
                        break;
                    case 20:
                        //Maths
                        mathQuestions.Add(card);
                        break;
                    case 24:
                        //English
                        englishQuestions.Add(card);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void SplitQuestionsUnder12()
    {
        QuestionCard card = new QuestionCard();
        card.question = "";
        card.answer = "";
        card.wrongOne = "";
        card.wrongTwo = "";
        int counter = 0;
        for (int i = 0; i < loadedQuestionsUnder12.Count - 1; i++)
        {
            for (int j = 1; j < 25; j++)
            {
                if (j == 1 || j == 5 || j == 9 || j == 13 || j == 17 || j == 21)
                {
                    card.question = loadedQuestionsUnder12[i][j].ToString();
                }
                else if (!loadedQuestionsUnder12[i][j].Contains("["))
                {
                    counter++;
                    if (counter < 2)
                    {
                        card.wrongOne = loadedQuestionsUnder12[i][j].ToString();
                    }
                    else
                    {
                        card.wrongTwo = loadedQuestionsUnder12[i][j].ToString();
                    }
                }
                else if (loadedQuestionsUnder12[i][j].Contains("["))
                {
                    card.answer = loadedQuestionsUnder12[i][j].ToString();
                    card.answer = card.answer.Replace("[", "");
                    card.answer = card.answer.Replace("]", "");
                }
                if (counter == 2)
                {
                    counter = 0;
                }

                switch (j)
                {
                    case 4:
                        //General
                        under12GeneralQuestions.Add(card);
                        break;
                    case 8:
                        //Geography
                        under12GeographyQuestions.Add(card);
                        break;
                    case 12:
                        //Science
                        under12ScienceQuestions.Add(card);
                        break;
                    case 16:
                        //History
                        under12HistoryQuestions.Add(card);
                        break;
                    case 20:
                        //Maths
                        under12MathQuestions.Add(card);
                        break;
                    case 24:
                        //English
                        under12EnglishQuestions.Add(card);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void DiscardQuestions(ref List<QuestionCard> questionCards) //Remove any non-viable questions (non mulitple answer questions)
    {
        questionCards.RemoveAt(0);
        for (int i = questionCards.Count - 1; i >= 0; i--)
        {
            if (questionCards[i].wrongOne == "X" && questionCards[i].wrongTwo == "X")
            {
                questionCards.RemoveAt(i);
            }
        }
    }
}
