using DYN = System.Dynamic;

namespace AplusCore.Runtime.Binder
{
    internal class SetMemberBinder : DYN.SetMemberBinder
    {
        public SetMemberBinder(string name)
            : base(name, /*ignore case*/ false)
        {
        }

        public override DYN.DynamicMetaObject FallbackSetMember(DYN.DynamicMetaObject target, DYN.DynamicMetaObject value, DYN.DynamicMetaObject errorSuggestion)
        {
            // NOTE: silently ignore everything for now...
            return null; // TODO: fix this!
        }
    }
}
