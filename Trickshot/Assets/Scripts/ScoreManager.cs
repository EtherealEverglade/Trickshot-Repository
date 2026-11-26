using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public int scoreTotal = 0;
    public int scoreTotalRun = 0;
    public float scoreMultiplier = 1f;

    public TMP_Text ScoreHUD;
    public TMP_Text ScoreTotalHUD;
    public TMP_Text ScoreTotalScreen;
    public TMP_Text multiplier;
    public int displayScore;

    [SerializeField] private LevelManager levelManager;

    [Header("Score Popup")]
    [SerializeField] private ScorePopup scorePopupPrefab;
    [SerializeField] private RectTransform scorePopupSpawnPoint;
    [SerializeField] private RectTransform popupParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelManager.UpdateStars(scoreTotal);
        ScoreRunText();
        ScoreTotalText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int targetPoints)
    {
        displayScore = scoreTotalRun;

        int pointsEarned = Mathf.RoundToInt(
            targetPoints * scoreMultiplier
        );

        scoreTotalRun += pointsEarned;

        ScorePopup popup = Instantiate(
            scorePopupPrefab,
            popupParent
        );

        popup.GetComponent<RectTransform>().position =
            scorePopupSpawnPoint.position;

        popup.Setup(pointsEarned);

        IncreaseMultiplier();

        StartCoroutine(AnimateScoreRoutine());
    }

    public void IncreaseMultiplier()
    {
        if (scoreMultiplier < 3)
        {
            scoreMultiplier += .2f;

            multiplier.text = "x" + scoreMultiplier.ToString();
        }
    }

    public void ResetMultiplier()
    {
        scoreMultiplier = 1f;

        multiplier.text = "x" + scoreMultiplier.ToString();
    }

    private IEnumerator AnimateScoreRoutine()
    {
        float elapsed = 0f;
        float duration = .5f;

        float startPoints = displayScore;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            displayScore = (int)Mathf.Lerp(startPoints, scoreTotalRun, elapsed / duration);

            levelManager.UpdateStars(displayScore + scoreTotal);
            ScoreRunText();

            yield return null;
        }
    }

    public void ScoreRunText()
    {
        ScoreHUD.text = displayScore.ToString();
    }

    public void ScoreTotalText()
    {
        int totalPoints = scoreTotal + scoreTotalRun;

        ScoreTotalHUD.text = totalPoints.ToString();
        ScoreTotalScreen.text = totalPoints.ToString();

        levelManager.SetPoints(totalPoints);
        levelManager.UpdateStars(totalPoints);
    }

    public void ScoreRunReset()
    {
        displayScore = 0;
        
        scoreTotal += scoreTotalRun;

        scoreTotalRun = 0;

        ResetMultiplier();
        ScoreRunText();
        ScoreTotalText();
    }
}
