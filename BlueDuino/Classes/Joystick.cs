using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BlueDuino.Classes
{
    class Joystick
    {
        private Vector2 joystick;
        private double _max;
        private long _old;
        private const long UpdateTime = 500000;
        private bool writing;

        public Joystick(double maxValue)
        {
            _max = maxValue;
            joystick = new Vector2(0, 0);
        }

        public void Move(double x, double y)
        {
            if (!Bluetooth.Instance.IsConnected || writing) return;
            if (DateTime.Now.Ticks - _old < UpdateTime) return;
            writing = true;
            x = 127 * (x / _max) + 128;
            y = -127 * (y / _max) + 128;
            byte[] bytes = {(byte) '*', (byte) ((int) x), (byte)',', (byte) ((int) y), (byte) '#'};

            try
            {
                Bluetooth.Instance.WriteAsync(bytes);

                foreach (var ch in bytes)
                {
                    if(ch == (byte)'*' || ch == (byte)',')
                    Debug.Write(ch);
                    else if (ch == (byte)'#')
                    {
                        Debug.WriteLine((char)ch);
                    }
                    else
                    {
                        Debug.Write((byte)ch);
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Sending joystick data failed");
                Bluetooth.Instance.IsConnected = false;
            }
            _old = DateTime.Now.Ticks;
            writing = false;
        }
    }
}
