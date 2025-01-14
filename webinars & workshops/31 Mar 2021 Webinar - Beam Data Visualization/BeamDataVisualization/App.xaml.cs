﻿using BeamDataVisualization.ViewModels;
using BeamDataVisualization.Views;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BeamDataVisualization
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private VMS.TPS.Common.Model.API.Application app;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var _patientId = String.Empty;
            var _courseId = string.Empty;
            var _planId = String.Empty;
            if (e.Args.Count() > 0)
            {
                _patientId = e.Args.ElementAt(0).Split(';').First();
                if (e.Args.Count() > 1) { _courseId = e.Args.ElementAt(0).Split(';').ElementAt(1); }
                if (e.Args.Count() > 2) { _planId = e.Args.ElementAt(0).Split(';').ElementAt(2); }
            }
            try
            {
                app = VMS.TPS.Common.Model.API.Application.CreateApplication();
                IEventAggregator eventAggregator = new EventAggregator();
                var mv = new MainView
                {
                    DataContext = new MainViewModel(
                        new PatientNavigationViewModel(app, _patientId, _courseId, _planId, eventAggregator),
                        new ScanPlotViewModel(eventAggregator))
                };
                mv.ShowDialog();
                app.Dispose();
            }
            catch (ApplicationException ex)
            {
                app.Dispose();
                MessageBox.Show(ex.Message);
            }
        }
    }
}
