namespace SqaleUi
{
    using System.Collections.ObjectModel;
    using System.IO;

    using SqaleManager;

    /// <summary>
    /// The sqale project.
    /// </summary>
    public class SqaleProject
    {

        public SqaleProject()
        {
            this.Manager = new SqaleManager();
        }

        public SqaleManager Manager { get; set; }

        public SqaleModel Model { get; set; }



        
    }
}
