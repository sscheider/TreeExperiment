using Sjs_Trees.Binary.Types;
using Sjs_Trees.Binary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Sjs_Trees.Enums;

namespace Sjs_Trees.Binary.Configuration
{
    public class BinarySearchTreeConfig<T> : IBinarySearchTreeConfig<T>
    {
        /// <summary>
        /// the compare delegate
        /// </summary>
        /// <remarks>if the comparator is null, the tree will be filled until the level is complete. The tree will be unordered.</remarks>
        public Compare<T> Comparator { get; set; }

        /// <summary>
        /// How to handle duplicates
        /// </summary>
        public PermissionDuplicates CanHaveDuplicates { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public BinarySearchTreeConfig()
        {
            Comparator = null;
        }
    }
}
