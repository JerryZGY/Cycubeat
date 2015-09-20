using System;

namespace Cycubeat
{
    public class TouchEventArgs: EventArgs
    {
        public object Tag { get; }

        public TouchEventArgs()
        {

        }

        public TouchEventArgs(object tag)
        {
            Tag = tag;
        }
    }
}
