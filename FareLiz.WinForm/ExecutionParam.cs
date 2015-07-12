using log4net;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Data;
using SkyDean.FareLiz.Data.Config;
using SkyDean.FareLiz.Data.Monitoring;
using SkyDean.FareLiz.WinForm.Properties;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace SkyDean.FareLiz.WinForm
{
    using SkyDean.FareLiz.Core.Utils;

    [Serializable]
    internal class ExecutionParam : ExecutionInfo
    {
        private const string DATE_FORMAT_PARAM = "yyyyMMdd";

        OperationMode _opMode = OperationMode.Unspecified;
        [DisplayName("Operation mode")]
        [Description("Opearation mode"), Category("Application Settings"), DefaultValue(OperationMode.GetFareAndSave)]
        public OperationMode OperationMode
        {
            get { return _opMode; }
            set { _opMode = value; }
        }

        private bool _exitAfterDone;
        [DisplayName("Exit after done")]
        [Description("Exit the application after everything is done"), Category("Application Settings"), DefaultValue(true)]
        public bool ExitAfterDone
        {
            get { return _exitAfterDone; }
            set { _exitAfterDone = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IObjectPersist ConfigHandler { get; set; }

        private static readonly SwitchForm[] switches = new[]
            {
                new SwitchForm("m", SwitchType.Simple, false, "Start the application minimized to tray"),                                                   // 0- Minimize
                new SwitchForm("sync", SwitchType.Simple, false, "Upload data packages using pre-configured data synchronizer after getting fare"),         // 1- Upload data to cloud service
                new SwitchForm("d", SwitchType.LimitedPostString, false, DATE_FORMAT_PARAM.Length, "Departure date in format " + DATE_FORMAT_PARAM),        // 2- Departure Date
                new SwitchForm("Dr", SwitchType.UnLimitedPostString, false, "Departure date range"),                                                        // 3- Departure range
                new SwitchForm("r", SwitchType.LimitedPostString, false, DATE_FORMAT_PARAM.Length, "Return date in format " + DATE_FORMAT_PARAM),           // 4- Return date
                new SwitchForm("Rr", SwitchType.UnLimitedPostString, false, "Return date range"),                                                           // 5- Return range
                new SwitchForm("sMin", SwitchType.UnLimitedPostString, false, "Minimum stay duration"),                                                     // 6- Min stay
                new SwitchForm("sMax", SwitchType.UnLimitedPostString, false,"Maximum stay duration"),                                                      // 7- Max stay
                new SwitchForm("pLim", SwitchType.UnLimitedPostString, false,"Price limit"),                                                                // 8- Price Limit
                new SwitchForm("op", SwitchType.UnLimitedPostString, false, @"Data operation type. It can be one of the following:
   +CloseAndExport: Get fare data and save to the database
   +LiveMonitor: Live fare monitor"),                                                                                                                       // 9- Operation Mode
                new SwitchForm("dept", SwitchType.UnLimitedPostString, false, "Departure location"),                                               // 10- Departure
                new SwitchForm("dest", SwitchType.UnLimitedPostString, false, "Journey destination"),                                                       // 11- Destination
                new SwitchForm("?", SwitchType.Simple, false, "Show help"),                                                                                 // 12- Help
                new SwitchForm("exit", SwitchType.Simple, false, "Exit application after getting data")                                                     // 13- Exit
            };

        public string GenerateCommandLine()
        {
            Validate();
            var sb = new StringBuilder();
            if (IsMinimized) sb.Append(" -" + switches[0]);
            if (AutoSync) sb.Append(" -" + switches[1]);
            sb.Append(" /" + switches[2] + Wrap(DepartureDate.ToString(DATE_FORMAT_PARAM)));
            sb.Append(String.Format(" /{0}+{1}-{2}", switches[3], DepartureDateRange.Plus, DepartureDateRange.Minus));
            sb.Append(" /" + switches[4] + Wrap(ReturnDate.ToString(DATE_FORMAT_PARAM)));
            sb.Append(String.Format(" /{0}+{1}-{2}", switches[5], ReturnDateRange.Plus, ReturnDateRange.Minus));
            sb.Append(" /" + switches[6] + Wrap(MinStayDuration));
            sb.Append(" /" + switches[7] + Wrap(MaxStayDuration));
            sb.Append(" /" + switches[8] + Wrap(PriceLimit));
            sb.Append(" /" + switches[9] + Wrap(OperationMode));
            sb.Append(" /" + switches[10] + Wrap(Departure.IATA));
            sb.Append(" /" + switches[11] + Wrap(Destination.IATA));
            // sb.Append(" /" + switches[12]);  // 12: (Show Help)
            if (ExitAfterDone) sb.Append(" /" + switches[13]);
            return sb.ToString().Trim();
        }

        private string Wrap(object cmdLine)
        {
            string str = (cmdLine == null ? String.Empty : cmdLine.ToString());
            return (str.Contains(" ") ? String.Format("\"{0}\"", str) : str);
        }

        public static void ShowHelp()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Specify parameters in this format: -{Parameter ConfiguredType}[{Parameter TypeConfiguration}]");
            sb.AppendLine("(without the { and } characters)\n");
            sb.AppendLine("If parameter key value contains space, wrap it in double-quotation mark\n");
            foreach (var p in switches)
            {
                sb.AppendFormat("-{0}{1}\t{2}\n", p.IDString, (p.Type == SwitchType.Simple ? String.Empty : ".."), p.Description);
            }

            var example = new ExecutionParam
            {
                Departure = AirportDataProvider.FromIATA("HEL"),
                Destination = AirportDataProvider.FromIATA("SGN"),
                DepartureDate = DateTime.Now.Date,
                ReturnDate = DateTime.Now.Date.AddDays(7),
                DepartureDateRange = new DateRangeDiff(1),
                ReturnDateRange = new DateRangeDiff(2),
                IsMinimized = true,
                ExitAfterDone = true,
                MinStayDuration = 7,
                MaxStayDuration = 8,
                PriceLimit = 2000,
                OperationMode = OperationMode.GetFareAndSave
            };
            var exCli = example.GenerateCommandLine();
            sb.AppendLine("\nFor Example:" + Environment.NewLine + exCli);
            MessageBox.Show(sb.ToString(), "Parameter Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool Parse(string[] paramArgs, string iniFilePath, ILog logger, out ExecutionParam param)
        {
            paramArgs = paramArgs ?? new string[0];
            var result = new ExecutionParam();

            int tempI;

            var parser = new Parser(switches.Length);
            parser.ParseStrings(switches, paramArgs);

            if (!String.IsNullOrEmpty(iniFilePath))
            {
                var iniParser = new ObjectIniConfig(iniFilePath, logger);
                iniParser.LoadINI();
                iniParser.ApplyData(result);
                result.ConfigHandler = iniParser;
            }

            result.IsMinimized = parser[0].ThereIs;
            result.AutoSync = parser[1].ThereIs;
            DateTime tempD;

            if (parser[2].ThereIs)
            {
                if (!DateTime.TryParseExact(parser[2].PostStrings[0].ToString(), DATE_FORMAT_PARAM, null,
                                            DateTimeStyles.AssumeLocal, out tempD))
                    throw new ApplicationException("Invalid departure date");
                result.DepartureDate = tempD;
            }

            if (parser[4].ThereIs)
            {
                if (!DateTime.TryParseExact(parser[4].PostStrings[0].ToString(), DATE_FORMAT_PARAM, null,
                                            DateTimeStyles.AssumeLocal, out tempD))
                    throw new ApplicationException("Invalid return date");
                result.ReturnDate = tempD;
            }

            if (parser[6].ThereIs)
            {
                if (!Int32.TryParse(parser[6].PostStrings[0].ToString(), out tempI))
                    throw new ApplicationException("Invalid minimum stay duration");
                result.MinStayDuration = tempI;
            }
            else
                result.MinStayDuration = Settings.Default.DefaultDurationMin;

            if (parser[7].ThereIs)
            {
                if (!Int32.TryParse(parser[7].PostStrings[0].ToString(), out tempI))
                    throw new ApplicationException("Invalid maximum stay duration");
                result.MaxStayDuration = tempI;
            }
            else
                result.MaxStayDuration = Settings.Default.DefaultDurationMax;

            if (parser[8].ThereIs)
            {
                if (!Int32.TryParse(parser[8].PostStrings[0].ToString(), out tempI))
                    throw new ApplicationException("Invalid price limit");
                result.PriceLimit = tempI;
            }
            else
                result.PriceLimit = Settings.Default.DefaultPriceLimit;

            if (parser[9].ThereIs)
            {
                result.OperationMode = (OperationMode)Enum.Parse(typeof(OperationMode), parser[9].PostStrings[0].ToString(), true);

                if (result.OperationMode != OperationMode.Unspecified)
                {
                    result.DepartureDateRange = DateRangeDiff.Parse(parser[3].PostStrings[0].ToString());
                    result.ReturnDateRange = DateRangeDiff.Parse(parser[5].PostStrings[0].ToString());
                }
            }

            if (parser[10].ThereIs)
                result.Departure = AirportDataProvider.FromIATA(parser[10].PostStrings[0].ToString());

            if (parser[11].ThereIs)
                result.Destination = AirportDataProvider.FromIATA(parser[11].PostStrings[0].ToString());

            if (parser[13].ThereIs)
                result.ExitAfterDone = true;

            result.Validate();
            param = result;

            if (parser[12].ThereIs)
                return false;

            return true;
        }

        public void Validate()
        {
            if (OperationMode != OperationMode.Unspecified)
            {
                string error = null;

                if (Departure == null || Destination == null)
                    error = "Departure and destination must be specified!";
                else if (String.Equals(Departure.IATA, Destination.IATA, StringComparison.OrdinalIgnoreCase))
                    error = "Departure and destination must be different!";
                if (DepartureDate.IsUndefined())
                    error = "Departure date must be specified";
                if (ReturnDate <= DepartureDate)
                {
                    ReturnDate = DateTime.MinValue;
                    MinStayDuration = MaxStayDuration = 0;
                }
                else
                {
                    if (MinStayDuration > MaxStayDuration)
                        error = "Minimum stay duration must be less than or equal to maximum stay duration!";
                }

                if (!String.IsNullOrEmpty(error))
                    throw new ApplicationException("Invalid execution parameter: " + error);
            }
        }
    }
}