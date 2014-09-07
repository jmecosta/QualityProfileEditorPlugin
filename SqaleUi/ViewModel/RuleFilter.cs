namespace SqaleUi.ViewModel
{
    using System;
    using System.Linq;

    using ExtensionTypes;

    using SqaleManager;

    using SqaleUi.helpers;

    public class RuleFilter : IFilter
    {
        private readonly IFilterOption filterOption;

        public RuleFilter(IFilterOption filterOption)
        {
            this.filterOption = filterOption;
        }

        public bool FilterFunction(object parameter)
        {
            var isTagPresent = this.IsTagPresent((Rule)parameter);
            var isRuleEnabled = this.IsRuleEnabled((Rule)parameter);

            var include = ((Rule)parameter).ConfigKey.IndexOf(this.filterOption.FilterTermConfigKey, StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                   ((Rule)parameter).Description.IndexOf(this.filterOption.FilterTermDescription, StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                   ((Rule)parameter).Key.IndexOf(this.filterOption.FilterTermKey, StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                   ((Rule)parameter).Name.IndexOf(this.filterOption.FilterTermName, StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                   ((Rule)parameter).Repo.IndexOf(this.filterOption.FilterTermRepo, StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                   (this.filterOption.FilterTermCategory == null || ((Rule)parameter).Category.Equals(this.filterOption.FilterTermCategory)) &&
                   (this.filterOption.FilterTermSubCategory == null || ((Rule)parameter).Subcategory.Equals(this.filterOption.FilterTermSubCategory)) &&
                   (this.filterOption.FilterTermRemediationFunction == null || ((Rule)parameter).RemediationFunction.Equals(this.filterOption.FilterTermRemediationFunction)) &&
                   (this.filterOption.FilterTermSeverity == null || ((Rule)parameter).Severity.Equals(this.filterOption.FilterTermSeverity));

            return include && isTagPresent && isRuleEnabled;
        }

        private bool IsRuleEnabled(Rule parameter)
        {
            if (parameter.Enabled && this.filterOption.FilterTermEnabled.Contains("Enabled")) return true;
            if (!parameter.Enabled && this.filterOption.FilterTermEnabled.Contains("Disabled")) return true;

            return false;
        }

        private bool IsTagPresent(Rule parameter)
        {
            if (string.IsNullOrEmpty(this.filterOption.FilterTermTag))
            {
                return true;
            }

            return parameter.Tags.Any(tag => tag.IndexOf(this.filterOption.FilterTermTag, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        public bool IsEnabled()
        {
            return !string.IsNullOrEmpty(this.filterOption.FilterTermConfigKey)    ||
                    !string.IsNullOrEmpty(this.filterOption.FilterTermDescription) ||
                    !string.IsNullOrEmpty(this.filterOption.FilterTermKey)         ||
                    !string.IsNullOrEmpty(this.filterOption.FilterTermName)        ||
                    !string.IsNullOrEmpty(this.filterOption.FilterTermRepo)        ||
                    !string.IsNullOrEmpty(this.filterOption.FilterTermTag) ||
                     this.filterOption.FilterTermCategory                  != null ||
                     this.filterOption.FilterTermSubCategory               != null ||
                     this.filterOption.FilterTermRemediationFunction        != null ||
                     this.filterOption.FilterTermSeverity                  != null;
        }
    }
}