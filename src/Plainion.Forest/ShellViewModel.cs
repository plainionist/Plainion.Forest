using System;
using System.ComponentModel.Composition;
using Plainion.Forest.Model;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Plainion.AppFw.Wpf.Services;
using Plainion.AppFw.Wpf.ViewModels;
using Plainion.Prism.Events;

namespace Plainion.Forest
{
    [Export( typeof( ShellViewModel ) )]
    public class ShellViewModel : BindableBase
    {
        private const string AppName = "Plainion.Forest";
        private IProjectService<Project> myProjectService;
        private IPersistenceService<Project> myPersistenceService;

        [ImportingConstructor]
        public ShellViewModel( IProjectService<Project> projectService, IEventAggregator eventAggregator, IPersistenceService<Project> persistenceService )
        {
            myProjectService = projectService;
            myPersistenceService = persistenceService;

            eventAggregator.GetEvent<ApplicationReadyEvent>().Subscribe( x => OnApplicationReady() );
        }

        [Import]
        public ProjectLifecycleViewModel<Project> ProjectLifecycleViewModel { get; set; }

        [Import]
        public TitleViewModel<Project> TitleViewModel { get; set; }

        private void OnApplicationReady()
        {
            TitleViewModel.ApplicationName = AppName;

            ProjectLifecycleViewModel.ApplicationName = AppName;
            ProjectLifecycleViewModel.FileFilter = "Plainion Forest Projects (*.bf)|*.bf";
            ProjectLifecycleViewModel.FileFilterIndex = 0;
            ProjectLifecycleViewModel.DefaultFileExtension = ".bf";

            var args = Environment.GetCommandLineArgs();
            if ( args.Length == 2 )
            {
                myProjectService.Project = myPersistenceService.Load( args[ 1 ] );
            }
            else
            {
                myProjectService.Project = myProjectService.CreateEmptyProject( null );
            }
        }
    }
}
