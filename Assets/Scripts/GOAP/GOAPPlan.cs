using System.Collections.Generic;
using System.Linq;
using FP;
using System;

namespace GOAP
{
    public class GOAPPlan
    {
        GOAPState initialState;

        internal GOAPPlan(IEnumerable<IGOAPAction> actions, Map<GOAPKeyEnum, object> initialState, Map<GOAPKeyEnum, Func<object, bool>> goalState, Func<IGraphNode<GOAPState>, float> heuristic)
        {
            this.initialState = new GOAPState(actions, initialState, goalState, null, heuristic);
        }

        public IEnumerable<IGOAPAction> Execute()
        {
            return GraphOperations.AStar(initialState, null).Skip(1).Reverse().Select(x => x.Content.GeneratingAction);
        }
    }
}