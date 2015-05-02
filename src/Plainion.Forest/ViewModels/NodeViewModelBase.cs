using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Plainion.Forest.Events;
using Plainion.Forest.Model;
using Plainion.Forest.Services;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Plainion;
using Plainion.AppFw.Wpf.Services;

namespace Plainion.Forest.ViewModels
{
    public abstract class NodeViewModelBase : BindableBase
    {
        private INode myNode;
        private ObservableCollection<NodeViewModel> myChildren;
        private ICollectionView myVisibleChildren;
        private INodeViewModelFactory myNodeViewModelFactory;
        private bool myShowContentHint;

        protected NodeViewModelBase( INodeViewModelFactory nodeViewModelFactory, IEventAggregator eventAggregator, IProjectService<Project> projectService )
        {
            Contract.RequiresNotNull( nodeViewModelFactory, "nodeViewModelFactory" );
            Contract.RequiresNotNull( eventAggregator, "eventAggregator" );
            Contract.RequiresNotNull( projectService, "projectRepository" );

            myNodeViewModelFactory = nodeViewModelFactory;
            EventAggregator = eventAggregator;
            ProjectService = projectService;

            NewCommand = new DelegateCommand( AddNewChild );
            ExpandAllCommand = new DelegateCommand( ExpandAll );
            CollapseAllCommand = new DelegateCommand( CollapseAll );

            ShowContentHint = false;
        }

        protected IEventAggregator EventAggregator
        {
            get;
            private set;
        }

        protected IProjectService<Project> ProjectService
        {
            get;
            private set;
        }

        public ICommand ExpandAllCommand
        {
            get;
            private set;
        }

        public ICommand CollapseAllCommand
        {
            get;
            private set;
        }

        protected virtual void ExpandAll()
        {
            foreach( var child in Children )
            {
                child.ExpandAll();
            }
        }

        protected virtual void CollapseAll()
        {
            foreach( var child in Children )
            {
                child.CollapseAll();
            }
        }

        private void AddNewChild()
        {
            var child = ProjectService.Project.CreateChild( Node );
            EventAggregator.GetEvent<NodeActivatedEvent>().Publish( child );
        }

        public ICommand NewCommand
        {
            get;
            private set;
        }

        public INode Node
        {
            get
            {
                return myNode;
            }
            protected set
            {
                if( myNode == value )
                {
                    return;
                }

                myNode = value;

                UpdateChildren();

                myNode.Children.CollectionChanged += OnNodeChildrenChanged;

                OnPropertyChanged( "Root" );
            }
        }

        private void OnNodeChildrenChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            if( e.Action == NotifyCollectionChangedAction.Add )
            {
                var pos = e.NewStartingIndex;
                foreach( Node node in e.NewItems )
                {
                    var m = myNodeViewModelFactory.Create( node );
                    m.ShowContentHint = ShowContentHint;
                    Children.Insert( pos, m );
                }
            }
            else if( e.Action == NotifyCollectionChangedAction.Move )
            {
                Children.Move( e.OldStartingIndex, e.NewStartingIndex );
            }
            else if( e.Action == NotifyCollectionChangedAction.Remove )
            {
                foreach( var child in Children.ToList() )
                {
                    if( e.OldItems.Contains( child.Node ) )
                    {
                        Children.Remove( child );
                    }
                }
            }
            else if( e.Action == NotifyCollectionChangedAction.Replace )
            {
                throw new NotImplementedException();
            }
            else if( e.Action == NotifyCollectionChangedAction.Reset )
            {
                UpdateChildren();
            }

            OnPropertyChanged( "ContentHint" );
        }

        public bool ShowContentHint
        {
            get { return myShowContentHint; }
            set
            {
                if( myShowContentHint == value )
                {
                    return;
                }

                myShowContentHint = value;

                foreach( var child in Children )
                {
                    child.ShowContentHint = myShowContentHint;
                }
            }
        }

        public string ContentHint
        {
            get
            {
                return ShowContentHint && Children.Count > 0
                    ? string.Format( "[{0}]", Children.Count )
                    : string.Empty;
            }
        }

        public ObservableCollection<NodeViewModel> Children
        {
            get
            {
                if( myChildren == null )
                {
                    myChildren = new ObservableCollection<NodeViewModel>();

                    UpdateChildren();

                    OnPropertyChanged( "Children" );
                }

                return myChildren;
            }
        }

        private void UpdateChildren()
        {
            Children.Clear();

            if( Node != null )
            {
                foreach( var child in Node.Children )
                {
                    var m = myNodeViewModelFactory.Create( child );
                    m.ShowContentHint = ShowContentHint;
                    Children.Add( m );
                }
            }

            OnPropertyChanged( "ContentHint" );
            VisibleChildren.Refresh();
        }

        public ICollectionView VisibleChildren
        {
            get
            {
                if( myVisibleChildren == null )
                {
                    myVisibleChildren = CollectionViewSource.GetDefaultView( Children );
                    myVisibleChildren.Filter = i => !( ( NodeViewModel )i ).IsFilteredOut;

                    OnPropertyChanged( "VisibleChildren" );
                }
                return myVisibleChildren;
            }
        }
    }
}
