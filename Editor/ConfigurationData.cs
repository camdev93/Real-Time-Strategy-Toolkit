using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConfigurationData
{
    public int levelWidth, levelLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    public float roomBottomCornerModifier;
    public float roomTopCornerModifier;
    public int roomOffset;
    public float chanceOfCover;
    public float coverPadding;
}