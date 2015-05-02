using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using Plainion.Forest.Model;
using Plainion;
using Plainion.AppFw.Wpf.Services;

namespace Plainion.Forest.Services
{
    internal class Serializer : ISerializer<Project>
    {
        private const int VERSION = 1;
        private string myProjectFile;

        public string ProjectFile
        {
            get { return myProjectFile; }
            private set
            {
                Contract.RequiresNotNullNotEmpty( value, "projectFile" );

                if ( Path.GetExtension( value ) != ".bf" )
                {
                    throw new ArgumentException( "Only files with .bf extension supported" );
                }

                myProjectFile = value;
                DbFolder = Path.Combine( Path.GetDirectoryName( myProjectFile ), Path.GetFileNameWithoutExtension( myProjectFile ) );
            }
        }

        public string DbFolder
        {
            get;
            private set;
        }

        public void Serialize( Project project )
        {
            ProjectFile = project.Location;

            if ( !Directory.Exists( DbFolder ) )
            {
                Directory.CreateDirectory( DbFolder );
            }

            WriteProject( project );

            foreach ( var root in project.Roots )
            {
                WriteTree( root, Path.Combine( DbFolder, root.Id + ".tree" ) );
            }

            WriteNodes( project.Roots );

            // cleanup deleted nodes
            var allNodes = project.AllNodes()
                .Select( n => n.Id )
                .ToList();
            var allFiles = Directory.GetFiles( DbFolder );
            var filesToDelete = allFiles
                .Where( f => !allNodes.Contains( Path.GetFileNameWithoutExtension( f ) ) );

            foreach ( var f in filesToDelete )
            {
                File.Delete( f );
            }
        }

        private void WriteProject( Project project )
        {
            var doc = new XElement( "Project",
                new XElement( "Version", VERSION ) );

            foreach ( var root in project.Roots )
            {
                var xNode = CreateXmlNode( root );

                doc.Add( xNode );
            }

            WriteXDocument( doc, ProjectFile );
        }

        private static XElement CreateXmlNode( INode root )
        {
            return new XElement( "Node",
                new XAttribute( "Id", root.Id ),
                new XElement( "Caption", new XCData( root.Caption ) ) );
        }

        private void WriteTree( INode root, string file )
        {
            var doc = new XElement( "Tree" );

            WriteTree( doc, root );

            WriteXDocument( doc, file );
        }

        private void WriteTree( XElement doc, INode root )
        {
            var xNode = CreateXmlNode( root );

            doc.Add( xNode );

            foreach ( var child in root.Children )
            {
                WriteTree( xNode, child );
            }
        }

        private static void WriteXDocument( XElement doc, string file )
        {
            var settings = new XmlWriterSettings();
            settings.CloseOutput = true;
            settings.Indent = true;
            using ( var writer = XmlWriter.Create( file, settings ) )
            {
                doc.WriteTo( writer );
            }
        }

        private void WriteNodes( IEnumerable<INode> nodes )
        {
            foreach ( var node in nodes )
            {
                WriteNode( node );
            }
        }

        private void WriteNode( INode node )
        {
            var doc = CreateXmlNode( node );
            doc.Add( new XAttribute( "Created", node.CreatedAt.Ticks ) );
            doc.Add( new XAttribute( "LastModified", node.LastModifiedAt.Ticks ) );
            doc.Add( new XElement( "Content", new XCData( node.Content != null ? node.Content : string.Empty ) ) );

            if ( node.Origin != null )
            {
                doc.Add( new XAttribute( "Origin", node.Origin ) );
            }

            WriteXDocument( doc, Path.Combine( DbFolder, node.Id + ".node" ) );

            WriteNodes( node.Children );
        }

        public Project Deserialize( string file )
        {
            ProjectFile = file;

            var project = new Project();

            foreach ( var rootId in ReadProjectRoots() )
            {
                ReadTree( project, rootId );
            }

            return project;
        }

        private IEnumerable<string> ReadProjectRoots()
        {
            return XElement.Load( ProjectFile )
                .Elements( "Node" )
                .Select( n => n.Attribute( "Id" ).Value )
                .ToList();
        }

        private void ReadTree( Project project, string rootId )
        {
            var doc = XElement.Load( Path.Combine( DbFolder, rootId + ".tree" ) );

            var root = ReadTree( doc.Elements( "Node" ).Single() );

            project.Roots.Add( root );

            // enable modification tracking

            foreach ( var node in project.AllNodes() )
            {
                ( (Node)node ).TrackModified = true;
            }
        }

        private Node ReadTree( XElement xRoot )
        {
            var root = ReadNode( xRoot.Attribute( "Id" ).Value );

            foreach ( var xChild in xRoot.Elements( "Node" ) )
            {
                root.Children.Add( ReadTree( xChild ) );
            }

            return root;
        }

        private Node ReadNode( string id )
        {
            var node = (Node)FormatterServices.GetUninitializedObject( typeof( Node ) );
            node.TrackModified = false;

            var xNode = XElement.Load( Path.Combine( DbFolder, id + ".node" ) );

            node.GetType()
                 .GetProperty( "Id", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance )
                 .SetValue( node, id );

            node.GetType()
                .GetProperty( "CreatedAt", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance )
                .SetValue( node, new DateTime( long.Parse( xNode.Attribute( "Created" ).Value ) ) );

            node.Caption = xNode.Element( "Caption" ).Value;
            node.Content = xNode.Element( "Content" ).Value;

            var originAttr = xNode.Attribute( "Origin" );
            if ( originAttr != null )
            {
                node.GetType()
                    .GetProperty( "Origin", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance )
                    .SetValue( node, originAttr.Value );
            }

            node.GetType()
                .GetProperty( "Children", BindingFlags.NonPublic | BindingFlags.Instance )
                .SetValue( node, new NodeCollection( node ) );

            // override "LastModified" at latest

            node.GetType()
                .GetProperty( "LastModifiedAt", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance )
                .SetValue( node, new DateTime( long.Parse( xNode.Attribute( "LastModified" ).Value ) ) );

            return node;
        }
    }
}
