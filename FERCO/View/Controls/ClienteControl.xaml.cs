﻿using FERCO.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FERCO.View.Controls
{
    /// <summary>
    /// Interaction logic for ClienteControl.xaml
    /// </summary>
    public partial class ClienteControl : UserControl
    {
        public ClienteControl()
        {
            InitializeComponent();
            this.DataContext = new ClienteControlViewModel();

            this.Loaded += (s, e) =>
            {
                BusquedaTextBox.Focus();
            };
        }
    }
}
