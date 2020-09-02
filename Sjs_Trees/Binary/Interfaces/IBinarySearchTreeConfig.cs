using Sjs_Trees.Binary.Types;
using Sjs_Trees.Enums;

namespace Sjs_Trees.Binary.Interfaces
{
    public interface IBinarySearchTreeConfig<T>
    {
        /// <summary>
        /// the compare delegate
        /// </summary>
        /// <remarks>if the comparator is null, the tree will be filled until the level is complete. The tree will be unordered.</remarks>
        Compare<T> Comparator { get; }

        /// <summary>
        /// how to handle duplicates
        /// </summary>
        PermissionDuplicates CanHaveDuplicates { get; }
    }
}
