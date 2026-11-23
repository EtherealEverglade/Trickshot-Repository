using System.Collections;
using System.Threading;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int scoreTotal = 0;
    public int scoreRun = 0;
    public float scoreMultiplier = 1f;

    //public txt wateva
    public int displayScore;

    [SerializeField] private LevelManager levelManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelManager.UpdateStars(scoreTotal);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int targetPoints)
    {
        displayScore = scoreTotal;
        scoreTotal = Mathf.RoundToInt((scoreRun + targetPoints) * scoreMultiplier);

        StartCoroutine(AnimateScoreRoutine());
    }

    public void IncreaseMultiplier()
    {
        scoreMultiplier += .5f;
    }

    public void ResetMultiplier()
    {
        scoreMultiplier = 1f;
    }

    private IEnumerator AnimateScoreRoutine()
    {
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            displayScore = (int)Mathf.Lerp(displayScore, scoreTotal, elapsed / duration);

            levelManager.UpdateStars(displayScore);

            yield return null;
        }
    }

}
