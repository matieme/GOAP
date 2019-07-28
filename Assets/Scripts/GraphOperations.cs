using System.Linq;
using System.Collections.Generic;
using System;
using FP;

namespace GOAP
{
    public static class GraphOperations
    {
        public static IEnumerable<IGraphNode<T>> AStar<T>(this IGraphNode<T> from, IGraphNode<T> to)
        {
            //'let'
            //Initial state of the algorithm
            var closedSet = new HashSet<IGraphNode<T>>();
            var openSet = new HashSet<IGraphNode<T>>() { from };
            var gScore = new Map<IGraphNode<T>, float>() { { from, 0 } };
            var fScore = new Map<IGraphNode<T>, float>() { { from, from.Heuristic(to) } };
            var previous = new Map<IGraphNode<T>, IGraphNode<T>>();
            var initialState = AStarState.New(closedSet, openSet, gScore, fScore, previous);

            //Compute final state from a sequence generated from the initial state
            //(the functional replacement of the queue), that is the last state of the following
            //generated sequence
            var finalState = Generate(initialState, state =>
            {
                //If we found the last node, we're finished generating states
                if (state.lastNode != null)
                    return null;

                //'let'
                //Get the one with lowest fScore, lazyness prevents the full query from executing
                var node = state.openSet.OrderBy(x => state.fScore[x]).First();
                //Reached the target, finish the state generation
                if (node.Satisfies(to) || openSet.Count == 0)
                    return FinalAStarState(state, node);

                //'in'
                return NextAStarState(CloseNode(state, node), node, to);
            }).Last();

            //'in'
            //Reconstruct the path
            return Generate(finalState.lastNode, node => finalState.previous[node]).Reverse();
        }

        static AStarState<T> NextAStarState<T>(AStarState<T> initialState, IGraphNode<T> node,
            IGraphNode<T> to
        )
        {
            //'let'
            var nodeGScore = initialState.gScore[node];

            //'in'
            return node.Where(x => !initialState.closedSet.Contains(x.Second)).Aggregate(
                initialState,
                (state, transition) =>
                {
                    var newGScore = nodeGScore + transition.First;
                    var neighbor = transition.Second;

                    if (!state.openSet.Contains(neighbor) || newGScore < state.gScore[neighbor])
                    {
                        state.gScore[neighbor] = newGScore;
                        state.fScore[neighbor] = newGScore + neighbor.Heuristic(to);
                        state.previous[neighbor] = node;
                        state.openSet.Add(neighbor);
                    }

                    return state;
                }
            );
        }

        static AStarState<T> CloseNode<T>(AStarState<T> state, IGraphNode<T> node)
        {
            state.openSet.Remove(node);
            state.closedSet.Add(node);
            return state;
        }

        static AStarState<T> FinalAStarState<T>(AStarState<T> state, IGraphNode<T> node)
        {
            state.lastNode = node;
            return state;
        }

        //Use of 'yield' as a 'lazy eval' operator, the exact opposite of 'seq' in Haskell 
        static IEnumerable<Src> Generate<Src>(Src start, Func<Src, Src> generator)
        {
            while (true)
            {
                if (start == null)
                    yield break;

                yield return start;
                start = generator(start);
            }
        }
    }

    class AStarState<T>
    {
        public HashSet<IGraphNode<T>> closedSet;
        public HashSet<IGraphNode<T>> openSet;
        public Map<IGraphNode<T>, float> fScore;
        public Map<IGraphNode<T>, float> gScore;
        public Map<IGraphNode<T>, IGraphNode<T>> previous;
        public IGraphNode<T> lastNode;
    }

    static class AStarState
    {
        public static AStarState<T> New<T>(
            HashSet<IGraphNode<T>> closedSet,
            HashSet<IGraphNode<T>> openSet,
            Map<IGraphNode<T>, float> fScore,
            Map<IGraphNode<T>, float> gScore,
            Map<IGraphNode<T>, IGraphNode<T>> previous
        )
        {
            var result = new AStarState<T>();
            result.closedSet = closedSet;
            result.openSet = openSet;
            result.fScore = fScore;
            result.gScore = gScore;
            result.previous = previous;
            return result;
        }
    }
}
