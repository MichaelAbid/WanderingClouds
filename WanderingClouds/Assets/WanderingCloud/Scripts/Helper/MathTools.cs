using System;

public static class MathTools
{
    public static int SnapAngle(int angle, int startOffset, int stepSize)
    {
        int positiveInt = Math.Abs(angle);

        int quotient = (positiveInt + startOffset) / stepSize;
        int remainder = (positiveInt + startOffset) % stepSize;

        if (remainder >= stepSize / 2f)
            quotient++;

        return Math.Sign(angle) * (quotient * stepSize - startOffset);
    }
    public static int PowerOfTwo(int power)
    {
        //Bitshift to the left = *2
        return 1 << power;
    }
}