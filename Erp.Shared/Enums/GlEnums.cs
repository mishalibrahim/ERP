namespace Erp.Shared.Enums
{
    public enum GlAccountCategory
    {
        Asset,
        Liability,
        Equity,
        Income,
        Expense
    }

    public enum GlAccountType
    {
        Ledger,
        Customer,
        Vendor,
        Bank
    }

    public enum GlPostingType
    {
        Header,
        Posting
    }

    public enum DimensionType
    {
        CostCenter,
        Project,
        Property,
        Department
    }

    public enum JournalVoucherStatus
    {
        Draft,
        PendingApproval,
        Approved,
        Posted,
        Rejected,
        Reversed
    }

    public enum JournalVoucherType
    {
        General,
        Adjusting,
        Accrual,
        Reversing,
        Opening
    }

    public enum JournalVoucherApprovalStage
    {
        Initiator,
        FinanceReview,
        CfoApprove,
        Posted
    }
}
