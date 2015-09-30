using System;
using System.Collections.Generic;
using System.Windows;

namespace Cycubeat
{
    public enum InputState
    {
        Close = 0,
        Open = 1
    }

    public class KinectInputArgs : EventArgs
    {
        public Point Posotion { get; }

        public InputState InputState { get; }

        public bool IsValid { get; }

        public KinectInputArgs(Point posotion, InputState state, bool isValid)
        {
            Posotion = posotion;
            InputState = state;
            IsValid = isValid;
        }
    }

    public class TouchBounds
    {
        public int SX { get; }//Start
        public int EX { get; }//End
        public int SY { get; }
        public int EY { get; }
        public TouchBounds(int sx, int ex, int sy, int ey)
        {
            SX = sx;
            EX = ex;
            SY = sy;
            EY = ey;
        }
    }

    public class KinectTouchMap
    {
        public List<TouchBounds> StartMap = new List<TouchBounds>() { new TouchBounds(400, 900, 200, 700) };

        public List<TouchBounds> PlayMap_Next = new List<TouchBounds>() { new TouchBounds(1133, 1313, 294, 474) };

        public List<TouchBounds> PlayMap_Beat = new List<TouchBounds>()
        {
            new TouchBounds(403, 583, 104, 284), new TouchBounds(593, 773, 104, 284), new TouchBounds(783, 963, 104, 284),
            new TouchBounds(403, 583, 294, 474), new TouchBounds(593, 773, 294, 474), new TouchBounds(783, 963, 294, 474),
            new TouchBounds(403, 583, 484, 664), new TouchBounds(593, 773, 484, 664), new TouchBounds(783, 963, 484, 664)
        };

        public List<TouchBounds> PlayMap_Numpad = new List<TouchBounds>()
        {
            new TouchBounds(403, 583, 104, 284), new TouchBounds(593, 773, 104, 284), new TouchBounds(783, 963, 104, 284), new TouchBounds(1133, 1313, 104, 284),
            new TouchBounds(403, 583, 294, 474), new TouchBounds(593, 773, 294, 474), new TouchBounds(783, 963, 294, 474), new TouchBounds(1133, 1313, 294, 474),
            new TouchBounds(403, 583, 484, 664), new TouchBounds(593, 773, 484, 664), new TouchBounds(783, 963, 484, 664), new TouchBounds(1133, 1313, 484, 664)
        };
    }

    public static class TouchMapHandler
    {
        public static void CheckTouch(Point pos, bool isOpen, Action leaveEvent, Action enterEvent, Action clickEvent)
        {
            foreach (var item in Switcher.pageSwitcher.Bounds)
            {
                leaveEvent();
                if (pos.X >= item.SX && pos.X <= item.EX && pos.Y >= item.SY && pos.Y <= item.EY)
                {
                    enterEvent();
                    if (isOpen)
                        clickEvent();
                }
            }
        }

        public static void CheckTouch(Point pos, bool isOpen, List<Action> clickEvent)
        {
            var i = 0;
            foreach (var item in Switcher.pageSwitcher.Bounds)
            {
                if (pos.X >= item.SX && pos.X <= item.EX && pos.Y >= item.SY && pos.Y <= item.EY)
                {
                    if (isOpen)
                        clickEvent[i]();
                }
                i++;
            }
        }

        public static void CheckTouch(Point pos, bool isOpen, List<Action> leaveEvent, List<Action> enterEvent, List<Action> clickEvent, List<Action> releaseEvent)
        {
            var i = 0;
            foreach (var item in Switcher.pageSwitcher.Bounds)
            {
                try
                {
                    leaveEvent[i]();
                }
                catch (Exception) { }
                
                if (pos.X >= item.SX && pos.X <= item.EX && pos.Y >= item.SY && pos.Y <= item.EY)
                {
                    enterEvent[i]();
                    if (isOpen)
                        clickEvent[i]();
                    else
                        releaseEvent[i]();
                }
                i++;
            }
        }
    }
}