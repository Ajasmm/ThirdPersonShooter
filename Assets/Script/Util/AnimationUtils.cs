using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationUtils
{
    public static float Increase(float currentValue, float targetValue, float incrementAmount)
    {
        if (currentValue == targetValue) return targetValue;

        if(currentValue < targetValue)
        {
            currentValue += incrementAmount;
            if (currentValue > targetValue) 
                currentValue = targetValue;
        }
        else
        {
            currentValue -= incrementAmount;
            if(currentValue < targetValue)
                currentValue= targetValue;
        }

        return currentValue;
    }
    
}
