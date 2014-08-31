namespace SqaleUi.ViewModel
{
    using System;

    using SqaleManager;

    public class RuleFilter : IFilter
    {
        private readonly IFilterOption filterOption;

        public RuleFilter(IFilterOption filterOption)
        {
            this.filterOption = filterOption;
        }

        public bool FilterFunction(object parameter)
        {
            return ((Rule)parameter).configKey.IndexOf(this.filterOption.FilterTermConfigKey, StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                   ((Rule)parameter).description.IndexOf(this.filterOption.FilterTermDescription, StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                   ((Rule)parameter).key.IndexOf(this.filterOption.FilterTermKey, StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                   ((Rule)parameter).name.IndexOf(this.filterOption.FilterTermName, StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                   ((Rule)parameter).repo.IndexOf(this.filterOption.FilterTermRepo, StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                   (this.filterOption.FilterTermCategory == null || ((Rule)parameter).category.Equals(this.filterOption.FilterTermCategory)) &&
                   (this.filterOption.FilterTermSubCategory == null || ((Rule)parameter).subcategory.Equals(this.filterOption.FilterTermSubCategory)) &&
                   (this.filterOption.FilterTermRemediationFunction == null || ((Rule)parameter).remediationFunction.Equals(this.filterOption.FilterTermRemediationFunction)) &&
                   (this.filterOption.FilterTermSeverity == null || ((Rule)parameter).severity.Equals(this.filterOption.FilterTermSeverity));
        }

        public bool IsEnabled()
        {
            return !string.IsNullOrEmpty(this.filterOption.FilterTermConfigKey)    ||
                    !string.IsNullOrEmpty(this.filterOption.FilterTermDescription) ||
                    !string.IsNullOrEmpty(this.filterOption.FilterTermKey)         ||
                    !string.IsNullOrEmpty(this.filterOption.FilterTermName)        ||
                    !string.IsNullOrEmpty(this.filterOption.FilterTermRepo)        ||
                     this.filterOption.FilterTermCategory                  != null ||
                     this.filterOption.FilterTermSubCategory               != null ||
                     this.filterOption.FilterTermRemediationFunction        != null ||
                     this.filterOption.FilterTermSeverity                  != null;
        }
    }
}