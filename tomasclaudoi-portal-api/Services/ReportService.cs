using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Utils;
using System.Net;
using System.Text.Json;

namespace SAPB1SLayerWebAPI.Services
{
    public class ReportService
    {

        #region VARIABLES
        private readonly HttpHelper httpHelper;
        private readonly AuthDbContext authDbContext;
        private readonly JsonSerializerOptions jsonOptions = new () { PropertyNameCaseInsensitive = true, };
        #endregion

        public ReportService(IHttpClientFactory httpClientFactory, AuthDbContext authDbContext)
        {
            httpHelper = new(httpClientFactory);
            this.authDbContext = authDbContext;
        }


        // GET CRYSTAL REPORT
        public async Task<Response> GenerateReportAsync(int userId, string companyDB, string docCode, dynamic parameters) => await Task.Run(async () =>
        {
            try
            {
                User? user = authDbContext.OUSR.Select(o => new User
                {
                    Id = o.Id,
                    EmpId = o.EmpId,
                    EmpCode = o.EmpCode,
                    Name = o.Name,
                    UserId = o.UserId,
                    UserCode = o.UserCode,
                    SapUser = o.SapUser,
                    SapPass = o.SapPass,
                }).FirstOrDefault(o => o.Id == userId);

                if (user != null)
                {
                    HttpResponseMessage reportResponse = await Task.Run(async () => await APIGatewayGetReport(docCode, parameters));

                    if (reportResponse.IsSuccessStatusCode)
                    {
                        string reportContentStream = await reportResponse.Content.ReadAsStringAsync();

                        return new Response
                        {
                            Status = "success",
                            Payload = reportContentStream
                        };
                    }
                    else
                    {
                        if (reportResponse.StatusCode == HttpStatusCode.Unauthorized)
                        {   
                            HttpResponseMessage authResponse = await Task.Run(async () => await APIGatewayLogin(companyDB, user!.SapUser, user!.SapPass));
                            if (authResponse.IsSuccessStatusCode)
                            {
                                using var authContentStream = await authResponse.Content.ReadAsStreamAsync();

                                GW_API_Auth_Response? authData = await JsonSerializer.DeserializeAsync<GW_API_Auth_Response>(authContentStream, jsonOptions);

                                if (authData != null)
                                {
                                    if (authData.Code == null)
                                    {
                                        HttpResponseMessage reportResponse1 = await Task.Run(async () => await APIGatewayGetReport(docCode, parameters));

                                        if (reportResponse1.IsSuccessStatusCode)
                                        {
                                            string reportContentStream1 = await reportResponse1.Content.ReadAsStringAsync();

                                            return new Response
                                            {
                                                Status = "success",
                                                Payload = reportContentStream1
                                            };
                                        } 
                                        else
                                        {
                                            return new Response
                                            {
                                                Status = "failed",
                                                Message = $"{reportResponse1.StatusCode} - {reportResponse1.Content}"
                                            };
                                        }
                                    }
                                    else
                                    {
                                        return new Response
                                        {
                                            Status = "failed",
                                            Message = $"{authData.Code} - {authData.Message.Value}"
                                        };
                                    }
                                }
                                else
                                {
                                    return new Response
                                    {
                                        Status = "failed",
                                        Message = "Something went wrong."
                                    };
                                }
                            }
                            else
                            {
                                return new Response
                                {
                                    Status = "failed",
                                    Message = $"{authResponse.StatusCode} - {authResponse.Content}"

                                };
                            }
                        }
                        else
                        {
                            return new Response
                            {
                                Status = "failed",
                                Message = $"{reportResponse.StatusCode} - {reportResponse.Content}",
                            };
                        }
                    }
                }
                else
                {
                    return new Response
                    {
                        Status = "failed",
                        Message = "Not Authorized."
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // API GATEWAY GET REPORT
        public async Task<HttpResponseMessage> APIGatewayGetReport(string docCode, dynamic parameters) => await httpHelper.PostRequest($"/rs/v1/ExportPDFData?DocCode={docCode}", "API_GATEWAY", Newtonsoft.Json.JsonConvert.SerializeObject(parameters));

        // API GATEWAY LOGIN
        public async Task<HttpResponseMessage> APIGatewayLogin(string companyDB, string username, string password) => await httpHelper.PostRequest("/login", "API_GATEWAY", Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            CompanyDB = companyDB,
            UserName = username,
            Password = password
        }));


        //// GET DEFAULT REPORT 
        //public async Task<Response> GetDefaultReportAsync(int userId, string companyDB, string reportCode) => await Task.Run(async () =>
        //{
        //    try
        //    {
        //        var connection = Main.GetConnection(userId, companyDB);

        //        var parameters = new { ReportParams = new { ReportCode = reportCode } };
        //        var result = await connection.Request($"{ActionsKeys.ReportLayoutsService}_GetDefaultReport").PostAsync<dynamic>(parameters);

        //        return new Response
        //        {
        //            Status = "success",
        //            Payload = result
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Response
        //        {
        //            Status = "failed",
        //            Message = ex.Message
        //        };
        //    }
        //});

        //// GET REPORT LAYOUT
        //public async Task<Response> GetReportLayoutAsync(int userId, string companyDB, string layoutCode) => await Task.Run(async () =>
        //{
        //    try
        //    {
        //        var connection = Main.GetConnection(userId, companyDB);

        //        var parameters = new { ReportLayoutParams = new { LayoutCode = layoutCode } };
        //        var result = await connection.Request($"{ActionsKeys.ReportLayoutsService}_GetReportLayout").PostAsync<dynamic>(parameters);

        //        return new Response
        //        {
        //            Status = "success",
        //            Payload = result
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Response
        //        {
        //            Status = "failed",
        //            Message = ex.Message
        //        };
        //    }
        //});
    }
}
