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
    public static class Client
    {
        static HttpClient _client = new HttpClient();

        static List<Swagger> _swaggers;

        static string _uri;

        public static void SetURI(string uri)
        {
            _uri = uri;
        }

        public static async Task<List<Swagger>> GetSwaggerAsync(string startDate,string endDate)
        {
          return await RunAsync(startDate,endDate);
        }

        private static async Task<List<Swagger>> RunAsync(string startDate, string endDate)
        {
            _swaggers = new List<Swagger>();

            DateTimeOffset startDateTimeOffset; //new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero);

            var s1 = DateTimeOffset.TryParse(startDate, null as IFormatProvider,
                            DateTimeStyles.AssumeLocal,
                            out startDateTimeOffset);

            DateTimeOffset endDateDateTimeOffset;  //new DateTimeOffset(2016, 1, 2, 23, 59, 0, TimeSpan.Zero);

            var s2 = DateTimeOffset.TryParse(endDate, null as IFormatProvider,
                           DateTimeStyles.AssumeLocal,
                           out endDateDateTimeOffset);

            var daysDifference = endDateDateTimeOffset.Subtract(startDateTimeOffset).TotalDays;

            for (var dayCount = 0; dayCount < Math.Round(daysDifference); dayCount++)
            {
                var parameters = "startDate=" + HttpUtility.UrlEncode(startDateTimeOffset.AddDays(dayCount).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffzzz")) + "&endDate=" + HttpUtility.UrlEncode(startDateTimeOffset.AddDays(dayCount + 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffzzz"));

                var parsedResult = await ProcessRequest(parameters);

                if (parsedResult.Count() >= 100)
                {
                    var hoursDifference = startDateTimeOffset.AddDays(dayCount).Subtract(startDateTimeOffset.AddDays(dayCount + 1)).TotalHours;

                    for (var hourCount = 0; hourCount < Math.Round(hoursDifference); hourCount++)
                    {
                        parameters = "startDate=" + HttpUtility.UrlEncode(startDateTimeOffset.AddHours(hourCount).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffzzz")) + "&endDate=" + HttpUtility.UrlEncode(startDateTimeOffset.AddHours(hourCount + 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffzzz"));

                        parsedResult = await ProcessRequest(parameters);

                        if (parsedResult.Count >= 100)
                        {
                            var secoundDifference = startDateTimeOffset.AddDays(dayCount).Subtract(startDateTimeOffset.AddDays(dayCount + 1)).Seconds;

                            for (var secondCount = 0; secondCount < secoundDifference; secondCount++)
                            {
                                parameters = "startDate=" + HttpUtility.UrlEncode(startDateTimeOffset.AddSeconds(secondCount).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffzzz")) + "&endDate=" + HttpUtility.UrlEncode(startDateTimeOffset.AddSeconds(secondCount + 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffzzz"));

                                parsedResult = await ProcessRequest(parameters);

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

        private static async Task<List<Swagger>> ProcessRequest(string parameters)
        {
            HttpResponseMessage response = await _client.GetAsync(_uri + parameters);
            var jsonString = await response.Content.ReadAsStringAsync();
            var parsedResult = JsonConvert.DeserializeObject<List<Swagger>>(jsonString);
            return parsedResult;
        }
    }
}