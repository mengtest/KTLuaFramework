using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Kernel.core
{
    public class TreeNode<T> : IEnumerable<TreeNode<T>>
    {
        public TreeNode(T data)
        {
            Data = data;
            Children = new LinkedList<TreeNode<T>>();
            ElementsIndex = new LinkedList<TreeNode<T>>();
            ElementsIndex.Add(this);
        }
        public bool IsLeaf => Children.Count == 0;
        public bool IsRoot => Parent == null;
        public int Level
        {
            get
            {
                if (IsRoot)
                {
                    return 0;
                }
                return Parent.Level + 1;
            }
        }
        public ICollection<TreeNode<T>> Children
        {
            get;
            set;
        }
        public T Data
        {
            get;
            set;
        }
        public TreeNode<T> Parent
        {
            get;
            set;
        }
        public override string ToString()
        {
            return Data != null ? Data.ToString() : "[data null]";
        }
        public TreeNode<T> AddChild(T child)
        {
            var childNode = new TreeNode<T>(child)
            {
                Parent = this
            };
            Children.Add(childNode);
            RegisterChildForSearch(childNode);
            return childNode;
        }
        public bool RemoveChild(T child)
        {
            var treeNode = FindTreeNode(node => node.Data.Equals(child));
            if (treeNode != null)
            {
                RetractChildForSearch(treeNode);
                return Children.Remove(treeNode);
            }
            return true;
        }
        public void Clear()
        {
            Children.Clear();
            ElementsIndex.Clear();
            Data = default(T);
        }
        #region searching
        private ICollection<TreeNode<T>> ElementsIndex
        {
            get;
            set;
        }
        private void RegisterChildForSearch(TreeNode<T> node)
        {
            ElementsIndex.Add(node);
            Parent?.RegisterChildForSearch(node);
        }
        private void RetractChildForSearch(TreeNode<T> node)
        {
            ElementsIndex.Remove(node);
            Parent?.RetractChildForSearch(node);
        }
        public TreeNode<T> FindTreeNode(Func<TreeNode<T>, bool> predicate)
        {
            return ElementsIndex.FirstOrDefault(predicate);
        }
        #endregion
        #region iterating
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            yield return this;
            foreach (var directChild in Children)
            {
                foreach (var anyChild in directChild)
                {
                    yield return anyChild;
                }
            }
        }
        #endregion
    }
}