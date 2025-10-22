#pragma once
#include "Support.h"
#include <Arduino.h>
#include <ESP32Servo.h>

class StewartPlatform
{
  public:
    //----------------------------------------------------
    // *** Enter the values for your platform/hardware here
    //----------------------------------------------------
    int servoPins[SERVO_COUNT] = {14, 32, 15, 33, 27, 12}; // servo pins
    int servoTrims[SERVO_COUNT] = {0, 0, 0, 0, 0, 0};     // fine tuning trim adjustments
    float hornLength = 14.1f;                              // Servo Horn Length
    float rodLength = 88.06f;                               // Connecting Rod Length
    
    float servoOffset = 0;                                 // Offset angle for "home"
    int backLeftServoIndex = 0;                            // The index of your back left servo (starting at zero)
    int servoOrderDirection = -1;                          // -1 for counter-clockwise, 1 for clockwise
    float servoAxisAngles[SERVO_COUNT] = {LEFT_BACK, RIGHT_BACK, RIGHT_MID, RIGHT_FRONT, LEFT_FRONT, LEFT_MID};

    enum DataModes
    {
        Input_8Bit = 0,
        Input_Float32,
        ASCII
    };
    DataModes dataMode = Input_8Bit;
    bool SHOW_DEBUG = true; // gets set false after one successful cycle

    //----------------------------------------------------------
    // Variables used for input
    //----------------------------------------------------------
    byte platformByteBuffer[SERVO_COUNT]; // byte array for 8 bit mode
    PlatformData platformDataBuffer;      // a custom struct to read in binary data (float 32 mode)

    uint8_t bytesToRead = 8; // 8 or 32 for byte or float mode, gets set by the SetDataMode() method
    //----------------------------------------------------------
    // Servo
    //----------------------------------------------------------
    Servo myServos[SERVO_COUNT];

    // DOFs - This array holds the raw values for the desired position and orientation of the platform
    // [0 = Sway (mm), 1 = Surge (mm), 2 = Heave (mm), 3 = Pitch (rads), 4 = Roll (rads), 5 = Yaw( rads)]
    float DOFs[SERVO_COUNT] = {0, 0, 0, 0, 0, 0};

    //----------------------------------------------------
    // Calculated Values
    //----------------------------------------------------
    float servoAnglesInRadians[SERVO_COUNT] = {0, 0, 0, 0, 0, 0};
    float platformDefaultHeight;
    Vector3 platformDefaultHeightVector;

    //----------------------------------------------------
    // Dynamic (Allocate memory only when needed)
    //----------------------------------------------------
    Vector3 basePoints[SERVO_COUNT];
    Vector3 platformPoints[SERVO_COUNT];

    void InitializePlatform(DataModes newState);
    void AttachServos();
    void SetDataMode(DataModes newState);
    void DefineBasePoints();
    void DefinePlatformPoints();
    void CalculateDefaultHeight();
    bool ParseMessage(byte inByte);
    uint8_t ProcessMessage(char *ckvp);
    bool CalculateServoValues();
    void UpdateServos();
    
    float Sway();
    float Surge();
    float Heave();
    float Pitch();
    float Roll();
    float Yaw();

    float MapRange(float val, float min, float max, float newMin, float newMax);
    void LogVector(String name, Vector3 vector, bool useLineSeparator);
    void LogValue(String name, String value, bool useLineSeparator);
    float GetMagnitude(Vector3 v);
    int Mod(int x, int m);
    Vector3 CrossProduct(Vector3 v1, Vector3 v2);
    Vector3 RotatePoint(Vector3 point, float degX, float degY, float degZ);
    float GetServoAngle(int i, Vector3 difference);
};