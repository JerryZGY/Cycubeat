using System;

namespace Cycubeat
{
    public interface ISwitchable
    {
        void InitializeProperty();
        void EnterStory();
        void ExitStory(Action callback);
    }
}