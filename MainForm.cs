using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Ionic.Zip;

namespace DailyDepositReader
{
    public enum DailyActivityVersion
    {
        V1 = 1,
        V2 = 2,
        V3 = 3,
        V4 = 4,
        V5 = 5
    }

    public enum ColumnIndex
    {
        Net = 4,
        TrxCount = 5,
        AvgSale = 6,
        CardReg = 7,
        CardMachine = 8,
        CashChecks = 9,
        CoffeeTrx = 10,
        CoffeeCups = 11,
        CoffeeAvgPrice = 12,
        FirstCat = 13,
        LastCat = 32,
        CatCount = 20,
        NotVoided = 33
    }
        
    public partial class MainForm : Form
    {
        private List<string> mMessages;
        private Boolean mShowAllDiagnostics = true;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.StartDate.CompareTo(new DateTime(2000, 1, 1)) > 0)
            {
                ctlStartDate.Value = Properties.Settings.Default.StartDate;
            }
            if (Properties.Settings.Default.EndDate.CompareTo(new DateTime(2000, 1, 1)) > 0)
            {
                ctlEndDate.Value = Properties.Settings.Default.EndDate;
            }
            txtFolder.Text = Properties.Settings.Default.SourcePath;
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            int consecutiveMissingFiles = 0;
            int cardMachineCountPrevious = 0;
            int filesLoaded = 0;
            mShowAllDiagnostics = chkShowAllWarnings.Checked;
            decimal[] dayOfWeekTotals = new decimal[7];
            int[] dayOfWeekCounts = new int[7];
            HourlyTotals hourlyTotals = new HourlyTotals();
            decimal[] columnTotals = new decimal[50];   // more columns than really needed
            StringBuilder lines = new StringBuilder();
            // Look for "//ADDCOL" for places to change to add a column.
            //ADDCOL - Add the column to the grid in the form designer.
            //ADDCOL - Add the column header to the statement below.
            lines.Append("Date\tDayOfWeek\tWeather\tCircumstances" +
                "\tNet\tTrx Count\tAvg Sale\tCard Reg\tCard Machine\tBank Dep" +
                "\tCoffee Trx\tCoffee Cups\tCoffee Price" +
                "\tGift\tBird Seed\tPlant\tGeneral\tSeasonal\tWater\tSoil" +
                "\tHouseplant\tTools\tBaked\tSeeds\tFert" +
                "\tPots\tPruners\tChocolate\tBulbs\tChemicals" +
                "\tBird Feeders\tUnused\tGC\tNot Voided" + Environment.NewLine);
            List<WeekAccumulator> weekTotals = new List<WeekAccumulator>();
            List<string> exportLines = new List<string>();
            mMessages = new List<string>();
            grdResults.Rows.Clear();
            DateTime startDate = ctlStartDate.Value.Date;
            DateTime endDate = ctlEndDate.Value.Date;
            Properties.Settings.Default.SourcePath = txtFolder.Text;
            Properties.Settings.Default.StartDate = startDate;
            Properties.Settings.Default.EndDate = endDate;
            Properties.Settings.Default.Save();
            for (DateTime currentDate = startDate;
                currentDate.CompareTo(endDate) <= 0;
                currentDate = currentDate.AddDays(1.0))
            {
                XmlNamespaceManager nsMgr;
                XmlElement spreadsheet;
                string fileName = GetFileName(currentDate);
                lblCurrentFile.Text = fileName;
                lblCurrentFile.Refresh();
                // Load the file, but do not parse contents.
                LoadFileForDate(fileName, currentDate, out spreadsheet, out nsMgr, ref consecutiveMissingFiles, weekTotals);
                if (spreadsheet != null)
                {
                    Sheet reconcileSheet = new Sheet(spreadsheet, "Reconcile", nsMgr, 20);
                    string line;
                    DataGridViewRow gridRow;
                    // This is what parses and computes results.
                    int cardMachineCountCurrent;
                    MakeSummaryLine(reconcileSheet, fileName, currentDate, out line, out gridRow, out cardMachineCountCurrent,
                        dayOfWeekTotals, dayOfWeekCounts, columnTotals, weekTotals, hourlyTotals, exportLines);
                    if (line != null)
                    {
                        lines.AppendLine(line);
                        grdResults.Rows.Add(gridRow);
                        if (cardMachineCountPrevious > 0 && cardMachineCountCurrent != cardMachineCountPrevious)
                        {
                            ShowFileWarning(fileName, "Previous file has different number of registers / card machines");
                        }
                        cardMachineCountPrevious = cardMachineCountCurrent;
                        filesLoaded++;
                    }
                }
                if (consecutiveMissingFiles > 2)
                {
                    ShowMessage("Too many missing files in a row - stopping.");
                    break;
                }
            }
            if (filesLoaded > 0)
            {
                AddTotalRow(columnTotals);
                AddPercentRow(columnTotals);
                ShowDayOfWeekTotals(dayOfWeekTotals, dayOfWeekCounts);
                ShowHourlyTotals(hourlyTotals);
                ShowWeekTotals(weekTotals, lines);
                //ShowExportLines(exportLines, lines);
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetText(lines.ToString());
                    ShowMessage("File(s) read and clipboard loaded.");
                }
                catch (Exception ex)
                {
                    ShowMessage("Error placing summary on clipboard: " + ex.Message);
                }
            }
            else
                ShowMessage("No files loaded");
            using (MessageListForm msgForm = new MessageListForm())
            {
                msgForm.Show(mMessages);
            }
        }

        private void ShowMessage(string msg)
        {
            //MessageBox.Show(msg);
            mMessages.Add(msg);
        }

        private void ShowFileWarning(string fileName, string msg)
        {
            ShowMessage("Warning on " + fileName + ": " + msg);
        }

        /// <summary>
        /// Loads the file into memory.
        /// Does not extract content.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="activityDate"></param>
        /// <param name="spreadsheet"></param>
        /// <param name="nsMgr"></param>
        /// <param name="consecutiveMissingFiles"></param>
        /// <param name="weekTotals"></param>
        private void LoadFileForDate(string fileName, DateTime activityDate,
            out XmlElement spreadsheet, out XmlNamespaceManager nsMgr,
            ref int consecutiveMissingFiles, List<WeekAccumulator> weekTotals)
        {
            nsMgr = null;
            spreadsheet = null;
            if (!File.Exists(fileName))
            {
                ShowMessage("Could not find " + fileName);
                consecutiveMissingFiles++;
                AddToWeekTotals(weekTotals, activityDate, 0.0M);
                return;
            }
            using (ZipFile zipFile = ZipFile.Read(fileName))
            {
                ZipEntry zipEntry = zipFile["Content.xml"];
                XmlDocument xmlDoc = new XmlDocument();
                nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
                xmlDoc.Load(zipEntry.OpenReader());
                nsMgr.AddNamespace("office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                nsMgr.AddNamespace("table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
                nsMgr.AddNamespace("text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                spreadsheet = (XmlElement)xmlDoc.DocumentElement.SelectSingleNode(
                    "office:body/office:spreadsheet", nsMgr);
                consecutiveMissingFiles = 0;
            }
        }

        private string GetFileName(DateTime activityDate)
        {
            string monthAbbr;
            switch (activityDate.Month)
            {
                case 1: monthAbbr = "Jan"; break;
                case 2: monthAbbr = "Feb"; break;
                case 3: monthAbbr = "Mar"; break;
                case 4: monthAbbr = "Apr"; break;
                case 5: monthAbbr = "May"; break;
                case 6: monthAbbr = "Jun"; break;
                case 7: monthAbbr = "Jul"; break;
                case 8: monthAbbr = "Aug"; break;
                case 9: monthAbbr = "Sep"; break;
                case 10: monthAbbr = "Oct"; break;
                case 11: monthAbbr = "Nov"; break;
                case 12: monthAbbr = "Dec"; break;
                default: throw new ArgumentException();
            }
            string fileName = string.Format("{0}\\{1}\\{2}\\{3}.ods",
                txtFolder.Text, activityDate.Year, monthAbbr, activityDate.Day);
            return fileName;
        }

        private DailyActivityVersion GetVersion(Sheet sheet)
        {
            string versionTag = sheet.GetValue("H", 2);
            if (versionTag == "Version3")
            {
                return DailyActivityVersion.V3;     // 1/2/2010->1/6/2010, 1/27/2011->4/20/2012
            }
            if (versionTag == "Version4")
            {
                return DailyActivityVersion.V4;     // 4/21/2012->11/??/2014
            }
            if (versionTag == "Version5")
            {
                return DailyActivityVersion.V5;     // 11/??/2014->
            }
            if (sheet.GetValue("F", 8) == "Circumstances:" &&
                sheet.GetValue("F", 2) == "Date:" &&
                sheet.GetValue("A", 32) == "NET" &&
                sheet.GetValue("A", 38) == "CHARGE")
            {
                return DailyActivityVersion.V1;     // 3/29/08, 3/30/08, 4/4/08
            }
            if (sheet.GetValue("A", 6) == "Circumstances:" &&
                sheet.GetValue("A", 3) == "Date:" &&
                sheet.GetValue("A", 37) == "NET" &&
                sheet.GetValue("A", 43) == "CHARGE")
            {
                return DailyActivityVersion.V2;     // 4/1/08->1/26/11 (except 4/4/08, 1/2/2010->1/6/2010)
            }
            throw new ArgumentException("Unrecognized daily activity spreadsheet format");
        }

        /// <summary>
        /// This is what reads the content of the file, transforms it,
        /// and computes summaries and all the output.
        /// </summary>
        /// <param name="reconcileSheet"></param>
        /// <param name="fileName"></param>
        /// <param name="expectedDate"></param>
        /// <param name="resultRow"></param>
        /// <param name="gridRow"></param>
        /// <param name="dayOfWeekTotals"></param>
        /// <param name="dayOfWeekCounts"></param>
        /// <param name="columnTotals"></param>
        /// <param name="weekTotals"></param>
        /// <param name="exportLines"></param>
        private void MakeSummaryLine(Sheet reconcileSheet, string fileName, DateTime expectedDate,
            out string resultRow, out DataGridViewRow gridRow, out int cardMachineCount, decimal[] dayOfWeekTotals, 
            int[] dayOfWeekCounts, decimal[] columnTotals, List<WeekAccumulator> weekTotals,
            HourlyTotals hourlyTotals, List<string> exportLines)
        {
            string activityDate;
            string weather;
            string circumstances;
            string net;
            string trxcount = string.Empty;
            string avgsale = string.Empty;
            string cardRegister;
            string cardMachine;
            //string cardMachine2;
            int cardColumn;
            List<string> cardMachines = new List<string>();
            string bankDeposit;
            string coffeeTrx = "0";
            int coffeeTrxInt;
            string coffeeCups = "0";
            int coffeeCupsInt = 0;
            string coffeeAvgPrice = "";
            int hourlyRow;
            string notVoided;
            decimal[] catAmounts = new decimal[(int)ColumnIndex.CatCount + 1];
            int largestCatNum = 0;
            decimal largestCatValue = -999999.0M;
            int firstCatRow;
            int totalsColumn;
            int firstRegisterColumn;
            int lastRegisterColumn;
            int firstHourlyRow;
            decimal shortAdjust;
            HourlyDay hourlyDay = new HourlyDay();
            try
            {
                switch (GetVersion(reconcileSheet))
                {
                    case DailyActivityVersion.V1:
                        activityDate = reconcileSheet.GetValue("G", 2);
                        System.Diagnostics.Debug.WriteLine("V1" + activityDate);
                        weather = reconcileSheet.GetValue("G", 6);
                        circumstances = reconcileSheet.GetValue("G", 8);
                        net = reconcileSheet.GetValue("D", 32);
                        cardRegister = reconcileSheet.GetValue("D", 38);
                        cardMachine = reconcileSheet.GetValue("D", 43);
                        cardMachines.Add(cardMachine);
                        bankDeposit = reconcileSheet.GetValue("D", 57);
                        notVoided = reconcileSheet.GetValue("D", 48);
                        firstCatRow = 5;
                        totalsColumn = 4;
                        break;
                    case DailyActivityVersion.V2:
                        activityDate = reconcileSheet.GetValue("B", 3);
                        System.Diagnostics.Debug.WriteLine("V2" + activityDate);
                        weather = reconcileSheet.GetValue("B", 5);
                        circumstances = reconcileSheet.GetValue("B", 6);
                        net = reconcileSheet.GetValue("D", 37);
                        cardRegister = reconcileSheet.GetValue("D", 43);
                        cardMachine = reconcileSheet.GetValue("D", 48);
                        cardMachines.Add(cardMachine);
                        bankDeposit = reconcileSheet.GetValue("D", 62);
                        notVoided = reconcileSheet.GetValue("D", 53);
                        firstCatRow = 10;
                        totalsColumn = 4;
                        break;
                    case DailyActivityVersion.V3:
                        activityDate = reconcileSheet.GetValue("B", 3);
                        System.Diagnostics.Debug.WriteLine("V3" + activityDate);
                        weather = reconcileSheet.GetValue("B", 5);
                        circumstances = reconcileSheet.GetValue("B", 6);
                        net = reconcileSheet.GetValue("D", 37);
                        trxcount = reconcileSheet.GetValue("D", 40);
                        avgsale = reconcileSheet.GetValue("B", 75);
                        cardRegister = reconcileSheet.GetValue("D", 43);
                        cardMachine = reconcileSheet.GetValue("D", 49);
                        cardMachines.Add(cardMachine);
                        bankDeposit = reconcileSheet.GetValue("D", 63);
                        notVoided = reconcileSheet.GetValue("D", 54);
                        firstCatRow = 10;
                        totalsColumn = 4;
                        break;
                    case DailyActivityVersion.V4:
                        // Same as V3, except V3 rows 68 and above moved down by 15
                        // to make room for the register transaction counts.
                        activityDate = reconcileSheet.GetValue("B", 3);
                        System.Diagnostics.Debug.WriteLine("V4" + activityDate);
                        weather = reconcileSheet.GetValue("B", 5);
                        circumstances = reconcileSheet.GetValue("B", 6);
                        net = reconcileSheet.GetValue("D", 37);
                        trxcount = reconcileSheet.GetValue("D", 40);
                        avgsale = reconcileSheet.GetValue("B", 90);
                        cardRegister = reconcileSheet.GetValue("D", 43);
                        cardMachine = reconcileSheet.GetValue("D", 49);
                        if (reconcileSheet.GetValue("B", 40) != "0")        // Trans count reg #1
                        {
                            if (reconcileSheet.GetValue("B", 49) != "")
                                cardMachines.Add(reconcileSheet.GetValue("B", 49));
                            else
                                cardMachines.Add(reconcileSheet.GetValue("B", 43));
                        }
                        if (reconcileSheet.GetValue("C", 40) != "0")        // Trans count reg #2
                        {
                            if (reconcileSheet.GetValue("C", 49) != "")
                                cardMachines.Add(reconcileSheet.GetValue("C", 49));
                            else
                                cardMachines.Add(reconcileSheet.GetValue("C", 43));
                        }
                        bankDeposit = reconcileSheet.GetValue("D", 63);
                        notVoided = reconcileSheet.GetValue("D", 54);
                        firstCatRow = 10;
                        totalsColumn = 4;
                        break;
                    case DailyActivityVersion.V5:
                        // Same as V4, except totals moved to column 2 and up to 6 registers
                        // in columns to the right..
                        activityDate = reconcileSheet.GetValue("B", 3);
                        System.Diagnostics.Debug.WriteLine("V5" + activityDate);
                        weather = reconcileSheet.GetValue("B", 5);
                        circumstances = reconcileSheet.GetValue("B", 6);
                        net = reconcileSheet.GetValue("B", 38);
                        trxcount = reconcileSheet.GetValue("B", 41);
                        avgsale = reconcileSheet.GetValue("B", 101);
                        cardRegister = reconcileSheet.GetValue("B", 44);
                        cardMachine = reconcileSheet.GetValue("B", 50);
                        firstRegisterColumn = 3;
                        lastRegisterColumn = 8;
                        firstHourlyRow = 79;
                        for (cardColumn = firstRegisterColumn; cardColumn <= lastRegisterColumn; cardColumn++)
                        {
                            string regNTransCount = reconcileSheet.GetValue(cardColumn, 41);
                            string regNNet1 = reconcileSheet.GetValue(cardColumn, 38);
                            if (regNTransCount != "0" || regNNet1 !="$0.00")
                            {
                                string cardMachineN = reconcileSheet.GetValue(cardColumn, 50);
                                cardMachines.Add(cardMachineN);
                                if (cardMachineN == "$0.00")
                                {
                                    ShowFileWarning(fileName, "Card machine #" +
                                        (cardColumn - firstRegisterColumn + 1).ToString() +
                                        " is $0.00 when that register has activity");
                                }
                                string cardRegN = reconcileSheet.GetValue(cardColumn, 44);
                                if (mShowAllDiagnostics && (cardMachineN != cardRegN))
                                {
                                    ShowFileWarning(fileName, "Card machine #" +
                                        (cardColumn - firstRegisterColumn + 1).ToString() +
                                        " is different than cards on that register");
                                }
                            }
                        }
                        // (Cash adjusted for short) - (less checks)
                        bankDeposit = (GetMoneyValue( reconcileSheet.GetValue("B", 71), "Cash to deposit")
                            - GetMoneyValue(reconcileSheet.GetValue("B", 70), "Checks to deposit")).ToString("C2");
                        coffeeTrxInt = 0;
                        for (hourlyRow = firstHourlyRow; hourlyRow <= 93; hourlyRow++)
                        {
                            for (int hourlyCol = firstRegisterColumn; hourlyCol <= lastRegisterColumn; hourlyCol++)
                            {
                                int hourTrx;
                                string cnt = reconcileSheet.GetValue(hourlyCol, hourlyRow);
                                hourTrx = GetIntegerValue(cnt, "Hourly trx count");
                                HourlyRegister hourlyReg = hourlyDay.Registers[hourlyCol - firstRegisterColumn];
                                hourlyReg.DataCount = 1;
                                hourlyReg.HourCounts[hourlyRow - firstHourlyRow] = hourTrx;
                                if (hourlyCol == 5)
                                {
                                    coffeeTrxInt += hourTrx;
                                }
                            }
                        }
                        coffeeTrx = coffeeTrxInt.ToString();
                        coffeeCups = reconcileSheet.GetValue(6, 62);
                        coffeeCupsInt = GetIntegerValue(coffeeCups, "Coffee cups");
                        notVoided = reconcileSheet.GetValue("B", 55);
                        firstCatRow = 10;
                        totalsColumn = 2;
                        shortAdjust = GetMoneyValue(reconcileSheet.GetValue("B", 73), "Adjustment for short");
                        if (mShowAllDiagnostics && (shortAdjust != 0))
                        {
                            ShowFileWarning(fileName, "Short adjustment " + shortAdjust);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                DateTime actDateValue = DateTime.Parse(activityDate);
                if (actDateValue != expectedDate)
                    throw new DataException("Wrong activity date");
                DayOfWeek dayOfWeek = actDateValue.DayOfWeek;
                string giftLabel = reconcileSheet.GetValue(1, firstCatRow);
                if (giftLabel.Substring(0, 3) != "D01")
                    throw new DataException("Expected categories to start on row " + firstCatRow.ToString());
                string netStripped = net.Replace("$", string.Empty);
                decimal notVoidedValue = GetMoneyValue(notVoided, "Ring errors");
                if (notVoidedValue != 0.0M)
                {
                    decimal adjustedNet = GetMoneyValue(net, "Net 1") - notVoidedValue;
                    net = adjustedNet.ToString("C2");
                    if (!string.IsNullOrEmpty(avgsale))
                    {
                        int parsedTrxCount;
                        if (Int32.TryParse(trxcount, out parsedTrxCount))
                        {
                            avgsale = (adjustedNet / parsedTrxCount).ToString("C2");
                        }
                    }
                    netStripped = net.Replace("$", string.Empty);
                }
                //ADDCOL - Add the column value to the statement below.
                resultRow =
                    activityDate + "\t" +
                    dayOfWeek + "\t" +
                    weather + "\t" +
                    circumstances + "\t" +
                    netStripped + "\t" +
                    trxcount + "\t" +
                    avgsale.Replace("$", string.Empty) + "\t" +
                    cardRegister.Replace("$", string.Empty) + "\t" +
                    cardMachine.Replace("$", string.Empty) + "\t" +
                    bankDeposit.Replace("$", string.Empty) + "\t" +
                    coffeeTrx + "\t" +
                    coffeeCups + "\t" +
                    coffeeAvgPrice;
                gridRow = new DataGridViewRow();
                //ADDCOL - Call AddCell() with the new column value. Cells must be added from left to right.
                //ADDCOL - Set the new column value in columnTotals[]. Create a new ColumnIndex enum value for the index.
                AddCell(gridRow, activityDate);
                AddCell(gridRow, dayOfWeek.ToString());
                AddCell(gridRow, weather);
                AddCell(gridRow, circumstances);
                AddCell(gridRow, net);
                columnTotals[(int)ColumnIndex.Net] += GetMoneyValue(net, "Net 1");
                AddCell(gridRow, trxcount);
                if (!string.IsNullOrEmpty(trxcount))
                    columnTotals[(int)ColumnIndex.TrxCount] += GetIntegerValue(trxcount, "Trx count");
                AddCell(gridRow, avgsale);
                AddCell(gridRow, cardRegister);
                columnTotals[(int)ColumnIndex.CardReg] += GetMoneyValue(cardRegister, "Card register");
                AddCell(gridRow, cardMachine);
                columnTotals[(int)ColumnIndex.CardMachine] += GetMoneyValue(cardMachine, "Card machine");
                AddCell(gridRow, bankDeposit);
                columnTotals[(int)ColumnIndex.CashChecks] += GetMoneyValue(bankDeposit, "Bank deposit");
                AddCell(gridRow, coffeeTrx);
                columnTotals[(int)ColumnIndex.CoffeeTrx] += GetIntegerValue(coffeeTrx, "Coffee trx count");
                AddCell(gridRow, coffeeCups);
                columnTotals[(int)ColumnIndex.CoffeeCups] += coffeeCupsInt;
                for (int catNum = 1; catNum <= (int)ColumnIndex.CatCount; catNum++)
                {
                    string catAmount = reconcileSheet.GetValue(totalsColumn, firstCatRow + catNum - 1).Replace("$", string.Empty);
                    decimal catAmountValue = GetMoneyValue(catAmount, "Category amount");
                    catAmounts[catNum] = catAmountValue;
                    if (catAmountValue > largestCatValue)
                    {
                        largestCatNum = catNum;
                        largestCatValue = catAmountValue;
                    }
                }
                if (notVoidedValue > 0)
                {
                    catAmounts[largestCatNum] -= notVoidedValue;
                }
                if (coffeeCupsInt > 0)
                    coffeeAvgPrice = (catAmounts[19] / coffeeCupsInt).ToString("C2");
                else
                    coffeeAvgPrice = "$0.00";
                AddCell(gridRow, coffeeAvgPrice);
                columnTotals[(int)ColumnIndex.CoffeeAvgPrice] += GetMoneyValue(coffeeAvgPrice, "Average coffee price");
                for (int catNum = 1; catNum <= (int)ColumnIndex.CatCount; catNum++)
                {
                    resultRow += ("\t" + catAmounts[catNum].ToString("F2"));
                    AddCell(gridRow, catAmounts[catNum].ToString("C2"));
                    columnTotals[(int)ColumnIndex.FirstCat + catNum - 1] += catAmounts[catNum];
                }
                resultRow += ("\t" + notVoided.Replace("$", string.Empty));
                AddCell(gridRow, notVoided);
                columnTotals[(int)ColumnIndex.NotVoided] += notVoidedValue;
                dayOfWeekTotals[(int)dayOfWeek] += decimal.Parse(netStripped);
                dayOfWeekCounts[(int)dayOfWeek]++;
                AddToWeekTotals(weekTotals, actDateValue, GetMoneyValue(net, "Net 1"));
                cardMachineCount = cardMachines.Count;
                //AddToExportLines(exportLines, actDateValue, cardMachines, bankDeposit);
                switch (dayOfWeek)
                {
                    case System.DayOfWeek.Monday:
                    case System.DayOfWeek.Tuesday:
                    case System.DayOfWeek.Wednesday:
                    case System.DayOfWeek.Thursday:
                    case System.DayOfWeek.Friday:
                        hourlyTotals.Weekday.Add(hourlyDay);
                        break;
                    case System.DayOfWeek.Saturday:
                        hourlyTotals.Weekend.Add(hourlyDay);
                        hourlyTotals.Saturday.Add(hourlyDay);
                        break;
                    case System.DayOfWeek.Sunday:
                        hourlyTotals.Weekend.Add(hourlyDay);
                        hourlyTotals.Sunday.Add(hourlyDay);
                        break;
                }
            }
            catch (Exception e)
            {
                ShowMessage("Exception processing " + fileName + ": " + e.Message);
                resultRow = null;
                gridRow = null;
                cardMachineCount = 0;
                return;
            }
        }

        private void AddToWeekTotals(List<WeekAccumulator> weekTotals, DateTime actDateValue, decimal netValue)
        {
            if (weekTotals.Count == 0 || weekTotals[weekTotals.Count - 1].DayCount >= 7)
            {
                weekTotals.Add(new WeekAccumulator(actDateValue));
            }
            weekTotals[weekTotals.Count - 1].Add(netValue);
        }

        //private void AddToExportLines(List<string> exportLines, DateTime actDateValue,
        //    IEnumerable<string> cardMachines, string bankDeposit)
        //{
        //    DayOfWeek dayOfWeek = actDateValue.DayOfWeek;
        //    int cardIndex = 1;
        //    foreach (string cardMachine in cardMachines)
        //    {
        //        string cardLine = actDateValue.ToShortDateString() + " " +
        //            "Credit_Card_Deposit_(#" + cardIndex.ToString() + ")_(" + actDateValue.Day.ToString("D2") + "_" + dayOfWeek.ToString() + ") " +
        //            GetMoneyValue(cardMachine, "Card machine").ToString("F2");
        //        exportLines.Add(cardLine);
        //        cardIndex++;
        //    }
        //    string cashLine = actDateValue.ToShortDateString() + " " +
        //        "Cash_&_Check_Deposit_(" + actDateValue.Day.ToString("D2") + "_" + dayOfWeek.ToString() + ") " +
        //        GetMoneyValue(bankDeposit, "Bank deposit").ToString("F2");
        //    exportLines.Add(cashLine);
        //}

        private decimal GetMoneyValue(string input, string errorLabel)
        {
            if (input.Trim() == string.Empty)
                return 0M;
            if (decimal.TryParse(input.Replace("$", string.Empty), out decimal result))
                return result;
            throw new Exception("Error converting " + errorLabel + " to decimal");
        }

        private int GetIntegerValue(string input, string errorLabel)
        {
            if (input.Trim() == string.Empty)
                return 0;
            if (Int32.TryParse(input, out int result))
                return result;
            throw new Exception("Error converting " + errorLabel + " to integer");
        }

        private void AddTotalRow(decimal[] columnTotals)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            AddCell(gridRow, "Totals");
            AddCell(gridRow, string.Empty);
            AddCell(gridRow, string.Empty);
            AddCell(gridRow, string.Empty);
            //ADDCOL - Call AddCell() for the new column, if necessary
            for (int columnNumber = (int)ColumnIndex.Net; columnNumber <= (int)ColumnIndex.NotVoided; columnNumber++)
            {
                if (columnNumber == (int)ColumnIndex.TrxCount ||
                    columnNumber == (int)ColumnIndex.CoffeeCups ||
                    columnNumber == (int)ColumnIndex.CoffeeTrx)
                    AddCell(gridRow, columnTotals[columnNumber].ToString("F0"));
                else if (columnNumber == (int)ColumnIndex.CoffeeAvgPrice)
                    AddCell(gridRow, string.Empty);
                else if (columnNumber == (int)ColumnIndex.AvgSale)
                {
                    if (columnTotals[(int)ColumnIndex.TrxCount] > 0.0M)
                    {
                        decimal totalAvgSale =
                            columnTotals[(int)ColumnIndex.Net] / columnTotals[(int)ColumnIndex.TrxCount];
                        AddCell(gridRow, totalAvgSale.ToString("C2"));
                    }
                    else
                        AddCell(gridRow, string.Empty);
                }
                else
                    AddCell(gridRow, columnTotals[columnNumber].ToString("C2"));
            }
            grdResults.Rows.Add(gridRow);
        }

        private void ShowHourlyTotals(HourlyTotals hourlyTotals)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            grdResults.Rows.Add(gridRow);
            gridRow = new DataGridViewRow();
            AddCell(gridRow, "Hourly Totals");
            grdResults.Rows.Add(gridRow);
            ShowHourlyDay(hourlyTotals.Weekday, "Weekday");
            ShowHourlyDay(hourlyTotals.Weekend, "Weekend");
            ShowHourlyDay(hourlyTotals.Saturday, "Saturday");
            ShowHourlyDay(hourlyTotals.Sunday, "Sunday");
        }

        private void ShowHourlyDay(HourlyDay hourlyDay, string title)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            grdResults.Rows.Add(gridRow);
            gridRow = new DataGridViewRow();
            AddCell(gridRow, title + " Hourly Totals");
            AddCell(gridRow, "Reg 1");
            AddCell(gridRow, "Reg 2");
            AddCell(gridRow, "Reg 3");
            AddCell(gridRow, "Reg 4");
            AddCell(gridRow, "Reg 5");
            AddCell(gridRow, "Reg 6");
            grdResults.Rows.Add(gridRow);
            for (int hour = 0; hour < hourlyDay.Registers[0].HourCounts.Length; hour++)
            {
                gridRow = new DataGridViewRow();
                string hourName;
                if ((hour + 7) > 12)
                    hourName = (hour + 7 - 12).ToString() + "pm";
                else
                    hourName = (hour + 7).ToString() + "am";
                AddCell(gridRow, hourName);
                for (int regCol = 0; regCol < hourlyDay.Registers.Length; regCol++)
                {
                    HourlyRegister hourlyReg = hourlyDay.Registers[regCol];
                    string cellText = (((double)hourlyReg.HourCounts[hour])/hourlyReg.DataCount).ToString("N2");
                    AddCell(gridRow, cellText);
                }
                grdResults.Rows.Add(gridRow);
            }
        }

        private void ShowWeekTotals(List<WeekAccumulator> weekTotals, StringBuilder lines)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            grdResults.Rows.Add(gridRow);
            lines.AppendLine(string.Empty);

            gridRow = new DataGridViewRow();
            AddCell(gridRow, "Weekly Totals");
            grdResults.Rows.Add(gridRow);
            lines.AppendLine("Weekly Totals");

            foreach (WeekAccumulator week in weekTotals)
            {
                gridRow = new DataGridViewRow();
                string weekStartDate = week.WeekStartDate.ToShortDateString();
                AddCell(gridRow, weekStartDate);
                AddCell(gridRow, week.WeekTotal.ToString("C2"));
                grdResults.Rows.Add(gridRow);
                lines.AppendLine(weekStartDate + "\t" + week.WeekTotal.ToString("F2"));
            }
        }

        //private void ShowExportLines(List<string> exportLines, StringBuilder lines)
        //{
        //    DataGridViewRow gridRow = new DataGridViewRow();
        //    grdResults.Rows.Add(gridRow);
        //    lines.AppendLine(string.Empty);

        //    gridRow = new DataGridViewRow();
        //    AddCell(gridRow, "Export Lines");
        //    grdResults.Rows.Add(gridRow);
        //    lines.AppendLine("Export Lines");

        //    foreach (string exportLine in exportLines)
        //    {
        //        gridRow = new DataGridViewRow();
        //        AddCell(gridRow, exportLine);
        //        grdResults.Rows.Add(gridRow);
        //        lines.AppendLine(exportLine);
        //    }
        //}

        private void AddPercentRow(decimal[] columnTotals)
        {
            //ADDCOL - Call AddCell() for the new column
            DataGridViewRow gridRow = new DataGridViewRow();
            decimal totalReceipts = columnTotals[(int)ColumnIndex.CardReg] + columnTotals[(int)ColumnIndex.CashChecks];
            AddCell(gridRow, "Percents");
            AddCell(gridRow, string.Empty);
            AddCell(gridRow, string.Empty);
            AddCell(gridRow, string.Empty);
            AddCell(gridRow, string.Empty); // net
            AddCell(gridRow, string.Empty); // trx count
            AddCell(gridRow, string.Empty); // avg sale
            AddCell(gridRow, ((columnTotals[(int)ColumnIndex.CardReg] / totalReceipts) * 100).ToString("F1") + "%");
            AddCell(gridRow, string.Empty); // card machine
            AddCell(gridRow, ((columnTotals[(int)ColumnIndex.CashChecks] / totalReceipts) * 100).ToString("F1") + "%");
            AddCell(gridRow, string.Empty); // coffee trx
            AddCell(gridRow, string.Empty); // coffee cups
            AddCell(gridRow, string.Empty); // coffee avg price
            decimal catTotal = 0;
            // Deliberately omit last cat (GC) in computing the category total
            for (int columnNumber = (int)ColumnIndex.FirstCat; columnNumber <= (int)ColumnIndex.LastCat - 1; columnNumber++)
            {
                catTotal += columnTotals[columnNumber];
            }
            // Include last cat column to give the GC redemption as % of cat total
            for (int columnNumber = (int)ColumnIndex.FirstCat; columnNumber <= (int)ColumnIndex.LastCat; columnNumber++)
            {
                AddCell(gridRow, ((columnTotals[columnNumber] / catTotal) * 100).ToString("F1") + "%");
            }
            grdResults.Rows.Add(gridRow);
        }

        private void AddCell(DataGridViewRow gridRow, string value)
        {
            DataGridViewCell gridCell = new DataGridViewTextBoxCell();
            gridCell.Value = value;
            gridRow.Cells.Add(gridCell);
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            ctlFolderBrowser.SelectedPath = txtFolder.Text;
            DialogResult dlgRes = ctlFolderBrowser.ShowDialog();
            if (dlgRes == DialogResult.OK)
                txtFolder.Text = ctlFolderBrowser.SelectedPath;
        }

        private void ShowDayOfWeekTotals(decimal[] dayOfWeekTotals, int[] dayOfWeekCounts)
        {
            StringBuilder result = new StringBuilder();
            decimal sumOfDayOfWeekAverages = 0;
            decimal dayOfWeekAverage;
            int dayIndex = 0;
            foreach (decimal dayAmount in dayOfWeekTotals)
            {
                if (dayAmount > 0)
                {
                    dayOfWeekAverage = dayAmount / dayOfWeekCounts[dayIndex];
                    sumOfDayOfWeekAverages += dayOfWeekAverage;
                }
                dayIndex++;
            }
            for (int dayOfWeekIdx = 6; dayOfWeekIdx < 13; dayOfWeekIdx++)
            {
                int dayOfWeek = dayOfWeekIdx % 7;
                double fraction;
                if (dayOfWeekCounts[dayOfWeek] > 0)
                {
                    dayOfWeekAverage = dayOfWeekTotals[dayOfWeek] / dayOfWeekCounts[dayOfWeek];
                    fraction = (double)(dayOfWeekAverage / sumOfDayOfWeekAverages);
                }
                else
                {
                    dayOfWeekAverage = 0.0M;
                    fraction = 0;
                }
                result.AppendLine((DayOfWeek)dayOfWeek + "   " + (fraction * 100.0).ToString("00.0") + "%   " +
                    dayOfWeekTotals[dayOfWeek].ToString("C2") + "/" +
                    dayOfWeekCounts[dayOfWeek].ToString() + " = " +
                    dayOfWeekAverage.ToString("C2"));
            }
            txtWeekDayPercents.Text = result.ToString();
        }
    }

    internal class SheetCell
    {
        public string Value;

        public SheetCell(string value)
        {
            Value = value;
        }
    }

    internal class SheetRow
    {
        public List<SheetCell> Cells = new List<SheetCell>();

        public SheetRow(XmlElement rowElement, XmlNamespaceManager nsMgr, int maxWidth)
        {
            // <table:table-cell><text:p>sdfasdf</text:p></table:table-cell>
            // <table:covered-table-cell />
            // table:number-columns-repeated=n
            XmlNodeList cells = rowElement.SelectNodes("table:table-cell|table:covered-table-cell", nsMgr);
            foreach (XmlNode cellNode in cells)
            {
                XmlElement cellElement = (XmlElement)cellNode;
                string repeatCount = cellElement.GetAttribute("table:number-columns-repeated");
                if (string.IsNullOrEmpty(repeatCount))
                    repeatCount = "1";
                int repeatCount2 = int.Parse(repeatCount);
                for (int cellCount = 1; cellCount <= repeatCount2; cellCount++)
                {
                    XmlElement contentElm = (XmlElement)cellElement.SelectSingleNode("text:p", nsMgr);
                    string content = string.Empty;
                    if (contentElm != null)
                        content = contentElm.InnerText;
                    SheetCell cell = new SheetCell(content);
                    Cells.Add(cell);
                    if (Cells.Count >= maxWidth)
                        break;
                }
            }
        }
    }

    internal class Sheet
    {
        public string Name;
        public List<SheetRow> Rows = new List<SheetRow>();
        private Encoding asciiEncoding;
        private byte letterACode;

        public Sheet(XmlElement spreadsheet, string sheetName, XmlNamespaceManager nsMgr, int maxWidth)
        {
            asciiEncoding = Encoding.ASCII;
            letterACode = asciiEncoding.GetBytes("A")[0];
            XmlElement sheetElm = (XmlElement)spreadsheet.SelectSingleNode(
                "table:table[@table:name='" + sheetName + "']", nsMgr);
            if (sheetElm == null)
                throw new ArgumentException("No sheet named " + sheetName);
            Name = sheetName;
            XmlNodeList rows = sheetElm.SelectNodes("table:table-row", nsMgr);
            foreach (XmlNode rowNode in rows)
            {
                XmlElement rowElement = (XmlElement)rowNode;
                SheetRow row = new SheetRow(rowElement, nsMgr, maxWidth);
                Rows.Add(row);
            }
        }

        public string GetValue(string columnLetter, int rowNumber)
        {
            int columnNumber = TranslateColumnLetter(columnLetter);
            return GetValue(columnNumber, rowNumber);
        }

        public string GetValue(int columnNumber, int rowNumber)
        {
            if (rowNumber > Rows.Count)
                return string.Empty;
            SheetRow row = Rows[rowNumber - 1];
            if (columnNumber > row.Cells.Count)
                return string.Empty;
            return row.Cells[columnNumber - 1].Value;
        }

        public int TranslateColumnLetter(string columnLetter)
        {
            return asciiEncoding.GetBytes(columnLetter.ToUpper())[0] - letterACode + 1;
        }

        public int CellCount
        {
            get
            {
                int cellCount = 0;
                foreach (SheetRow row in Rows)
                {
                    cellCount += row.Cells.Count;
                }
                return cellCount;
            }
        }
    }

    internal class WeekAccumulator
    {
        public readonly DateTime _WeekStartDate;
        private decimal _WeekTotal;
        private int _DayCount;

        public WeekAccumulator(DateTime weekStartDate)
        {
            _WeekStartDate = weekStartDate;
            _WeekTotal = 0.0M;
            _DayCount = 0;
        }

        public DateTime WeekStartDate
        {
            get { return _WeekStartDate; }
        }

        public decimal WeekTotal
        {
            get { return _WeekTotal; }
        }

        public int DayCount
        {
            get { return _DayCount; }
        }

        public void Add(decimal dayNet)
        {
            _WeekTotal += dayNet;
            _DayCount++;
        }
    }

    internal class HourlyRegister
    {
        public int[] HourCounts;
        public int DataCount;

        public HourlyRegister()
        {
            HourCounts = new int[15];
            DataCount = 0;
        }

        public void Add(HourlyRegister register)
        {
            for (int i = 0; i < HourCounts.Length; i++)
            {
                HourCounts[i] += register.HourCounts[i];
            }
            DataCount += register.DataCount;
        }
    }

    internal class HourlyDay
    {
        public HourlyRegister[] Registers;

        public HourlyDay()
        {
            Registers = new HourlyRegister[6];
            for (int i = 0; i < Registers.Length; i++)
            {
                Registers[i] = new HourlyRegister();
            }
        }

        public void Add(HourlyDay day)
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                Registers[i].Add(day.Registers[i]);
            }
        }
    }

    internal class HourlyTotals
    {
        public HourlyDay Weekend = new HourlyDay();
        public HourlyDay Saturday = new HourlyDay();
        public HourlyDay Sunday = new HourlyDay();
        public HourlyDay Weekday = new HourlyDay();
    }
}
