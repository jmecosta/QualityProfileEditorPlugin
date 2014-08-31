using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqaleUi.ViewModel
{
    using PropertyChanged;

    using SqaleManager;

    /// <summary>
    /// The import log view model.
    /// </summary>
    [ImplementPropertyChanged]
    public class ImportLogViewModel
    {
        public ImportLogViewModel()
        {
            this.ImportLog = new List<ImportLogEntry>();
        }

        public List<ImportLogEntry> ImportLog { get; set; }
    }
}
