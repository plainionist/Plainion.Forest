using System.Collections.Generic;
using System.Linq;
using Plainion.AppFw.Wpf.Model;

namespace Plainion.Forest.Model
{
    public class Project : ProjectBase
    {
        public Project()
        {
            Roots = new List<INode>();
        }

        // TODO: set IsDirty = true
        public IList<INode> Roots
        {
            get;
            private set;
        }

        public void ChangeCaption( INode node, string caption )
        {
            if ( node.Caption == caption )
            {
                return;
            }

            ( (Node)node ).Caption = caption;

            IsDirty = true;
        }

        internal void ChangeContent( INode node, string content )
        {
            if ( node.Content == content )
            {
                return;
            }

            ( (Node)node ).Content = content;

            IsDirty = true;
        }
        
        public void MoveNode( INode nodeToMove, INode targetNode, MoveOperation operation )
        {
            MoveNode( (Node)nodeToMove, (Node)targetNode, operation );

            IsDirty = true;
        }

        private void MoveNode( Node nodeToMove, Node targetNode, MoveOperation operation )
        {
            var siblings = (NodeCollection)targetNode.Parent.Children;
            var dropPos = siblings.IndexOf( targetNode );

            if ( operation == MoveOperation.MoveAfter )
            {
                dropPos++;
            }

            if ( siblings.Contains( nodeToMove ) )
            {
                var oldPos = siblings.IndexOf( nodeToMove );
                if ( oldPos < dropPos )
                {
                    // ObservableCollection first removes the item and then reinserts which invalidates the index
                    dropPos--;
                }

                siblings.Move( oldPos, dropPos );
            }
            else
            {
                if ( dropPos < siblings.Count )
                {
                    siblings.Insert( dropPos, nodeToMove );
                }
                else
                {
                    siblings.Add( nodeToMove );
                }
            }

            IsDirty = true;
        }

        public void AddChildTo( INode child, INode newParent )
        {
            ( (Node)newParent ).Children.Add( (Node)child );

            IsDirty = true;
        }

        public INode GetScope( string scope )
        {
            return Roots.SingleOrDefault( n => n.Caption == scope );
        }

        internal INode CreateChild( INode parent )
        {
            try
            {
                var node = new Node();
                ( ( Node )parent ).Children.Add( node );
                return node;
            }
            finally
            {
                IsDirty = true;
            }
        }

        internal void DeleteNode( INode Node )
        {
            ( (Node)Node ).Parent = null;
            IsDirty = true;
        }

        internal INode GetNode( string origin )
        {
            return this.AllNodes().First( n => n.Id == origin );
        }
    }
}
