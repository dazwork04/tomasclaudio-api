using B1SLayer;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Entities.Main;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.EFCore;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;

namespace SAPB1SLayerWebAPI.Services
{

    public class ApprovalRequestService
    {
        private readonly MainDbContext mainDbContext;
        private readonly string connString;
        public ApprovalRequestService(MainDbContext mainDbContext)
        {
            this.mainDbContext = mainDbContext;
            connString = mainDbContext.Database.GetDbConnection().ConnectionString;
        }

        public async Task<Response> GetApprovalRequestsAsync(int userId, string companyDB, ApprovalRequestBody body) => await Task.Run(async () =>
        {
            try
            {
                mainDbContext.Database.GetDbConnection().ConnectionString = string.Format(connString, companyDB);

                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = body.Paginate.OrderBy[0].ToString().ToUpper() + body.Paginate.OrderBy[1..];

                List<SLApprovalRequest> approvalRequests = [];

                if (body.Approvals.Count > 0)
                {
                    for (int i = 0; i < body.Approvals.Count; i++)
                    {
                        var appReqs = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ApprovalTemplatesID eq {body.Approvals[i]} and IsDraft eq 'Y' and Status eq '{body.Status}' and ObjectType eq '{body.ObjectType}'").GetAllAsync<SLApprovalRequest>();

                        foreach (var appReq in appReqs)
                        {
                            bool toAdd = true;
                            // remove cancelled
                            if (appReq.Status == "arsApproved")
                            {
                                AppReqProcesStat appReqProcesStat = mainDbContext.OWDD.Where(o => o.WddCode == appReq.Code).Select(o => new AppReqProcesStat
                                {
                                    Code = o.WddCode,
                                    DraftEntry = o.DraftEntry,
                                    DocEntry = o.DocEntry,
                                    ObjType = o.ObjType,
                                    ProcesStat = o.ProcesStat
                                }).First();

                                if (appReqProcesStat.ProcesStat == 'C') toAdd = false;
                            }

                            if (toAdd)
                            {
                                //originator
                                if (appReq.OriginatorID == body.UserId) approvalRequests.Add(appReq);
                                else
                                {
                                    //approver
                                    var currStage = await connection.Request(EntitiesKeys.ApprovalStages, appReq.CurrentStage).GetAsync<ApprovalStage>();
                                    if (currStage.ApprovalStageApprovers.Any(asa => asa.UserID == body.UserId)) approvalRequests.Add(appReq);
                                }
                            }
                        }
                    }
                }

                List<int> draftEntries = approvalRequests.Select(ar => ar.DraftEntry).ToList();

                string queryFilter = "";
                for (int i = 0; i < draftEntries.Count; i++)
                {
                    queryFilter += $"DocEntry eq {draftEntries[i]}";
                    if (i < draftEntries.Count - 1) queryFilter += " or ";
                }
                //if (queryFilter != string.Empty) queryFilter = $"({queryFilter})" + body.Paginate.Filter;

                long count = 0;
                List<dynamic> result = [];
                if (draftEntries.Count != 0)
                {
                    count = await connection.Request(EntitiesKeys.Drafts)
                    .Filter(queryFilter)
                    .GetCountAsync();

                    result = await connection.Request(EntitiesKeys.Drafts)
                    .Filter(queryFilter)
                    .Skip(body.Paginate.Page * body.Paginate.Size)
                    .Top(body.Paginate.Size)
                    .OrderBy($"{orderBy} {body.Paginate.Direction}")
                    .GetAsync<List<dynamic>>();
                }


                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        ApprovalRequests = approvalRequests,
                        Drafts = new
                        {
                            Count = count,
                            Data = result
                        }
                    }
                };
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
        public async Task<Response> ApproveApprovalRequestAsync(int userId, string companyDB, dynamic body) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = EntitiesKeys.ApprovalRequests + $"({body.code})";

                await connection.Request(reqParam).PatchAsync(new
                {
                    ApprovalRequestDecisions = new List<dynamic>
                    {
                        new { Status = "ardApproved",  Remarks = body.remarks }
                    }
                });

                var approvalRequest = await connection.Request(reqParam).GetAsync<SLApprovalRequest>();

                return new Response
                {
                    Status = "success",
                    Message = $"Approval Request successfully approved.",
                    Payload = approvalRequest
                };
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
        public async Task<Response> RejectApprovalRequestAsync(int userId, string companyDB, dynamic body) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = EntitiesKeys.ApprovalRequests + $"({body.code})";

                await connection.Request(reqParam).PatchAsync(new
                {
                    ApprovalRequestDecisions = new List<dynamic>
                    {
                        new { Status = "ardNotApproved", Remarks = body.remarks  }
                    }
                });

                var approvalRequest = await connection.Request(reqParam).GetAsync<SLApprovalRequest>();

                return new Response
                {
                    Status = "success",
                    Message = $"Approval Request is set to Not Approved.",
                    Payload = approvalRequest
                };
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
        public async Task<Response> AddToDocumentApprovalRequestAsync(int userId, string companyDB, dynamic body) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                if (body.draft != null) await connection.Request(EntitiesKeys.Drafts, body.docEntry).PutAsync(body.draft);

                await connection.Request($"{ActionsKeys.DraftsService}_SaveDraftToDocument").PostAsync(new
                {
                    Document = new { DocEntry = body.docEntry }
                });

                string reqParam = ObjectTypesHelper.GetObjectType(int.Parse(body.objCode.ToString()));

                var result = await connection.Request(reqParam).OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                var newDoc = result.First();

                if (body.objCode == 17)
                    CostCenterService.AddCostCenterFromSO(userId, companyDB, newDoc.DocNum.ToString());

                Logger.CreateLog(false, "ADD TO DOCUMENT", "SUCCESS", Newtonsoft.Json.JsonConvert.SerializeObject(body));
                return new Response
                {
                    Status = "success",
                    Message = $"#{newDoc.DocNum} successfully added to Posted Document.",
                    Payload = newDoc
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(false, "ADD TO DOCUMENT", ex.Message, Newtonsoft.Json.JsonConvert.SerializeObject(body));

                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });
        public async Task<Response> CreateDraftAsync(int userId, string companyDB, dynamic draft) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                await connection.Request(EntitiesKeys.Drafts).PostAsync(draft);
                var results = await connection.Request(EntitiesKeys.Drafts).OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                var result = results.First();

                return new Response
                {
                    Status = "success",
                    Message = $"Document Draft #{result.DocEntry} created successfully.",
                    Payload = result,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "success",
                    Message = ex.Message
                };
            }
        });
        public async Task<Response> UpdateDraftAsync(int userId, string companyDB, dynamic draft) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                await connection.Request(EntitiesKeys.Drafts, draft.DocEntry).PutAsync(draft);
                var result = await connection.Request(EntitiesKeys.Drafts, draft.DocEntry).GetAsync<dynamic>();

                return new Response
                {
                    Status = "success",
                    Message = $"Document Draft #{draft.DocEntry} updated successfully.",
                    Payload = result,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "success",
                    Message = ex.Message
                };
            }
        });
        public async Task<Response> UpdateDraftPaymentAsync(int userId, string companyDB, dynamic draft) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                await connection.Request(EntitiesKeys.PaymentDrafts, draft.DocEntry).PutAsync(draft);
                var result = await connection.Request(EntitiesKeys.PaymentDrafts, draft.DocEntry).GetAsync<dynamic>();

                return new Response
                {
                    Status = "success",
                    Message = $"Payment Draft #{draft.DocEntry} updated successfully.",
                    Payload = result,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "success",
                    Message = ex.Message
                };
            }
        });
        public async Task<Response> GetApprovalRequestsBillingAsync(int userId, string companyDB, ApprovalRequestBody body) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = body.Paginate.OrderBy[0].ToString().ToUpper() + body.Paginate.OrderBy[1..];

                List<SLApprovalRequest> approvalRequests = [];

                if (body.Approvals.Count > 0)
                {
                    for (int i = 0; i < body.Approvals.Count; i++)
                    {

                        var appReqs = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ApprovalTemplatesID eq {body.Approvals[i]} and IsDraft eq 'Y' and Status eq '{body.Status}' and ObjectType eq '13'").GetAllAsync<SLApprovalRequest>();
                        foreach (var appReq in appReqs)
                        {
                            if (body.Status == "arsApproved")
                            {
                                approvalRequests.Add(appReq);
                            }
                            else
                            {
                                // originator
                                if (appReq.OriginatorID == body.UserId) approvalRequests.Add(appReq);
                                else
                                {
                                    //approver
                                    var currStage = await connection.Request(EntitiesKeys.ApprovalStages, appReq.CurrentStage).GetAsync<ApprovalStage>();
                                    if (currStage.ApprovalStageApprovers.Any(asa => asa.UserID == body.UserId)) approvalRequests.Add(appReq);
                                }
                            }
                        }
                    }
                }

                List<int> draftEntries = approvalRequests.Select(ar => ar.DraftEntry).ToList();

                string queryFilter = "";
                for (int i = 0; i < draftEntries.Count; i++)
                {
                    queryFilter += $"DocEntry eq {draftEntries[i]}";
                    if (i < draftEntries.Count - 1) queryFilter += " or ";
                }
                //if (queryFilter != string.Empty) queryFilter = $"({queryFilter})" + body.Paginate.Filter;

                long count = 0;
                List<dynamic> result = [];
                if (draftEntries.Count != 0)
                {
                    count = await connection.Request(EntitiesKeys.Drafts)
                    .Filter(queryFilter)
                    .GetCountAsync();

                    result = await connection.Request(EntitiesKeys.Drafts)
                    .Filter(queryFilter)
                    .Skip(body.Paginate.Page * body.Paginate.Size)
                    .Top(body.Paginate.Size)
                    .OrderBy($"{orderBy} {body.Paginate.Direction}")
                    .GetAsync<List<dynamic>>();
                }


                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        ApprovalRequests = approvalRequests,
                        Drafts = new
                        {
                            Count = count,
                            Data = result
                        }
                    }
                };
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
        public async Task<Response> GetApprovalRequestsSalesOrderAsync(int userId, string companyDB, ApprovalRequestBody body) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = body.Paginate.OrderBy[0].ToString().ToUpper() + body.Paginate.OrderBy[1..];

                List<SLApprovalRequest> approvalRequests = [];

                if (body.Approvals.Count > 0)
                {
                    for (int i = 0; i < body.Approvals.Count; i++)
                    {

                        var appReqs = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ApprovalTemplatesID eq {body.Approvals[i]} and IsDraft eq 'Y' and Status eq '{body.Status}' and ObjectType eq '17'").GetAllAsync<SLApprovalRequest>();
                        foreach (var appReq in appReqs)
                        {
                            if (body.Status == "arsApproved")
                            {
                                approvalRequests.Add(appReq);
                            }
                            else
                            {
                                // originator
                                if (appReq.OriginatorID == body.UserId) approvalRequests.Add(appReq);
                                else
                                {
                                    //approver
                                    var currStage = await connection.Request(EntitiesKeys.ApprovalStages, appReq.CurrentStage).GetAsync<ApprovalStage>();
                                    if (currStage.ApprovalStageApprovers.Any(asa => asa.UserID == body.UserId)) approvalRequests.Add(appReq);
                                }
                            }
                        }
                    }
                }

                List<int> draftEntries = approvalRequests.Select(ar => ar.DraftEntry).ToList();

                string queryFilter = "";
                for (int i = 0; i < draftEntries.Count; i++)
                {
                    queryFilter += $"DocEntry eq {draftEntries[i]}";
                    if (i < draftEntries.Count - 1) queryFilter += " or ";
                }
                //if (queryFilter != string.Empty) queryFilter = $"({queryFilter})" + body.Paginate.Filter;

                long count = 0;
                List<dynamic> result = [];
                if (draftEntries.Count != 0)
                {
                    count = await connection.Request(EntitiesKeys.Drafts)
                    .Filter(queryFilter)
                    .GetCountAsync();

                    result = await connection.Request(EntitiesKeys.Drafts)
                    .Filter(queryFilter)
                    .Skip(body.Paginate.Page * body.Paginate.Size)
                    .Top(body.Paginate.Size)
                    .OrderBy($"{orderBy} {body.Paginate.Direction}")
                    .GetAsync<List<dynamic>>();
                }


                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        ApprovalRequests = approvalRequests,
                        Drafts = new
                        {
                            Count = count,
                            Data = result
                        }
                    }
                };
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
        public async Task<Response> GetApprovalRequestsDeliveryAsync(int userId, string companyDB, ApprovalRequestBody body) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = body.Paginate.OrderBy[0].ToString().ToUpper() + body.Paginate.OrderBy[1..];

                List<SLApprovalRequest> approvalRequests = [];

                if (body.Approvals.Count > 0)
                {
                    for (int i = 0; i < body.Approvals.Count; i++)
                    {

                        var appReqs = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ApprovalTemplatesID eq {body.Approvals[i]} and IsDraft eq 'Y' and Status eq '{body.Status}' and ObjectType eq '15'").GetAllAsync<SLApprovalRequest>();
                        foreach (var appReq in appReqs)
                        {
                            if (body.Status == "arsApproved")
                            {
                                approvalRequests.Add(appReq);
                            }
                            else
                            {
                                // originator
                                if (appReq.OriginatorID == body.UserId) approvalRequests.Add(appReq);
                                else
                                {
                                    //approver
                                    var currStage = await connection.Request(EntitiesKeys.ApprovalStages, appReq.CurrentStage).GetAsync<ApprovalStage>();
                                    if (currStage.ApprovalStageApprovers.Any(asa => asa.UserID == body.UserId)) approvalRequests.Add(appReq);
                                }
                            }
                        }
                    }
                }

                List<int> draftEntries = approvalRequests.Select(ar => ar.DraftEntry).ToList();

                string queryFilter = "";
                for (int i = 0; i < draftEntries.Count; i++)
                {
                    queryFilter += $"DocEntry eq {draftEntries[i]}";
                    if (i < draftEntries.Count - 1) queryFilter += " or ";
                }
                //if (queryFilter != string.Empty) queryFilter = $"({queryFilter})" + body.Paginate.Filter;

                long count = 0;
                List<dynamic> result = [];
                if (draftEntries.Count != 0)
                {
                    count = await connection.Request(EntitiesKeys.Drafts)
                    .Filter(queryFilter)
                    .GetCountAsync();

                    result = await connection.Request(EntitiesKeys.Drafts)
                    .Filter(queryFilter)
                    .Skip(body.Paginate.Page * body.Paginate.Size)
                    .Top(body.Paginate.Size)
                    .OrderBy($"{orderBy} {body.Paginate.Direction}")
                    .GetAsync<List<dynamic>>();
                }


                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        ApprovalRequests = approvalRequests,
                        Drafts = new
                        {
                            Count = count,
                            Data = result
                        }
                    }
                };
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
        public async Task<Response> GetApprovalStatusAsync(int userId, string companyDB, ApprovalRequestBody body) => await Task.Run(async () =>
        {
            try
            {
                mainDbContext.Database.GetDbConnection().ConnectionString = string.Format(connString, companyDB);

                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = body.Paginate.OrderBy[0].ToString().ToUpper() + body.Paginate.OrderBy[1..];

                List<SLApprovalRequest> approvalRequests = [];

                if (body.Approvals.Count > 0)
                {
                    for (int i = 0; i < body.Approvals.Count; i++)
                    {

                        var appReqs = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ApprovalTemplatesID eq {body.Approvals[i]} and IsDraft eq '{body.IsDraft}' and Status eq '{body.Status}' and ObjectType eq '{body.ObjectType}'").GetAllAsync<SLApprovalRequest>();
                        foreach (var appReq in appReqs)
                        {
                            bool toAdd = false;
                            var approvalTemplate = await connection.Request(EntitiesKeys.ApprovalTemplates, appReq.ApprovalTemplatesID).GetAsync<ApprovalTemplate>();

                            // originator
                            if (appReq.OriginatorID == body.UserId)
                            {
                                //approvalRequests.Add(appReq);
                                toAdd = true;
                            }
                            else
                            {
                                //approver
                                foreach (var stage in approvalTemplate.ApprovalTemplateStages)
                                {
                                    var approvalStage = await connection.Request(EntitiesKeys.ApprovalStages, stage.ApprovalStageCode).GetAsync<ApprovalStage>();
                                    if (approvalStage.ApprovalStageApprovers.Any(ata => ata.UserID == body.UserId))
                                    {
                                        //approvalRequests.Add(appReq);
                                        toAdd = true;
                                        break;
                                    }
                                }
                            }

                            if (appReq.ObjectType == "17" && UserIDs.FINAL_POSTING.SO.Contains(body.UserId ?? 0)) toAdd = true;

                            if (body.UserId == UserIDs.MANAGER) toAdd = true;

                            // remove cancelled
                            AppReqProcesStat appReqProcesStat = mainDbContext.OWDD.Where(o => o.WddCode == appReq.Code).Select(o => new AppReqProcesStat
                            {
                                Code = o.WddCode,
                                DraftEntry = o.DraftEntry,
                                DocEntry = o.DocEntry,
                                ObjType = o.ObjType,
                                ProcesStat = o.ProcesStat
                            }).First();

                            if (appReqProcesStat.ProcesStat == 'C') toAdd = false;

                            if (toAdd)
                            {
                                var appTempStages = approvalTemplate.ApprovalTemplateStages.OrderBy(x => x.SortID);

                                List<ApprovalRequestLine> appReqLines = [];
                                foreach (var ats in appTempStages)
                                {
                                    var lines = appReq.ApprovalRequestLines.Where(a => a.StageCode == ats.ApprovalStageCode);
                                    appReqLines.AddRange(lines);
                                }

                                appReq.ApprovalRequestLines = appReqLines;
                                approvalRequests.Add(appReq);
                            }
                        }
                    }
                }

                List<dynamic> resultDrafts = [];

                List<int> draftEntries = [.. approvalRequests.OrderByDescending(ar => ar.Code).Select(ar => ar.DraftEntry).Distinct()];
                long countDrafts = draftEntries.Count;


                if (countDrafts > 0)
                {
                    draftEntries = draftEntries.Skip(body.Paginate.Page * body.Paginate.Size).Take(body.Paginate.Size).ToList();

                    string queryFilter = "";
                    for (int i = 0; i < draftEntries.Count; i++)
                    {
                        queryFilter += $"DocEntry eq {draftEntries[i]}";
                        if (i < draftEntries.Count - 1) queryFilter += " or ";
                    }

                    resultDrafts = (List<dynamic>)await connection.Request(EntitiesKeys.Drafts)
                    .Filter(queryFilter)
                    .OrderBy($"DocNum desc")
                    .GetAllAsync<dynamic>();
                }


                List<dynamic> resultDocs = [];
                if (body.Status == "arsApproved" && body.IsDraft == "N")
                {
                    List<int?> docEntries = [.. approvalRequests.Where(ar => draftEntries.Contains(ar.DraftEntry)).Select(ar => ar.ObjectEntry).Distinct()];

                    if (docEntries.Count != 0)
                    {
                        string reqParam = ObjectTypesHelper.GetObjectType(int.Parse(approvalRequests[0].ObjectType));

                        string queryFilter = "";
                        for (int i = 0; i < docEntries.Count; i++)
                        {
                            queryFilter += $"DocEntry eq {docEntries[i]}";
                            if (i < docEntries.Count - 1) queryFilter += " or ";
                        }

                        resultDocs = (List<dynamic>)await connection.Request(reqParam)
                        .Filter(queryFilter)
                        .GetAllAsync<dynamic>();
                    }
                }

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        ApprovalRequests = approvalRequests,
                        Drafts = new
                        {
                            Count = countDrafts,
                            Data = resultDrafts,
                        },
                        Docs = resultDocs
                    }
                };
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
        public async Task<Response> UpdateApprovalRequestAsync(int userId, string companyDB, dynamic body) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                //await connection.Request(reqParam).PatchAsync(new
                //{
                //    ApprovalRequestDecisions = new List<dynamic>
                //    {
                //        new { Status = "ardPending"  }
                //    }
                //});

                await connection.Request(EntitiesKeys.Drafts, body.draft.DocEntry).PutAsync(body.draft);

                await connection.Request($"{ActionsKeys.DraftsService}_SaveDraftToDocument").PostAsync(new
                {
                    Document = new { body.draft.DocEntry }
                });

                var draft = await connection.Request(EntitiesKeys.Drafts, body.draft.DocEntry).GetAsync<dynamic>();
                var appReqs = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"DraftEntry eq {draft.DocEntry}").Top(1).GetAsync<List<SLApprovalRequest>>();

                //string reqParam = EntitiesKeys.ApprovalRequests + $"({body.code})";
                //var appReq = await connection.Request(reqParam).GetAsync<SLApprovalRequest>();

                return new Response
                {
                    Status = "success",
                    Message = $"Approval Request successfully updated.",
                    Payload = new
                    {
                        Draft = draft,
                        ApprovalRequest = appReqs.First()
                    }
                };
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

        // ADD TO DOCUMENT PAYMENT DRAFT
        public async Task<Response> AddToDocumentPaymentDraftAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request($"PaymentDrafts({docEntry})/SaveDraftToDocument").PostAsync();

                var result = await connection.Request(EntitiesKeys.IncomingPayments).OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                var newDoc = result.First();

                return new Response
                {
                    Status = "success",
                    Message = $"#{newDoc.DocNum} successfully added to Posted Payment.",
                    Payload = newDoc
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "ADD TO DOCUMENT INCOMING PAYMENT DRAFT", ex.Message, JsonConvert.SerializeObject(docEntry));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // REMOVE PAYMENT DRAFT
        public async Task<Response> RemovePaymentDraftAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.PaymentDrafts, docEntry).DeleteAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"Payment Draft successfully Deleted.",
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "DELETE PAYMENT DRAFT", ex.Message, JsonConvert.SerializeObject(docEntry));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });
    }
}
