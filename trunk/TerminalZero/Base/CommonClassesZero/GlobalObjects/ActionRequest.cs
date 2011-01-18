using System;

namespace ZeroCommonClasses.GlobalObjects
{
    public class ActionRequest
    {
        public static ActionRequest CreateInstance(object owner)
        {
            return new ActionRequest(owner);
        }

        private object sender;
        private ActionRequest(object owner)
        {
            sender = owner;
        }

        public event EventHandler Canceled;
        public event EventHandler Accepted;

        public void Cancel()
        {
            if (Canceled != null)
            {
                Canceled(sender,EventArgs.Empty);
            }
        }

        public void Acept()
        {
            if (Accepted != null)
            {
                Accepted(sender, EventArgs.Empty);
            }
        }
    }
}
