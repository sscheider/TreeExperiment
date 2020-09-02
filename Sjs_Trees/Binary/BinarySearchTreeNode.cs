using Sjs_Trees.Binary.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Sjs_Trees.Enums;

namespace Sjs_Trees.Binary
{
    [DebuggerDisplay("ParentInUse? {Parent != null}; LeftChildInUse? {LeftChild != null} RightChildInUse? {RightChild != null}")]
    internal class BinarySearchTreeNode<T> : IBinarySearchTreeNode<T> where T : ICloneable 
    {
        /// <summary>
        /// State payload contained in this node
        /// </summary>
        public T State { get; set; }

        /// <summary>
        /// parent node
        /// </summary>
        public IBinarySearchTreeNode<T> Parent { get; set; }

        /// <summary>
        /// left child node
        /// </summary>
        public IBinarySearchTreeNode<T> LeftChild { get; set; }

        /// <summary>
        /// right child node
        /// </summary>
        public IBinarySearchTreeNode<T> RightChild { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public BinarySearchTreeNode()
        {
            State = default;
            Parent = null;
            LeftChild = null;
            RightChild = null;
            HasBeenVisited = false;
        }

        /// <summary>
        /// recursive - add a new node beneath this node
        /// </summary>
        /// <param name="nodeToInsert">a simple tree node to insert</param>
        /// <param name="configInfo">the tree configuration</param>
        /// <returns>the state of the added node on success</returns>
        public T Add(ref IBinarySearchTreeNode<T> nodeToInsert, IBinarySearchTreeConfig<T> configInfo)
        {
            if (IsRoot() && (State == null))
            {
                // this is the root and it is blank
                State = nodeToInsert.State;
                return State;
            }

            if (configInfo.Comparator == null)
            {
                // if there is no comparator, fill based on left bias instead of value
                if (LeftChild == null)
                {
                    nodeToInsert.Parent = this;
                    LeftChild = nodeToInsert;
                }
                else if (RightChild == null)
                {
                    nodeToInsert.Parent = this;
                    RightChild = nodeToInsert;
                }
                else
                {
                    // follow the bias
                    LeftChild.Add(ref nodeToInsert, configInfo);
                }
            }
            else if (configInfo.Comparator(this.State, nodeToInsert.State) <= 0)
            {
                // comparator is non-null
                // the state of this node is greater than or equal to the state of the incoming node

                if((configInfo.CanHaveDuplicates == PermissionDuplicates.NotAllowed) &&
                    Contains(nodeToInsert.State, configInfo))
                {
                    throw new ArgumentException("Duplicates are not allowed.");
                }

                if (LeftChild == null)
                {
                    // this is the new left child
                    nodeToInsert.Parent = this;
                    LeftChild = nodeToInsert;
                }
                else
                {
                    // pass it down the left side 
                    LeftChild.Add(ref nodeToInsert, configInfo);
                }
            }
            else
            {
                // comparator is non-null
                // the state of this node is less than the state of the incoming node

                if ((configInfo.CanHaveDuplicates == PermissionDuplicates.NotAllowed) &&
                    Contains(nodeToInsert.State, configInfo))
                {
                    throw new ArgumentException("Duplicates are not allowed.");
                }
                
                if (RightChild == null)
                {
                    // this is the new right child
                    nodeToInsert.Parent = this;
                    RightChild = nodeToInsert;
                }
                else
                {
                    // pass it down the right side
                    RightChild.Add(ref nodeToInsert, configInfo);
                }
            }

            return nodeToInsert.State;
        }

        /// <summary>
        /// recursive - Search for the state in the tree
        /// </summary>
        /// <param name="state">the specified state</param>
        /// <param name="configInfo">the tree config</param>
        /// <returns>true if found, false otherwise</returns>
        public bool Contains(T state, IBinarySearchTreeConfig<T> configInfo)
        {
            bool returnValue = false;

            if (configInfo.Comparator == null)
            {
                // no comparator -- traversal search using Equals
                returnValue = TraversalContains(x => x.Equals(state));
            }
            else if (configInfo.Comparator(this.State, state) == 0)
            {
                // comparator exists
                // this IS the node we are looking for
                returnValue = true;
            }
            else if (configInfo.Comparator(this.State, state) > 0)
            {
                // comparator exists -- binary search
                // this node is less than the one we are seeking
                // higher values to the right
                if (RightChild != null)
                {
                    returnValue = RightChild.Contains(state, configInfo);
                }

                // if the right child is null, the search ends and the value wasn't found
            }
            else
            {
                // comparator exists -- binary search
                // this node is greater than the one we are seeking
                if (LeftChild != null)
                {
                    // smaller values to the left
                    returnValue = LeftChild.Contains(state, configInfo);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// recursive - search the tree using postorder traversal
        /// </summary>
        /// <param name="query">a function that returns a bool</param>
        /// <returns>true if the query is true for any one node, otherwise false</returns>
        public bool TraversalContains(Func<T, bool> query)
        {
            bool returnValue = false;

            if (LeftChild != null)
            {
                returnValue = LeftChild.TraversalContains(query);
            }

            if (!returnValue && (RightChild != null))
            {
                returnValue = RightChild.TraversalContains(query);
            }

            if (!returnValue)
            {
                returnValue = query(State);
            }

            return returnValue;
        }

        /// <summary>
        /// an asynchronous search of the tree
        /// </summary>
        /// <param name="query">a function returning a bool</param>
        /// <returns>the async task that holds an enumerable collection of states</returns>
        public async Task<List<T>> WhereAsync(Func<T, bool> query)
        {
            List<T> returnList = new List<T>();
            List<Task<List<T>>> taskList = new List<Task<List<T>>>();
            if (LeftChild != null)
            {
                taskList.Add(LeftChild.WhereAsync(query));
            }

            if (RightChild != null)
            {
                taskList.Add(RightChild.WhereAsync(query));
            }

            if (query(State))
            {
                returnList.Add(State);
            }

            if (taskList.Any())
            {
                List<T>[] completion = await Task.WhenAll(taskList);

                // not iterating, since there's only two
                if (completion[0].Any())
                {
                    returnList.AddRange(completion[0]);
                }
                if ((completion.Count() > 1) && completion[1].Any())
                {
                    returnList.AddRange(completion[1]);
                }
            }

            return returnList;
        }

        /// <summary>
        /// recursive - find the node with a given state in the tree
        /// </summary>
        /// <param name="state">the state to find</param>
        /// <param name="configInfo">the configuration of the tree</param>
        /// <returns>the node with the sought state, otherwise null</returns>
        public IBinarySearchTreeNode<T> Find(T state, IBinarySearchTreeConfig<T> configInfo)
        {
            IBinarySearchTreeNode<T> returnObject = null;

            if (configInfo.Comparator == null)
            {
                // no comparator -- iterate 
                IBinarySearchTreeNode<T> currentNode = this;
                bool found = false;
                ClearVisited();
                while((!HasBeenVisited) && (!found))
                {
                    if (currentNode.State.Equals(state))
                    {
                        found = true;
                        returnObject = currentNode;
                    }
                    else
                    {
                        currentNode = currentNode.Iterate();
                    }
                }
            }
            else if (configInfo.Comparator(this.State, state) == 0)
            {
                // comparator exists
                // this IS the node we are looking for
                returnObject = this;
            }
            else if (configInfo.Comparator(this.State, state) > 0)
            {
                // comparator exists -- binary search
                // this node is less than the one we are seeking
                // higher values to the right
                if (RightChild != null)
                {
                    returnObject = RightChild.Find(state, configInfo);
                }

                // if the right child is null, the search ends and the value wasn't found
            }
            else
            {
                // comparator exists -- binary search
                // this node is greater than the one we are seeking
                if (LeftChild != null)
                {
                    // smaller values to the left
                    returnObject = LeftChild.Find(state, configInfo);
                }
            }

            return returnObject;
        }

        /// <summary>
        /// recursive - clones the branch
        /// </summary>
        /// <param name="parent">the parent node of this clone</param>
        /// <returns>the clone of this branch</returns>
        public IBinarySearchTreeNode<T> Clone(IBinarySearchTreeNode<T> parent)
        {
            BinarySearchTreeNode<T> clone = new BinarySearchTreeNode<T>()
            {
                Parent = parent
            };
            
            if (State != null)
            {
                clone.State = (T)State.Clone();
            }

            if (LeftChild != null)
            {
                clone.LeftChild = LeftChild.Clone(clone);
            }

            if (RightChild != null)
            {
                clone.RightChild = RightChild.Clone(clone);
            }

            return clone;
        }

        /// <summary>
        /// helper method to determine handedness of a child
        /// </summary>
        /// <returns>true if this is the left child, false otherwise</returns>
        private bool IsLeftChildOfParent()
        {
            bool returnValue = false;
            if(!IsRoot())
            {
                returnValue = this.Equals(Parent.LeftChild);
            }

            return returnValue;
        }

        /// <summary>
        /// determines if this node is a leaf
        /// </summary>
        /// <returns>true if it is a leaf, otherwise false</returns>
        private bool IsLeaf()
        {
            return ((LeftChild == null) && (RightChild == null));
        }

        /// <summary>
        /// determines if this node is root
        /// </summary>
        /// <returns>true if it is the root, false otherwise</returns>
        public bool IsRoot()
        {
            return (Parent == null);
        }

        /// <summary>
        /// recursive - get the minimum value of State in a tree
        /// </summary>
        /// <param name="configInfo">the tree configuration info</param>
        /// <returns>null if there is no comparator, the minimum otherwise</returns>
        public IBinarySearchTreeNode<T> Minimum(IBinarySearchTreeConfig<T> configInfo)
        {
            IBinarySearchTreeNode<T> returnObject = null;
            if (configInfo.Comparator != null)
            {
                // left child is always the smaller one
                if (LeftChild != null)
                {
                    returnObject = LeftChild.Minimum(configInfo);
                }
                else
                {
                    returnObject = this;
                }
            }

            return returnObject;
        }

        /// <summary>
        /// recursive - get the maximum value of State in a tree
        /// </summary>
        /// <param name="configInfo">the tree configuration info</param>
        /// <returns>null if there is no comparator, the maximum otherwise</returns>
        public IBinarySearchTreeNode<T> Maximum(IBinarySearchTreeConfig<T> configInfo)
        {
            IBinarySearchTreeNode<T> returnObject = null;
            if (configInfo.Comparator != null)
            {
                // right child is always the bigger one
                if (RightChild != null)
                {
                    returnObject = RightChild.Maximum(configInfo);
                }
                else
                {
                    returnObject = this;
                }
            }

            return returnObject;
        }

        /// <summary>
        /// recursive - get any leaf
        /// </summary>
        /// <returns>a leaf</returns>
        public IBinarySearchTreeNode<T> GetALeaf()
        {
            IBinarySearchTreeNode<T> returnObject;
            // this alg will get the right-most leaf

            if (RightChild != null)
            {
                returnObject = RightChild.GetALeaf();
            }
            else if (LeftChild != null)
            {
                returnObject = LeftChild.GetALeaf();
            }
            else
            {
                // this is the leaf
                returnObject = this;
            }

            return returnObject;
        }


        public void RemoveRoot(BinarySearchTree<T> sourceTree)
        {
            // four cases
            if (IsLeaf())
            {
                // Parent and both children are null, 
                // set the state to default
                State = default;
                return;
            }

            // only left child
            if (RightChild == null)
            {
                // promote the child to Root
                BinarySearchTreeNode<T> childCopy = LeftChild as BinarySearchTreeNode<T>;
                LeftChild = null;
                sourceTree.Root = childCopy;
                Dispose();
                return;
            }

            // only right child
            if (LeftChild == null)
            {
                // promote the child to Root
                BinarySearchTreeNode<T> childCopy = RightChild as BinarySearchTreeNode<T>;
                RightChild = null;
                sourceTree.Root = childCopy;
                Dispose();
                return;
            }

            // both children
            // if the path gets to here, then the node has two children
            // the solution depends if there is a comparator.
            // no comparator - choose any leaf, swap the state to the node,
            //      and delete the leaf
            // comparator exists - find the minimum in the right branch, swap the state
            //      to the node, and recursively delete the minimum node.
            if (sourceTree.Configuration.Comparator == null)
            {
                IBinarySearchTreeNode<T> victim = sourceTree.Root.GetALeaf();
                State = victim.State;
                victim.Dispose();
                return;
            }

            IBinarySearchTreeNode<T> candidate = RightChild.Minimum(sourceTree.Configuration);
            State = candidate.State;
            candidate.RemoveNotRoot(sourceTree);
            // don't dispose of anything, since that will be handled in the recursion
            return;
        }

        public void RemoveNotRoot(BinarySearchTree<T> sourceTree)
        {
            if (IsLeaf())
            {
                // node is a leaf

                // remove reference from Parent
                if (IsLeftChildOfParent())
                { 
                    ((BinarySearchTreeNode<T>)Parent).LeftChild = null;
                }
                else
                {
                    ((BinarySearchTreeNode<T>)Parent).RightChild = null;
                }

                Dispose();
                return;
            }

            if (LeftChild == null)
            {
                // all of the code paths in the leaf end in returns, 
                // so this is a node with a single right child
                if (IsLeftChildOfParent())
                {
                    ((BinarySearchTreeNode<T>)Parent).LeftChild = RightChild;
                    RightChild.Parent = Parent;
                }
                else
                {
                    ((BinarySearchTreeNode<T>)Parent).RightChild = RightChild;
                    RightChild.Parent = Parent;
                }

                RightChild = null;
                Dispose();
                return;
            }

            if (RightChild == null)
            {
                // all of the code paths in the leaf end in returns, 
                // so this is a node with a single left child
                if (IsLeftChildOfParent())
                {
                    ((BinarySearchTreeNode<T>)Parent).LeftChild = LeftChild;
                    LeftChild.Parent = Parent;
                }
                else
                {
                    ((BinarySearchTreeNode<T>)Parent).RightChild = LeftChild;
                    LeftChild.Parent = Parent;
                }

                LeftChild = null;
                Dispose();
                return;
            }

            // if the path gets to here, then the node has two children
            // the solution depends if there is a comparator.
            // no comparator - choose any leaf, swap the state to the node,
            //      and delete the leaf
            // comparator exists - find the minimum in the right branch, swap the state
            //      to the node, and recursively delete the minimum node.
            if (sourceTree.Configuration.Comparator == null)
            {
                IBinarySearchTreeNode<T> victim = sourceTree.Root.GetALeaf();
                State = victim.State;
                victim.Dispose();
                return;
            }

            IBinarySearchTreeNode<T> candidate = RightChild.Minimum(sourceTree.Configuration);
            State = candidate.State;
            candidate.RemoveNotRoot(sourceTree);
            // don't dispose of anything, since that will be handled in the recursion
            return;
        }

        #region ISimpleTree ienumerable support

        public bool HasBeenVisited { get; set; }

        /// <summary>
        /// sets the HasBeenVisited flag to false using a pre order traversal
        /// </summary>
        public void ClearVisited()
        {
            HasBeenVisited = false;

            if (LeftChild != null)
            {
                LeftChild.ClearVisited();
            }

            if (RightChild != null)
            {
                RightChild.ClearVisited();
            }
        }

        /// <summary>
        /// a post order recursive traversal of the tree
        /// </summary>
        /// <returns>the next node</returns>
        public IBinarySearchTreeNode<T> Iterate()
        {
            IBinarySearchTreeNode<T> returnValue = default;

            if ((LeftChild != null) && (!LeftChild.HasBeenVisited))
            {
                returnValue = LeftChild.Iterate();
            }

            if ((returnValue == default) && (RightChild != null) && (!RightChild.HasBeenVisited))
            {
                returnValue = RightChild.Iterate();
            }

            if (returnValue == default)
            {
                if (HasBeenVisited)
                {
                    returnValue = Parent.Iterate();
                }
                else
                {
                    HasBeenVisited = true;
                    returnValue = this;
                }
            }

            return returnValue;
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // don't dispose the Parent
                    if (LeftChild != null)
                    {
                        LeftChild.Dispose();
                        LeftChild = null;
                    }

                    if (RightChild != null)
                    {
                        RightChild.Dispose();
                        RightChild = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SimpleTreeNode()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        
        #endregion
    }
}
