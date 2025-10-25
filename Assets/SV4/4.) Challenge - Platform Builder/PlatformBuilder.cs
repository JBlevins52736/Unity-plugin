using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBuilder : MonoBehaviour
{
    float hornLength = 14.1f;
    float rodLength = 88.06f; // 0.) *** Replace the 0 with the length of your rods here

    public bool showRodLines = true;
    public float[] inputValues = { 0, 0, 0, 0, 0, 0 }; //Sway Surge Heave Pitch Roll Yaw
    Vector3[] platformPointOrigins; // defined later
    Vector3[] platformPoints; // defined later
    Vector3[] basePoints; // defined later
    float platformDefaultHeight = 0;

    float actuatorLength; // used to represent the linear distance our servo/actuator can move (in mm)
    LineRenderGroup platformGroup;
    LineRenderGroup baseGroup;
    public Material lineMaterial;

    void Start()
    {
        actuatorLength = hornLength; // This value represents how much each actuator can move

        // 1.) *** fill in all 6 values with your values from solidworks
        basePoints = new Vector3[6];
        basePoints[0] = new Vector3(-22.25f, -58.43f, 0.0f);
        basePoints[1] = new Vector3(22.25f, -58.43f, 0.0f);
        basePoints[2] = new Vector3(61.73f, 9.95f, 0.0f);
        basePoints[3] = new Vector3(39.48f, 48.49f, 0.0f);
        basePoints[4] = new Vector3(-39.48f, 48.49f, 0.0f);
        basePoints[5] = new Vector3(-61.73f, 9.95f, 0.0f);

        // 2.) *** fill in all 6 values with your values from solidworks
        platformPoints = new Vector3[6];
        platformPoints[0] = new Vector3(-46.17f, -38.20f, 0.0f);
        platformPoints[1] = new Vector3(46.17f, -38.20f, 0.0f);
        platformPoints[2] = new Vector3(56.17f, -20.88f, 0.0f);
        platformPoints[3] = new Vector3(10.00f, 59.09f, 0.0f);
        platformPoints[4] = new Vector3(-10.00f, 59.09f, 0.0f);
        platformPoints[5] = new Vector3(-56.17f, -20.88f, 0.0f);

        // Utility Functions and Setup

        // LineRenderGroup is a (custom class) collection of line renderers, vector references, and parameters.
        baseGroup = new LineRenderGroup(ref basePoints, Color.green, lineMaterial, "BaseGroup", true, true, 1, this.transform);
        platformGroup = new LineRenderGroup(ref platformPoints, Color.cyan, lineMaterial, "PlatformGroup", true, true, 1, this.transform);

        platformPointOrigins = new Vector3[6]; // used to store default/identity values
        System.Array.Copy(platformPoints, platformPointOrigins, platformPoints.Length); // copy the values
        UtilityLib.ConvertV3ToCppSyntax(basePoints, "basePoints"); // Logs values for later use when we copy to c++
        UtilityLib.ConvertV3ToCppSyntax(platformPoints, "platformPoints"); // Logs values for later use when we copy to c++

        CalculateDefaultHeight();
    }

    void CalculateDefaultHeight()
    {
        Vector3 hornDirection = new Vector3(1, 0, 0); // back-left servo faces right when flat
        Vector3 hornEndPoint = basePoints[3] + (hornDirection * hornLength);

        // Calculate default height (Pythagorean theorem)
        // c^2 = a^2 + b^2
        // c^2 - b^2 = a^2
        // a = sqrt(c^2 - b^2)
        float sideC = rodLength;
        float sideB = (platformPointOrigins[3] - hornEndPoint).magnitude; // Discuss
        // 3.) *** replace the 0 with the equation to calculate sideA
        float sideA = Mathf.Sqrt(sideC * sideC - sideB * sideB);

        platformDefaultHeight = sideA;

        // Update the z value (up) with our newly calculated height
        for (int i = 0; i < platformPointOrigins.Length; i++)
        {
            platformPointOrigins[i].z = platformDefaultHeight;
        }
    }

    void Update()
    {
        // Remember our inputValues[] array holds the desired values for:
        // Sway [0], Surge[1], Heave [2], Pitch [3], Roll [4], Yaw [5]

        for (int i = 0; i < platformPoints.Length; i++)
        {
            // Apply the input values to the top platform (rotate, translate, and height)

            // 4.) *** Enter the desired values (from inputValues[x]) for pitch, roll, and yaw
            float pitch = inputValues[3] * Mathf.Deg2Rad;    // convert from degrees to radians
            float roll = inputValues[4] * Mathf.Deg2Rad;     // convert from degrees to radians
            float yaw = inputValues[5] * Mathf.Deg2Rad;      // convert from degrees to radians
            platformPoints[i] = RotatePoint(i, pitch, roll, yaw);

            // 5.) *** Translate each point by the input values for sway, surge, and heave
            platformPoints[i] += new Vector3(inputValues[0], inputValues[1], inputValues[2]);

            // 6.) *** Add in the default height
            platformPoints[i].z += platformDefaultHeight; // *** replace zero with our calculated height (platformDefaultHeight);

            // Error check: measure distances and see if the platform is capable
            // of acheiving the desired position/orientation
            if (showRodLines)
            {
                // To determine if the newly positioned platform is in a valid position,
                // we compare the virtual vector length to the actual rod/horn length.

                // Measure the distance (arm/rod length)
                // 7.) *** replace zero with the vector difference magnitude
                float measuredLegnth = (platformPoints[i] - basePoints[i]).magnitude;

                // Check the distance of the requested position compared to the
                // length parameters of our platform (actuator rod or servo/horn)
                if (Mathf.Abs(measuredLegnth - rodLength) < actuatorLength) // delete
                {
                    // Valid length
                    Debug.DrawLine(platformPoints[i], basePoints[i], Color.yellow);
                }
                else
                {
                    // Invalid length
                    Debug.DrawLine(platformPoints[i], basePoints[i], Color.red);
                }
            }
        }

        baseGroup.UpdateLineRenderers();
        platformGroup.UpdateLineRenderers();
    }

    // Angle and Rotation functions
    Vector3 RotatePoint(int pointIndex, float angRadX, float angRadZ, float angRadY)
    {
        Vector3 point = platformPointOrigins[pointIndex];
        float x = point.x * Mathf.Cos(angRadZ) * Mathf.Cos(angRadY) + point.y * (Mathf.Sin(angRadX) * Mathf.Sin(angRadZ) * Mathf.Cos(angRadZ) - Mathf.Cos(angRadX) * Mathf.Sin(angRadY));
        float y = point.x * Mathf.Cos(angRadZ) * Mathf.Sin(angRadY) + point.y * (Mathf.Cos(angRadX) * Mathf.Cos(angRadY) + Mathf.Sin(angRadX) * Mathf.Sin(angRadZ) * Mathf.Sin(angRadY));
        float z = -point.x * Mathf.Sin(angRadZ) + point.y * Mathf.Sin(angRadX) * Mathf.Cos(angRadZ);
        return new Vector3(x, y, z);
    }
}

/* 
Notes:
basePoints = new Vector3[6];
basePoints[0] = {-18.75f, -52.46f, 0.0f};
basePoints[1] = {18.75f, -52.46f, 0.0f};
basePoints[2] = {54.81f, 9.99f, 0.0f};
basePoints[3] = {36.06f, 42.47f, 0.0f};
basePoints[4] = {-36.06f, 42.47f, 0.0f};
basePoints[5] = {-54.81f, 9.99f, 0.0f};
platformPoints = new Vector3[6];
platformPoints[0] = {-33.43f, -26.75f, 0.0f};
platformPoints[1] = {33.43f, -26.75f, 0.0f};
platformPoints[2] = {39.88f, -15.58f, 0.0f};
platformPoints[3] = {6.45f, 42.32f, 0.0f};
platformPoints[4] = {-6.45f, 42.32f, 0.0f};
platformPoints[5] = {-39.88f, -15.58f, 0.0f};
rodLength = 79.4f // instructor platform
Mathf.Sqrt(sideC * sideC - sideB * sideB)
inputValues[3] * Mathf.Deg2Rad, inputValues[5] * Mathf.Deg2Rad, inputValues[4] * Mathf.Deg2Rad
inputValues[0], inputValues[1], inputValues[2]
platformDefaultHeight
(platformPoints[i] - basePoints[i]).magnitude;
*/