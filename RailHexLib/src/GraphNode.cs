using System;
using System.Collections.Generic;

namespace RailHexLib
{
    public class GraphNode<T> where T : IEquatable<T>, new()
    {
        public GraphNode(T v)
        {
            Value = v;
            Children = new List<GraphNode<T>>();
        }
        public T Value;
        public List<GraphNode<T>> Children;


        public GraphNode<T> AddToChildrenBy(T newItem, Func<T, T, bool> CanBeMerged)
        {
            // same nodes
            if (CanBeMerged(Value, newItem))
            {
                GraphNode<T> newNode = new GraphNode<T>(newItem);
                Children.Add(newNode);
                return newNode;
            }
            else
            {
                foreach (var child in Children)
                {
                    var added = child.AddToChildrenBy(newItem, CanBeMerged);
                    if (added != null) return added;
                }
                return null;
            }
        }
        /// <summary>
        /// Return reversed path from current node to target. First element is a parent of target element.
        /// </summary>
        /// <param name="target">Try to build path to this cell</param>
        /// <returns></returns>
        public List<T> PathTo(T target)
        {
            if (Value.Equals(target)) return new List<T>();
            else
            {
                foreach (var child in Children)
                {
                    var path = child.PathTo(target);
                    if (path != null)
                    {
                        path.Add(Value);
                        return path;
                    }
                }
            }
            return null;
        }
        public override string ToString()
        {
            return $"{Value} -> [{Children.Count}]";
        }
        public GraphNode<T> Merge(GraphNode<T> a, GraphNode<T> b)
        {
            throw new NotImplementedException("");
        }
    }

    public class HexNode<T> where T : IEquatable<T>
    {
        public T Value;
        // links
        public HexNode<T> Left;
        public HexNode<T> UpLeft;
        public HexNode<T> UpRight;
        public HexNode<T> Right;
        public HexNode<T> DownRight;
        public HexNode<T> DownLeft;
    }
}
