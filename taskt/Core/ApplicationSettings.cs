﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using taskt.Core.IO;

namespace taskt.Core
{
    /// <summary>
    /// Defines settings for the entire application
    /// </summary>
    [Serializable]
    public class ApplicationSettings
    {
        public ServerSettings ServerSettings { get; set; } = new ServerSettings();
        public EngineSettings EngineSettings { get; set; } = new EngineSettings();
        public ClientSettings ClientSettings { get; set; } = new ClientSettings();
        public LocalListenerSettings ListenerSettings { get; set; } = new LocalListenerSettings();
        public ApplicationSettings()
        {

        }


        public void Save(ApplicationSettings appSettings)
        {
            //create settings directory
           
            var settingsDir = Core.IO.Folders.GetFolder(Folders.FolderType.SettingsFolder);

            //if directory does not exist then create directory
            if (!System.IO.Directory.Exists(settingsDir))
            {
                System.IO.Directory.CreateDirectory(settingsDir);
            }

            //create file path
            var filePath =  System.IO.Path.Combine(settingsDir, "AppSettings.xml");

            //create filestream
            var fileStream = System.IO.File.Create(filePath);

            //output to xml file
            XmlSerializer serializer = new XmlSerializer(typeof(ApplicationSettings));
            serializer.Serialize(fileStream, appSettings);
            fileStream.Close();
        }
        public ApplicationSettings GetOrCreateApplicationSettings()
        {
            //create settings directory
            var settingsDir = Core.IO.Folders.GetFolder(Folders.FolderType.SettingsFolder);

            //create file path
            var filePath = System.IO.Path.Combine(settingsDir, "AppSettings.xml");

            ApplicationSettings appSettings;
            if (System.IO.File.Exists(filePath))
            {
                //open file and return it or return new settings on error
                var fileStream = System.IO.File.Open(filePath, FileMode.Open);

                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ApplicationSettings));
                    appSettings = (ApplicationSettings)serializer.Deserialize(fileStream);
                }
                catch (Exception)
                {
                    appSettings = new ApplicationSettings();
                }

                fileStream.Close();
            }
            else
            {
                appSettings = new ApplicationSettings();
            }

            return appSettings;
        }
    }
    /// <summary>
    /// Defines Server settings for tasktServer if using the server component to manage the client
    /// </summary>
    [Serializable]
    public class ServerSettings
    {
        public bool ServerConnectionEnabled { get; set; }
        public bool ConnectToServerOnStartup { get; set; }
        public bool RetryServerConnectionOnFail { get; set; }
        public bool BypassCertificateValidation { get; set; }
        public string ServerURL { get; set; }
        public string ServerPublicKey { get; set; }

        public string HTTPServerURL { get; set; }
        public Guid HTTPGuid { get; set; }

        public ServerSettings()
        {
            HTTPServerURL = "https://localhost:44377/";
        }
    }
    /// <summary>
    /// Defines engine settings which can be managed by the user
    /// </summary>
    [Serializable]
    public class EngineSettings
    {
        public bool ShowDebugWindow { get; set; }
        public bool AutoCloseDebugWindow { get; set; }
        public bool EnableDiagnosticLogging { get; set; }
        public bool ShowAdvancedDebugOutput { get; set; }
        public bool CreateMissingVariablesDuringExecution { get; set; }
        public bool TrackExecutionMetrics { get; set; }
        public string VariableStartMarker { get; set; }
        public string VariableEndMarker { get; set; }
        public System.Windows.Forms.Keys CancellationKey { get; set; }
        public int DelayBetweenCommands { get; set; }
        public bool OverrideExistingAppInstances { get; set; }
        public bool AutoCloseMessagesOnServerExecution { get; set; }
        public bool AutoCloseDebugWindowOnServerExecution { get; set; }
        public bool AutoCalcVariables { get; set; }
        public string CurrentWindowKeyword { get; set; }
        public string DesktopKeyword { get; set; }
        public string AllWindowsKeyword { get; set; }
        public string CurrentWindowPositionKeyword { get; set; }
        public string CurrentWindowXPositionKeyword { get; set; }
        public string CurrentWindowYPositionKeyword { get; set; }
        public string CurrentWorksheetKeyword { get; set; }
        public string NextWorksheetKeyword { get; set; }
        public bool ExportIntermediateXML { get; set; }
        public string PreviousWorksheetKeyword { get; set; }
        private static string InterStartVariableMaker = "{{{";
        private static string InterEndVariableMaker = "}}}";
        private static string InterCurrentWindowKeyword = "%kwd_current_window%";
        private static string InterDesktopKeyword = "%kwd_desktop%";
        private static string InterAllWindowsKeyword = "%kwd_all_windows%";
        private static string InterCurrentWindowPositionKeyword = "%kwd_current_position%";
        private static string InterCurrentWindowXPositionKeyword = "%kwd_current_xposition%";
        private static string InterCurrentWindowYPositionKeyword = "%kwd_current_yposition%";
        private static string InterCurrentWorksheetKeyword = "%kwd_current_worksheet%";
        private static string InterNextWorksheetKeyword = "%kwd_next_worksheet%";
        private static string InterPreviousWorksheetKeyword = "%kwd_previous_worksheet%";

        private static string[] m_KeyNameList = new string[]
        {
            "BACKSPACE", "BS", "BKSP",
            "BREAK",
            "CAPSLOCK",
            "DELETE", "DEL",
            "UP", "DOWN", "LEFT", "RIGHT",
            "END",
            "ENTER",
            "INSERT", "INS",
            "NUMLOCK",
            "PGDN",
            "PGUP",
            "SCROLLROCK",
            "TAB",
            "F1", "F2", "F3", "F4", "F5", "F6",
            "F7", "F8", "F9", "F10", "F11", "F12",
            "ADD", "SUBTRACT", "MULTIPLY", "DIVIDE",
            "WIN_KEY"
        };
        private static string[] m_DisallowVariableCharList = new string[]
        {
            "+", "-", "*", "%",
            "[", "]", "{", "}",
            ".",
            " ",
            "\u2983", "\u2984",
            "\U0001D542", "\U0001D54E"
        };

        public EngineSettings()
        {
            ShowDebugWindow = true;
            AutoCloseDebugWindow = true;
            EnableDiagnosticLogging = true;
            ShowAdvancedDebugOutput = false;
            CreateMissingVariablesDuringExecution = true;
            TrackExecutionMetrics = true;
            VariableStartMarker = "{";
            VariableEndMarker = "}";
            CancellationKey = System.Windows.Forms.Keys.Pause;
            DelayBetweenCommands = 250;
            OverrideExistingAppInstances = false;
            AutoCloseMessagesOnServerExecution = true;
            AutoCloseDebugWindowOnServerExecution = true;
            AutoCalcVariables = true;
            CurrentWindowKeyword = "Current Window";
            DesktopKeyword = "Desktop";
            AllWindowsKeyword = "All Windows";
            CurrentWindowPositionKeyword = "Current Position";
            CurrentWindowXPositionKeyword = "Current XPosition";
            CurrentWindowYPositionKeyword = "Current YPosition";
            CurrentWorksheetKeyword = "Current Sheet";
            NextWorksheetKeyword = "Next Sheet";
            PreviousWorksheetKeyword = "Previous Sheet";
            ExportIntermediateXML = true;
        }

        public string[] KeyNameList()
        {
            return m_KeyNameList;
        }

        public string[] DisallowVariableCharList()
        {
            return m_DisallowVariableCharList;
        }

        public string replaceEngineKeyword(string targetString)
        {
            return targetString.Replace(InterStartVariableMaker, this.VariableStartMarker)
                    .Replace(InterEndVariableMaker, this.VariableEndMarker)
                    .Replace(InterCurrentWindowKeyword, this.CurrentWindowKeyword)
                    .Replace(InterCurrentWindowPositionKeyword, this.CurrentWindowPositionKeyword)
                    .Replace(InterCurrentWindowXPositionKeyword, this.CurrentWindowXPositionKeyword)
                    .Replace(InterCurrentWindowYPositionKeyword, this.CurrentWindowYPositionKeyword)
                    .Replace(InterCurrentWorksheetKeyword, this.CurrentWorksheetKeyword)
                    .Replace(InterNextWorksheetKeyword, this.NextWorksheetKeyword)
                    .Replace(InterPreviousWorksheetKeyword, this.PreviousWorksheetKeyword);
        }

        public string convertToIntermediate(string targetString)
        {
            return targetString.Replace(this.VariableStartMarker, Convert.ToChar(10627).ToString())
                    .Replace(this.VariableEndMarker, Convert.ToChar(10628).ToString());
        }

        public string convertToRaw(string targetString)
        {
            return targetString.Replace(Convert.ToChar(10627).ToString(), this.VariableStartMarker)
                    .Replace(Convert.ToChar(10628).ToString(), this.VariableEndMarker);
        }

        public string convertToIntermediateExcelSheet(string targetString)
        {
            return convertToIntermediate(
                    targetString.Replace(this.CurrentWorksheetKeyword, wrapKeyword(InterCurrentWorksheetKeyword))
                        .Replace(this.NextWorksheetKeyword, wrapKeyword(InterNextWorksheetKeyword))
                        .Replace(this.PreviousWorksheetKeyword, wrapKeyword(InterPreviousWorksheetKeyword))
                    );
        }

        public string convertToRawExcelSheet(string targetString)
        {
            return convertToRaw(
                    targetString.Replace(wrapKeyword(InterCurrentWorksheetKeyword), this.CurrentWorksheetKeyword)
                        .Replace(wrapKeyword(InterNextWorksheetKeyword), this.NextWorksheetKeyword)
                        .Replace(wrapKeyword(InterPreviousWorksheetKeyword), this.PreviousWorksheetKeyword)
                );
        }

        public string convertToIntermediateWindowName(string targetString)
        {
            return convertToIntermediate(
                    targetString.Replace(this.CurrentWindowKeyword, wrapKeyword(InterCurrentWindowKeyword))
                        .Replace(this.DesktopKeyword, wrapKeyword(InterDesktopKeyword))
                        .Replace(this.AllWindowsKeyword, wrapKeyword(InterAllWindowsKeyword))
                );
        }

        public string convertToRawWindowName(string targetString)
        {
            return convertToRaw(
                    targetString.Replace(wrapKeyword(InterCurrentWindowKeyword), this.CurrentWindowKeyword)
                        .Replace(wrapKeyword(InterDesktopKeyword), this.DesktopKeyword)
                        .Replace(wrapKeyword(InterAllWindowsKeyword), this.AllWindowsKeyword)
                );
        }

        public string convertToIntermediateWindowPosition(string targetString)
        {
            return convertToIntermediate(
                    targetString.Replace(this.CurrentWindowPositionKeyword, wrapKeyword(InterCurrentWindowPositionKeyword))
                        .Replace(this.CurrentWindowXPositionKeyword, wrapKeyword(InterCurrentWindowXPositionKeyword))
                        .Replace(this.CurrentWindowYPositionKeyword, wrapKeyword(InterCurrentWindowYPositionKeyword))
                );
        }

        public string convertToRawWindowPosition(string targetString)
        {
            return convertToRaw(
                    targetString.Replace(wrapKeyword(InterCurrentWindowPositionKeyword), this.CurrentWindowPositionKeyword)
                        .Replace(wrapKeyword(InterCurrentWindowXPositionKeyword), this.CurrentWindowXPositionKeyword)
                        .Replace(wrapKeyword(InterCurrentWindowYPositionKeyword), this.CurrentWindowYPositionKeyword)
                );
        }

        private static string wrapKeyword(string kw)
        {
            return Char.ConvertFromUtf32(120130).ToString() + kw + Char.ConvertFromUtf32(120142).ToString();
        }

        public bool isValidVariableName(string vName)
        {
            foreach(string s in m_KeyNameList)
            {
                if (vName == s)
                {
                    return false;
                }
            }
            foreach(string s in m_DisallowVariableCharList)
            {
                if (vName.Contains(s))
                {
                    return false;
                }
            }
            return true;
        }
    }
    /// <summary>
    /// Defines application/client-level settings which can be managed by the user
    /// </summary>
    [Serializable]
    public class ClientSettings
    {
        public bool AntiIdleWhileOpen { get; set; }
        public string RootFolder { get; set; }
        public bool MinimizeToTray { get; set; }
        public string AttendedTasksFolder { get; set; }
        public string StartupMode { get; set; }
        public bool PreloadBuilderCommands { get; set; }
        public bool UseSlimActionBar { get; set; }
        public bool InsertCommandsInline { get; set; }
        public bool EnableSequenceDragDrop { get; set; }
        public bool InsertVariableAtCursor { get; set; }
        public bool InsertElseAutomatically { get; set; }
        public bool InsertCommentIfLoopAbove { get; set; }
        public bool GroupingBySubgroup { get; set; }
        public bool DontShowValidationMessage { get; set; }
        public bool ShowSampleUsageInDescription { get; set; }
        public bool ShowIndentLine { get; set; }
        private int _IndentWidth = 16;
        public int IndentWidth 
        {
            get
            {
                return this._IndentWidth;
            }
            set
            {
                if (value >= 1 && value <= 32)
                {
                    this._IndentWidth = value;
                }
            }
        }
        public string DefaultBrowserInstanceName { get; set; }
        public string DefaultStopWatchInstanceName { get; set; }
        public string DefaultExcelInstanceName { get; set; }
        public string DefaultWordInstanceName { get; set; }
        public string DefaultDBInstanceName { get; set; }
        public ClientSettings()
        {
            MinimizeToTray = false;
            AntiIdleWhileOpen = false;
            RootFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "taskt");
            StartupMode = "Builder Mode";
            AttendedTasksFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "taskt", "My Scripts");
            PreloadBuilderCommands = false;
            UseSlimActionBar = true;
            InsertCommandsInline = true;
            EnableSequenceDragDrop = true;
            InsertVariableAtCursor = true;
            InsertElseAutomatically = false;
            InsertCommentIfLoopAbove = false;
            GroupingBySubgroup = true;
            DontShowValidationMessage = false;
            ShowSampleUsageInDescription = true;
            ShowIndentLine = true;
            DefaultBrowserInstanceName = "RPABrowser";
            DefaultStopWatchInstanceName = "RPAStopwatch";
            DefaultExcelInstanceName = "RPAExcel";
            DefaultWordInstanceName = "RPAWord";
            DefaultDBInstanceName = "RPADB";
        }
    }
    /// <summary>
    /// Defines Server settings for tasktServer if using the server component to manage the client
    /// </summary>
    [Serializable]
    public class LocalListenerSettings
    {
        public bool StartListenerOnStartup { get; set; }
        public bool LocalListeningEnabled { get; set; }
        public bool RequireListenerAuthenticationKey { get; set; }
        public int ListeningPort { get; set; }
        public string AuthKey { get; set; }
        public bool EnableWhitelist { get; set; }
        public string IPWhiteList { get; set; }
        public LocalListenerSettings()
        {
            StartListenerOnStartup = false;
            LocalListeningEnabled = false;
            RequireListenerAuthenticationKey = false;
            EnableWhitelist = false;
            ListeningPort = 19312;
            AuthKey = Guid.NewGuid().ToString();
            IPWhiteList = "";
        }
    }

    [Serializable]
    public class WhiteListIP
    {
        string _value;
        public WhiteListIP(string s)
        {
            _value = s;
        }
        public string Value { get { return _value; } set { _value = value; } }
    }
}
