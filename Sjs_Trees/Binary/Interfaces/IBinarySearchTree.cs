using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sjs_Trees.Binary.Interfaces
{
    public interface IBinarySearchTree<T> : IDisposable, ICloneable, IEnumerable<T> where T : ICloneable 
    {
        /// <summary>
        /// The root node
        /// </summary>
        //IBinarySearchTreeNode<T> Root { get; }

        /// <summary>
        /// the congiruation used during construction of the tree
        /// </summary>
        IBinarySearchTreeConfig<T> Configuration { get; }

        /// <summary>
        /// Add a node to the tree with this state
        /// </summary>
        /// <param name="state">an object of type T</param>
        /// <returns>the added node</returns>
        T Add(T state);

        /// <summary>
        /// an asynchronous search of the tree
        /// </summary>
        /// <param name="query">a function returning a bool</param>
        /// <returns>the async task that holds an enumerable collection of states</returns>
        /* async */ Task<IEnumerable<T>> WhereAsync(Func<T, bool> query);

        /// <summary>
        /// Search for the state in the tree
        /// </summary>
        /// <param name="state">the specified state</param>
        /// <returns>true if found, false otherwise</returns>
        bool Contains(T state);

        /// <summary>
        /// delete a node that matches state in the tree
        /// </summary>
        /// <param name="state">the state to remove</param>
        void Remove(T state);
    }
}
