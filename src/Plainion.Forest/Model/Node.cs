using System;
using Plainion.Collections;
using Microsoft.Practices.Prism.Mvvm;

namespace Plainion.Forest.Model
{
    public class Node : BindableBase, INode
    {
        private Node myParent;
        private string myCaption;
        private string myContent;
        private DateTime myLastModifiedAt;
        private bool myIsExpanded;
        private string myOrigin;

        public Node()
        {
            Parent = null;

            Id = Guid.NewGuid().ToString();
            Children = new NodeCollection( this );

            CreatedAt = DateTime.Now;

            TrackModified = true;
        }

        internal bool TrackModified
        {
            get;
            set;
        }

        public string Id { get; private set; }

        public string Caption
        {
            get
            {
                return myCaption;
            }
            set
            {
                SetProperty( ref myCaption, value );
                MarkAsModified();
            }
        }

        private void MarkAsModified()
        {
            if( TrackModified )
            {
                LastModifiedAt = DateTime.Now;
            }
        }

        public string Content
        {
            get
            {
                return myContent;
            }
            set
            {
                SetProperty( ref myContent, value );
                MarkAsModified();
            }
        }

        public DateTime CreatedAt
        {
            get;
            private set;
        }

        public DateTime LastModifiedAt
        {
            get
            {
                return myLastModifiedAt;
            }
            private set
            {
                SetProperty( ref myLastModifiedAt, value );
            }
        }

        INode INode.Parent
        {
            get
            {
                return Parent;
            }
        }

        public Node Parent
        {
            get { return myParent; }
            internal set
            {
                if( myParent == value )
                {
                    return;
                }

                if( myParent != null && value != null )
                {
                    if( myParent.GetRoot() != value.GetRoot() )
                    {
                        Origin = myParent.Id;
                    }
                }

                if( myParent != null )
                {
                    myParent.Children.Remove( this );
                }

                SetProperty( ref myParent, value );
                MarkAsModified();
            }
        }

        public string Origin
        {
            get { return myOrigin; }
            private set { SetProperty( ref myOrigin, value ); }
        }

        /// <summary>
        /// lets keep this here for now - we might want to persist this anyway to give user ui back as he lost it.
        /// probably we still store it separate from actual data
        /// </summary>
        // TODO: move to "presentation state"
        public bool IsExpanded
        {
            get
            {
                return myIsExpanded;
            }
            set
            {
                SetProperty( ref myIsExpanded, value );
            }
        }

        IObservableEnumerable<INode> INode.Children
        {
            get
            {
                return Children;
            }
        }

        internal NodeCollection Children
        {
            get;
            private set;
        }
    }
}
