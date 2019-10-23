using System.Collections.Generic;
using System;
using GameUtils;

namespace GOAP
{

    public class GOAPAction : IGOAPAction
    {
        Map<GOAPKeyEnum, Func<object, bool>> preconditions;
        Map<GOAPKeyEnum, Func<object, object>> effects;
        GOAPActionKeyEnum name;

        public GOAPAction(GOAPActionKeyEnum name)
        {
            this.name = name;
            preconditions = new Map<GOAPKeyEnum, Func<object, bool>>();
            effects = new Map<GOAPKeyEnum, Func<object, object>>();
            Cost = 1;
        }

        public GOAPActionKeyEnum Name { get { return name; } }

        public GOAPAction SetPrecondition(GOAPKeyEnum name, Func<object, bool> value)
        {
            preconditions[name] = value;
            return this;
        }

        public GOAPAction SetEffect(GOAPKeyEnum name, Func<object, object> value)
        {
            effects[name] = value;
            return this;
        }

        public GOAPAction SetCost(float value)
        {
            Cost = value;
            return this;
        }

        public float Cost { get; set; }

        public IEnumerable<KeyValuePair<GOAPKeyEnum, Func<object, bool>>> Preconditions
        {
            get
            {
                return preconditions;
            }
        }

        public IEnumerable<KeyValuePair<GOAPKeyEnum, Func<object, object>>> Effects
        {
            get
            {
                return effects;
            }
        }
    }
}