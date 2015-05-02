using System.Collections.Generic;
using System.Linq;

namespace Plainion.Forest.Model
{
    public static class TreeAlgorithms
    {
        public static IEnumerable<INode> AllNodes( this Project project )
        {
            return project.Roots
                .SelectMany( n => n.AllNodes() );
        }

        public static IEnumerable<INode> AllNodes( this INode root )
        {
            yield return root;

            foreach( var child in root.Children )
            {
                foreach( var n in child.AllNodes() )
                {
                    yield return n;
                }
            }
        }

        public static INode GetRoot( this INode self )
        {
            if( self == null )
            {
                return null;
            }

            while( self.Parent != null )
            {
                self = self.Parent;
            }

            return self;
        }

        public static IEnumerable<INode> GetPathToRoot( this INode self )
        {
            if( self == null )
            {
                yield break;
            }

            while( self != null )
            {
                yield return self;

                self = self.Parent;
            }
        }
    }
}
