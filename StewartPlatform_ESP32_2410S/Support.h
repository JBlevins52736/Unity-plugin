#include <Arduino.h>
#pragma once

//----------------------------------------------------------
// Constants
//----------------------------------------------------------
#define FRAME_START '!' // ASCII char (byte number 33)
#define FRAME_END '#'   // ASCII char (byte number 35)
#define ANGLE_MIN 30
#define ANGLE_MAX 150
#define D2R PI / 180
#define R2D 180 / PI
#define SERVO_COUNT 6
#define FLOAT_BYTE_SIZE 4

#define RIGHT_FRONT 300
#define RIGHT_MID 120
#define RIGHT_BACK 180
#define LEFT_BACK 0
#define LEFT_MID 60
#define LEFT_FRONT 240

//----------------------------------------------------------
// Custom Data Types
//----------------------------------------------------------

// This union is common way in c++ to convert between binary
// and floating point values. Unions share the same memory
// but can be defined as different types as long as they
// are the same size. (Little Endian - least significant bit first)
typedef union
{
    float floatValues[SERVO_COUNT];
    byte binaryValues[SERVO_COUNT * FLOAT_BYTE_SIZE];
} PlatformData;

// Basic Vector3 structure for use with the Stewart platform
struct Vector3
{
    float x;
    float y;
    float z;

    Vector3(float _x = 0.0f, float _y = 0.0f, float _z = 0.0f)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    Vector3 operator+(const Vector3 &a) const
    {
        return Vector3(x + a.x, y + a.y, z + a.z);
    }
    Vector3 operator-(const Vector3 &a) const
    {
        return Vector3(x - a.x, y - a.y, z - a.z);
    }
    Vector3 operator*(const float &a) const
    {
        return Vector3(a * x, a * y, a * z);
    }
    Vector3 operator+=(const Vector3 &a)
    {
        x = x + a.x;
        y = y + a.y;
        z = z + a.z;
        return Vector3(x, y, z);
    }
};