using System;

namespace AV.Cyclone.Katrina.Executor.Interfaces
{
    [Serializable]
    public class ToStringValue
    {
        public ToStringValue(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public override string ToString()
        {
            return Value;
        }
    }
}