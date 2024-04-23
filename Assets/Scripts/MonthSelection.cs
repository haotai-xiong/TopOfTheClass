using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonthSelection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectedOnePlayer()
    {
        GetComponent<GameManager>().numOfPlayerMoveToMonth = 1;
    }

    public void SelectedTwoPlayer()
    {
        GetComponent<GameManager>().numOfPlayerMoveToMonth = 2;
    }

    public void SelectedJan()
    {
        GetComponent<GameManager>().selectedMonth = "January Square";
        GetComponent<GameManager>().monthSelected = true;
    }

    public void SelectedFebruary()
    {
        GetComponent<GameManager>().selectedMonth = "February Square";
        GetComponent<GameManager>().monthSelected = true;
    }

    public void SelectedMarch()
    {
        GetComponent<GameManager>().selectedMonth = "March Square";
        GetComponent<GameManager>().monthSelected = true;
    }

    public void SelectedApril() 
    {
        GetComponent<GameManager>().selectedMonth = "April Square";
        GetComponent<GameManager>().monthSelected = true;
    }

    public void SelectedMay()
    {
        GetComponent<GameManager>().selectedMonth = "May Square";
        GetComponent<GameManager>().monthSelected = true;
    }

    public void SelectedJune()
    {
        GetComponent<GameManager>().selectedMonth = "June Square";
        GetComponent<GameManager>().monthSelected = true;
    }

    public void SelectedSeptember()
    {
        GetComponent<GameManager>().selectedMonth = "September Square";
        GetComponent<GameManager>().monthSelected = true;
    }

    public void SelectedOctober()
    {
        GetComponent<GameManager>().selectedMonth = "October Square";
        GetComponent<GameManager>().monthSelected = true;
    }

    public void SelectedNovember()
    {
        GetComponent<GameManager>().selectedMonth = "November Square";
        GetComponent<GameManager>().monthSelected = true;
    }

    public void SelectedDecember()
    {
        GetComponent<GameManager>().selectedMonth = "December Square";
        GetComponent<GameManager>().monthSelected = true;
    }
}
