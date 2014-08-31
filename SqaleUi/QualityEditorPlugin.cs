// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityEditorPlugin.cs" company="">
//   
// </copyright>
// <summary>
//   The quality editor plugin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SqaleUi
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using System.Windows.Controls;

    using ExtensionTypes;

    using SqaleUi.View;

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
        public SqaleEditorControl Editor { get; set; }

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
        public UserControl GetUserControl(ConnectionConfiguration configuration, Resource project, IVsEnvironmentHelper vshelper)
        {
            if (this.Editor == null)
            {
                this.Editor = new SqaleEditorControl();
            }

            return this.Editor;
        }

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
        public void UpdateConfiguration(ConnectionConfiguration configuration, Resource project, IVsEnvironmentHelper vshelper)
        {
            this.Editor.UpdateConfiguration(configuration, project, vshelper);
        }

        #endregion
    }
}