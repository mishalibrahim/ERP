using System.Reflection;

namespace Erp.Shared.Constants
{
    /// <summary>
    /// Single source of truth for all permission keys.
    /// Format: "Module:Action:Resource"
    /// These constants are used at compile time in [RequirePermission] attributes
    /// and at runtime for JWT claims and seeding.
    /// </summary>
    public static class Permissions
    {
        public static class CompanySetup
        {
            public const string Read   = "CompanySetup:Read:Tenant";
            public const string Update = "CompanySetup:Update:Tenant";
        }

        public static class Users
        {
            public const string Read   = "Users:Read:Tenant";
            public const string Create = "Users:Create:Tenant";
            public const string Update = "Users:Update:Tenant";
            public const string Delete = "Users:Delete:Tenant";
            public const string Manage = "Users:Manage:Tenant";
            public const string Invite = "Users:Invite:Tenant";
        }

        public static class Roles
        {
            public const string Read   = "Roles:Read:Tenant";
            public const string Create = "Roles:Create:Tenant";
            public const string Update = "Roles:Update:Tenant";
            public const string Delete = "Roles:Delete:Tenant";
        }

        public static class Customers
        {
            public const string Read   = "Customers:Read:Tenant";
            public const string Create = "Customers:Create:Tenant";
            public const string Update = "Customers:Update:Tenant";
            public const string Delete = "Customers:Delete:Tenant";
        }

        public static class Invoices
        {
            public const string Read    = "Invoices:Read:Tenant";
            public const string Create  = "Invoices:Create:Tenant";
            public const string Update  = "Invoices:Update:Tenant";
            public const string Delete  = "Invoices:Delete:Tenant";
            public const string Approve = "Invoices:Approve:Any";
            public const string Void    = "Invoices:Void:Any";
        }

        public static class GeneralLedger
        {
            public const string Read          = "GeneralLedger:Read:Tenant";
            public const string Create        = "GeneralLedger:Create:Tenant";
            public const string Update        = "GeneralLedger:Update:Tenant";
            public const string Delete        = "GeneralLedger:Delete:Tenant";
            public const string PostJournals  = "GeneralLedger:PostJournals:Tenant";
        }

        public static class Reports
        {
            public const string ViewOwn = "Reports:Read:Own";
            public const string ViewAll = "Reports:Read:Any";
            public const string Export  = "Reports:Export:Tenant";
        }

        /// <summary>
        /// Uses reflection to collect every permission string from all nested classes.
        /// Called by the DatabaseSeeder to auto-create Permission records.
        /// </summary>
        public static List<string> GetAll()
        {
            var permissions = new List<string>();

            // Get all nested public static classes (Customers, Invoices, etc.)
            var nestedTypes = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var type in nestedTypes)
            {
                // Get all public const string fields in each nested class
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

                foreach (var field in fields)
                {
                    var value = field.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        permissions.Add(value);
                    }
                }
            }

            return permissions;
        }
    }
}
