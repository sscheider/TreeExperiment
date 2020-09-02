using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sjs_Trees.Binary.Interfaces
{
    internal interface IBinarySearchTreeNode<T> : IDisposable where T : ICloneable
    {
        /// <summary>
        /// State payload contained in this node
        /// </summary>
        T State { get; }

        /// <summary>
        /// parent node
        /// </summary>
        IBinarySearchTreeNode<T> Parent { get; set; }

        /// <summary>
        /// left child node
        /// </summary>
        IBinarySearchTreeNode<T> LeftChild { get; }

        /// <summary>
        /// right child node
        /// </summary>
        IBinarySearchTreeNode<T> RightChild { get; }

        /// <summary>
        /// recursive - add a new node beneath this node
        /// </summary>
        /// <param name="nodeToInsert">a simple tree node to insert</param>
        /// <param name="configInfo">the tree configuration</param>
        /// <returns>the state of the added node on success</returns>
        T Add(ref IBinarySearchTreeNode<T> nodeToInsert, IBinarySearchTreeConfig<T> configInfo);

        /// <summary>
        /// an asynchronous search of the tree
        /// </summary>
        /// <param name="query">a function returning a bool</param>
        /// <returns>the async task that holds an enumerable collection of states</returns>
        /* async */ Task<List<T>> WhereAsync(Func<T, bool> query);

        /// <summary>
        /// recursive - search the tree using postorder traversal
        /// </summary>
        /// <param name="query">a function that returns a bool</param>
        /// <returns>true if the query is true for any one node, otherwise false</returns>
        bool TraversalContains(Func<T, bool> query);

        /// <summary>
        /// recursive - Search for the state in the tree
        /// </summary>
        /// <param name="state">the specified state</param>
        /// <param name="configInfo">the tree config</param>
        /// <returns>true if found, false otherwise</returns>
        bool Contains(T state, IBinarySearchTreeConfig<T> configInfo);

        /// <summary>
        /// recursive - find the node with a given state in the tree
        /// </summary>
        /// <param name="state">the state to find</param>
        /// <param name="configInfo">the configuration of the tree</param>
        /// <returns>the node with the sought state, otherwise null</returns>
        IBinarySearchTreeNode<T> Find(T state, IBinarySearchTreeConfig<T> configInfo);

        /// <summary>
        /// a post order recursive traversal of the tree
        /// </summary>
        /// <returns>the next node</returns>
        IBinarySearchTreeNode<T> Iterate();
        

        bool HasBeenVisited { get; }

        /// <summary>
        /// sets the HasBeenVisited flag to false using a pre order traversal
        /// </summary>
        void ClearVisited();

        /// <summary>
        /// recursive - clones the branch
        /// </summary>
        /// <param name="parent">the parent node of this clone</param>
        /// <returns>the clone of this branch</returns>
        IBinarySearchTreeNode<T> Clone(IBinarySearchTreeNode<T> parent);

        /// <summary>
        /// determines if this node is root
        /// </summary>
        /// <returns>true if it is the root, false otherwise</returns>
        bool IsRoot();

        /// <summary>
        /// recursive - get the maximum value of State in a tree
        /// </summary>
        /// <param name="configInfo">the tree configuration info</param>
        /// <returns>null if there is no comparator, the maximum otherwise</returns>
        IBinarySearchTreeNode<T> Maximum(IBinarySearchTreeConfig<T> configInfo);

        /// <summary>
        /// recursive - get the minimum value of State in a tree
        /// </summary>
        /// <param name="configInfo">the tree configuration info</param>
        /// <returns>null if there is no comparator, the minimum otherwise</returns>
        IBinarySearchTreeNode<T> Minimum(IBinarySearchTreeConfig<T> configInfo);

        /// <summary>
        /// recursive - get any leaf
        /// </summary>
        /// <returns>a leaf</returns>
        IBinarySearchTreeNode<T> GetALeaf();

        void RemoveNotRoot(BinarySearchTree<T> sourceTree);

        void RemoveRoot(BinarySearchTree<T> sourceTree);
    }
}
