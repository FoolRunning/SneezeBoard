using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SneezeBoardClient.Properties;
using SneezeBoardCommon;

namespace SneezeBoardClient
{
    public partial class SneezeBoardForm : Form
    {
        private const int numberPadding = 10;

        private readonly Regex ipAddressRegex = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$", RegexOptions.Compiled);

        private DatabaseErrorType dbError = DatabaseErrorType.None;
        private bool failedToConnect;
        private bool connectionOpening;
        private bool connectionClosed;
        private SneezeRecord lastHoveredSneeze;

        #region Constructor
        public SneezeBoardForm()
        {
            InitializeComponent();

            CalculateSneezeScrollBar();
            SneezeClientListener.FailedToConnect += SneezeClientListener_FailedToConnect;
            SneezeClientListener.GotDatabase += SneezeClientListener_DatabaseUpdated;
            SneezeClientListener.DatabaseUpdated += SneezeClientListener_DatabaseUpdated;
            SneezeClientListener.PersonSneezed += SneezeClientListener_PersonSneezed;
            SneezeClientListener.ConnectionClosed += SneezeClientListener_ConnectionClosed;
            SneezeClientListener.ConnectionOpened += SneezeClientListener_ConnectionOpened;
            SneezeClientListener.UserUpdated += SneezeClientListener_UserUpdated;
            SneezeClientListener.SneezeUpdated += SneezeClientListener_SneezeUpdated;
            SneezeClientListener.SneezeRemoved += SneezeClientListener_SneezeRemoved;
            SneezeClientListener.DatabaseError += SneezeClientListener_DatabaseError;

            txtbx_ip.Text = Settings.Default.ServerIP;
            lbl_apocalypse.Text = "";
            UpdateUIState();
            //if (Environment.OSVersion.Platform == PlatformID.Unix)
            //{
            //    try
            //    {
            //        notifyIcon.Dispose();
            //        components.Remove(notifyIcon);
            //        notifyIcon = null;
            //    }
            //    catch
            //    {
            //        // This can crash on Linux
            //    }
            //}
        }
        #endregion

        #region Properties
        private UserInfo CurrentUser
        {
            get { return cmb_sneezers.SelectedItem as UserInfo; }
        }
        #endregion

        #region Event handlers
        private void SneezeClientListener_ConnectionClosed()
        {
            connectionClosed = true;
            lbl_sneeze_display.Invalidate();
        }

        private void SneezeClientListener_ConnectionOpened()
        {
	        connectionOpening = false;
        }

        private void SneezeClientListener_FailedToConnect()
        {
            failedToConnect = true;
            lbl_sneeze_display.Invalidate();
        }

        private void SneezeClientListener_DatabaseError(DatabaseErrorType errorType)
        {
            dbError = errorType;
            lbl_sneeze_display.Invalidate();
        }

        private void SneezeClientListener_PersonSneezed(SneezeRecord sneeze)
        {
            BeginInvoke(new Action(() =>
            {
                lbl_sneeze_display.Invalidate();
                if (sneeze.UserId != CurrentUser?.UserGuid)
                {
                    string userName = SneezeClientListener.Database.IdToUser[sneeze.UserId].Name;
                    notifyIcon?.ShowBalloonTip(2000, "Sneeze Countdown", $"{userName} sneezed!", ToolTipIcon.None);
                }
                CalculateApocalypse();
            }));
        }

        private void SneezeClientListener_UserUpdated()
        {
            BeginInvoke(new Action(() =>
            {
                lbl_sneeze_display.Invalidate();
                UpdateSneezers();
                UpdateUIState();
            }));
        }

        private void SneezeClientListener_SneezeUpdated()
        {
            BeginInvoke(new Action(() =>
            {
                lbl_sneeze_display.Invalidate();
            }));
        }

        private void SneezeClientListener_SneezeRemoved()
        {
            BeginInvoke(new Action(() =>
            {
                lbl_sneeze_display.Invalidate();
            }));
        }

        private void SneezeClientListener_DatabaseUpdated()
        {
            BeginInvoke(new Action(() =>
            {
                SneezeDatabase database = SneezeClientListener.Database;
                if (database == null)
                    return; // Could have been set to null after the BeginInvoke was called

                lbl_sneeze_display.Invalidate();
                CalculateSneezeScrollBar();
                UpdateUIState();
                UpdateSneezers();

                cmb_sneezers.SelectedItem = database.IdToUser.Values.FirstOrDefault(u => u.UserGuid == Settings.Default.LastSneezer);
                if (database.Sneezes.Count > 0)
                {
                    Point sneezeLoc = GetSneezeLocation(SneezeClientListener.Database.Sneezes.Count - 1);
                    scroll_horizontal.Value = Math.Max(0, sneezeLoc.X - scroll_horizontal.Width / 2);
                }

                CalculateApocalypse();
            }));
        }

        private void SneezeBoardForm_SizeChanged(object sender, EventArgs e)
        {
            CalculateSneezeScrollBar();
        }

        private void SneezeBoardForm_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  

            if (WindowState == FormWindowState.Minimized && Environment.OSVersion.Platform != PlatformID.Unix)
            {
                Hide();
            }
        }

        private void SneezeBoardForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SneezeClientListener.ShutDown();
        }

        private void lbl_sneeze_display_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Settings.Default.BoardBackColor);
            Font font = lbl_sneeze_display.Font;
            SneezeDatabase database = SneezeClientListener.Database;
            if (database == null)
            {
                if (failedToConnect)
                    DrawTextCentered("Failed to connect to server at specified IP.", g, font, Color.Red);
                else if (dbError != DatabaseErrorType.None)
                {
                    if (dbError == DatabaseErrorType.VersionNumberConflict)
                        DrawTextCentered("Database version number does not match.\nPlease update your client to the latest version.", g, font, Color.Red);
                }
                else if (connectionClosed)
                    DrawTextCentered("Connection to server was lost.", g, font, Color.Red);
                else if (connectionOpening)
                    DrawTextCentered("Connecting...", g, font, Color.Blue);
                else
                    DrawTextCentered("Not connected to server.", g, font, Color.Black);
                return;
            }

            if (database.Sneezes.Count == 0)
            {
                DrawTextCentered("No recorded sneezes.", g, font, Color.Black);
                return;
            }

            int viewLeftEdge = scroll_horizontal.Value;
            int maxSneezesPerColumn = lbl_sneeze_display.Height / font.Height;
            string maxWidthNumber = new string('9', database.CountdownStart.ToString().Length);
            Size numberSize = TextRenderer.MeasureText(g, maxWidthNumber, font);
            int startingIndex = Math.Min(viewLeftEdge / (numberSize.Width + numberPadding) * maxSneezesPerColumn, database.Sneezes.Count - 1);
            int endingIndex = Math.Min((viewLeftEdge + lbl_sneeze_display.Width + numberSize.Width) / (numberSize.Width + numberPadding) * maxSneezesPerColumn, database.Sneezes.Count - 1);
            for (int i = startingIndex; i <= endingIndex; i++)
            {
                int sneezeNum = database.CountdownStart - i;
                //DO NOT DELETE! May need this later.
                //if (sneezeNum < 26359)
                //    sneezeNum -= 5;
                //if (sneezeNum < 24423)
                //    sneezeNum += 2;
                int columnNum = i / maxSneezesPerColumn;
                int rowNum = i % maxSneezesPerColumn;
                Point textLoc = new Point(columnNum * (numberSize.Width + numberPadding) - viewLeftEdge, rowNum * font.Height);

                SneezeRecord sneeze = database.Sneezes[i];
                if (!string.IsNullOrEmpty(sneeze.Comment))
                {
                    using (Pen p = new Pen(Color.Goldenrod))
                        g.DrawRectangle(p, textLoc.X, textLoc.Y, numberSize.Width, numberSize.Height);
                }

                UserInfo userInfo;
                Color sneezeColor = Color.Black;
                if (database.IdToUser.TryGetValue(sneeze.UserId, out userInfo))
                    sneezeColor = userInfo.Color;

                TextRenderer.DrawText(g, sneezeNum.ToString(), font, textLoc, sneezeColor);
            }
        }

        private void lbl_sneeze_display_MouseMove(object sender, MouseEventArgs e)
        {
            int sneezeIndex;
            SneezeRecord record = GetSneezeAtLocation(lbl_sneeze_display.PointToClient(MousePosition), out sneezeIndex);
            if (record == null)
            {
                toolTip.ToolTipTitle = "";
                toolTip.SetToolTip(lbl_sneeze_display, null);
                lastHoveredSneeze = null;
                return;
            }

            if (record == lastHoveredSneeze)
                return;
            lastHoveredSneeze = record;

            SneezeDatabase database = SneezeClientListener.Database;
            string toolTipText = $"Username: {database.IdToUser[record.UserId].Name}\n" +
                                 $"Date: {record.Date.ToLocalTime():g}";

            if (!String.IsNullOrEmpty(record.Comment))
                toolTipText += "\n\n" + record.Comment;

            toolTip.ToolTipTitle = "Sneeze " + (database.CountdownStart - sneezeIndex);
            toolTip.SetToolTip(lbl_sneeze_display, toolTipText);
        }
        
        private void lbl_sneeze_display_MouseLeave(object sender, EventArgs e)
        {
            toolTip.ToolTipTitle = "";
            lastHoveredSneeze = null;
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
	        connectionOpening = true;
	        lbl_sneeze_display.Invalidate();
            Settings.Default.ServerIP = txtbx_ip.Text;
            Settings.Default.Save();

            failedToConnect = false;
            connectionClosed = false;
            dbError = DatabaseErrorType.None;

            SneezeClientListener.StartListener(txtbx_ip.Text);
        }

        private void cmb_sneezers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_sneezers.SelectedIndex == 0)
            {
                AddUserForm addUserDialog = new AddUserForm();
                if (addUserDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (UserInfo user in SneezeClientListener.Database.IdToUser.Values)
                    {
                        if (user.Name == addUserDialog.UserName)
                        {
                            MessageBox.Show(this,
                                "Sorry, that Username is already taken. Please select another Username.",
                                "Username Taken",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    UserInfo recruit = new UserInfo(addUserDialog.UserName, Guid.NewGuid(), GetRandomColor());
                    cmb_sneezers.Items.Add(recruit);
                    SneezeClientListener.AddUser(recruit);
                    cmb_sneezers.SelectedItem = recruit;
                }
            }

            if (CurrentUser != null)
            {
                lbl_display_color.BackColor = CurrentUser.Color;
                Settings.Default.LastSneezer = CurrentUser.UserGuid;
                Settings.Default.Save();
            }

            UpdateUIState();
        }

        private void btn_change_color_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AnyColor = true;
            if (colorDlg.ShowDialog(this) == DialogResult.OK)
            {
                if (ColorTaken(colorDlg.Color, CurrentUser))
                {
                    MessageBox.Show(this, "Sorry, that color is already taken. Please select another color.", "Color Taken",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                CurrentUser.Color = colorDlg.Color;
                lbl_display_color.BackColor = CurrentUser.Color;
                SneezeClientListener.UpdateUser(CurrentUser);
                lbl_sneeze_display.Invalidate();
            }
        }
        private void btn_add_sneeze_Click(object sender, EventArgs e)
        {
            string message = GetLongestStreak(); // Calculate this first

            SneezeDatabase database = SneezeClientListener.Database;
            int sneezeNum = database.CountdownStart - database.Sneezes.Count;
            DateTime sneezeDate = CommonInfo.GetDateOfSneeze(sneezeNum);
            SneezeClientListener.Sneeze(new SneezeRecord(CurrentUser.UserGuid, sneezeDate, txtbx_commentary.Text));
            txtbx_commentary.Clear();

            if (!String.IsNullOrEmpty(message))
                MessageBox.Show(message, "Sneeze Streak Status");
        }

        private void btn_stats_Click(object sender, EventArgs e)
        {
            using (StatsForm form = new StatsForm())
                form.ShowDialog(this);
        }

        private void btn_settings_Click(object sender, EventArgs e)
        {
            using (SettingsForm form = new SettingsForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    lbl_sneeze_display.Invalidate();
                    CalculateApocalypse();
                }
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void mnu_Restore_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void mnu_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void scroll_horizontal_Scroll(object sender, ScrollEventArgs e)
        {
            lbl_sneeze_display.Invalidate();
        }

        private void txtbx_ip_TextChanged(object sender, EventArgs e)
        {
            btn_connect.Enabled = ipAddressRegex.IsMatch(txtbx_ip.Text);
        }

        private void lbl_sneeze_display_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
                return;
            int sneezeIndex;
            SneezeRecord sneeze = GetSneezeAtLocation(lbl_sneeze_display.PointToClient(MousePosition), out sneezeIndex);
            if (sneeze == null)
                return;

            if (sneeze.UserId != CurrentUser?.UserGuid)
                return;

            ContextMenuStrip strip = new ContextMenuStrip();
            strip.Items.Add("Edit sneeze...", null, (s, a) =>
            {
                using (SneezeCommentEditorForm form = new SneezeCommentEditorForm(sneeze.Comment))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        sneeze.Comment = form.UpdatedText;
                        SneezeClientListener.UpdateSneeze(sneeze);
                    }
                }
            });
            strip.Items.Add("Remove sneeze...", null, (s, a) =>
            {
                if (DialogResult.Yes == MessageBox.Show(this,
                                "Are you sure you want to delete this sneeze?",
                                "Delete Sneeze",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    SneezeClientListener.RemoveSneeze(sneeze);
                }
            });
            strip.Show(MousePosition);
        }

        private void tmr_updateApocalypse_Tick(object sender, EventArgs e)
        {
            CalculateApocalypse();
        }
        #endregion

        #region Private helper methods
        private void UpdateUIState()
        {
            SneezeDatabase database = SneezeClientListener.Database;
            bool hasDatabase = database != null;
            bool hasUser = CurrentUser != null;
            cmb_sneezers.Enabled = hasDatabase;
            btn_add_sneeze.Enabled = hasDatabase && hasUser && database.Sneezes.Count < database.CountdownStart;
            btn_change_color.Enabled = hasDatabase && hasUser;
            txtbx_commentary.Enabled = hasDatabase && hasUser;
            btn_stats.Enabled = hasDatabase;
            lbl_we_win.Visible = hasDatabase && database.Sneezes.Count >= database.CountdownStart;
        }

        private void UpdateSneezers()
        {
            UserInfo currentUser = CurrentUser;

            cmb_sneezers.BeginUpdate();

            cmb_sneezers.Items.Clear();
            cmb_sneezers.Items.Add("New...");
            foreach (UserInfo info in SneezeClientListener.Database.IdToUser.Values.OrderBy(info => info.Name))
                cmb_sneezers.Items.Add(info);

            cmb_sneezers.EndUpdate();

            cmb_sneezers.SelectedItem = currentUser;
        }

        private Point GetSneezeLocation(int index)
        {
            Font font = lbl_sneeze_display.Font;
            string maxWidthNumber = new string('9', SneezeClientListener.Database.CountdownStart.ToString().Length);
            Size numberSize;
            using (Graphics g = CreateGraphics())
                numberSize = TextRenderer.MeasureText(g, maxWidthNumber, font);

            int maxSneezesPerColumn = lbl_sneeze_display.Height / font.Height;
            if(maxSneezesPerColumn <= 0)
                return Point.Empty;
            int columnNum = index / maxSneezesPerColumn;
            int rowNum = index % maxSneezesPerColumn;
            return new Point(columnNum * (numberSize.Width + numberPadding), rowNum * font.Height);
        }

        private SneezeRecord GetSneezeAtLocation(Point mousePosition, out int index)
        {
            Font font = lbl_sneeze_display.Font;
            SneezeDatabase database = SneezeClientListener.Database;
            index = -1;
            if (database == null)
                return null;

            int viewLeftEdge = scroll_horizontal.Value;
            int maxSneezesPerColumn = lbl_sneeze_display.Height / font.Height;
            string maxWidthNumber = new string('9', database.CountdownStart.ToString().Length);
            Size numberSize;
            using (Graphics g = CreateGraphics())
                numberSize = TextRenderer.MeasureText(g, maxWidthNumber, font);
            int startingIndex = Math.Min(viewLeftEdge / (numberSize.Width + numberPadding) * maxSneezesPerColumn, database.Sneezes.Count - 1);
            int endingIndex = Math.Min((viewLeftEdge + lbl_sneeze_display.Width + numberSize.Width) / (numberSize.Width + numberPadding) * maxSneezesPerColumn, database.Sneezes.Count - 1);
            for (int i = startingIndex; i <= endingIndex; i++)
            {
                int columnNum = i / maxSneezesPerColumn;
                int rowNum = i % maxSneezesPerColumn;
                Point textLoc = new Point(columnNum * (numberSize.Width + numberPadding) - viewLeftEdge, rowNum * font.Height);
                Rectangle rect = new Rectangle(textLoc, numberSize);
                if (rect.Contains(mousePosition))
                {
                    index = i;
                    return database.Sneezes[i];
                }
            }
            return null;
        }

        private void DrawTextCentered(string text, Graphics g, Font font, Color color)
        {
            Size textSize = TextRenderer.MeasureText(g, text, font);
            Point centered = new Point((pnl_scroller.Width - textSize.Width) / 2, (pnl_scroller.Height - textSize.Height) / 2);
            TextRenderer.DrawText(g, text, font, centered, color);
        }

        private void CalculateSneezeScrollBar()
        {
            SneezeDatabase database = SneezeClientListener.Database;
            int countdownStart = database?.CountdownStart ?? 100;
            Font font = lbl_sneeze_display.Font;
            if (lbl_sneeze_display.Height == 0 || lbl_sneeze_display.Width == 0)
                return;

            int maxSneezesPerColumn = lbl_sneeze_display.Height / font.Height;
            string maxWidthNumber = new string('9', countdownStart.ToString().Length);
            using (Graphics g = CreateGraphics())
            {
                Size numberSize = TextRenderer.MeasureText(g, maxWidthNumber, font);
                scroll_horizontal.Maximum = (int)Math.Ceiling((countdownStart / (float)maxSneezesPerColumn + 1) * (numberSize.Width + numberPadding) - lbl_sneeze_display.Width + numberSize.Width);
                scroll_horizontal.Minimum = 0;
            }
        }

        private bool ColorTaken(Color desired, UserInfo currentUser)
        {
            foreach (UserInfo user in SneezeClientListener.Database.IdToUser.Values)
            {
                if (user.Color == desired && user.UserGuid != currentUser?.UserGuid)
                    return true;
            }

            return false;
        }

        private Color GetRandomColor()
        {
            Random r = new Random();
            Color c;
            do
            {
                c = Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
            }
            while (ColorTaken(c, null));
            return c;
        }

        private string GetLongestStreak()
        {
            SneezeDatabase database = SneezeClientListener.Database;
            if (database.Sneezes.Count == 0 || CurrentUser.UserGuid == CommonInfo.UnknownUserId)
                return "";

            Dictionary<Guid, int> streaks = database.FindLongestStreaks();
            int longestStreak = 0;
            Guid streakWinnerId = database.Sneezes[0].UserId; //The first person to reach the longest streak is the winner if there is a tie
            foreach (KeyValuePair<Guid, int> streak in streaks)
            {
                if (streak.Value > longestStreak)
                {
                    longestStreak = streak.Value;
                    streakWinnerId = streak.Key;
                }
            }

            int currentStreak = 1;
            int index = 1;
            while (index <= database.Sneezes.Count && database.Sneezes[database.Sneezes.Count - index].UserId == CurrentUser.UserGuid)
            {
                index++;
                currentStreak++;
            }

            int sneezesToVictory = longestStreak + 1 - currentStreak; //I definitely need to calculate the current streak rather than using 1
            string retStr = "";


            if(currentStreak == 3 && CurrentUser.UserGuid != streakWinnerId && sneezesToVictory > 1)
                retStr = String.Format("The person with the longest streak is {0} with {1} sneezes.\nYou need to sneeze {2} times in order to beat {0}.", database.IdToUser[streakWinnerId].Name, longestStreak, sneezesToVictory);
            else if(sneezesToVictory == 1)
                retStr = String.Format("Way to go!\nYou are only ONE sneeze away from breaking the sneeze streak record which is currently held by {0}.", database.IdToUser[streakWinnerId].Name);
            else if (sneezesToVictory == 0)
            {
                if (CurrentUser.UserGuid != streakWinnerId)
                {
                    retStr = String.Format("!!! Congratulations {0} !!! \nYou set a new record for the longest sneeze streak!\nYour {1} sneeze streak beat {2}'s {3} sneeze streak.", CurrentUser.Name, currentStreak, SneezeClientListener.Database.IdToUser[streakWinnerId].Name, longestStreak);
                    if (!String.IsNullOrEmpty(txtbx_commentary.Text))
                        txtbx_commentary.Text += "\n";
                    txtbx_commentary.Text += String.Format("{0} beat {1}'s sneeze streak record.", CurrentUser.Name, database.IdToUser[streakWinnerId].Name);
                }
                else
                {
                    retStr = String.Format("!!! Congratulations {0} !!! \nYou have become a sneezing legend!\nYou have increased your lead and set a new sneeze streak record!", CurrentUser.Name);
                    if (!String.IsNullOrEmpty(txtbx_commentary.Text))
                        txtbx_commentary.Text += "\n";
                    txtbx_commentary.Text += "The legend continues!";
                }
            }
            return retStr;
        }

        private void CalculateApocalypse()
        {
            SneezeDatabase database = SneezeClientListener.Database;
            if (database == null)
            {
                lbl_apocalypse.Text = "";
                return;
            }

            DateTime startingDate;
            switch (Settings.Default.DateRange)
            {
                case DateRangeType.OneWeek: startingDate = DateTime.Now.AddDays(-7); break;
                case DateRangeType.TwoWeeks: startingDate = DateTime.Now.AddDays(-14); break;
                case DateRangeType.OneMonth: startingDate = DateTime.Now.AddMonths(-1); break;
                case DateRangeType.ThreeMonths: startingDate = DateTime.Now.AddMonths(-3); break;
                case DateRangeType.SixMonths: startingDate = DateTime.Now.AddMonths(-6); break;
                case DateRangeType.Year: startingDate = DateTime.Now.AddYears(-1); break;
                default:
                case DateRangeType.AllTime: startingDate = DateTime.MinValue; break;
            }

            int countOfSneezesInDateRange = 0;
            DateTime dateOfFirstSneezeInRange = DateTime.MinValue;
            for (int i = database.Sneezes.Count - 1; i >= 0; i--)
            {
                if (database.Sneezes[i].Date < startingDate)
                    break;
                countOfSneezesInDateRange++;
                dateOfFirstSneezeInRange = database.Sneezes[i].Date;
            }

            if (dateOfFirstSneezeInRange == DateTime.MinValue)
            {
                lbl_apocalypse.Text = "No sneezes in range";
                return;
            }

            TimeSpan timeRange = DateTime.Now - dateOfFirstSneezeInRange;
            double millisecondsBetweenSneezes = timeRange.TotalMilliseconds / countOfSneezesInDateRange;
            double millisecondsUntilComplete = (database.CountdownStart - database.Sneezes.Count) * millisecondsBetweenSneezes;
            DateTime apocalypseDate = DateTime.Now + TimeSpan.FromMilliseconds(millisecondsUntilComplete);
            lbl_apocalypse.Text = apocalypseDate.ToString("g");
        }
        #endregion
    }
}
