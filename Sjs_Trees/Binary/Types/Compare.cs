using System;
using System.Collections.Generic;
using System.Text;

namespace Sjs_Trees.Binary.Types
{
    /// <summary>
    /// Compare delegate Type
    /// </summary>
    /// <param name="first">an object to compare</param>
    /// <param name="second">another object for comparison</param>
    /// <returns>=0 if equal, &lt;0 if first &lt; second, &gt;0 if first &gt; second</second></returns>
    public delegate int Compare<T>(T first, T second);
}
