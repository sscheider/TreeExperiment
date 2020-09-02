using Sjs_Trees.Binary;
using Sjs_Trees.Binary.Configuration;
using Sjs_Trees.Binary.Interfaces;
using Sjs_Trees.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TreeExperiment
{
    public class Program
    {
        static void Main(string[] args)
        {
            TestSimpleTree();
        }

        static void TestSimpleTree()
        {
            IBinarySearchTreeConfig<TestState> config = new BinarySearchTreeConfig<TestState>()
            {
                Comparator = TestState.Comparator,
                CanHaveDuplicates = PermissionDuplicates.Permitted
            };

            IBinarySearchTree<TestState> iTree = new BinarySearchTree<TestState>(config);

            TestState rootState = new TestState()
            {
                name = "node 1",
                value = "value 1"
            };


            TestState state1 = new TestState()
            {
                name = "node 2",
                value = "value 2"
            };

            TestState state2 = new TestState()
            {
                name = "node 3",
                value = "value 3"
            };

            TestState state11 = new TestState()
            {
                name = "node 2",
                value = "value 1"
            };
            

            iTree.Add(rootState);
            iTree.Add(state1);
            iTree.Add(state2);
            iTree.Add(state11);
            iTree.Add(state11);
            iTree.Add(state11);

            IEnumerable<TestState> reply03 = iTree.WhereAsync(x => x.Equals(state11)).Result;
            IEnumerable<TestState> reply04 = iTree.Where(x => x.Equals(state11));
            //bool reply05 = iTree.Any(x => x.Equals(state11)); // unintended consequences with idisposable
            bool reply05 = iTree.Any();

            // test ienumerable<t> and ienumerate
            foreach (TestState item in iTree)
            {
                Console.WriteLine(item.ToString());
            }

            IBinarySearchTree<TestState> iTree2 = (IBinarySearchTree<TestState>)iTree.Clone();


            /* 
            
            structure:
            root (name: node 1, value: value 1)
            |
            +--[left]--- null
            |
            +--[right]-- state1 (name: node 2, value: value 2)
                          |
                          +--[left]--- state11 (name: node 2, value: value 1)
                          |             |
                          |             +--[left]--- state11 (name: node 2, value: value 1)
                          |             |             |
                          |             |             +--[left]--- state11 (name: node 2, value: value 1)
                          |             |             |
                          |             |             +--[right]-- null
                          |             |
                          |             +--[right]-- null
                          |
                          +--[right]-- state2 (name: node 3, value: value 3)

            name: node 2, value: value 1 -- state11
            name: node 2, value: value 1 -- state11
            name: node 2, value: value 1 -- state11
            name: node 3, value: value 3 -- state2
            name: node 2, value: value 2 -- state1
            name: node 1, value: value 1 -- root
            
             */
            // iTree.Remove(state1); // both left and right 
            // iTree.Remove(state11); // only left
            // iTree.Remove(rootState); // root, only right

            int i = 9;
        }
    }

    [DebuggerDisplay("name: {name}; value: {value}")]
    public class TestState : ICloneable
    {
        public string name { get; set; }
        public string value { get; set; }

        /// <summary>
        /// the comparator for the Compare delegate used by simpleTree
        /// </summary>
        /// <param name="first">a TestState object</param>
        /// <param name="second">another TestState object</param>
        /// <returns>0 if the two are equal, less than 0 if second is greater than first, greater than 0 otherwise</returns>
        /// <remarks>for this class, name is compared, then value</remarks>
        public static int Comparator(TestState first, TestState second)
        {
            int returnValue = second.name.ToLower().CompareTo(first.name.ToLower());
            if (returnValue == 0)
            {
                // the two names are equal, test the values
                returnValue = second.value.ToLower().CompareTo(first.value.ToLower());
            }
            return returnValue;
        }

        /// <summary>
        /// Equality operation
        /// </summary>
        /// <param name="obj">another object</param>
        /// <returns>true if the object is TestState and the poco properties are equal (case insensitive)</returns>
        /// <remarks>when operator == is defined, operator != must be defined</remarks>
        public override bool Equals(object obj)
        {
            return (obj is TestState) && (this == (TestState)obj);
        }

        /// <summary>
        /// logical equality comparison operator
        /// </summary>
        /// <param name="x">the first TestState object</param>
        /// <param name="y">the second TestState object</param>
        /// <returns>true if the properties are equal (case insensitive)</returns>
        public static bool operator ==(TestState x, TestState y)
        {
            return x.name.Equals(y.name, StringComparison.InvariantCultureIgnoreCase) &&
                x.value.Equals(y.value, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// logical inequality comparison operator
        /// </summary>
        /// <param name="x">the first TestState object</param>
        /// <param name="y">the second TestState object</param>
        /// <returns>true if the properties are unequal (case insensitive)</returns>
        public static bool operator !=(TestState x, TestState y)
        {
            return !(x == y);
        }

        /// <summary>
        /// gets the hash code for this object
        /// </summary>
        /// <returns>an integer hash code</returns>
        /// required when overriding Equals, operator ==, and operator !=
        public override int GetHashCode()
        {
            return Tuple.Create(name, value).GetHashCode();
        }

        /// <summary>
        /// simple output of properties
        /// </summary>
        /// <returns>a string with the values</returns>
        public override string ToString()
        {
            return $"name: {name}, value: {value}";
        }

        /// <summary>
        /// the icloneable interface realization
        /// </summary>
        /// <returns>a copy of this object</returns>
        public object Clone()
        {
            return new TestState()
            {
                name = this.name,
                value = this.value
            };
        }
    }
}
