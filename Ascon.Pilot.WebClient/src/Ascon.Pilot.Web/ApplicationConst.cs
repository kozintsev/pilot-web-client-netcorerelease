using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Web.Models;
using Microsoft.Extensions.Configuration;

namespace Ascon.Pilot.Web
{
    public static class ApplicationConst
    {
        public static IConfigurationRoot Configuration { get; set; }
        
        static ApplicationConst()
        {
            var executeAssLocation = Assembly.GetExecutingAssembly().Location;
            var path = Path.GetDirectoryName(executeAssLocation);

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(path);
            builder.AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            PilotServerUrl = Configuration.GetValue<string>("PilotServer:Url", string.Empty);
            Database = Configuration.GetValue<string>("PilotServer:Database", string.Empty);
        }

        public static readonly string PilotServerUrl;
        public static readonly string Database;
        public static readonly string PilotMiddlewareInstanceName = "AsconPilotMiddlewareInstance";

        public static readonly string HttpSchemeName = "http";
        public static readonly string SchemeDelimiter = "://";
        public static readonly string AppName = "Web-клиент Pilot ICE";

        public static readonly string DefaultGlyphicon = "";
        public static readonly IDictionary<string, string> TypesGlyphiconDictionary = new Dictionary<string, string>()
        {
            { SystemTypes.PROJECT_FOLDER, "glyphicon glyphicon-folder-open"},
            { SystemTypes.PROJECT_FILE, "glyphicon glyphicon-file" },
            { SystemTypes.SMART_FOLDER, "glyphicon glyphicon-book" }
        };

        //implicitly default files panel type is LIST cause its index in enum is 0
        public const FilesPanelType DefaultFilesPanelType = 0;

        public const int SourcefolderTypeid = -24;
    }
    
    public static class Roles
    {
        public static readonly string Admin = "Администратор";
        public static readonly string User = "Пользователь";
    }
}
