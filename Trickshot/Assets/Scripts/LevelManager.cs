using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Score Thresholds")]
    [SerializeField] private float firstStarPercent = 0.3f;
    [SerializeField] private float secondStarPercent = 0.6f;

    [Header("Score Thresholds")]
    // Set the size to 3 in the Inspector and enter your values (e.g., 1000, 2500, 5000)
    [SerializeField] private int[] starThresholds = new int[] { 0, 0, 0 };

    [Header("UI Star Icons")]
    [SerializeField] private GameObject[] filledStars;

    public Slider slider;


    void Start()
    {
        SetMaxPoints();

        CalculateLevelPoints();
    }

    public void CalculateLevelPoints()
    {
        TargetScript[] targets =
            FindObjectsByType<TargetScript>(FindObjectsSortMode.None);

        int totalPossiblePoints = 0;

        foreach (TargetScript target in targets)
        {
            if (target.data != null)
            {
                totalPossiblePoints += target.data.points;

                
            }
        }

        totalPossiblePoints = Mathf.RoundToInt(totalPossiblePoints * 1.5f);

        // Generate star thresholds from the level's total points
        starThresholds[0] =
            Mathf.RoundToInt(totalPossiblePoints * firstStarPercent);

        starThresholds[1] =
            Mathf.RoundToInt(totalPossiblePoints * secondStarPercent);

        starThresholds[2] = totalPossiblePoints;

        SetMaxPoints();

        Debug.Log("Total possible level points: " + totalPossiblePoints);
    }


    public void UpdateStars(int currentScore)
    {
        for (int i = 0; i < starThresholds.Length; i++)
        {
            filledStars[i].SetActive(
                currentScore >= starThresholds[i]
            );
        }

        SetPoints(currentScore);
    }



    public void SetMaxPoints()
    {
        slider.minValue = 0;
        slider.maxValue = starThresholds[2];
        slider.value = 0;
    }

    public void SetPoints(int points)
    {
        slider.value = points;
    }
}