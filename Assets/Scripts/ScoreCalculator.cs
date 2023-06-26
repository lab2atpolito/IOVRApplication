using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreCalculator
{
    private const int MAX_SCORE_VALUE = 10000;

    public static int CalculateScore(float totalTime, float positionPrecision, float inclinationPrecision)
    {
        int partialScore = Mathf.RoundToInt((MAX_SCORE_VALUE / totalTime) * 100);

        // Normalize the position and inclination precision values to a range of [0, 1]
        float normalizedPositionPrecision = positionPrecision / 100f;
        float normalizedInclinationPrecision = inclinationPrecision / 100f;

        // Calculate the score using the weighted formula
        float floatScore = partialScore * normalizedPositionPrecision * normalizedInclinationPrecision;

        int score = Mathf.RoundToInt(floatScore);
        return score;
    }
}
