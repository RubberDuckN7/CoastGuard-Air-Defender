
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Air_Delta
{
    
    public static class Accelerometer
    {
#if WINDOWS_PHONE
        private static Microsoft.Devices.Sensors.Accelerometer accelerometer = new Microsoft.Devices.Sensors.Accelerometer();
#endif

        private static bool isInitialized = false;

        private static object threadLock = new object();

        private static Vector3 nextValue = new Vector3();

        private static bool isActive = false;

        public static void Initialize()
        {
            if (isInitialized)
            {
                throw new InvalidOperationException("Initialize can only be called once");
            }

#if WINDOWS_PHONE
        
            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Device)
            {
                try
                {
                    accelerometer.ReadingChanged += new EventHandler<Microsoft.Devices.Sensors.AccelerometerReadingEventArgs>(sensor_ReadingChanged);
                    accelerometer.Start();
                    isActive = true;
                }
                catch (Microsoft.Devices.Sensors.AccelerometerFailedException)
                {
                    isActive = false;
                }
            }
            else
            {
                isActive = true;
            }
#endif

            isInitialized = true;
        }

#if WINDOWS_PHONE
        private static void sensor_ReadingChanged(object sender, Microsoft.Devices.Sensors.AccelerometerReadingEventArgs e)
        {
            lock (threadLock)
            {
                nextValue = new Vector3((float)e.X, (float)e.Y, (float)e.Z);
            }
        }
#endif


        public static void Stop()
        {
            accelerometer.Stop();
        }

        public static AccelerometerState GetState()
        {

            if (!isInitialized)
            {
                throw new InvalidOperationException("You must Initialize before you can call GetState");
            }

            Vector3 stateValue = new Vector3();

            if (isActive)
            {
                if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Device)
                {

                    lock (threadLock)
                    {
                        stateValue = nextValue;
                    }
                }
                else
                {
                    KeyboardState keyboardState = Keyboard.GetState();

                    stateValue.Z = -1;

                    if (keyboardState.IsKeyDown(Keys.Left))
                        stateValue.X--;
                    if (keyboardState.IsKeyDown(Keys.Right))
                        stateValue.X++;
                    if (keyboardState.IsKeyDown(Keys.Up))
                        stateValue.Y++;
                    if (keyboardState.IsKeyDown(Keys.Down))
                        stateValue.Y--;

                    stateValue.Normalize();
                }
            }

            return new AccelerometerState(stateValue, isActive);
        }
    }

    public struct AccelerometerState
    {

        public Vector3 Acceleration { get; private set; }

        public bool IsActive { get; private set; }

        public AccelerometerState(Vector3 acceleration, bool isActive)
            : this()
        {
            Acceleration = acceleration;
            IsActive = isActive;
        }

        public override string ToString()
        {
            return string.Format("Acceleration: {0}, IsActive: {1}", Acceleration, IsActive);
        }
    }
}
