using Sjs_Trees.Binary.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;

namespace Sjs_Trees.Binary
{
    public class BinarySearchTree<T> : IBinarySearchTree<T> where T : ICloneable
    {
        // public properties
        /// <summary>
        /// the congiruation used during construction of the tree
        /// </summary>
        public IBinarySearchTreeConfig<T> Configuration { get; }

        /// <summary>
        /// The root node
        /// </summary>
        internal IBinarySearchTreeNode<T> Root { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="initConfig">the configuration for tree options</param>
        public BinarySearchTree(IBinarySearchTreeConfig<T> initConfig)
        {
            Configuration = initConfig;
            Root = new BinarySearchTreeNode<T>();
        }

        // private class to support enumerators
        private class BinarySearchTreeEnumerator<U> : IEnumerator, IEnumerator<U> where U : T, ICloneable
        {
            /// <summary>
            /// a copy of the root node for enumeration
            /// </summary>
            IBinarySearchTreeNode<U> Root;

            /// <summary>
            /// default constructor
            /// </summary>
            /// <param name="rootNode">the root</param>
            /// <remarks>the node passed will be disposed, so use clone</remarks>
            public BinarySearchTreeEnumerator(IBinarySearchTreeNode<U> rootNode)
            {
                Root = rootNode;
                _currentNode = rootNode;
            }

            // this private prop keeps track of where we are in the traversal
            private IBinarySearchTreeNode<U> _currentNode;

            // get the state for the default enumerator
            public object Current => _currentNode.State;

            // get the state for the generic enumerator
            U IEnumerator<U>.Current => _currentNode.State;

            /// <summary>
            /// go to the next enumerated item
            /// </summary>
            /// <returns>true if the enumeration is not complete, false otherwise</returns>
            public bool MoveNext()
            {
                if ((_currentNode == null) || (Root.HasBeenVisited))
                {
                    return false;
                }

                _currentNode = _currentNode.Iterate();

                return true;
            }

            /// <summary>
            /// Setup the enumeration starting point
            /// </summary>
            public void Reset()
            {
                Root.ClearVisited();
                _currentNode = Root;
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            /// <summary>
            /// dispose the enumeration object
            /// </summary>
            /// <param name="disposing">false when called from destructor, true otherwise</param>
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).
                        if (Root != null)
                        {
                            Root.Dispose();
                            Root = null;
                        }
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~SimpleTreeEnumerator()
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

        /// <summary>
        /// Add a node to the tree with this state
        /// </summary>
        /// <param name="state">an object of type T</param>
        /// <returns>the added node</returns>
        public T Add(T state)
        {
            IBinarySearchTreeNode<T> newNode = new BinarySearchTreeNode<T>()
            {
                State = state
            };

            return Root.Add(ref newNode, Configuration);
        }

        /// <summary>
        /// an asynchronous search of the tree
        /// </summary>
        /// <param name="query">a function returning a bool</param>
        /// <returns>the async task that holds an enumerable collection of states</returns>
        public async Task<IEnumerable<T>> WhereAsync(Func<T, bool> query)
        {
            return await Root.WhereAsync(query);
        }

        /// <summary>
        /// Search for the state in the tree
        /// </summary>
        /// <param name="state">the specified state</param>
        /// <returns>true if found, false otherwise</returns>
        public bool Contains(T state)
        {
            return Root.Contains(state, Configuration);
        }

        /// <summary>
        /// delete a node that matches state in the tree
        /// </summary>
        /// <param name="state">the state to remove</param>
        public void Remove(T state)
        {
            // two parts -- find and then remove
            // part one -- find
            IBinarySearchTreeNode<T> soughtNode = Root.Find(state, Configuration);

            // if the node wasn't found, return
            if (soughtNode == null)
            {
                return;
            }

            // part two -- remove
            if (soughtNode.IsRoot())
            {
                // is root
                soughtNode.RemoveRoot(this);
                return;
            }

            // not root
            soughtNode.RemoveNotRoot(this);
            return;
        }

        #region ICloneable support

        /// <summary>
        /// Clone this tree
        /// </summary>
        /// <returns>a duplicate tree</returns>
        public object Clone()
        {
            IBinarySearchTree<T> clone = new BinarySearchTree<T>(Configuration)
            {
                Root = (IBinarySearchTreeNode<T>)this.Root.Clone(null)
            };
            
            return clone;
        }

        #endregion
        #region IEnumerator Support

        /// <summary>
        /// gets the generic enumerator
        /// </summary>
        /// <returns>an enumerator for the generic</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new BinarySearchTreeEnumerator<T>(Root.Clone(null));
        }

        /// <summary>
        /// gets the default enumerator
        /// </summary>
        /// <returns>an enumerator for the default</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BinarySearchTreeEnumerator<T>(Root.Clone(null));
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose the tree
        /// </summary>
        /// <param name="disposing">true when called from the destructor, false otherwise</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Root != null)
                    {
                        // the Dispose will cascade down through the tree
                        Root.Dispose();
                        Root = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BinarySearchTree()
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
