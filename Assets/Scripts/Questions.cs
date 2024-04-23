using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;


public class Questions : MonoBehaviour
{
    public List<string> subjects = new List<string>();
    public string nextSubject;

    public List<List<string>> loadedQuestionsUnder12;
    public List<List<string>> loadedQuestionsOver12;

    public TextMeshProUGUI subjectText;
    public TextMeshProUGUI questionText;

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

        /////If a perma-link to raw online text for questions is established(Comment this out if the perma-link method is not used)
        //string url = "https://raw.githubusercontent.com/itcOnlineGaming/TopOfTheClass/main/Resources/12OverEdit.csv?token=GHSAT0AAAAAACGDH72J2B2MPLIPDHLARFZYZHOOGRA";
        //string urlUnder = "https://raw.githubusercontent.com/itcOnlineGaming/TopOfTheClass/main/Resources/Under12.csv?token=GHSAT0AAAAAACGDH72JUEPL6WMPNPXMKVMYZHOOGLA";
        //WWW www = new WWW(url);
        //WWW wwwTwo = new WWW(urlUnder);
        //StartCoroutine(WaitForRequestOver(www));
        //StartCoroutine(WaitForRequestUnder(wwwTwo));
        /////

        /////If questions are not loaded from an up to date perma-link online source(Comment this out if the perma-link method is used)
        
        var loadedOne = Resources.Load<TextAsset>("Under12");
        var loadedTwo = Resources.Load<TextAsset>("12OverEdit");

        loadedQuestionsUnder12 = yutokun.CSVParser.LoadFromString(loadedOne.text);
        loadedQuestionsOver12 = yutokun.CSVParser.LoadFromString(loadedTwo.text);

        SplitQuestionsOver12();
        SplitQuestionsUnder12();
        StartCoroutine("RemoveQuestions");
        /////
        Debug.Log(generalQuestions.Count);
        Debug.Log(geographyQuestions.Count);
        Debug.Log(scienceQuestions.Count);
        Debug.Log(historyQuestions.Count);
        Debug.Log(mathQuestions.Count);
        Debug.Log(englishQuestions.Count);
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

    private IEnumerator RemoveQuestions()
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

    public void SelectQuestionOver12()
    {
        if (nextSubject != "")
        {
            subjectText.text = nextSubject;
            switch (nextSubject)
            {
                case "General":
                    randomQuestion = Random.Range(1, generalQuestions.Count);
                    questionText.text = generalQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = generalQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = generalQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = generalQuestions[randomQuestion].wrongTwo;
                    generalQuestions.RemoveAt(randomQuestion);//Remove from pile after question is asked
                    break;
                case "Geography":
                    randomQuestion = Random.Range(1, geographyQuestions.Count);
                    questionText.text = geographyQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = geographyQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = geographyQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = geographyQuestions[randomQuestion].wrongTwo;
                    geographyQuestions.RemoveAt(randomQuestion);//Remove from pile after question is asked
                    break;
                case "Science":
                    randomQuestion = Random.Range(1, scienceQuestions.Count);
                    questionText.text = scienceQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = scienceQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = scienceQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = scienceQuestions[randomQuestion].wrongTwo;
                    scienceQuestions.RemoveAt(randomQuestion);//Remove from pile after question is asked
                    break;
                case "History":
                    randomQuestion = Random.Range(1, historyQuestions.Count);
                    questionText.text = historyQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = historyQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = historyQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = historyQuestions[randomQuestion].wrongTwo;
                    historyQuestions.RemoveAt(randomQuestion);//Remove from pile after question is asked
                    break;
                case "Maths":
                    randomQuestion = Random.Range(1, mathQuestions.Count);
                    questionText.text = mathQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = mathQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = mathQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = mathQuestions[randomQuestion].wrongTwo;
                    mathQuestions.RemoveAt(randomQuestion);//Remove from pile after question is asked
                    break;
                case "English":
                    randomQuestion = Random.Range(1, englishQuestions.Count);
                    questionText.text = englishQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = englishQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = englishQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = englishQuestions[randomQuestion].wrongTwo;
                    englishQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                default:
                    break;
            }
            Debug.Log(randomQuestion);
        }
        for (int i = 0; i < GetComponent<GameManager>().answerButtons.Length; i++)
        {
            if (GetComponent<GameManager>().answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text == "X")
            {
                GetComponent<GameManager>().answerButtons[i].SetActive(false);
            }
        }
    }

    public void SelectQuestionUnder12()
    {
        if (nextSubject != "")
        {
            subjectText.text = nextSubject;
            switch (nextSubject)
            {
                case "General":
                    randomQuestion = Random.Range(1, under12GeneralQuestions.Count);
                    questionText.text = under12GeneralQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = under12GeneralQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = under12GeneralQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = under12GeneralQuestions[randomQuestion].wrongTwo;
                    under12GeneralQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                case "Geography":
                    randomQuestion = Random.Range(1, under12GeographyQuestions.Count);
                    questionText.text = under12GeographyQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = under12GeographyQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = under12GeographyQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = under12GeographyQuestions[randomQuestion].wrongTwo;
                    under12GeographyQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                case "Science":
                    randomQuestion = Random.Range(1, under12ScienceQuestions.Count);
                    questionText.text = under12ScienceQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = under12ScienceQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = under12ScienceQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = under12ScienceQuestions[randomQuestion].wrongTwo;
                    under12ScienceQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                case "History":
                    randomQuestion = Random.Range(1, under12HistoryQuestions.Count);
                    questionText.text = under12HistoryQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = under12HistoryQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = under12HistoryQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = under12HistoryQuestions[randomQuestion].wrongTwo;
                    under12HistoryQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                case "Maths":
                    randomQuestion = Random.Range(1, under12MathQuestions.Count);
                    questionText.text = under12MathQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = under12MathQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = under12MathQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = under12MathQuestions[randomQuestion].wrongTwo;
                    under12MathQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                case "English":
                    randomQuestion = Random.Range(1, under12EnglishQuestions.Count);
                    questionText.text = under12EnglishQuestions[randomQuestion].question;
                    GetComponent<GameManager>().answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = under12EnglishQuestions[randomQuestion].answer;
                    GetComponent<GameManager>().answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = under12EnglishQuestions[randomQuestion].wrongOne;
                    GetComponent<GameManager>().answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = under12EnglishQuestions[randomQuestion].wrongTwo;
                    under12EnglishQuestions.RemoveAt(randomQuestion); //Remove from pile after question is asked
                    break;
                default:
                    break;
            }
            Debug.Log(randomQuestion);
        }
        for (int i = 0; i < GetComponent<GameManager>().answerButtons.Length; i++)
        {
            if (GetComponent<GameManager>().answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text == "X")
            {
                GetComponent<GameManager>().answerButtons[i].SetActive(false);
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
                //Debug.Log(j.ToString() + data[j].ToString());
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
        for (int i = 0; i < loadedQuestionsUnder12.Count; i++)
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
        for (int i = questionCards.Count-1; i >= 0; i--)
        {
            if (questionCards[i].wrongOne == "X" && questionCards[i].wrongTwo == "X")
            {
                questionCards.RemoveAt(i);
            }
        }
    }

    public void Setup(GameObject questionCard)
    {
        FindQuestionCardVariables(questionCard);
    }

    public void FindQuestionCardVariables(GameObject questionCard)
    {
        if (questionCard != null)
        {
            subjectText = questionCard.transform.Find("QuestionSubject")?.GetComponent<TextMeshProUGUI>();
            questionText = questionCard.transform.Find("QuestionText")?.GetComponent<TextMeshProUGUI>();

            if (subjectText == null || questionText == null)
            {
                Debug.LogError("One or both TextMeshProUGUI components not found on Question Card.");
            }
        }
        else
        {
            Debug.LogError("Question Card not found in the scene.");
        }
    }

}
