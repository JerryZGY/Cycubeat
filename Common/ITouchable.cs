using System;

namespace Cycubeat
{
    public interface ITouchable
    {
        void EnterStory();
        void ExitStory(Action callback);
        void RemoveSelf();
    }
}
