using System;
using UnityEngine;

internal struct ClockDirections
{
    internal static void MoveArrow(DateTime currentDT, Transform hourArrow, Transform minuteArrow, Transform secArrow){
        float hourPos = 360f / 12f;
        float minutePos = 360f / 60f;
        float secPos = minutePos;

        hourArrow.localRotation = Quaternion.Euler(0f, 0f, currentDT.Hour * -hourPos);
        minuteArrow.localRotation = Quaternion.Euler(0f, 0f, currentDT.Minute * -minutePos);
        secArrow.localRotation = Quaternion.Euler(0f, 0f, currentDT.Second * -secPos);
    }
}
