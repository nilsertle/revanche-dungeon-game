using System.Collections.Generic;
using System.Linq;

namespace Revanche.Extensions;

public static class DictionaryExtensions
{
    public static Dictionary<TK, TV> Copy<TK,TV>(this Dictionary<TK,TV> dict)
    {
        return dict.ToDictionary(entry => entry.Key,
            entry => entry.Value);
    }
}