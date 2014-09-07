namespace SqaleUi.helpers
{
    using ExtensionTypes;

    public interface IFilterOption
    {
        string FilterTermConfigKey { get; set; }

        string FilterTermKey { get; set; }

        string FilterTermName { get; set; }

        string FilterTermEnabled { get; set; }

        string FilterTermRepo { get; set; }

        string FilterTermDescription { get; set; }

        string FilterTermTag  { get; set; }

        Severity? FilterTermSeverity { get; set; }

        RemediationFunction? FilterTermRemediationFunction { get; set; }

        RemediationUnit? FilterTermRemdiationUnitOffset { get; set; }

        RemediationUnit? FilterTermRemdiationUnitVal { get; set; }

        Category? FilterTermCategory { get; set; }

        SubCategory? FilterTermSubCategory { get; set; }
    }
}