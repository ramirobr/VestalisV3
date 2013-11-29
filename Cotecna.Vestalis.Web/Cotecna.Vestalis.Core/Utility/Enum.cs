
namespace Cotecna.Vestalis.Core
{

    /// <summary>
    /// XML form definition related to a service order or inspection report
    /// </summary>
    public enum FormType
    {
        ServiceOrder,
        InspectionReport
    }

    public enum RulesForm
    {
        RuleMandatory,
        RuleMinLength,
        RuleMaxLength,
        RuleStartDate,
        RuleEndDate,
        RuleNumDigit,
        RuleMaxValue,
        RuleMinValue,
        RuleExpression,
        RuleTime
    }

    public enum ApprovalStatus : int
    {
        None = 0,
        Waiting = 1,
        Ready = 2,
        Completed = 3
    }

    public enum ScreenOpenMode : int
    {
        None=0,
        Add = 1,
        Edit = 2,
        View = 3
    }
}
