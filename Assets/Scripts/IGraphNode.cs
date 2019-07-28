using System.Collections.Generic;
using FP;

namespace GOAP
{
    public interface IGraphNode<T> : IEnumerable<Tuple<float, IGraphNode<T>>>
    {
        T Content { get; }
        bool Satisfies(IGraphNode<T> goal);
        float Heuristic(IGraphNode<T> goal);
    }
}