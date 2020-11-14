using UnityEngine;

public class PointsManager : MonoBehaviour
{
    [SerializeField] private int maxComboMultiplier = 10;
    [SerializeField] private int comboMultiplierStep = 5;

    [SerializeField] private float easyMultiplier = 0.5f;
    [SerializeField] private float normalMultiplier = 1;
    [SerializeField] private float hardMultiplier = 2;
    [SerializeField] private float expertMultiplier = 4;

    [SerializeField] private int pointsPerNote = 100;

    private float difficultMultiplier;
    private int comboMultiplier = 1;

    private float points = 0;
    private float hitsPercentage = 0;

    private int hits = 0;
    private int miss = 0;
    private int consecutiveHits = 0;

    public void hitNote()
    {
        points += pointsPerNote * comboMultiplier * difficultMultiplier;
        hits++;
        consecutiveHits++;

        if (comboMultiplier < maxComboMultiplier && consecutiveHits % comboMultiplierStep == 0)
            comboMultiplier++;

        updateUI();
    }

    public void missNote()
    {
        comboMultiplier = 1;
        consecutiveHits = 0;
        miss++;
        updateUI();
    }

    public void updateUI()
    {

    }

    public void calculatePercentage()
    {
        hitsPercentage = hits * 100 / hits + miss;
    }

    public void reset()
    {
        points = 0;
        hits = 0;
        miss = 0;
        consecutiveHits = 0;
        comboMultiplier = 1;
    }

    public void setEasy()
    {
        difficultMultiplier = easyMultiplier;
    }

    public void setMedium()
    {
        difficultMultiplier = normalMultiplier;
    }

    public void setHard()
    {
        difficultMultiplier = hardMultiplier;
    }

    public void setExpert()
    {
        difficultMultiplier = expertMultiplier;
    }
}
