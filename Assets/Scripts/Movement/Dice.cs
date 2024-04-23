using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    Transform[] numbers = new Transform[6];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            numbers[i] = transform.GetChild(i);
        }
    }

    public void Roll()
    {
        transform.rotation = Quaternion.Euler(
            UnityEngine.Random.Range(0f,360f),
            UnityEngine.Random.Range(0f, 360f),
            UnityEngine.Random.Range(0f, 360f)
        );
    }

    public int GetNumberFromRoll()
    {
        float highest = 0f;
        int number = 1;
        for (int i = 0; i < 6; i++)
        {
            if (numbers[i].transform.position.y > highest)
            {
                highest = numbers[i].transform.position.y;
                number = i + 1;
            }
        }
        return number;
    }
}
