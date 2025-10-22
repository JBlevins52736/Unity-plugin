#include "StewartPlatform.h"
#include <ESP32Servo.h>

StewartPlatform stewartPlatform; // main stewart platform class

//----------------------------------------------------
// Setup
//----------------------------------------------------
void setup()
{
    Serial.begin(115200);
    while (!Serial && millis() < 4000)
    {
        delay(1); // wait for Serial
    }

    stewartPlatform.DefineBasePoints();                                 // The x and y points for each base attachment (center point of the servo horn attachment)
    stewartPlatform.DefinePlatformPoints();                             // The x and y points for each platform attachment (center of ball joint on upper rod attachment)
    stewartPlatform.CalculateDefaultHeight();                          // Mathematically determine the default height (all servos horns horizontal at 90 degrees)
    stewartPlatform.InitializePlatform(StewartPlatform::Input_Float32); // operation mode
}

void loop()
{
    if (Serial.available() > 0)
    {
        byte inByte = Serial.read(); // Read one byte at a time from the serial port
        if (stewartPlatform.ParseMessage(inByte)) // Pass bytes in until a completed messages is received (returns true)
            if (stewartPlatform.CalculateServoValues()) // This will return true if the incoming values are calculated to be in range
                stewartPlatform.UpdateServos(); // Updates the servos to the new position based on the above
    }
}