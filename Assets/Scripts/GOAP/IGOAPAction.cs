using System;
using System.Collections.Generic;

namespace GOAP
{
    public interface IGOAPAction
    {
        GOAPActionKeyEnum Name { get; }
        float Cost { get; }
        IEnumerable<KeyValuePair<GOAPKeyEnum, Func<object, bool>>> Preconditions { get; }
        IEnumerable<KeyValuePair<GOAPKeyEnum, Func<object, object>>> Effects { get; }
    }
}
