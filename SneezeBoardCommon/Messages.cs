using System;

namespace SneezeBoardCommon
{
    public static class Messages
    {
        public const string DatabaseObject = "Database object";
        public const string Sneeze = "Sneeze";
        public const string AddUser = "AddUser";
        public const string DatabaseRequested = "DatabaseRequested";
        public const string UpdateUser = "UpdateUser";
        public const string UpdateSneeze = "UpdateSneeze";
        public const string PersonSneezed = "PersonSneezed";
    }

    public static class CommonInfo
    {
        public const int ServerPort = 57632;
        public static readonly Guid UnknownUserId = new Guid("944616BD-20A1-4659-87AF-04563043FFDE");

        public static DateTime GetDateOfSneeze(int sneezeNum)
        {
            DateTime sneezeDate = DateTime.Now;
            if (sneezeNum <= 27002 && sneezeNum >= 26007) // First 1000
            {
                DateTime startDate = new DateTime(2014, 11, 15, 12, 0, 0);
                DateTime endDate = new DateTime(2016, 2, 8, 12, 0, 0);
                TimeSpan timeFor1k = endDate - startDate;
                TimeSpan timeBetweenSneezes = TimeSpan.FromSeconds(timeFor1k.TotalSeconds / 995);
                sneezeDate = startDate + TimeSpan.FromSeconds(timeBetweenSneezes.TotalSeconds * (27002 - sneezeNum));
            }
            if (sneezeNum < 26007 && sneezeNum >= 25424) // To FY 2016
            {
                DateTime startDate = new DateTime(2016, 2, 8, 12, 0, 0);
                DateTime endDate = new DateTime(2016, 9, 30, 12, 0, 0);
                TimeSpan timeFor1k = endDate - startDate;
                TimeSpan timeBetweenSneezes = TimeSpan.FromSeconds(timeFor1k.TotalSeconds / 582);
                sneezeDate = startDate + TimeSpan.FromSeconds(timeBetweenSneezes.TotalSeconds * (26001 - sneezeNum));
            }
            if (sneezeNum < 25424 && sneezeNum >= 25007) // End of 2nd 1000
            {
                DateTime startDate = new DateTime(2016, 9, 30, 12, 0, 0);
                DateTime endDate = new DateTime(2017, 4, 25, 12, 0, 0);
                TimeSpan timeFor1k = endDate - startDate;
                TimeSpan timeBetweenSneezes = TimeSpan.FromSeconds(timeFor1k.TotalSeconds / 416);
                sneezeDate = startDate + TimeSpan.FromSeconds(timeBetweenSneezes.TotalSeconds * (25423 - sneezeNum));
            }
            if (sneezeNum < 25007 && sneezeNum >= 24585) // To start of 2018
            {
                DateTime startDate = new DateTime(2017, 4, 25, 12, 0, 0);
                DateTime endDate = new DateTime(2018, 1, 1, 12, 0, 0);
                TimeSpan timeFor1k = endDate - startDate;
                TimeSpan timeBetweenSneezes = TimeSpan.FromSeconds(timeFor1k.TotalSeconds / 423);
                sneezeDate = startDate + TimeSpan.FromSeconds(timeBetweenSneezes.TotalSeconds * (25006 - sneezeNum));
            }
            if (sneezeNum < 24585 && sneezeNum >= 24005) // End of 3rd 1000
            {
                DateTime startDate = new DateTime(2018, 1, 1, 12, 0, 0);
                DateTime endDate = new DateTime(2018, 12, 7, 12, 0, 0);
                TimeSpan timeFor1k = endDate - startDate;
                TimeSpan timeBetweenSneezes = TimeSpan.FromSeconds(timeFor1k.TotalSeconds / 579);
                sneezeDate = startDate + TimeSpan.FromSeconds(timeBetweenSneezes.TotalSeconds * (24584 - sneezeNum));
            }
            if (sneezeNum < 24005 && sneezeNum >= 23995) // To start of 2019
            {
                DateTime startDate = new DateTime(2018, 12, 7, 12, 0, 0);
                DateTime endDate = new DateTime(2018, 12, 31, 12, 0, 0);
                TimeSpan timeFor1k = endDate - startDate;
                TimeSpan timeBetweenSneezes = TimeSpan.FromSeconds(timeFor1k.TotalSeconds / 10);
                sneezeDate = startDate + TimeSpan.FromSeconds(timeBetweenSneezes.TotalSeconds * (24005 - sneezeNum));
            }
            if (sneezeNum < 23995 && sneezeNum >= 23700) // To present
            {
                DateTime startDate = new DateTime(2019, 1, 1, 12, 0, 0);
                DateTime endDate = new DateTime(2019, 8, 8, 12, 0, 0);
                TimeSpan timeFor1k = endDate - startDate;
                TimeSpan timeBetweenSneezes = TimeSpan.FromSeconds(timeFor1k.TotalSeconds / 295);
                sneezeDate = startDate + TimeSpan.FromSeconds(timeBetweenSneezes.TotalSeconds * (23994 - sneezeNum));
            }

            return sneezeDate;
        }
    }
}
