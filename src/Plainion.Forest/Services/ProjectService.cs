using System.ComponentModel.Composition;
using Plainion.Forest.Model;
using Plainion.AppFw.Wpf.Services;

namespace Plainion.Forest.Services
{
    internal class ProjectService : ProjectService<Project>
    {
        public override Project CreateEmptyProject( string location )
        {
            var project = base.CreateEmptyProject( location );

            project.Roots.Add( new Node { Caption = "Backlog" } );
            project.Roots.Add( new Node { Caption = "Planning" } );

            return project;
        }
    }
}
