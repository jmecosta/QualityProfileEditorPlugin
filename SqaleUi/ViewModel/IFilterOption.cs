namespace SqaleUi.ViewModel
{
    using SqaleManager;

    public interface IFilterOption
    {
        string FilterTermConfigKey { get; set; }

        string FilterTermKey { get; set; }

        string FilterTermName { get; set; }

        string FilterTermRepo { get; set; }

        string FilterTermDescription { get; set; }

        Severity? FilterTermSeverity { get; set; }

        RemediationFunction? FilterTermRemediationFunction { get; set; }

        RemediationUnit? FilterTermRemdiationUnitOffset { get; set; }

        RemediationUnit? FilterTermRemdiationUnitVal { get; set; }

        Category? FilterTermCategory { get; set; }

        SubCategory? FilterTermSubCategory { get; set; }
    }
}