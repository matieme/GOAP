using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using System.Linq;

public class Navigation : SingletonObject<Navigation>
{
    private List<Node> _nodes = new List<Node>();

    void Start()
    {
        foreach (Transform xf in transform)
        {
            var wp = xf.GetComponent<Node>();
            if (wp != null)
                _nodes.Add(wp);
        }
    }

    public IEnumerable<Node> All()
    {
        return _nodes;
    }

    public Node NearestTo(Vector3 pos)
    {
        return All()
            .OrderBy(wp => {
                var d = wp.transform.position - pos;
                d.y = 0;
                return d.sqrMagnitude;
            })
            .First();
    }
}
