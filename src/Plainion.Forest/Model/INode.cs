using System;
using System.ComponentModel;
using Plainion.Collections;

namespace Plainion.Forest.Model
{
    public interface INode : INotifyPropertyChanged
    {
        string Id { get; }
        string Caption { get; }
        INode Parent { get; }
        bool IsExpanded { get; set; }
        IObservableEnumerable<INode> Children { get; }
        string Content { get; }
        DateTime CreatedAt { get; }
        DateTime LastModifiedAt { get; }
        string Origin { get; }
  }
}
