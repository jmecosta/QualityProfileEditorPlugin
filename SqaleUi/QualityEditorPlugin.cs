// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityEditorPlugin.cs" company="Copyright © 2014 Tekla Corporation. Tekla is a Trimble Company">
//     Copyright (C) 2013 [Jorge Costa, Jorge.Costa@tekla.com]
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

namespace SqaleUi
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Media;

    using ExtensionTypes;

    using SonarRestService;

    using SqaleUi.View;
    using SqaleUi.ViewModel;

    using VSSonarPlugins;

    /// <summary>
    /// The quality editor plugin.
    /// </summary>
    [Export(typeof(IPlugin))]
    public class QualityEditorPlugin : IMenuCommandPlugin
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the editor.
        /// </summary>
        public SqaleGridVs Editor { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get assembly path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAssemblyPath()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        public void UpdateTheme(Color backgroundColor, Color foregroundColor)
        {
            if (this.Editor != null)
            {
                this.Model.ForeGroundColor = foregroundColor;
                this.Model.BackgroundColor = backgroundColor;
            }
        }

        /// <summary>
        /// The get header.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetHeader()
        {
            return "Sonar Quality Editor";
        }

        /// <summary>
        /// The get plugin description.
        /// </summary>
        /// <param name="vsinter">
        /// The vsinter.
        /// </param>
        /// <returns>
        /// The <see cref="PluginDescription"/>.
        /// </returns>
        public PluginDescription GetPluginDescription(IVsEnvironmentHelper vsinter)
        {
            string isEnabled = vsinter.ReadOptionFromApplicationData(GlobalIds.PluginEnabledControlId, "QualityEditorPlugin");

            var desc = new PluginDescription
                           {
                               Description = "Quality Editor Plugin", 
                               Enabled = true, 
                               Name = "QualityEditorPlugin", 
                               SupportedExtensions = "*", 
                               Version = this.GetVersion()
                           };

            if (string.IsNullOrEmpty(isEnabled))
            {
                desc.Enabled = true;
            }
            else if (isEnabled.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                desc.Enabled = true;
            }
            else
            {
                desc.Enabled = false;
            }

            return desc;
        }

        /// <summary>
        /// The get user control.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <param name="vshelper">
        /// The vshelper.
        /// </param>
        /// <returns>
        /// The <see cref="UserControl"/>.
        /// </returns>
        public UserControl GetUserControl(ISonarConfiguration configuration, Resource project, IVsEnvironmentHelper vshelper)
        {
            if (this.Editor == null)
            {
                this.Model = new SqaleGridVmVs(project, new SonarRestService(new JsonSonarConnector()), configuration);
                this.Editor = new SqaleGridVs(this.Model);                
            }

            return this.Editor;
        }

        public SqaleGridVmVs Model { get; set; }

        /// <summary>
        /// The get version.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// The update configuration.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <param name="vshelper">
        /// The vshelper.
        /// </param>
        public void UpdateConfiguration(ISonarConfiguration configuration, Resource project, IVsEnvironmentHelper vshelper)
        {
            this.Model.UpdateConfiguration(configuration, project, vshelper);
        }

        #endregion
    }
}