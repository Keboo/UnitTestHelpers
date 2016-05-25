using System;
using System.ComponentModel;

namespace UnitTestHelpers
{
    public class MyEvents
    {
        public delegate void CustomDelegate(int @int, bool @bool);

        public event EventHandler EventHandler;
        public void RaiseEventHandler(object sender, EventArgs e)
        {
            EventHandler?.Invoke(sender, e);
        }

        public event EventHandler<CancelEventArgs> EventWithArgs;
        public void RaiseEventWithArgs(object sender, CancelEventArgs e)
        {
            EventWithArgs?.Invoke(sender, e);
        }

        public event CustomDelegate CustomEvent;
        public void RaiseCustomEvent(int @int, bool @bool)
        {
            CustomEvent?.Invoke(@int, @bool);
        }

        public event Action ActionEvent;
        public void RaiseActionEvent()
        {
            ActionEvent?.Invoke();
        }

        public static event EventHandler StaticEvent;
        public static void RaiseStaticEvent(object sender, EventArgs e)
        {
            StaticEvent?.Invoke(sender, e);
        }


    }
}