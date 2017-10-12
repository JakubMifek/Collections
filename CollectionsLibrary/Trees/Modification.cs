using System;

namespace CollectionsLibrary.Trees
{
    public enum Modification
    {
        Add, Remove, Change, Move, Abandon, Adopt
    }

    public class ModifyArgs : EventArgs
    {
        public ModifyArgs(Modification modification)
        {
            Modification = modification;
        }

        public Modification Modification { get; }
    }

    public interface IModifiable
    {
        event EventHandler<ModifyArgs> OnModify;
    }
}
