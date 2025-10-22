using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StewartPlatformUtility : MonoBehaviour
{
    [SerializeField] float platformSideLength; // top triangle side length
    [SerializeField] float platformPointSpacing; // top spacing between points
    [SerializeField] float baseSideLength; // bottom triangle side length
    [SerializeField] float basePointSpacing; // bottom spacing between points

    void Start()
    {
        CalcualteTopPoints(platformSideLength, platformPointSpacing);
        CalcualteBasePoints(baseSideLength, basePointSpacing);
    }
    
    public Vector3[] CalcualteBasePoints(float sideLength, float spacing)
    {
        float circumradius, angle;
        UtilityLib.GetRadiusAndOffset(sideLength, spacing, out circumradius, out angle);
        Vector3[] points = UtilityLib.CalculateBasePoints(circumradius, angle);
        UtilityLib.ConvertV3ToCppSyntax(points, "basePoints");
        UtilityLib.ConvertV3ToCSharpSyntax(points, "basePoints");
        return points;
    }

    public Vector3[] CalcualteTopPoints(float sideLength, float spacing)
    {
        float circumradius, angle;
        UtilityLib.GetRadiusAndOffset(sideLength, spacing, out circumradius, out angle);
        Vector3[] points = UtilityLib.CalculatePlatformPoints(circumradius, angle);
        UtilityLib.ConvertV3ToCppSyntax(points, "platformPoints");
        UtilityLib.ConvertV3ToCSharpSyntax(points, "platformPoints");
        return points;
    }
}
