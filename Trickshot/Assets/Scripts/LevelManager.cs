using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Score Thresholds")]
    // Set the size to 3 in the Inspector and enter your values (e.g., 1000, 2500, 5000)
    [SerializeField] private int[] starThresholds = new int[] { 1000, 2500, 5000 };

    [Header("UI Star Icons")]
    [SerializeField] private Image[] starIcons; // Drag your 3 UI star images here
    [SerializeField] private Sprite filledStarSprite;
    [SerializeField] private Sprite emptyStarSprite;

    public void UpdateStars(int currentScore)
    {
        for (int i = 0; i < starThresholds.Length; i++)
        {
            if (currentScore >= starThresholds[i])
            {
                starIcons[i].sprite = filledStarSprite;
            }
            else
            {
                starIcons[i].sprite = emptyStarSprite;
            }
        }
    }
}