namespace Domain
{
    public class PermissionsSD
    {
        #region General
        // role
        public const String QueryRole = "QueryRole";
        public const String CommandRole = "CommandRole";

        // user
        public const String UsersList = "UsersList";
        public const String UsersRequests = "UsersRequests";
        public const String RemoveUser = "RemoveUser";
        public const String ManageUserRole = "ManageUserRole";
        #endregion

        #region System
        // Companies
        public const String QueryCompany = "QueryCompany";
        public const String CommandCompany = "CommandCompany";

        public const String FilterCompany = "FilterCompany";

        // Programs
        public const String QueryProgramYear = "QueryProgramYear";
        public const String CommandProgramYear = "CommandProgramYear";

        // Strategies
        public const String QueryStrategy = "QueryStrategy";
        public const String CommandStrategy = "CommandStrategy";

        // SWOT
        public const String QuerySWOT = "QuerySWOT";
        public const String CommandSWOT = "CommandSWOT";

        // ProgramYears
        public const String QueryProgram = "QueryProgram";
        public const String CommandProgram = "CommandProgram";
        public const String SeePerspective = "SeePerspective";
        public const String UpsertPerspective = "UpsertPerspective";

        // IndicatorCategories
        public const String QueryIndicatorCategory = "QueryIndicatorCategory";
        public const String CommandIndicatorCategory = "CommandIndicatorCategory";

        // IndicatorType
        public const String QueryIndicatorType = "QueryIndicatorType";
        public const String CommandIndicatorType = "CommandIndicatorType";
        #endregion

        #region Company
        // big goal
        public const String QueryBigGoal = "QueryBigGoal";
        public const String CommandBigGoal = "CommandBigGoal";

        // operational objective
        public const String QueryOperationalObjective = "QueryOperationalObjective";
        public const String CommandOperationalObjective = "CommandOperationalObjective";

        // transition
        public const String QueryTransition = "QueryTransition";
        public const String CommandTransition = "CommandTransition";

        // person
        public const String QueryPerson = "QueryPerson";
        public const String CommandPerson = "CommandPerson";

        // hardware equipment
        public const String QueryHardwareEquipment = "QueryHardwareEquipment";
        public const String CommandHardwareEquipment = "CommandHardwareEquipment";

        // system
        public const String QuerySystem = "QuerySystem";
        public const String CommandSystem = "CommandSystem";

        #endregion
    }
}
