using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameUtils;
using UnityEngine;

namespace GOAP
{

    public class GOAPState : IGraphNode<GOAPState>
    {
        public IGOAPAction GeneratingAction { get; set; }

        IEnumerable<IGOAPAction> actions;
        public Map<GOAPKeyEnum, object> currentValues;
        public Map<GOAPKeyEnum, Func<object, bool>> goalValues;
        Func<IGraphNode<GOAPState>, float> heuristic;

        public GOAPState(IEnumerable<IGOAPAction> actions, Map<GOAPKeyEnum, object> initialValues, Map<GOAPKeyEnum, Func<object, bool>> goalValues, IGOAPAction generatingAction, Func<IGraphNode<GOAPState>, float> heuristic)
        {
            this.actions = actions;
            this.currentValues = initialValues;
            this.goalValues = goalValues;
            this.heuristic = heuristic;
            GeneratingAction = generatingAction;
        }

        public GOAPState Content { get { return this; } }

        public IEnumerator<GameUtils.Tuple<float, IGraphNode<GOAPState>>> GetEnumerator()
        {
            //This optimization shortens search time greatly by pruning actions that do not contribute to
            //reaching the goal
            var satisfyingActions = actions.Where(x => x.Effects.Any(
                //Filter only actions with any effect that applies to an unsatisfied goal and satisfies it
                e => goalValues.ContainsKey(e.Key) ?   // Que la accion satisfaga alguna Key de los goalValues 
                     !goalValues[e.Key](currentValues[e.Key]) &&  // Que el goalValue no este satisfecho
                     !e.Value(currentValues[e.Key]).Equals(currentValues[e.Key]) //Que el objeto cambie y no devuelva el mismo
                     : false   // La accion no nos sirve
                               //goalValues[e.Key](e.Value(currentValues[e.Key]))

            ));

            foreach (var action in satisfyingActions)
                yield return GameUtils.Tuple.New(
                    //Use a cost of at least the precondition count, since our heuristic is "unsatisfied goal count"
                    //this guarantees that we are not overestimating
                    Math.Max(action.Cost, action.Preconditions.Count()),
                    (IGraphNode<GOAPState>)Apply(action)
                );
        }

        public bool Satisfies(IGraphNode<GOAPState> goalState)
        {
            //The satisfaction condition is every goal is satisfied
            //return goalValues.All(goal => currentValues[goal.Key].Equals(goal.Value));
            return goalValues.All(goal => goalValues[goal.Key](currentValues[goal.Key]));
        }

        public float Heuristic(IGraphNode<GOAPState> goalState)
        {
            //Hueristic: count of unsatisfied conditions
            //It doesn't overestimate the total cost as long as the Action.Cost > Action.Preconditions.Count()
            //return goalValues.Count(goal => !currentValues[goal.Key].Equals(goal.Value));
            if (heuristic != null) return heuristic(this);
            else return goalValues.Count(goal => !goalValues[goal.Key](currentValues[goal.Key]));
        }

        GOAPState Apply(IGOAPAction action)
        {
            //Add preconditions of the action as goals to the next state
            var newCurrentValues = new Map<GOAPKeyEnum, object>(currentValues);
            var newGoalValues = new Map<GOAPKeyEnum, Func<object, bool>>(goalValues);
            var completeGoal = true;

            foreach (var effect in action.Effects)
            {
                newCurrentValues[effect.Key] = effect.Value(currentValues[effect.Key]);

                if (goalValues.ContainsKey(effect.Key) && !goalValues[effect.Key](newCurrentValues[effect.Key])) completeGoal = false;
            }

            foreach (var precondition in action.Preconditions)
            {
                if (completeGoal) newGoalValues[precondition.Key] = precondition.Value;
            }


            return new GOAPState(
                actions,
                newCurrentValues,//.Merge(action.Effects),
                newGoalValues,//.Merge(action.Preconditions),
                action,
                heuristic
            );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IGraphNode<string>>)this).GetEnumerator();
        }
    }
}
