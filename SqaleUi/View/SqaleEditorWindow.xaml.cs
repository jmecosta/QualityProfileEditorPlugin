﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqaleEditorControl.xaml.cs" company="Copyright © 2014 jmecsoftware">
//     Copyright (C) 2014 [jmecsoftware, jmecsoftware2014@tekla.com]
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
// You should have received a copy of the GNU Lesser General Public License along with this program; if not, write to the Free
// Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// --------------------------------------------------------------------------------------------------------------------

namespace SqaleUi.View
{
    using ExtensionHelpers;

    using ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SqaleEditorWindow
    {

        public SqaleEditorWindow()
        {
            InitializeComponent();

            this.DataContext = new SqaleEditorControlViewModel(new ConfigurationHelper(), null, null);
        }

        public SqaleEditorWindow(SqaleEditorControlViewModel model)
        {
            InitializeComponent();
            this.DataContext = model;
        }
    }
}
