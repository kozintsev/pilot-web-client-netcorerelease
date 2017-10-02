using System.Collections.Generic;

namespace Ascon.Pilot.Core
{
    public static class SystemAttributes
    {
        public const string DELETE_DATE = "DeleteDate 9349ED7C-C2D7-4B8B-852A-83140F158611";
        public const string DELETE_INITIATOR_PERSON = "DeleteInitiatorPerson BE56ECD1-F4C5-40E2-A83C-274EAE4D02A9";
        public const string DELETE_INITIATOR_POSITION = "DeleteInitiatorPosition 35E355AC-97B3-40FC-9636-1648402040D4";

        public const string SEARCH_CRITERIA = "SearchCriteria 52F6E73A-D736-49CD-8807-5AD955506A37";
        public const string SMART_FOLDER_TITLE = "SmartFolderTitle 3AE3FFC8-A776-4E61-87D0-FFD8B50CBBA8";
        public const string SEARCH_CONTEXT_OBJECT_ID = "SearchContextObjectId 257E6DB2-F3A3-4231-83D4-DB57C3FF059E";

        public const string IS_HIDDEN = "IsHidden FDF5475C-93FA-41F3-8243-F1810854DEBD";
        public const string PROJECT_ITEM_NAME = "Title 4C281306-E329-423A-AF45-7B39EC30273F";

        public const string TASK_EXECUTION_DEPARTMENT = "ExecutionDepartment E3BA8854-0E65-4F1F-9E9B-53EC11A97161";
        public const string TASK_EXECUTOR_POSITION = "Executor 7620B1D4-B476-4169-8401-6A0C3D3A22F3";
        public const string TASK_EXECUTOR_ROLE = "ExecutorRole B9628414-E52A-483A-98B0-58C11ED1756B";
        public const string TASK_STATE = "TaskState B65D6C5B-7D8E-4055-852F-D1AAB060CD22";
        public const string TASK_DEADLINE_DATE = "TaskDeadlineDate DE08B9D6-59AF-42BE-9DA9-2B327D439A05";
        public const string TASK_WITH_VALIDATION_PHASE = "TaskWithValidationPhase 7A626A12-E416-4F1C-BC8B-70F178DFD264";
        public const string TASK_INITIATOR_POSITION = "Initiator CEA10431-5A8C-4F2A-965B-E780A58142B7";
        public const string TASK_TITLE = "Title 578C65E3-A95E-44A5-942A-D3FA04956B82";
        public const string TASK_DESCRIPTION = "Description 6589B3AC-CE5B-43B4-902B-F88A1F396E25";
        public const string TASK_STAGE_ORDER = "Order F06F94A0-2C3B-4C0C-B900-4AE422F2FAC7";
        public const string TASK_DATE_OF_ASSIGNMENT = "DateOfAssignment B8A06EEF-8572-4875-B02D-BA3D37C31BDD";
        public const string TASK_MESSAGE_TEXT_ATTRIBUTE_NAME = "Task_message_text 05DD1EC5-6BC6-4901-B008-B1B648EC6ACD";
        public const string TASK_AUDITORS = "TaskByCopyTo 6B50090E-67DD-421E-8169-0BE25EB8F2E7";
        public const string TASK_IS_VERSION = "TaskIsVersion_EECBDB6A-E16B-4BE6-B901-AF895301C8BD";
        public const string TASK_DATE_OF_COMPLETION = "DateOfCompletion_C8FEC8EC-B9DD-4231-A46F-DCD2D0A3D082";
        public const string TASK_DATE_OF_START = "DateOfStart_697B8750-588C-4D70-8BCB-0F0D2BFDDEF0";
        public const string TASK_DATE_OF_REVOKATION = "DateOfRevokation_FC69398D-5C11-4274-B68C-A3B2C6FF6FA5";
        public const string TASK_KIND = "TaskKind_A87C8C5E-18EC-4D2A-B92B-8E596C8F3BDC";
        public const string TASK_IS_CHECKED_SIGN_ME_AS = "IsCheckedSignMeAs B5DC6759-C0F2-485C-9707-C261FE2977E3";
        public const string TASK_SELECTED_ROLE = "SelectedRole 8F3A3A68-1C0E-4BA1-9BF2-79F8830ECAC7";

        public const string EXTENSION_FOLDER_NAME = "Extension_folder_name EE4DE4B7-FFB7-455E-8E15-C185CFDB34FA";
        public const string EXTENSION_NAME = "Extension_name 53522556-0749-46DE-913E-EDA195AD1299";
        public const string EXTENSION_ADDITIONAL = "Extension_additional 9A428222-2031-4F5E-A367-C5BAA18DFCA5";

        public const string SHORTCUT_OBJECT_ID = "Shortcut_object_id_CF11FD82-3D56-4DBC-B7F8-DF91CA1F9885";
        public const string REPORT_FOLDER_NAME = "Report_folder_name_BDBDEDD1-BFCB-4E2C-BE44-3E4BEBBB58F9";
        public const string REPORT_NAME = "Report_name_D745D627-E7CD-4E3A-B30E-F9EDC1A09D77";

        public const string TASK_TEMPLATE_NAME = "Task_template_name_CE25F101-E758-4A68-9F16-E73B72F39FBC";
        public const string TASK_TEMPLATE_VALUE = "Task_template_value_3C190DF3-3644-47F4-AF64-45C140207054";
        public const string TASK_TEMPLATE_FOLDER_NAME = "Task_template_folder_name_DC250687-50D3-40E9-8B9A-71ACD01D50F7";

        public const string CREATION_TIME = "creation_time";

        public const string TAGS = "Tags_BCA19031-E5A1-49D3-A55F-47B30A8F7243";
        public const string DELETE_SOURCE_PARENT = "Delete_source_EFED1D30-E3E2-49E1-BDF3-13C544DADE9F";

        public const string DEFAULT_PUBLISH_FILE_ID = "Default_publish_file_id_BA9EA041-93E8-4AE9-9E09-0C40221DE75D";

        public const string DOCUMENT_TEMPLATE_NAME = "Document_template_name_3626C317-57EF-4FD4-AEEB-0FF4719B432C";
        public const string DOCUMENT_TEMPLATE_TYPE_ID = "Document_template_type_id_0D8595C0-45CF-4C98-8AB2-EE5C9276DE3E";
        public const string DOCUMENT_TEMPLATE_FOLDER_NAME = "Document_template_folder_name_D6C7CC9A-0850-47B7-88D0-1045F8D5561D";


        public static IEnumerable<string> GetTextSystemAttributes()
        {
            yield return PROJECT_ITEM_NAME;
            yield return TASK_TITLE;
            yield return TASK_DESCRIPTION;
            yield return TASK_MESSAGE_TEXT_ATTRIBUTE_NAME;
        }

        public static IEnumerable<string> GetTaskSystemAttributes()
        {
            yield return TASK_EXECUTOR_POSITION;
            yield return TASK_EXECUTOR_ROLE;
            yield return TASK_STATE;
            yield return TASK_DEADLINE_DATE;
            yield return TASK_WITH_VALIDATION_PHASE;
            yield return TASK_INITIATOR_POSITION;
            yield return TASK_TITLE;
            yield return TASK_DESCRIPTION;
            yield return TASK_DATE_OF_ASSIGNMENT;
            yield return TASK_KIND;
            yield return TASK_AUDITORS;
            yield return TASK_IS_VERSION;
            yield return TASK_DATE_OF_COMPLETION;
            yield return TASK_DATE_OF_START;
            yield return TASK_DATE_OF_REVOKATION;
            yield return TAGS;
        }

        public static IEnumerable<string> GetWorkflowSystemAttributes()
        {
            yield return TASK_TITLE;
            yield return TASK_DESCRIPTION;
            yield return TASK_INITIATOR_POSITION;
        }

        public static IEnumerable<string> GetAllSystemAttributes()
        {
            yield return SEARCH_CRITERIA;
            yield return SMART_FOLDER_TITLE;
            yield return SEARCH_CONTEXT_OBJECT_ID;

            yield return IS_HIDDEN;
            yield return PROJECT_ITEM_NAME;

            yield return TASK_EXECUTOR_POSITION;
            yield return TASK_EXECUTOR_ROLE;
            yield return TASK_STATE;
            yield return TASK_DEADLINE_DATE;
            yield return TASK_WITH_VALIDATION_PHASE;
            yield return TASK_INITIATOR_POSITION;
            yield return TASK_TITLE;
            yield return TASK_DESCRIPTION;
            yield return TASK_STAGE_ORDER;
            yield return TASK_DATE_OF_ASSIGNMENT;
            yield return TASK_MESSAGE_TEXT_ATTRIBUTE_NAME;
            yield return TASK_KIND;
            yield return TASK_AUDITORS;
            yield return TASK_IS_VERSION;
            yield return TASK_DATE_OF_COMPLETION;
            yield return TASK_DATE_OF_START;
            yield return TASK_DATE_OF_REVOKATION;

            yield return EXTENSION_NAME;
            yield return EXTENSION_FOLDER_NAME;
            yield return EXTENSION_ADDITIONAL;

            yield return REPORT_FOLDER_NAME;
            yield return REPORT_NAME;

            yield return TASK_TEMPLATE_NAME;
            yield return TASK_TEMPLATE_VALUE;
            yield return TASK_TEMPLATE_FOLDER_NAME;

            yield return CREATION_TIME;

            yield return TAGS;
            yield return DELETE_SOURCE_PARENT;
        }
    }

    public static class SystemTypes
    {
        public const string PROJECT_FOLDER = "Project_folder";
        public const string PROJECT_FILE = "File";
        public const string TASK = "Task";
        public const string TASK_STAGE = "Task_Stage";
        public const string TASK_WORKFLOW = "Task_Workflow";
        public const string TASK_FOLDER = "Task_Folder";
        public const string TASK_CHAT = "Chat_type";
        public const string TASK_CHAT_MESSAGE = "Chat_message_type";
        public const string EXTENSION = "Extension";
        public const string EXTENSION_FOLDER = "Extension_folder";
        public const string SHORTCUT = "Shortcut_E67517F1-93F5-4756-B651-133B816D43C8";
        public const string REPORT = "Report_6088AF81-061E-456E-9225-CF65B7B25368";
        public const string REPORT_FOLDER = "Report_Folder_F2CC6F1D-70E1-4E9B-B32F-BEB3E991318F";
        public const string SMART_FOLDER = "Smart_folder_type";
        public const string TASK_TEMPLATE_FOLDER = "Task_template_folder_A0A09765-E6FB-4272-87EE-37793283DBC5";
        public const string TASK_TEMPLATE = "Task_template_05339782-CCBA-4786-80C3-0F6C7E0EF3C5";
        public const string DOCUMENT_TEMPLATE_FOLDER = "Document_template_folder_793D0CE8-65E6-484E-AAF9-7E095AF9DBD2";
        public const string DOCUMENT_TEMPLATE = "Document_template_89B9E233-A6F9-4B9C-B970-55B3B3A77CED";

        public static IEnumerable<string> All()
        {
            yield return PROJECT_FOLDER;
            yield return PROJECT_FILE;
            yield return TASK;
            yield return TASK_STAGE;
            yield return TASK_WORKFLOW;
            yield return TASK_FOLDER;
            yield return TASK_CHAT;
            yield return EXTENSION;
            yield return EXTENSION_FOLDER;
            yield return SHORTCUT;
            yield return REPORT;
            yield return REPORT_FOLDER;
            yield return SMART_FOLDER;
            yield return TASK_TEMPLATE_FOLDER;
            yield return TASK_TEMPLATE;
            yield return DOCUMENT_TEMPLATE_FOLDER;
            yield return DOCUMENT_TEMPLATE;
        }
    }
}
