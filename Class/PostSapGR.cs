﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEV_Z_GOODSMVT_CREATE1.Class;

namespace SAP_Batch_GR_TR.Class
{
    class PostSapGR
    {
        public void PostSapGRClass(string partno, int Qty, string Custid, string Store, string postdate, string headerText)  //List<ZsgmDetail> zsgms, List<T_barcode_trans> t_Barcodes, string Kanban
        {
            //example from excel postdate = 2022-09-01
            try
            {
                partno = partno.Trim();
                BarcodeEntities db = new BarcodeEntities();

                TKC_MASTER_SAPEntities Master = new TKC_MASTER_SAPEntities();
                //var ws_service = new PRD_Z_GOODSMVT_CREATE.Z_GOODSMVT_CREATE_SRV();
                //var ws_res = new PRD_Z_GOODSMVT_CREATE.ZGoodsmvtCreateResponse();
                //var ws_fn_head = new PRD_Z_GOODSMVT_CREATE.ZsgmHeader();
                //var ws_fn_det = new PRD_Z_GOODSMVT_CREATE.ZsgmDetail();
                var ws_service = new Z_GOODSMVT_CREATE1_SRV();
                var ws_res = new ZGoodsmvtCreate1Response();
                var ws_fn_head = new ZsgmHeader();
                var ws_fn_det = new ZsgmDetail1();
                var GmCode = new Bapi2017GmCode();
                ws_fn_head.BillOfLading = "";
                ws_fn_head.DocDate = DateTime.Now.ToString("yyyyMMdd");
                ws_fn_head.GrGiSlipNo = "";
                string UserID = "";
                if (headerText.Contains("|"))
                {
                    UserID = headerText.Split('|')[0];
                    headerText = headerText.Split('|')[1];
                }

                var RefdocNo = headerText;
                ws_fn_head.HeaderTxt = headerText;//"ADDSTOCKBYDEV";
                //ws_fn_head.PstngDate = "20220901";
                ws_fn_head.PstngDate = postdate.Replace("-", "");
                //ws_fn_head.PstngDate = DateTime.Now.ToString("yyyyMM01");
                var Storage_GR = Master.T_LOCATION_SAP.ToList();
                GmCode.GmCode = "05";

                var Time = DateTime.Now.ToString("yyyy-MM-dd");
                var _Time = Time.Split('-');
                var year = _Time[0].Substring(_Time[0].Length - 1);
                var Month = abcBase36(Convert.ToInt32(_Time[1]));
                var Day = abcBase36(Convert.ToInt32(_Time[2]));

                List<ZsgmDetail1> Detail_GR = new List<ZsgmDetail1>();

                var StgLoc_GR = Storage_GR.Where(x => x.LOC_SAP_ID == Store).FirstOrDefault();

                var Lot = Custid + year + Month + Day + "N00";
                ZsgmDetail1 temp = new ZsgmDetail1();
                temp.Batch = "DUMMYBATCH";
                temp.EntryQnt = Convert.ToDecimal(Qty);
                temp.EntryUom = "Pcs";
                temp.FacNo = StgLoc_GR.FAC.Trim();
                temp.Material = partno;
                temp.StgeLoc = Store;
                temp.MoveType = "521";
                temp.Plant = StgLoc_GR.PLANT;
                temp.Custid = Custid; //tmp.CUSTID;
                temp.Kanban = ""; //tmp.KANBANID;
                Detail_GR.Add(temp);

                T_LOG_ADD_STOCK _STOCK = new T_LOG_ADD_STOCK();
                _STOCK.Batch = "DUMMYBATCH";
                _STOCK.EntryQnt = Qty;
                _STOCK.EntryUom = "Pcs";
                _STOCK.FacNo = StgLoc_GR.FAC.Trim();
                _STOCK.Material = partno + "|" + UserID;
                _STOCK.StgeLoc = Store;
                _STOCK.MoveType = "521";
                _STOCK.Plant = StgLoc_GR.PLANT;
                _STOCK.Custid = Custid; //tmp.CUSTID;
                _STOCK.Kanban = ""; //tmp.KANBANID;
                _STOCK.StockDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                _STOCK.UpdDate = DateTime.Now;

                List<ZsgmDetail1> result = new List<ZsgmDetail1>();

                result = Detail_GR.GroupBy(l => l.Kanban)
                                  .Select(cl => new ZsgmDetail1
                                  {
                                      Batch = cl.First().Batch,
                                      Material = cl.First().Material,
                                      EntryQnt = cl.Sum(c => c.EntryQnt),
                                      EntryUom = cl.First().EntryUom,
                                      FacNo = cl.First().FacNo,
                                      StgeLoc = cl.First().StgeLoc,
                                      MoveStloc = cl.First().MoveStloc,
                                      MoveType = cl.First().MoveType,
                                      Plant = cl.First().Plant,
                                      SoldTo = cl.First().SoldTo,
                                      Custid = cl.First().Custid,
                                      Kanban = cl.First().Kanban,
                                  }).ToList();

                var ws_fn_partosap = new ZGoodsmvtCreate1();
                ws_fn_partosap.IsHeader = ws_fn_head;
                ws_fn_partosap.ItDetail = result.ToArray();
                ws_fn_partosap.IGoodsmvtCode = GmCode;
                //ส่งไปให้ SAP
                ws_res = ws_service.ZGoodsmvtCreate1(ws_fn_partosap);


                var Log_Gr = new List<T_LOG_GR_STOCK>();
                var Log_Error = new List<T_LOG_STOCK_ERROR>();

                if (ws_res.ItDetail.Count() > 0)
                {
                    foreach (var item in ws_res.ItDetail)
                    {
                        if (string.IsNullOrEmpty(item.Error) && !string.IsNullOrEmpty(ws_res.EMaterailDoc.MatDoc)
                            )
                        {
                            T_LOG_GR_STOCK __STOCK = new T_LOG_GR_STOCK();
                            __STOCK.Batch = item.Batch;
                            __STOCK.EntryQnt = (int)item.EntryQnt;
                            __STOCK.EntryUom = item.EntryUom;
                            __STOCK.FacNo = item.FacNo;
                            __STOCK.Material = item.Material;
                            if (string.IsNullOrEmpty(item.MoveStloc))
                            {
                                __STOCK.StgeLoc = item.StgeLoc;

                            }
                            else
                            {
                                __STOCK.StgeLoc = item.StgeLoc + "|" + item.MoveStloc;
                            }
                            __STOCK.MoveType = item.MoveType;
                            if (string.IsNullOrEmpty(item.MovePlant))
                            {
                                __STOCK.Plant = item.Plant;

                            }
                            else
                            {
                                __STOCK.Plant = item.Plant + "|" + item.MovePlant;

                            }
                            __STOCK.Custid = item.Custid; //tmp.CUSTID;
                            __STOCK.Kanban = item.Kanban; //tmp.KANBANID;
                            __STOCK.StockDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                            __STOCK.UpdDate = DateTime.Now;
                            __STOCK.DocMat = ws_res.EMaterailDoc.MatDoc + "|" + UserID;
                            __STOCK.EMessage = "ADDSTOCKBYEXCEL : " + ws_res.EMessage;
                            Log_Gr.Add(__STOCK);
                        }
                        else// case error
                        {
                            T_LOG_STOCK_ERROR errorLog = new T_LOG_STOCK_ERROR();
                            errorLog.RefDocNo = RefdocNo + "|" + UserID;
                            errorLog.Batch = item.Batch;
                            errorLog.Material = item.Material;
                            errorLog.EntryQnt = (int)item.EntryQnt;
                            errorLog.EntryUom = item.EntryUom;
                            errorLog.FacNo = item.FacNo;
                            if (string.IsNullOrEmpty(item.MoveStloc))
                            {
                                errorLog.StgeLoc = item.StgeLoc;
                            }
                            else
                            {
                                errorLog.StgeLoc = item.StgeLoc + "|" + item.MoveStloc;
                            }

                            errorLog.MoveType = item.MoveType;
                            if (string.IsNullOrEmpty(item.MovePlant))
                            {
                                errorLog.Plant = item.Plant;

                            }
                            else
                            {
                                errorLog.Plant = item.Plant + "|" + item.MovePlant;
                            }
                            errorLog.Custid = item.Custid;
                            errorLog.Kanban = item.Kanban;
                            errorLog.StockDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                            errorLog.UpdDate = DateTime.Now;
                            errorLog.EMessage = item.Error == "" ? "ADDSTOCKBYEXCEL : " + ws_res.EMessage : "ADDSTOCKBYEXCEL : " + item.Error;

                            Log_Error.Add(errorLog);
                        }
                    }

                    if (Log_Gr.Count > 0)
                        db.T_LOG_GR_STOCK.AddRange(Log_Gr);

                    if (Log_Error.Count > 0)
                        db.T_LOG_STOCK_ERROR.AddRange(Log_Error);

                    db.SaveChanges();

                }

                var msg = ws_res.EMaterailDoc.MatDoc + "|" + ws_res.EMaterailDoc.DocYear + "|" + ws_res.EMessage;
                linePrd("GR PROGRAMMER 521 \n" + msg);

                if (ws_res.EMaterailDoc.MatDoc != "")
                {
                    _STOCK.DocMat = ws_res.EMaterailDoc.MatDoc;
                    db.T_LOG_ADD_STOCK.Add(_STOCK);
                    db.SaveChanges();
                }
                return msg;
            }
            catch (Exception ex)
            {
                linePrd("GR PROGRAMMER 521 \n" + ex.ToString());
                return "Fail";
            }
        }
    }
}
