using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static Dictionary<string, Transform> items = new Dictionary<string, Transform>();

    private void Awake()
    {
        items.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            items.Add(transform.GetChild(i).name, transform.GetChild(i).transform);
        }
    }
}
