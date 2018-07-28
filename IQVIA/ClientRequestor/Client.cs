using IQVIA.Controllers;
using IQVIA.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IQVIA.ClientRequestor
{
    public enum ProcessRequestBy {Day,Hour,Second };

    public static class Client
    {
        private const string _dateTimeOffsetFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffzzz";
        private const int _maxResponse = 100;
        private static HttpClient _client = new HttpClient();
        private static List<Swagger> _swaggers;
        private static string _uri;

        public static void SetURI(string uri)
        {
            _uri = uri;
        }

        public static async Task<List<Swagger>> GetSwaggerAsync(string startDate,string endDate)
        {
          return await ProcessRequestAsync(startDate,endDate);
        }

        private static async Task<List<Swagger>> ProcessRequestAsync(string startDate, string endDate)
        {
            _swaggers = new List<Swagger>();

            DateTimeOffset startDateTimeOffset = DateTimeOffsetParse(startDate); //new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero);

            DateTimeOffset endDateDateTimeOffset = DateTimeOffsetParse(endDate); //new DateTimeOffset(2016, 1, 2, 23, 59, 0, TimeSpan.Zero);

            var daysDifference = endDateDateTimeOffset.Subtract(startDateTimeOffset).TotalDays;

            for (var dayCount = 0; dayCount < Math.Round(daysDifference); dayCount++)
            {
                var queryParameter = GetParameter(ref startDateTimeOffset, dayCount, ProcessRequestBy.Day);

                var parsedResult = await ProcessRequest(queryParameter);

                if (parsedResult.Count() >= _maxResponse)
                {
                    var hoursDifference = startDateTimeOffset.AddDays(dayCount).Subtract(startDateTimeOffset.AddDays(dayCount + 1)).TotalHours;

                    for (var hourCount = 0; hourCount < Math.Round(hoursDifference); hourCount++)
                    {
                        queryParameter = GetParameter(ref startDateTimeOffset, hourCount, ProcessRequestBy.Hour);

                        parsedResult = await ProcessRequest(queryParameter);

                        if (parsedResult.Count >= _maxResponse)
                        {
                            var secoundDifference = startDateTimeOffset.AddDays(dayCount).Subtract(startDateTimeOffset.AddDays(dayCount + 1)).Seconds;

                            for (var secondCount = 0; secondCount < secoundDifference; secondCount++)
                            {
                                queryParameter = GetParameter(ref startDateTimeOffset, secondCount, ProcessRequestBy.Second);

                                parsedResult = await ProcessRequest(queryParameter);

                                _swaggers.AddRange(parsedResult);
                            }

                        }
                        else
                        {
                            _swaggers.AddRange(parsedResult);
                        }
                    }
                }
                else
                {
                    _swaggers.AddRange(parsedResult);
                }

            }
            return _swaggers;
        }

        private static DateTimeOffset DateTimeOffsetParse(string date)
        {
            DateTimeOffset dateTimeOffset;
           if(DateTimeOffset.TryParse(date, null as IFormatProvider,
                           DateTimeStyles.AssumeLocal,
                           out dateTimeOffset))
            {

            }
            else
            {
                _swaggers.Add(new Swagger() { Id = "Error", Stamp = "Unable to parse Date Time Offeset since it is wrong" });
            }
            return dateTimeOffset;
        }

        private static string GetParameter(ref DateTimeOffset startDateTimeOffset, int count, ProcessRequestBy processRequestBy)
        {
            switch (processRequestBy)
            {
                case ProcessRequestBy.Day:
                    return "startDate=" 
                        + HttpUtility.UrlEncode(startDateTimeOffset.AddDays(count).ToString(_dateTimeOffsetFormat)) 
                        + "&endDate=" 
                        + HttpUtility.UrlEncode(startDateTimeOffset.AddDays(count + 1).ToString(_dateTimeOffsetFormat));

                case ProcessRequestBy.Hour:
                    return "startDate=" 
                        + HttpUtility.UrlEncode(startDateTimeOffset.AddHours(count).ToString(_dateTimeOffsetFormat)) 
                        + "&endDate=" 
                        + HttpUtility.UrlEncode(startDateTimeOffset.AddHours(count + 1).ToString(_dateTimeOffsetFormat));

                case ProcessRequestBy.Second:
                    return "startDate=" 
                        + HttpUtility.UrlEncode(startDateTimeOffset.AddSeconds(count).ToString(_dateTimeOffsetFormat))
                        + "&endDate=" 
                        + HttpUtility.UrlEncode(startDateTimeOffset.AddSeconds(count + 1).ToString(_dateTimeOffsetFormat));
            }
            return string.Empty;
        }

        private static async Task<List<Swagger>> ProcessRequest(string queryParameter)
        {
            HttpResponseMessage response = await _client.GetAsync(_uri + queryParameter);
            var jsonString = await response.Content.ReadAsStringAsync();
           return JsonConvert.DeserializeObject<List<Swagger>>(jsonString);

        }
    }
}