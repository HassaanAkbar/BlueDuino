using System;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using BlueDuino.Classes;

namespace BlueDuino.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class JoystickPage : Page
    {
        Joystick joy;

        private bool _controllerPressed;
        private double _controlRadius;

        public JoystickPage()
        {

            this.InitializeComponent();
            _controlRadius = controlArea.MaxHeight / 2;
            joy = new Joystick(_controlRadius);
            this.PointerReleased += FrameOnPointerReleased;
            this.PointerMoved += OnPointerMoved;
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_controllerPressed) return;

            var x = e.GetCurrentPoint(controlArea).Position.X - _controlRadius;
            var y = e.GetCurrentPoint(controlArea).Position.Y - _controlRadius;
            var disp = Math.Sqrt(x * x + y * y);

            if (disp < _controlRadius)
            {
                myTransform.X = x;
                myTransform.Y = y;

            }
            else
            {
                myTransform.X = (_controlRadius*x)/disp;
                myTransform.Y = (_controlRadius*y)/disp;
            }

            joy.Move(myTransform.X, myTransform.Y);
        }

        private void FrameOnPointerReleased(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            _controllerPressed = false;
            myTransform.X = 0;
            myTransform.Y = 0;
        }

        private void controller_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _controllerPressed = true;
        }
    }
}
