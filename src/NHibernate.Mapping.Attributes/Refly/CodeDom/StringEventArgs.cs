using System;

namespace Refly.CodeDom
{
    public sealed class StringEventArgs : EventArgs
    {
        private string value;
        public StringEventArgs(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get
            {
                return this.value;
            }
        }
    }

    public delegate void StringEventHandler(
        Object sender, StringEventArgs args);
}
