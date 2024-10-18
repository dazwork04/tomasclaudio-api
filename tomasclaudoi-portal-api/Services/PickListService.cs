using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Models;
using SLayerConnectionLib;
using B1SLayer;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Utils;
using Models;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace SAPB1SLayerWebAPI.Services
{
    public class PickListService
    {
        public async Task<Response> GetPickListsAsync(int userId, string companyDB, int baseType, string status, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"{EntitiesKeys.PickLists}/Status eq '{status}'" + paginate.Filter; // and {EntitiesKeys.PickLists}/PickDate ge '{dateFrom}' and {EntitiesKeys.PickLists}/PickDate le '{dateTo}'

                string request = $"$crossjoin({EntitiesKeys.PickLists},{EntitiesKeys.PickLists}/PickListsLines)";
                string expand = $"{EntitiesKeys.PickLists}($select=Absoluteentry)";
                string filter = $"{EntitiesKeys.PickLists}/Absoluteentry eq {EntitiesKeys.PickLists}/PickListsLines/AbsoluteEntry and {EntitiesKeys.PickLists}/PickListsLines/BaseObjectType eq {baseType} and ";
                filter += queryFilter;

                var plResults = await connection.Request(request).Expand(expand).Filter(filter).GetAsync<List<dynamic>>();

                List<int> plAbsEntries = [];
                for (int i = 0; i < plResults.Count; i++)
                {
                    int absEntry = plResults[i][EntitiesKeys.PickLists]["Absoluteentry"];
                    if (!plAbsEntries.Contains(absEntry)) plAbsEntries.Add(absEntry);
                }

                long count = 0;
                List<PickList> result = [];
                if (plAbsEntries.Count > 0)
                {
                    string plFilter = "";
                    for (int i = 0; i < plAbsEntries.Count; i++)
                    {
                        plFilter += $"Absoluteentry eq {plAbsEntries[i]}";
                        if (i < plAbsEntries.Count - 1) plFilter += " or ";
                    }

                    count = await connection.Request(EntitiesKeys.PickLists)
                    .Filter(plFilter)
                    .GetCountAsync();

                    result = await connection.Request(EntitiesKeys.PickLists)
                        .Filter(plFilter)
                        .Skip(paginate.Page * paginate.Size)
                        .Top(paginate.Size)
                        .OrderBy($"{orderBy} {paginate.Direction}")
                        .GetAsync<List<PickList>>();

                    for (int i = 0; i < result.Count; i++)
                    {
                        var user = await connection.Request(EntitiesKeys.Users, result[i].OwnerCode).GetAsync();

                        result[i].UserName = user.UserName;
                        result[i].Status = result[i].Status == string.Empty ? "ps_Picked" : result[i].Status;

                        var lines = result[i].PickListsLines;

                        for (int j = 0; j < lines.Count; j++)
                        {
                            var line = lines[j];
                            if (baseType == 17) // SALES ORDER
                            {
                                var salesOrder = await connection.Request(EntitiesKeys.Orders, line.OrderEntry).GetAsync();

                                line.BaseDocNum = (int)salesOrder.DocNum;
                                line.OrderDate = salesOrder.DocDate.ToString();
                                line.CardCode = salesOrder.CardCode;
                                line.CardName = salesOrder.CardName;

                                line.PickStatus = line.PickStatus == string.Empty ? "ps_Picked" : line.PickStatus;

                                var soLines = salesOrder.DocumentLines;

                                foreach (var soLine in soLines)
                                {
                                    if (soLine.LineNum == line.OrderRowID)
                                    {
                                        Item item = await connection.Request(EntitiesKeys.Items, soLine.ItemCode).GetAsync<Item>();
                                        Uom uom = await connection.Request(EntitiesKeys.UnitOfMeasurements, soLine.UoMEntry).GetAsync<Uom>();
                                        Warehouse warehouse = await connection.Request(EntitiesKeys.Warehouses, soLine.WarehouseCode).GetAsync<Warehouse>();

                                        var warehouseInfo = item.ItemWarehouseInfoCollection.Where(iwic => iwic.WarehouseCode == warehouse.WarehouseCode).FirstOrDefault()!;

                                        line.DueDate = soLine.ShipDate.ToString();
                                        line.ItemCode = soLine.ItemCode;
                                        line.Description = soLine.ItemDescription;
                                        line.UomCode = soLine.UoMCode;
                                        line.UomName = soLine.MeasureUnit;
                                        //line.ItemsPerUnit = item.SalesItemsPerUnit;
                                        line.WarehouseCode = soLine.WarehouseCode;
                                        line.WarehouseName = warehouse.WarehouseName;
                                        //double availToPick = warehouseInfo.InStock - warehouseInfo.Committed;
                                        //line.AvailToPick = availToPick > 0 ? availToPick : warehouseInfo.InStock;
                                    }
                                }
                            }
                            else // PRODUCTION ORDER
                            {
                                var productionOrder = await connection.Request(EntitiesKeys.ProductionOrders, line.OrderEntry).GetAsync();

                                line.BaseDocNum = (int)productionOrder.DocumentNumber;
                                line.DueDate = productionOrder.DueDate.ToString();
                                line.OrderDate = productionOrder.PostingDate.ToString();

                                var prodLines = productionOrder.ProductionOrderLines;

                                foreach (var prodLine in prodLines)
                                {
                                    if (prodLine.LineNumber == line.OrderRowID)
                                    {
                                        Item item = await connection.Request(EntitiesKeys.Items, line.ItemCode).GetAsync<Item>();
                                        Uom uom = await connection.Request(EntitiesKeys.UnitOfMeasurements, prodLine.UoMEntry).GetAsync<Uom>();
                                        Warehouse warehouse = await connection.Request(EntitiesKeys.Warehouses, prodLine.Warehouse).GetAsync<Warehouse>();

                                        var warehouseInfo = item.ItemWarehouseInfoCollection.Where(iwic => iwic.WarehouseCode == warehouse.WarehouseCode).FirstOrDefault()!;

                                        line.ItemCode = prodLine.ItemCode;
                                        line.Description = prodLine.ItemDescription;
                                        line.UomCode = uom.Code;
                                        line.UomName = uom.Name;
                                        //line.ItemsPerUnit = item.SalesItemsPerUnit;
                                        line.WarehouseCode = prodLine.WarehouseCode;
                                        line.WarehouseName = warehouse.WarehouseName;
                                        //double availToPick = warehouseInfo.InStock - warehouseInfo.Committed;
                                        //line.AvailToPick = availToPick > 0 ? availToPick : warehouseInfo.InStock;
                                    }
                                }
                            }
                            lines[j] = line;
                        }

                        result[i].PickListsLines = lines;
                    }
                }

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Count = count,
                        Data = result
                    }
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                    Payload = new
                    {
                        Count = 0,
                        Data = new List<dynamic>()
                    }
                };
            }
        });

        public async Task<Response> GetPickListAsync(int userId, string companyDB, char actionType, int absEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string reqParam = EntitiesKeys.PickLists;
                var result1 = await connection.Request(reqParam).OrderBy("Absoluteentry asc").Top(1).GetAsync<List<PickList>>(); //list
                var result2 = await connection.Request(reqParam).OrderBy("Absoluteentry desc").Top(1).GetAsync<List<PickList>>(); // list
                var firstData = result1.First();
                var lastData = result2.First();

                dynamic? result = null;

                switch (actionType)
                {
                    case 'F': // First
                        result = firstData;
                        break;
                    case 'P': // Prev
                        if (absEntry == (int)firstData.Absoluteentry || absEntry == 0) result = lastData;
                        else result = await connection.Request(reqParam, absEntry - 1).GetAsync<PickList>();
                        break;
                    case 'N': // Next
                        if (absEntry == (int)lastData.Absoluteentry || absEntry == 0) result = firstData;
                        else result = await connection.Request(reqParam, absEntry + 1).GetAsync<PickList>();
                        break;
                    case 'L': // Last
                        result = lastData;
                        break;
                    case 'R': // Refresh
                        result = await connection.Request(reqParam, absEntry).GetAsync<PickList>();
                        break;
                    default:
                        return new Response
                        {
                            Status = "failed",
                            Message = "Invalid Action Type."
                        };
                }

                var user = await connection.Request(EntitiesKeys.Users, result.OwnerCode).GetAsync();

                result.UserName = user.UserName;
                result.Status = result.Status == string.Empty ? "ps_Picked" : result.Status;

                var lines = result.PickListsLines;

                for (int j = 0; j < lines.Count; j++)
                {
                    var line = lines[j];
                    if (line.BaseObjectType == 17) // SALES ORDER
                    {
                        var salesOrder = await connection.Request(EntitiesKeys.Orders, line.OrderEntry).GetAsync();

                        line.BaseDocNum = (int)salesOrder.DocNum;
                        line.CardCode = salesOrder.CardCode;
                        line.CardName = salesOrder.CardName;
                        line.OrderDate = salesOrder.DocDate.ToString();
                        line.PickStatus = line.PickStatus == string.Empty ? "ps_Picked" : line.PickStatus;

                        var soLines = salesOrder.DocumentLines;

                        foreach (var soLine in soLines)
                        {
                            if (soLine.LineNum == (int)line.OrderRowID)
                            {
                                Item item = await connection.Request(EntitiesKeys.Items, (string)soLine.ItemCode).GetAsync<Item>();
                                //Uom uom = await connection.Request(EntitiesKeys.UnitOfMeasurements, (int)soLine.UoMEntry).GetAsync<Uom>();
                                Warehouse warehouse = await connection.Request(EntitiesKeys.Warehouses, (string)soLine.WarehouseCode).GetAsync<Warehouse>();

                                //var otherSO = await connection.Request(EntitiesKeys.Orders).Filter($"DocEntry eq {line.OrderEntry}")

                                var warehouseInfo = item.ItemWarehouseInfoCollection.Where(iwic => iwic.WarehouseCode == warehouse.WarehouseCode).FirstOrDefault()!;

                                line.DueDate = soLine.ShipDate.ToString();
                                line.ItemCode = soLine.ItemCode;
                                line.Description = soLine.ItemDescription;
                                line.UomCode = soLine.UoMCode;
                                line.UomName = soLine.MeasureUnit;//uom.UnitMeasure;
                                //line.ItemsPerUnit = item.SalesItemsPerUnit;
                                line.WarehouseCode = soLine.WarehouseCode;
                                line.WarehouseName = warehouse.WarehouseName;
                                //double availToPick = warehouseInfo.InStock - warehouseInfo.Committed;
                                //line.AvailToPick = availToPick > 0 ? availToPick : warehouseInfo.InStock;
                            }
                        }
                    }
                    else // PRODUCTION ORDER
                    {
                        var productionOrder = await connection.Request(EntitiesKeys.ProductionOrders, line.OrderEntry).GetAsync();

                        line.BaseDocNum = (int)productionOrder.DocumentNumber;
                        line.DueDate = productionOrder.DueDate.ToString();
                        line.OrderDate = productionOrder.PostingDate.ToString();
                        line.PickStatus = line.PickStatus == string.Empty ? "ps_Picked" : line.PickStatus;

                        var prodLines = productionOrder.ProductionOrderLines;

                        foreach (var prodLine in prodLines)
                        {
                            if (prodLine.LineNumber == (int)line.OrderRowID)
                            {
                                Item item = await connection.Request(EntitiesKeys.Items, (string)prodLine.ItemNo).GetAsync<Item>();
                                Uom uom = await connection.Request(EntitiesKeys.UnitOfMeasurements, (int)prodLine.UoMEntry).GetAsync<Uom>();
                                Warehouse warehouse = await connection.Request(EntitiesKeys.Warehouses, (string)prodLine.Warehouse).GetAsync<Warehouse>();

                                var warehouseInfo = item.ItemWarehouseInfoCollection.Where(iwic => iwic.WarehouseCode == warehouse.WarehouseCode).FirstOrDefault()!;

                                line.ItemCode = prodLine.ItemNo;
                                line.Description =prodLine.ItemName;
                                line.UomCode = uom.Code;
                                line.UomName =  uom.Name;
                                //line.ItemsPerUnit = item.SalesItemsPerUnit;
                                line.WarehouseCode = warehouse.WarehouseCode;
                                line.WarehouseName = warehouse.WarehouseName;
                                //double availToPick = warehouseInfo.InStock - warehouseInfo.Committed;
                                //line.AvailToPick = availToPick > 0 ? availToPick : warehouseInfo.InStock;
                            }
                        }
                    }
                    lines[j] = line;
                }

                result.PickListsLines = lines;

                return new Response
                {
                    Status = "success",
                    Payload = (PickList) result,
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                    Payload = null,
                };
            }
        });

        public async Task<Response> UpdatePickListAsync(int userId, string companyDB, SLPickList pickList) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.PickLists, pickList.Absoluteentry).PatchAsync(pickList);

                var result = await connection.Request(EntitiesKeys.PickLists, pickList.Absoluteentry).GetAsync<PickList>();

                var user = await connection.Request(EntitiesKeys.Users, result.OwnerCode).GetAsync();

                result.UserName = user.UserName;
                result.Status = result.Status == string.Empty ? "ps_Picked" : result.Status;

                var lines = result.PickListsLines;

                for (int j = 0; j < lines.Count; j++)
                {
                    var line = lines[j];
                    if (line.BaseObjectType == 17) // SALES ORDER
                    {
                        var salesOrder = await connection.Request(EntitiesKeys.Orders, line.OrderEntry).GetAsync();

                        line.BaseDocNum = (int)salesOrder.DocNum;
                        line.CardCode = salesOrder.CardCode;
                        line.PickStatus = line.PickStatus == string.Empty ? "ps_Picked" : line.PickStatus;

                        var soLines = salesOrder.DocumentLines;

                        foreach (var soLine in soLines)
                        {
                            if (soLine.LineNum == (int)line.OrderRowID)
                            {
                                Item item = await connection.Request(EntitiesKeys.Items, (string)soLine.ItemCode).GetAsync<Item>();
                                Uom uom = await connection.Request(EntitiesKeys.UnitOfMeasurements, (int)soLine.UoMEntry).GetAsync<Uom>();
                                Warehouse warehouse = await connection.Request(EntitiesKeys.Warehouses, (string)soLine.WarehouseCode).GetAsync<Warehouse>();

                                //var otherSO = await connection.Request(EntitiesKeys.Orders).Filter($"DocEntry eq {line.OrderEntry}")

                                var warehouseInfo = item.ItemWarehouseInfoCollection.Where(iwic => iwic.WarehouseCode == warehouse.WarehouseCode).FirstOrDefault()!;

                                line.DueDate = soLine.ShipDate.ToString();
                                line.ItemCode = (string)soLine.ItemCode;
                                line.Description = (string)soLine.ItemDescription;
                                line.UomCode = (string)soLine.UoMCode;
                                line.UomName = uom.Name;
                                //line.ItemsPerUnit = item.SalesItemsPerUnit;
                                line.WarehouseCode = soLine.WarehouseCode;
                                line.WarehouseName = warehouse.WarehouseName;
                                //double availToPick = warehouseInfo.InStock - warehouseInfo.Committed;
                                //line.AvailToPick = availToPick > 0 ? availToPick : warehouseInfo.InStock;
                            }
                        }
                    }
                    else // PRODUCTION ORDER
                    {
                        var productionOrder = await connection.Request(EntitiesKeys.ProductionOrders, line.OrderEntry).GetAsync();

                        line.BaseDocNum = (int)productionOrder.DocumentNumber;
                        line.DueDate = productionOrder.DueDate.ToString();
                        line.PickStatus = line.PickStatus == string.Empty ? "ps_Picked" : line.PickStatus;

                        var prodLines = productionOrder.ProductionOrderLines;

                        foreach (var prodLine in prodLines)
                        {
                            if (prodLine.LineNumber == (int)line.OrderRowID)
                            {
                                Item item = await connection.Request(EntitiesKeys.Items, (string)prodLine.ItemNo).GetAsync<Item>();
                                Uom uom = await connection.Request(EntitiesKeys.UnitOfMeasurements, (int)prodLine.UoMEntry).GetAsync<Uom>();
                                Warehouse warehouse = await connection.Request(EntitiesKeys.Warehouses, (string)prodLine.Warehouse).GetAsync<Warehouse>();

                                var warehouseInfo = item.ItemWarehouseInfoCollection.Where(iwic => iwic.WarehouseCode == warehouse.WarehouseCode).FirstOrDefault()!;

                                line.ItemCode = (string)prodLine.ItemNo;
                                line.Description = (string)prodLine.ItemName;
                                line.UomCode = uom.Code;
                                line.UomName = uom.Name;
                                //line.ItemsPerUnit = item.SalesItemsPerUnit;
                                line.WarehouseCode = warehouse.WarehouseCode;
                                line.WarehouseName = warehouse.WarehouseName;
                                //double availToPick = warehouseInfo.InStock - warehouseInfo.Committed;
                                //line.AvailToPick = availToPick > 0 ? availToPick : warehouseInfo.InStock;
                            }
                        }
                    }
                    lines[j] = line;
                }

                result.PickListsLines = lines;

                Logger.CreateLog(false, "UPDATE PICK LIST", "SUCCESS", JsonConvert.SerializeObject(pickList));
                return new Response
                {
                    Status = "success",
                    Message = $"PL #{result.Absoluteentry} UPDATED successfully!",
                    Payload = result
                };

            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "UPDATE PICK LIST", ex.Message, JsonConvert.SerializeObject(pickList));

                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                };
            }
        });

        public async Task<Response> CreateAutomaticDeliveryAsync(int userId, string companyDB, SLPickList pickList) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                List<Document> deliveries = [];

                foreach (var pickLine in pickList.PickListsLines.Where(pll => pll.PickStatus != "ps_Closed").ToList())
                {
                    var soDocEntry = pickLine.OrderEntry;
                    var soDoc = await connection.Request(EntitiesKeys.Orders, soDocEntry).GetAsync<Document>();
                    var soLine = soDoc.DocumentLines.Find(docLine => docLine.LineNum == pickLine.LineNumber)!;

                    int drInd = deliveries.FindIndex(dr => dr.CardCode == soDoc.CardCode);
                    if (drInd > -1)
                    {
                        soLine.Quantity = pickLine.PickedQuantity;
                        soLine.BaseType = pickLine.BaseObjectType;
                        soLine.BaseEntry = pickLine.OrderEntry;
                        soLine.BaseLine = pickLine.OrderRowID;

                        if (deliveries[drInd].DocumentLines.Find(dl => dl.BaseEntry == soDoc.DocEntry) != null) 
                            deliveries[drInd].Comments += $",{soDoc.DocNum}";
                        deliveries[drInd].DocumentLines.Add(soLine);
                    }
                    else
                    {
                        Document drDoc = soDoc;
                        // Heeader
                        drDoc.Series = 6;
                        drDoc.Comments += $" Based on Sales Orders {soDoc.DocNum}";
                        drDoc.DocumentLines = [];

                        // Line
                        soLine.Quantity = pickLine.PickedQuantity;
                        soLine.BaseType = pickLine.BaseObjectType; 
                        soLine.BaseEntry = pickLine.OrderEntry;
                        soLine.BaseLine = pickLine.OrderRowID;

                        drDoc.DocEntry = 0;

                        drDoc.DocumentLines.Add(soLine);
                        deliveries.Add(drDoc);
                    }
                }


                List<string> drDocNums = [];
                List<Response> responses = [];
                foreach (var delivery in deliveries)
                {
                    try
                    {
                        delivery.Comments += ".";
                        await connection.Request(EntitiesKeys.DeliveryNotes).PostAsync(delivery);
                        var result = await connection.Request(EntitiesKeys.DeliveryNotes).OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                        var newDR = result.First();

                        Logger.CreateLog(false, "UPDATE PICK LIST", "SUCCESS", JsonConvert.SerializeObject(pickList));

                        responses.Add(new Response
                        {
                            Status = "success",
                            Message = $"DR {newDR.DocNum} CREATED successfully!",
                            Id = newDR.DocEntry,
                        });
                    }
                    catch (Exception ex)
                    {
                        Logger.CreateLog(true, "UPDATE PICK LIST", ex.Message, JsonConvert.SerializeObject(pickList));
                        responses.Add(new Response
                        {
                            Status = "failed",
                            Message = ex.Message
                        });
                    }
                }

                return new Response
                {
                    Status = "success",
                    Payload = responses
                };

            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "UPDATE PICK LIST", ex.Message, JsonConvert.SerializeObject(pickList));

                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                };
            }
        });

        public async Task<Response> GetNewIssueForProductionAsync(int userId, string companyDB, SLPickList pickList) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var seriesLies = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").Filter($"Series eq 20").Top(1).PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 60 } });

                var series = seriesLies[0];

                var issueForProduction = new IssueForProduction
                {
                    DocEntry = series.NextNumber,
                    DocNum = series.NextNumber,
                    Series = series.Series,
                    SeriesName = series.Name,
                    DocumentLines = [],
                };

                foreach (var pickListLine in pickList.PickListsLines)
                {
                    var productionOrder = await connection.Request(EntitiesKeys.ProductionOrders, pickListLine.OrderEntry).GetAsync<ProductionOrder>();

                    var prodOrdLine = productionOrder.ProductionOrderLines.Find(pol => pol.LineNumber == pickListLine.OrderRowID)!;

                    var item = await connection.Request(EntitiesKeys.Items, prodOrdLine.ItemNo).GetAsync<Item>();
                    var uom = await connection.Request(EntitiesKeys.UnitOfMeasurements, prodOrdLine.UomEntry).GetAsync<Uom>();

                    if (item.InventoryItem == "tYES")
                    {
                        issueForProduction.DocumentLines.Add(new IssueForProductionLine
                        {
                            BaseDoc = productionOrder.DocumentNumber,
                            BaseLine = prodOrdLine.LineNumber,
                            ItemCode = prodOrdLine.ItemNo,
                            Description = prodOrdLine.ItemName,
                            Quantity = pickListLine.PickedQuantity,
                            WarehouseCode = prodOrdLine.Warehouse,
                            UoMCode = uom.Code,
                            //
                            BaseType = pickListLine.BaseObjectType,
                            BaseEntry = pickListLine.OrderEntry,
                            UoMEntry = prodOrdLine.UomEntry
                        });
                    }
                }

                return new Response
                {
                    Status = "success",
                    Payload = issueForProduction
                };

            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "GET NEW ISSUE FOR PRODUCTION", ex.Message, JsonConvert.SerializeObject(pickList));

                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                };
            }
        });

        public async Task<Response> CreateIssueForProductionAsync(int userId, string companyDB, dynamic issueForProduction) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                
                await connection.Request(EntitiesKeys.InventoryGenExits).PostAsync(issueForProduction);
                var result = await connection.Request(EntitiesKeys.InventoryGenExits).OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                var newIFP = result.First();

                Logger.CreateLog(false, "CREATE ISSUE FOR PRODUCTION", "SUCCESS", JsonConvert.SerializeObject(issueForProduction));
                return new Response
                {
                    Status = "success",
                    Message = $"IFP #{newIFP.DocNum} CREATED successfully!",
                };

            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE ISSUE FOR PRODUCTION", ex.Message, JsonConvert.SerializeObject(issueForProduction));

                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                };
            }
        });

        public async Task<Response> GetPickListDeliveryAsync(int userId, string companyDB, int absEntry, string orderEntries) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                PickList pickList = await connection.Request(EntitiesKeys.PickLists, absEntry).GetAsync<PickList>();
                List<int> soDocEntries = orderEntries.Split(',').Select(oe => int.Parse(oe)).ToList();
                var pickListLines = pickList.PickListsLines.Where(pll => soDocEntries.Contains(pll.OrderEntry) && pll.PickedQuantity > 0).ToList();
                List<dynamic> soDocs = [];
                dynamic? drDoc = null;
                JArray documentLines = [];

                foreach (var pickLine in pickListLines)
                {
                    if (!soDocs.Any(sd => sd.DocEntry == pickLine.OrderEntry))
                    {
                        dynamic soDoc = await connection.Request(EntitiesKeys.Orders, pickLine.OrderEntry).GetAsync<dynamic>();

                        if (drDoc == null)
                        {
                            drDoc = soDoc;
                            drDoc["DocEntry"] = 0;
                            drDoc["Series"] = 6;
                            drDoc["DocDate"] = DateTime.Now.ToString();
                            drDoc["DocDueDate"] = DateTime.Now.ToString();
                            drDoc["TaxDate"] = DateTime.Now.ToString();
                            drDoc["Comments"] += $" Based on Sales Orders {soDoc.DocNum}";
                        }
                        //else
                        //{
                            //drDoc!["Comments"] += $",{soDoc["DocNum"]}";
                        //}

                        foreach (dynamic soLine in soDoc["DocumentLines"])
                        {
                           if (soLine["LineNum"] == pickLine.OrderRowID)
                            {
                                dynamic line = soLine;
                                line["Quantity"] = pickLine.PickedQuantity;
                                line["BaseType"] = pickLine.BaseObjectType;
                                line["BaseEntry"] = pickLine.OrderEntry;
                                line["BaseLine"] = pickLine.OrderRowID;
                                line["CostingCode4"] = soDoc.DocNum.ToString();
                                documentLines.Add(line);
                            }
                        }
                        soDocs.Add(soDoc);
                    }
                    else
                    {
                        dynamic soDoc = soDocs.Find(sd => sd["DocEntry"] == pickLine.OrderEntry)!;

                        foreach (dynamic soLine in soDoc["DocumentLines"])
                        {
                            if (soLine["LineNum"] == pickLine.OrderRowID)
                            {
                                dynamic line = soLine;
                                line["Quantity"] = pickLine.PickedQuantity;
                                line["BaseType"] = pickLine.BaseObjectType;
                                line["BaseEntry"] = pickLine.OrderEntry;
                                line["BaseLine"] = pickLine.OrderRowID;
                                line["CostingCode4"] = soDoc.DocNum.ToString();
                                documentLines.Add(line);
                            }
                        }
                    }
                }

                drDoc!["DocumentLines"] = documentLines;

                Logger.CreateLog(false, "GET PICK LIST DELIVERY", "SUCCESS", JsonConvert.SerializeObject(new
                {
                    AbsEntry = absEntry,
                    OrderEntries = orderEntries
                }));


                return new Response
                {
                    Status = "success",
                    Payload = drDoc,
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "GET PICK LIST DELIVERY", ex.Message, JsonConvert.SerializeObject(new
                {
                    AbsEntry = absEntry,
                    OrderEntries = orderEntries
                }));

                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                };
            }
        });
    }
}
