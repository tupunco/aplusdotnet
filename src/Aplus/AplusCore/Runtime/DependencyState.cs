
namespace AplusCore.Runtime
{

    /// <summary>
    /// Descibes the state of a dependency.
    /// </summary>
    public enum DependencyState
    {
        /// <summary>
        /// Dependency is valid, nothing to do with it.
        /// </summary>
        Valid,

        /// <summary>
        /// Dependency is invalid, re-evaluation is required.
        /// </summary>
        Invalid,

        /// <summary>
        /// Dependency is currently under evaulation.
        /// </summary>
        Evaluating,
    }
}
