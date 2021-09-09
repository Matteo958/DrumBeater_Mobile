using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public enum Precision
    {
        OK,
        GOOD,
        PERFECT
    }

    [Tooltip("The maximum combo multiplier")]
    [SerializeField] private int maxComboMultiplier = 10;
    [Tooltip("The consecutive hits needed to increase combo multiplier")]
    [SerializeField] private int comboMultiplierStep = 5;
    [Tooltip("The consecutive hits needed to activate the auto perfect")]
    [SerializeField] private int autoModeHits = 50;

    [Tooltip("The multiplier for the easy mode")]
    [SerializeField] private float easyMultiplier = 0.5f;
    [Tooltip("The multiplier for the normal mode")]
    [SerializeField] private float normalMultiplier = 1;
    [Tooltip("The multiplier for the hard mode")]
    [SerializeField] private float hardMultiplier = 2;
    [Tooltip("The multiplier for the expert mode")]
    [SerializeField] private float expertMultiplier = 4;

    [Tooltip("The points given by each perfect")]
    [SerializeField] private int perfectPoints = 100;
    [Tooltip("The points given by each good")]
    [SerializeField] private int goodPoints = 75;
    [Tooltip("The points given by each ok")]
    [SerializeField] private int okPoints = 50;

    private float difficultMultiplier = 1;
    private int comboMultiplier = 1;

    private float points = 0;
    private float hitsPercentage = 0;

    private int hits = 0;
    private int miss = 0;
    private int consecutiveHits = 0;

    public static PointsManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;
        }
    }

    public void hitNote(Precision precision)
    {
        switch (precision)
        {
            case Precision.OK:
                points += okPoints * comboMultiplier * difficultMultiplier;
                break;

            case Precision.GOOD:
                points += goodPoints * comboMultiplier * difficultMultiplier;
                break;

            case Precision.PERFECT:
                points += perfectPoints * comboMultiplier * difficultMultiplier;
                break;
        }

        hits++;
        consecutiveHits++;

        if (comboMultiplier < maxComboMultiplier && consecutiveHits % comboMultiplierStep == 0)
            comboMultiplier++;
        if(consecutiveHits == autoModeHits)
        {
            GameManager.instance.hasAutoMode = true;
            // TODO: Modificare UI
        }

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
        // TODO
    }

    public void calculatePercentage()
    {
        hitsPercentage = (hits * 100) / (hits + miss);
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
