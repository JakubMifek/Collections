using System;

namespace CollectionsLibrary.Updateable
{
    public delegate void UpdateHandler<T>(IUpdateable<T> sender, UpdateEventArgs<T> args);

    public enum Change
    {
        Add, Remove, Edit
    }

    public interface IUpdateable<T>
    {
        event UpdateHandler<T> OnUpdate;
    }

    public class UpdateEventArgs<T> : EventArgs
    {
        public T What { get; private set; }
        public Change How { get; private set; }
        public UpdateEventArgs(Change how, T what) { How = how; What = what; }
    }
}
