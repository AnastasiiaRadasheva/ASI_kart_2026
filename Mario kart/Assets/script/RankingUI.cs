using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class RankingUI : MonoBehaviour
{
    public TextMeshProUGUI rankingText;

    private List<CarProgress> cars;

    void Start()
    {
        cars = FindObjectsOfType<CarProgress>().ToList();
    }

    void Update()
    {
        cars = cars
            .OrderByDescending(c => c.GetProgress())
            .ToList();

        string text = "";

        for (int i = 0; i < cars.Count; i++)
        {
            text += (i + 1) + ". " + cars[i].gameObject.name + "\n";
        }

        rankingText.text = text;
    }
}
