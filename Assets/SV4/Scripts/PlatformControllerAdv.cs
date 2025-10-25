using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Linq;

public class PlatformControllerAdv : MonoBehaviour
{
    [SerializeField] Slider[] sliders; // list of references to the sliders
    [SerializeField] bool useSliders = false;

    public enum PlatformModes { Mode_8Bit, Mode_Float32, Mode_ASCII };
    [SerializeField] PlatformModes mode = PlatformModes.Mode_Float32;

    SerialPort serialPort;
    public string comPort;
    public int baudRate;

    bool initialized = false; // a bool to check if this controller has been initialized

    // 6 DOF Axis Order for Simviz Stewart Platform: [Sway, Surge, Heave, Pitch, Roll, Yaw]
    public byte[] byteValues; // six byte values to be sent to the platform (8Bit Mode)
    public float[] floatValues; // six 32bit float values (Float32 mode)

    private string startFrame = "!"; // '!' startFrame character (33) (to indicate the start of a message)
    private string endFrame = "#"; // '#' endFrame character (35) (to indicate the end of a message)

    private float nextSendTimestamp = 0;
    [SerializeField] private float nextSendDelay = 0.01f; // delay in seconds (100 Hz) - increased from 0.02f for smoother motion

    private void Start()
    {
        if (!initialized) { Init(comPort, baudRate); }
    }

    public void Init(string _com, int _baud)
    {
        if (initialized)
        {
            Debug.LogWarning(typeof(PlatformControllerAdv).ToString() + ": is already initialized");
            return;
        }

        initialized = true;

        // Define and set some default values
        comPort = _com;
        baudRate = _baud;
        byteValues = new byte[] { 128, 128, 128, 128, 128, 128 };
        floatValues = new float[] { 0, 0, 0, 0, 0, 0 };

        // Create SerialPort instance(this does not open the connection)
        if (serialPort == null)
        {
            serialPort = new SerialPort(@"\\.\" + comPort); // special port formating to force Unity to recognize ports beyond COM9
            serialPort.BaudRate = baudRate;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.ReadTimeout = 20; // miliseconds
            serialPort.WriteTimeout = 50; // milliseconds - prevent blocking
            serialPort.WriteBufferSize = 4096; // increase buffer for smoother data flow
            serialPort.ReadBufferSize = 4096;
        }

        // Attempt to open the SerialPort and log any errors
        try
        {
            serialPort.Open();
            Debug.Log("Initialize Serial Port: " + comPort);
        }
        catch (System.IO.IOException ex)
        {
            Debug.LogError("Error opening " + comPort + "\n" + ex.Message);
        }

        // Reset sliders, if in use
        if (useSliders) { InitializeSliders(); }

        // Reset platform values
        HomePlatform();
    }

    void Update()
    {
        //PlatformTestCode();

        // if true this will override user set values with slider values
        if (useSliders == true) { UpdateValuesFromSliders(); }

        if (Time.time > nextSendTimestamp)
        {
            SendSerial(); // Send the data out on a fixed timeStamp (0.01s = 100 Hz)
            nextSendTimestamp = Time.time + nextSendDelay; // update time stamp
        }

        // ReadSerial();
    }

    void PlatformTestCode()
    {
        // Input code here for testing platform values or for the bouncing ball assignment
    }

    //-------------
    // MapRange
    //-------------
    // This function will map a value from one range of values to a new range of values.
    // A value of 50 in a range from 0 to 100 can be remapped to a range of 10 to 20
    // which would result in a new value of 15.
    // Syntax: MapRange(inputValue, minimumVal, maximumVal, newMinimumVal, newMaximumVal);
    // Example: float myVal = MapRange(50, 0, 100, 10, 20); // Should equal 15
    public static float MapRange(float val, float oldMin, float oldMax, float newMin, float newMax)
    {
        float slope = (newMax - newMin) / (oldMax - oldMin);
        float newVal = newMin + slope * (val - oldMin);
        return Mathf.Clamp(newVal, Mathf.Min(newMin, newMax), Mathf.Max(newMin, newMax));
    }

    public void SendSerial()
    {
        if (serialPort == null || !serialPort.IsOpen)
        {
            return; // EARLY RETURN if no port open
        }

        if (mode == PlatformModes.Mode_8Bit)
        {
            serialPort.Write(startFrame); // start frame of message
            // Packet Data: 6 bytes (6 bytes)
            serialPort.Write(byteValues, 0, byteValues.Length);
            serialPort.Write(endFrame); // end frame of message
        }
        else if (mode == PlatformModes.Mode_Float32)
        {
            serialPort.Write(startFrame); // start frame of message
            // Packet Data: 6 Floats (24 bytes)
            for (int i = 0; i < floatValues.Length; i++)
            {
                byte[] myBytes = System.BitConverter.GetBytes(floatValues[i]);
                serialPort.Write(myBytes, 0, myBytes.Length);
            }
            serialPort.Write(endFrame); // end frame of message
        }
        else if (mode == PlatformModes.Mode_ASCII)
        {
            serialPort.Write("DOF=");
            string str = "";
            for (int i = 0; i < floatValues.Length; i++)
            {
                str += floatValues[i].ToString("F2") + (i < floatValues.Length - 1 ? "," : "");
            }
            serialPort.Write(str);
            serialPort.Write("\n");
        }
    }

    string stringBuffer = "";
    bool endOfMsg = false;
    void ReadSerial()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    char inputRead = (char)serialPort.ReadChar();

                    if (inputRead == '\r' || inputRead == '\n')
                    {
                        if (stringBuffer.Length > 0)
                        {
                            endOfMsg = true;
                        }
                    }
                    else
                    {
                        stringBuffer += inputRead;
                    }

                    if (endOfMsg)
                    {
                        print(stringBuffer);
                        stringBuffer = "";
                        endOfMsg = false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                //if (ex.InnerException is not System.TimeoutException)
                {
                    print(ex.Message);
                }
            }
        }
    }

    public void HomePlatform()
    {
        // 8 bit int mode (a range from 0 to 255)
        if (mode == PlatformModes.Mode_8Bit)
        {
            for (int i = 0; i < byteValues.Length; i++)
            {
                byteValues[i] = 128;
            }
        }
        // 32 bit float mode
        else if (mode == PlatformModes.Mode_Float32 || mode == PlatformModes.Mode_ASCII)
        {
            for (int i = 0; i < floatValues.Length; i++)
            {
                floatValues[i] = 0;
            }
        }

        if (useSliders) { ResetSliders(); }
        SendSerial();
    }

    #region Slider Code
    void InitializeSliders()
    {
        if (mode == PlatformModes.Mode_8Bit)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].wholeNumbers = true;
                sliders[i].minValue = 0;
                sliders[i].maxValue = 255;
                sliders[i].value = mode == PlatformModes.Mode_8Bit ? 128 : 0;
            }
        }
        else if (mode == PlatformControllerAdv.PlatformModes.Mode_Float32 || mode == PlatformModes.Mode_ASCII)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].wholeNumbers = false;
                sliders[i].minValue = -30;
                sliders[i].maxValue = 30;
                sliders[i].value = mode == PlatformModes.Mode_8Bit ? 128 : 0;
            }
        }
    }

    public void UpdateValuesFromSliders()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            if (mode == PlatformModes.Mode_Float32) { floatValues[i] = sliders[i].value; }
            else if (mode == PlatformModes.Mode_ASCII) { floatValues[i] = sliders[i].value; }
            else if (mode == PlatformModes.Mode_8Bit) { byteValues[i] = (byte)sliders[i].value; }
        }
    }

    public void ResetSliders()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            // reset the sliders to their midpoint,
            // 128 for byte value, or 0 as a float
            sliders[i].value = mode == PlatformModes.Mode_8Bit ? 128 : 0;
        }
    }
    #endregion

    public float Sway
    {
        get { return floatValues[0]; }
        set { floatValues[0] = value; }
    }
    public float Surge
    {
        get { return floatValues[1]; }
        set { floatValues[1] = value; }
    }
    public float Heave
    {
        get { return floatValues[2]; }
        set { floatValues[2] = value; }
    }
    public float Pitch
    {
        get { return floatValues[3]; }
        set { floatValues[3] = value; }
    }
    public float Roll
    {
        get { return floatValues[4]; }
        set { floatValues[4] = value; }
    }
    public float Yaw
    {
        get { return floatValues[5]; }
        set { floatValues[5] = value; }
    }

    // At shutdown, attempt to reset values and close ports   
    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            HomePlatform();
            serialPort.Close();
        }
    }

    // In some cases, a singleton implementation of this controller is very convenient
    // for switching scenes, maintaining persistence, and easy access.

    private static PlatformControllerAdv _singleton;
    public static PlatformControllerAdv singleton
    {
        get
        {
            // check if singleton instance exists
            if (_singleton == null)
            {
                // create a gameobject
                GameObject go = new GameObject("PlatformControllerAdv");
                // mark it to be persistent (not destroyed on scene change)
                DontDestroyOnLoad(go);
                // attach/create the instance of the script
                _singleton = go.AddComponent<PlatformControllerAdv>();
            }

            // return the singleton instance
            return _singleton;
        }
    }
}