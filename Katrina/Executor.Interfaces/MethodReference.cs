using System;

namespace AV.Cyclone.Katrina.Executor.Interfaces
{
    public class MethodReference
    {
        public MethodReference(string fileName, string methodName)
        {
            this.FileName = fileName;
            this.MethodName = methodName;
        }

        public string FileName { get; private set; }

        public string MethodName { get; private set; }

        protected bool Equals(MethodReference other)
        {
            return string.Equals(FileName, other.FileName, StringComparison.InvariantCultureIgnoreCase) && string.Equals(MethodName, other.MethodName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MethodReference)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FileName != null ? FileName.GetHashCode() : 0)*397) ^ (MethodName != null ? MethodName.GetHashCode() : 0);
            }
        }
    }
}