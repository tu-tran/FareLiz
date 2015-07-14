namespace SkyDean.FareLiz.WinForm
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Data;
    using SkyDean.FareLiz.Data.Config;
    using SkyDean.FareLiz.Data.Monitoring;
    using SkyDean.FareLiz.WinForm.Properties;

    /// <summary>The execution param.</summary>
    [Serializable]
    internal class ExecutionParam : ExecutionInfo
    {
        /// <summary>The dat e_ forma t_ param.</summary>
        private const string DATE_FORMAT_PARAM = "yyyyMMdd";

        /// <summary>The switches.</summary>
        private static readonly SwitchForm[] switches =
            {
                new SwitchForm("m", SwitchType.Simple, false, "Start the application minimized to tray"), 

                // 0- Minimize
                new SwitchForm(
                    "sync", 
                    SwitchType.Simple, 
                    false, 
                    "Upload data packages using pre-configured data synchronizer after getting fare"), 

                // 1- Upload data to cloud service
                new SwitchForm(
                    "d", 
                    SwitchType.LimitedPostString, 
                    false, 
                    DATE_FORMAT_PARAM.Length, 
                    "Departure date in format " + DATE_FORMAT_PARAM), 

                // 2- Departure Date
                new SwitchForm("Dr", SwitchType.UnLimitedPostString, false, "Departure date range"), 

                // 3- Departure range
                new SwitchForm(
                    "r", 
                    SwitchType.LimitedPostString, 
                    false, 
                    DATE_FORMAT_PARAM.Length, 
                    "Return date in format " + DATE_FORMAT_PARAM), 

                // 4- Return date
                new SwitchForm("Rr", SwitchType.UnLimitedPostString, false, "Return date range"), 

                // 5- Return range
                new SwitchForm("sMin", SwitchType.UnLimitedPostString, false, "Minimum stay duration"), 

                // 6- Min stay
                new SwitchForm("sMax", SwitchType.UnLimitedPostString, false, "Maximum stay duration"), 

                // 7- Max stay
                new SwitchForm("pLim", SwitchType.UnLimitedPostString, false, "Price limit"), 

                // 8- Price Limit
                new SwitchForm("op", 
                    SwitchType.UnLimitedPostString, 
                    false, 
                    @"Data operation type. It can be one of the following:
   +CloseAndExport: Get fare data and save to the database
   +LiveMonitor: Live fare monitor"), // 9- Operation Mode
                new SwitchForm("dept", SwitchType.UnLimitedPostString, false, "Departure location"), 

                // 10- Departure
                new SwitchForm("dest", SwitchType.UnLimitedPostString, false, "Journey destination"), 

                // 11- Destination
                new SwitchForm("?", SwitchType.Simple, false, "Show help"), 

                // 12- Help
                new SwitchForm("exit", SwitchType.Simple, false, "Exit application after getting data")

                // 13- Exit
            };

        /// <summary>The _exit after done.</summary>
        private bool _exitAfterDone;

        /// <summary>The _op mode.</summary>
        private OperationMode _opMode = OperationMode.Unspecified;

        /// <summary>Gets or sets the operation mode.</summary>
        [DisplayName("Operation mode")]
        [Description("Opearation mode")]
        [Category("Application Settings")]
        [DefaultValue(OperationMode.GetFareAndSave)]
        public OperationMode OperationMode
        {
            get
            {
                return this._opMode;
            }

            set
            {
                this._opMode = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether exit after done.</summary>
        [DisplayName("Exit after done")]
        [Description("Exit the application after everything is done")]
        [Category("Application Settings")]
        [DefaultValue(true)]
        public bool ExitAfterDone
        {
            get
            {
                return this._exitAfterDone;
            }

            set
            {
                this._exitAfterDone = value;
            }
        }

        /// <summary>Gets or sets the config handler.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IObjectPersist ConfigHandler { get; set; }

        /// <summary>The generate command line.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public string GenerateCommandLine()
        {
            this.Validate();
            var sb = new StringBuilder();
            if (this.IsMinimized)
            {
                sb.Append(" -" + switches[0]);
            }

            if (this.AutoSync)
            {
                sb.Append(" -" + switches[1]);
            }

            sb.Append(" /" + switches[2] + this.Wrap(this.DepartureDate.ToString(DATE_FORMAT_PARAM)));
            sb.Append(string.Format(" /{0}+{1}-{2}", switches[3], this.DepartureDateRange.Plus, this.DepartureDateRange.Minus));
            sb.Append(" /" + switches[4] + this.Wrap(this.ReturnDate.ToString(DATE_FORMAT_PARAM)));
            sb.Append(string.Format(" /{0}+{1}-{2}", switches[5], this.ReturnDateRange.Plus, this.ReturnDateRange.Minus));
            sb.Append(" /" + switches[6] + this.Wrap(this.MinStayDuration));
            sb.Append(" /" + switches[7] + this.Wrap(this.MaxStayDuration));
            sb.Append(" /" + switches[8] + this.Wrap(this.PriceLimit));
            sb.Append(" /" + switches[9] + this.Wrap(this.OperationMode));
            sb.Append(" /" + switches[10] + this.Wrap(this.Departure.IATA));
            sb.Append(" /" + switches[11] + this.Wrap(this.Destination.IATA));

            // sb.Append(" /" + switches[12]);  // 12: (Show Help)
            if (this.ExitAfterDone)
            {
                sb.Append(" /" + switches[13]);
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// The wrap.
        /// </summary>
        /// <param name="cmdLine">
        /// The cmd line.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string Wrap(object cmdLine)
        {
            var str = cmdLine == null ? string.Empty : cmdLine.ToString();
            return str.Contains(" ") ? string.Format("\"{0}\"", str) : str;
        }

        /// <summary>The show help.</summary>
        public static void ShowHelp()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Specify parameters in this format: -{Parameter ConfiguredType}[{Parameter TypeConfiguration}]");
            sb.AppendLine("(without the { and } characters)\n");
            sb.AppendLine("If parameter key value contains space, wrap it in double-quotation mark\n");
            foreach (var p in switches)
            {
                sb.AppendFormat("-{0}{1}\t{2}\n", p.IDString, p.Type == SwitchType.Simple ? string.Empty : "..", p.Description);
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

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="paramArgs">
        /// The param args.
        /// </param>
        /// <param name="iniFilePath">
        /// The ini file path.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public static bool Parse(string[] paramArgs, string iniFilePath, ILogger logger, out ExecutionParam param)
        {
            paramArgs = paramArgs ?? new string[0];
            var result = new ExecutionParam();

            int tempI;

            var parser = new Parser(switches.Length);
            parser.ParseStrings(switches, paramArgs);

            if (!string.IsNullOrEmpty(iniFilePath))
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
                if (!DateTime.TryParseExact(parser[2].PostStrings[0].ToString(), DATE_FORMAT_PARAM, null, DateTimeStyles.AssumeLocal, out tempD))
                {
                    throw new ApplicationException("Invalid departure date");
                }

                result.DepartureDate = tempD;
            }

            if (parser[4].ThereIs)
            {
                if (!DateTime.TryParseExact(parser[4].PostStrings[0].ToString(), DATE_FORMAT_PARAM, null, DateTimeStyles.AssumeLocal, out tempD))
                {
                    throw new ApplicationException("Invalid return date");
                }

                result.ReturnDate = tempD;
            }

            if (parser[6].ThereIs)
            {
                if (!int.TryParse(parser[6].PostStrings[0].ToString(), out tempI))
                {
                    throw new ApplicationException("Invalid minimum stay duration");
                }

                result.MinStayDuration = tempI;
            }
            else
            {
                result.MinStayDuration = Settings.Default.DefaultDurationMin;
            }

            if (parser[7].ThereIs)
            {
                if (!int.TryParse(parser[7].PostStrings[0].ToString(), out tempI))
                {
                    throw new ApplicationException("Invalid maximum stay duration");
                }

                result.MaxStayDuration = tempI;
            }
            else
            {
                result.MaxStayDuration = Settings.Default.DefaultDurationMax;
            }

            if (parser[8].ThereIs)
            {
                if (!int.TryParse(parser[8].PostStrings[0].ToString(), out tempI))
                {
                    throw new ApplicationException("Invalid price limit");
                }

                result.PriceLimit = tempI;
            }
            else
            {
                result.PriceLimit = Settings.Default.DefaultPriceLimit;
            }

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
            {
                result.Departure = AirportDataProvider.FromIATA(parser[10].PostStrings[0].ToString());
            }

            if (parser[11].ThereIs)
            {
                result.Destination = AirportDataProvider.FromIATA(parser[11].PostStrings[0].ToString());
            }

            if (parser[13].ThereIs)
            {
                result.ExitAfterDone = true;
            }

            result.Validate();
            param = result;

            if (parser[12].ThereIs)
            {
                return false;
            }

            return true;
        }

        /// <summary>The validate.</summary>
        /// <exception cref="ApplicationException"></exception>
        public void Validate()
        {
            if (this.OperationMode != OperationMode.Unspecified)
            {
                string error = null;

                if (this.Departure == null || this.Destination == null)
                {
                    error = "Departure and destination must be specified!";
                }
                else if (string.Equals(this.Departure.IATA, this.Destination.IATA, StringComparison.OrdinalIgnoreCase))
                {
                    error = "Departure and destination must be different!";
                }

                if (this.DepartureDate.IsUndefined())
                {
                    error = "Departure date must be specified";
                }

                if (this.ReturnDate <= this.DepartureDate)
                {
                    this.ReturnDate = DateTime.MinValue;
                    this.MinStayDuration = this.MaxStayDuration = 0;
                }
                else
                {
                    if (this.MinStayDuration > this.MaxStayDuration)
                    {
                        error = "Minimum stay duration must be less than or equal to maximum stay duration!";
                    }
                }

                if (!string.IsNullOrEmpty(error))
                {
                    throw new ApplicationException("Invalid execution parameter: " + error);
                }
            }
        }
    }
}