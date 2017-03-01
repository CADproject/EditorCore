﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using CADController;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using CADView.Dialogs;
using Application = System.Windows.Application;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.MessageBox;
using System.Threading.Tasks;

namespace CADView
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
#region Public

        public MainWindowViewModel()
        {
            Controller = new ApplicationController();
            RenderPanel.Loaded += RenderPanelOnLoad;
            RenderPanel.Resized += RenderPanelOnResize;
            RenderPanel.Rendered += RenderPanelOnRender;
            RenderPanel.MouseFired += RenderPanelOnMouseFire;
        }

        ~MainWindowViewModel()
        {
            foreach (var documentViewModel in DocumentViewModels)
            {
                Controller.finalDocument(Session, documentViewModel.DocumentID);
            }
            Controller.finalSession(Session);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal ApplicationController Controller { get; private set; }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public void Init()
        {
            Session = Controller.initSession();
            _inited = true;

            _timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 33);
            _timer.Tick+= delegate
            {
                if (DocumentViewModels.Count == 0) return;
                Controller.draw(Session, DocumentViewModels[SelectedDocumentIndex].DocumentID);
            };
            _timer.Start();
        }

        public void CreateDocument()
        {
            var host = new WindowsFormsHost()
            {
                Child = new RenderPanel(),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            var model = new DocumentViewModel();
            DocumentViewModelsTabs.Add(new TabItem()
            {
                Content = host,
                DataContext = model
            });
            DocumentViewModels.Add(model);
            _selectedDocumentIndex = DocumentViewModelsTabs.Count - 1;
            OnPropertyChanged("SelectedDocumentIndex");
        }

        public int SelectedDocumentIndex
        {
            get { return _selectedDocumentIndex; }
            set
            {
                _selectedDocumentIndex = value;
                OnPropertyChanged();

                var size = ((WindowsFormsHost) DocumentViewModelsTabs[SelectedDocumentIndex].Content)
                    .Child.Size;
                Controller.activateDocement(Session, DocumentViewModels[SelectedDocumentIndex].DocumentID, size.Width,
                    size.Height);
            }
        }

        public ObservableCollection<TabItem> DocumentViewModelsTabs
        {
            get { return _documentViewModelsTabs; }
            set
            {
                _documentViewModelsTabs = value; 
                OnPropertyChanged("DocumentViewModels");
                OnPropertyChanged();
            }
        }

        public List<DocumentViewModel> DocumentViewModels
        {
            get { return _documentViewModels; }
        }

        public double WindowWidth
        {
            get { return _windowWidth; }
            set
            {
                _windowWidth = value;
                OnPropertyChanged();
            }
        }

        public double WindowHeight
        {
            get { return _windowHeight; }
            set
            {
                _windowHeight = value;
                OnPropertyChanged();
            }
        }

#endregion

#region Protected

#if OLDDOTNET
        protected virtual void OnPropertyChanged(string propertyName)
#else
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
#endif
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

#endregion

#region Private

        private uint _session;
        bool _inited;
        private bool _isActive;
        private RelayCommand _documentWorkCommand;
        private RelayCommand _controllerWorkCommand;
        private ObservableCollection<TabItem> _documentViewModelsTabs = new ObservableCollection<TabItem>();
        private int _selectedDocumentIndex;
        private DispatcherTimer _timer;
        private double _windowWidth;
        private double _windowHeight;
        private readonly List<DocumentViewModel> _documentViewModels = new List<DocumentViewModel>();

        private uint Session
        {
            get
            {
                if (!_inited)
                    throw new ApplicationException("No active session.");
                return _session;
            }
            set { _session = value; }
        }

        private void ProcessDocumentWork(object obj)
        {
            CreateDocument();
        }

        private void RenderPanelOnLoad(IntPtr hwnd, int w, int h)
        {
            var activeDocument = Controller.initDocument(Session, hwnd);
            Controller.activateDocement(Session, activeDocument, w, h);
            DocumentViewModels[SelectedDocumentIndex].Title = "Document # " + activeDocument;
            DocumentViewModels[SelectedDocumentIndex].DocumentID = activeDocument;
            IsActive = true;
        }

        private void RenderPanelOnResize(int w, int h)
        {
            Controller.resizeDocument(Session, DocumentViewModels[SelectedDocumentIndex].DocumentID, w, h);
        }

        private void RenderPanelOnRender()
        {
            if (DocumentViewModels.Count == 0) return;
            Controller.draw(Session, DocumentViewModels[SelectedDocumentIndex].DocumentID);
        }

        private void RenderPanelOnMouseFire(MouseEventArgs args)
        {
            //TODO: передавать инты и внутри пытаться аккуратно преобразовать.
            Controller.eventHendling(DocumentViewModels[SelectedDocumentIndex].DocumentID, (ApplicationController.MouseButtons) (int) args.Button, args.X, args.Y, args.Delta);
        }

        private async void ProcessControllerWork(object obj)
        {
            IsActive = false;
            ApplicationController.operations type = (ApplicationController.operations) obj;

            object [] data= new object[0];

            Window modalWindow = null;
            Window separatedWindow = null;

            switch (type)
            {
                case ApplicationController.operations.OpPointCreate:
                    modalWindow = new OnePointDialog();
                    modalWindow.Title = "Create Point";
                    break;
                case ApplicationController.operations.OpLineCreate:
                    modalWindow = new TwoPointDialog();
                    modalWindow.Title = "Create Line";
                    break;
                case ApplicationController.operations.OpCircleCreate:
                    modalWindow = new TwoPointDialog();
                    modalWindow.Title = "Create Circle";
                    break;
                case ApplicationController.operations.OpContourCreate:
                    modalWindow = new ElementIdInputDialog();
                    modalWindow.Title = "Create Contour";
                    break;
                case ApplicationController.operations.OpDeleteObject:
                    modalWindow = new ElementIdInputDialog();
                    modalWindow.Title = "Create Contour";
                    break;
                case ApplicationController.operations.OpDestroyContour:
                    modalWindow = new ElementIdInputDialog();
                    modalWindow.Title = "Create Contour";
                    break;
                case ApplicationController.operations.OpSetBackgroundColor:
                    separatedWindow = Dialogs.ColorDialog.Instance;
                    separatedWindow.Title = "Set color number";
                    break;
                case ApplicationController.operations.OpSetLayersToShow:
                    separatedWindow = LayersDialog.Instance;
                    separatedWindow.Title = "Set layer number";
                    break;
            }
            
            try
            {
                if (modalWindow != null)
                {
                    bool start = true;
                    if (modalWindow.ShowDialog() == true)
                    {
                        if (modalWindow is IDataDialog)
                            data = ((IDataDialog) modalWindow).Data.ToArray();
                    }
                    else
                        start = false;
                    if (start)
                        await Task.Run(delegate
                        {
                            Controller.procOperation(Session, DocumentViewModels[SelectedDocumentIndex].DocumentID,
                                (ApplicationController.operations)obj, data);
                        });
                }
                if (separatedWindow != null)
                {
                    separatedWindow.Show();
                    separatedWindow.Activate();
                    if (separatedWindow is ICallbackDialog)
                        ((ICallbackDialog) separatedWindow).DataChanged += async delegate(object sender, EventArgs args)
                        {
                            try
                            {
                                List<object> cdata = (List<object>) sender;
                                await Task.Run(delegate
                                {
                                    IsActive = false;
                                    Controller.procOperation(Session,
                                        DocumentViewModels[SelectedDocumentIndex].DocumentID,
                                        (ApplicationController.operations) obj, cdata.ToArray());
                                });
                            }
                            catch (Exception e)
                            {
                                if (modalWindow != null && modalWindow.IsVisible)
                                    modalWindow.Close();
                                MessageBox.Show("Exception: " + e.Message);
                            }
                            finally
                            {
                                ((ICallbackDialog)separatedWindow).DataProcessComplete();
                                IsActive = true;
                            }
                        };
                }
            }
            catch (Exception e)
            {
                if (modalWindow != null && modalWindow.IsVisible)
                    modalWindow.Close();
                MessageBox.Show("Exception: " + e.Message);
            }
            finally
            {
                IsActive = true;
            }
        }

#endregion

#region Commands

        public RelayCommand DocumentWorkCommand
        {
            get
            {
                return _documentWorkCommand ?? (_documentWorkCommand = new RelayCommand(ProcessDocumentWork,
                    o => (DocumentViewModels.Count > 0 && IsActive) || DocumentViewModels.Count == 0));
            }
        }

        public RelayCommand ControllerWorkCommand
        {
            get
            {
                return _controllerWorkCommand ?? (_controllerWorkCommand = new RelayCommand(ProcessControllerWork));
            }
        }

#endregion
    }
}
