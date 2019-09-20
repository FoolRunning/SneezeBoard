using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SneezeBoardClient.Properties;
using SneezeBoardCommon;

namespace SneezeBoardClient
{
    public partial class StatsForm : Form
    {
        private const int headingSpacing = 5;
        private const int startXMargin = 10;
        private const int statDisplaySpacing = 20;

        private readonly Dictionary<Guid, int> streakMap;
        private readonly Dictionary<Guid, UserStats> userStats;
        private readonly Dictionary<int, List<Guid>> streakCountToUserMap;
        private readonly Font font;
        private readonly Font headingFont;
        private readonly Font subheadingFont;

        public StatsForm()
        {
            InitializeComponent();

            lbl_stats_display.Size = new Size(panel1.Width, 1200);

            SneezeDatabase database = SneezeClientListener.Database;
            streakMap = database.FindLongestStreaks();
            streakCountToUserMap = database.FindAllStreaks();
            userStats = database.FindUserStats();
            font = lbl_stats_display.Font;
            headingFont = new Font(lbl_stats_display.Font.FontFamily, lbl_stats_display.Font.Size * 1.5f, FontStyle.Bold);
            subheadingFont = new Font(lbl_stats_display.Font, FontStyle.Bold);
        }

        private void lbl_stats_display_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Settings.Default.BoardBackColor);

            int y = 10;
            DrawLongestStreaks(g, ref y);
            DrawStreakFames(g, ref y);
            DrawRandomStats(g, ref y);
            DrawContributions(g, ref y);
        }

        private void DrawLongestStreaks(Graphics g, ref int y)
        {
            Dictionary<Guid, UserInfo> userMap = SneezeClientListener.Database.IdToUser;
            TextRenderer.DrawText(g, "Longest Streaks", headingFont, new Point(startXMargin, y), Color.Black);
            y += headingFont.Height + headingSpacing;
            foreach (KeyValuePair<Guid, int> streak in streakMap.OrderBy(st => -st.Value))
            {
                UserInfo user = userMap[streak.Key];
                TextRenderer.DrawText(g, user.Name, font, new Point(startXMargin, y), user.Color);
                TextRenderer.DrawText(g, streak.Value.ToString(), font, new Point(startXMargin + 90, y), user.Color);
                y += font.Height;
            }

            y += statDisplaySpacing;
        }

        private void DrawStreakFames(Graphics g, ref int y)
        {
            const int gridSize = 15;
            const int graphRightBounds = 350;
            const int barWidth = 10;
            const int barSpacing = 10;

            Dictionary<Guid, UserInfo> userMap = SneezeClientListener.Database.IdToUser;

            TextRenderer.DrawText(g, "Streak Hall of Fame", headingFont, new Point(startXMargin, y + headingSpacing), Color.Black);
            y += headingFont.Height + headingSpacing;
            Pen gridPen = new Pen(Color.Black);
            List<KeyValuePair<int, List<Guid>>> streakCounts = streakCountToUserMap.OrderBy(st => -st.Key).ToList();
            int maxStreak = streakCounts[0].Key;
            for (int s = maxStreak; s >= 0; s--)
            {
                TextRenderer.DrawText(g, (maxStreak - s).ToString(), font, new Point(startXMargin, y + s * gridSize), Color.Black);
                g.DrawLine(gridPen, startXMargin + 25, y + s * gridSize + gridSize / 2, graphRightBounds, y + s * gridSize + gridSize / 2);
            }

            int x = startXMargin + 30;
            y += gridSize / 2;
            foreach (KeyValuePair<int, List<Guid>> streakCountToUsers in streakCountToUserMap.OrderBy(st => -st.Key))
            {
                List<Guid> users = streakCountToUsers.Value;
                for (int i = 0; i < users.Count; i++)
                {
                    UserInfo user = userMap[users[i]];
                    SolidBrush brush = new SolidBrush(user.Color);
                    g.FillRectangle(brush, x, y + (maxStreak - streakCountToUsers.Key) * gridSize, barWidth, streakCountToUsers.Key * gridSize);
                    x += barWidth + barSpacing;
                    if (x + barWidth >= graphRightBounds)
                        break;
                }

                if (x + barWidth >= graphRightBounds)
                    break;
            }

            y += maxStreak * gridSize + statDisplaySpacing;
        }

        private void DrawRandomStats(Graphics g, ref int y)
        {
            SneezeDatabase database = SneezeClientListener.Database;
            Dictionary<Guid, UserInfo> userMap = database.IdToUser;
            const int columnSpacing = 110;
            y += font.Height + headingSpacing;
            TextRenderer.DrawText(g, "Random Stats", headingFont, new Point(startXMargin, y), Color.Black);
            y += headingFont.Height + headingSpacing;

            TextRenderer.DrawText(g, "User name", subheadingFont, new Point(startXMargin, y), Color.Black);
            TextRenderer.DrawText(g, "Total sneezes", subheadingFont, new Point(startXMargin + columnSpacing, y), Color.Black);
            TextRenderer.DrawText(g, "Participation days", subheadingFont, new Point(startXMargin + columnSpacing * 2, y), Color.Black);
            TextRenderer.DrawText(g, "Avg. days btwn sneezes", subheadingFont, new Point(startXMargin + columnSpacing * 3, y), Color.Black);
            y += subheadingFont.Height;
            foreach (KeyValuePair<Guid, UserStats> stats in userStats.OrderBy(u => -u.Value.TotalSneezes))
            {
                if (stats.Key == CommonInfo.UnknownUserId)
                    continue;

                UserInfo user = userMap[stats.Key];
                TextRenderer.DrawText(g, user.Name, font, new Point(startXMargin, y), user.Color);

                TextRenderer.DrawText(g, stats.Value.TotalSneezes.ToString(), font, new Point(startXMargin + columnSpacing, y), user.Color);
                double sneezePercentage = stats.Value.TotalSneezes * 100.0 / database.Sneezes.Count;
                TextRenderer.DrawText(g, $"({sneezePercentage:F2}%)", font, new Point(startXMargin + columnSpacing + 40, y), user.Color);

                double daysAsSneezer = (stats.Value.LastSneezeDate - stats.Value.FirstSneezeDate).TotalDays;
                TextRenderer.DrawText(g, daysAsSneezer.ToString("F0"), font, new Point(startXMargin + columnSpacing * 2, y), user.Color);
                
                double daysPerSneeze = daysAsSneezer / stats.Value.TotalSneezes;
                TextRenderer.DrawText(g, daysPerSneeze.ToString("F1"), font, new Point(startXMargin + columnSpacing * 3, y), user.Color);

                y += font.Height;
            }

            y += statDisplaySpacing;
        }

        private void DrawContributions(Graphics g, ref int y)
        {
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;

            TextRenderer.DrawText(g, "Sneeze contributions", headingFont, new Point(startXMargin, y), Color.Black);
            y += headingFont.Height + headingSpacing;
            SneezeDatabase database = SneezeClientListener.Database;
            Dictionary<Guid, UserInfo> userMap = database.IdToUser;
            float totalAngle = 0.0f;
            float totalSneezes = database.Sneezes.Count - userStats[CommonInfo.UnknownUserId].TotalSneezes;
            SolidBrush background = new SolidBrush(Color.Black);
            g.FillEllipse(background, startXMargin, y, 500, 500);
            foreach (KeyValuePair<Guid, UserStats> stats in userStats.OrderBy(u => -u.Value.TotalSneezes))
            {
                if (stats.Key == CommonInfo.UnknownUserId)
                    continue;

                UserInfo user = userMap[stats.Key];
                float sneezePercentage = stats.Value.TotalSneezes / totalSneezes;
                float angleForUser = 360.0f * sneezePercentage;
                SolidBrush brush = new SolidBrush(user.Color);
                g.FillPie(brush, startXMargin, y, 500, 500, totalAngle, angleForUser);
                totalAngle += angleForUser;
            }
        }
    }
}
