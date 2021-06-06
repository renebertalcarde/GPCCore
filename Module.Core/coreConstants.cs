using System;
using System.Collections;

namespace Module.Core
{
    public static class ModuleConstants
    {
        public const int PHILIPPINES_COUNTRY_ID = 446;
        public const string BAD_REQUEST = "Bad request";
        public const string ID_NOT_FOUND = "Id not found";
        public const string RECORD_ID_NOT_FOUND = "Record Id not found";
        public const string PRINT_OPTIONS = "Would you like to view the report in PDF or in MSWord? Press \'Yes\' to view in PDF";
    }
    public static class ModuleConstants_Authorization
    {
        public const string MODULEACCESS_MODULE_IS_NOT_ENABLED = "Module Access: This module is not enabled";

        public const string USERACCESS_UNAUTHORIZED_ACCESS_TO_RESOURCE = "User Access: You are not authorized to access the resource";
        public const string USERACCESS_UNAUTHORIZED_ACTION = "User Access: You are not authorized to perform this action";

        public const string EMPLOYEEACCESS_UNAUTHORIZED_ACCESS_TO_RESOURCE = "Employee Access: You are not authorized to access the employee record";

        public const string WAREHOUSEACCESS_UNAUTHORIZED = "You are not authorized to access this warehouse";
    }
}
