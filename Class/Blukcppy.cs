using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Configuration;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Drawing;



namespace PostSap_GR_TR.Class
{
    class Blukcppy
    {
        internal void FNBlukcppy(int SL)
        {
            AssignOwnerCash(SL);
        }

        public string AssignOwnerCash(int lot)
        {

            try
            {
                //ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
                //string connString = "";
                //if (setting != null)
                //{
                //    SQLConString = setting.ConnectionString;
                //}
                //SqlConnection oConn = new SqlConnection(SQLConString);

                SqlConnection oConn;
                string SQLConnectionStr = "MyConn";
                string SQLConString;
                SQLConString = ConfigurationManager.ConnectionStrings[SQLConnectionStr].ToString();
                Console.WriteLine(SQLConString);

                SQLConString = SQLConString + "MultipleActiveResultSets=True;";
                oConn = new SqlConnection(SQLConString);
                oConn.Open();
                Console.WriteLine("SQL Connection Opened, OK");

                string queryStr = "";
                string queryStrMgr = "";
                string queryStrCMT = "";
                string queryStrCMTTrain = "";


                //------------ Fetch Implement Cash owner list -------------------
                queryStr = "SELECT cmt_ctrl_users.userID, userName, displayName, cmt_ctrl_user_roles.userRoleID " +
                "FROM dbo.cmt_ctrl_users, dbo.cmt_ctrl_user_roles " +
                "WHERE dbo.cmt_ctrl_users.userID = dbo.cmt_ctrl_user_roles.userID " +
                "AND cmt_ctrl_user_roles.RoleID = 2 AND cmt_ctrl_user_roles.systemID = 2";
                SqlCommand command = new SqlCommand(queryStr, oConn);
                command.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                SqlDataReader reader = command.ExecuteReader();
                //DataTable dt = new DataTable();
                List<string> OwnerID_rr = new List<string>();
                List<string> OwnerUser_rr = new List<string>();
                List<string> OwnerName_rr = new List<string>();
                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        OwnerID_rr.Add(reader[0].ToString());
                        OwnerUser_rr.Add(reader[1].ToString());
                        OwnerName_rr.Add(reader[2].ToString());
                    }
                }
                else
                {
                    OwnerID_rr.Add("");
                    OwnerUser_rr.Add("");
                    OwnerName_rr.Add("");
                }
                int Owner_Count = OwnerID_rr.Count;
                if (Owner_Count == 0)
                {
                    return "Invalid Implement role setting for support";
                }
                reader.Close();

                //------------ Fetch CMT owner list -------------------
                queryStrCMT = "SELECT cmt_ctrl_users.userID, userName, displayName, cmt_ctrl_user_roles.userRoleID " +
                "FROM dbo.cmt_ctrl_users, dbo.cmt_ctrl_user_roles " +
                "WHERE dbo.cmt_ctrl_users.userID = dbo.cmt_ctrl_user_roles.userID " +
                "AND cmt_ctrl_user_roles.RoleID = 1";
                SqlCommand CMTcommand = new SqlCommand(queryStrCMT, oConn);
                CMTcommand.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                SqlDataReader readerCMT = CMTcommand.ExecuteReader();
                List<string> CMTID = new List<string>();
                List<string> CMTUser = new List<string>();
                List<string> CMTName = new List<string>();
                if (readerCMT.HasRows)
                {
                    while (readerCMT.Read())
                    {
                        CMTID.Add(readerCMT[0].ToString());
                        CMTUser.Add(readerCMT[1].ToString());
                        CMTName.Add(readerCMT[2].ToString());
                    }
                }
                else
                {
                    CMTID.Add("");
                    CMTUser.Add("");
                    CMTName.Add("");
                }
                int CMT_Count = CMTID.Count;
                if (CMT_Count == 0)
                {
                    return "Invalid Inquiry role setting for support";
                }
                readerCMT.Close();

                //------------ Fetch CMT trainer list -------------------
                queryStrCMTTrain = "SELECT cmt_ctrl_users.userID, userName, displayName, cmt_ctrl_user_roles.userRoleID " +
                "FROM dbo.cmt_ctrl_users, dbo.cmt_ctrl_user_roles " +
                "WHERE dbo.cmt_ctrl_users.userID = dbo.cmt_ctrl_user_roles.userID " +
                "AND cmt_ctrl_user_roles.RoleID = 3";
                SqlCommand CMTTcommand = new SqlCommand(queryStrCMTTrain, oConn);
                CMTTcommand.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                SqlDataReader readerCMTT = CMTTcommand.ExecuteReader();
                List<string> CMTTID = new List<string>();
                List<string> CMTTUser = new List<string>();
                List<string> CMTTName = new List<string>();
                if (readerCMTT.HasRows)
                {
                    while (readerCMTT.Read())
                    {
                        CMTTID.Add(readerCMTT[0].ToString());
                        CMTTUser.Add(readerCMTT[1].ToString());
                        CMTTName.Add(readerCMTT[2].ToString());
                    }
                }
                else
                {
                    CMTTID.Add("");
                    CMTTUser.Add("");
                    CMTTName.Add("");
                }
                int CMTT_Count = CMTTID.Count;
                if (CMTT_Count == 0)
                {
                    return "Invalid Trainer role setting for support";
                }
                readerCMTT.Close();

                //------------ Fetch CMT Manager list -------------------
                queryStrMgr = "SELECT cmt_ctrl_users.userID, userName, displayName, cmt_ctrl_user_roles.userRoleID " +
                    "FROM dbo.cmt_ctrl_users, dbo.cmt_ctrl_user_roles " +
                    "WHERE dbo.cmt_ctrl_users.userID = dbo.cmt_ctrl_user_roles.userID " +
                    "AND cmt_ctrl_user_roles.RoleID = 6 AND (cmt_ctrl_user_roles.systemID = 2 OR cmt_ctrl_user_roles.systemID = 3)";
                SqlCommand Mgrcommand = new SqlCommand(queryStrMgr, oConn);
                Mgrcommand.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                SqlDataReader readerMgr = Mgrcommand.ExecuteReader();
                List<string> MgrID = new List<string>();
                List<string> MgrUser = new List<string>();
                List<string> MgrName = new List<string>();
                if (readerMgr.HasRows)
                {
                    while (readerMgr.Read())
                    {
                        MgrID.Add(readerMgr[0].ToString());
                        MgrUser.Add(readerMgr[1].ToString());
                        MgrName.Add(readerMgr[2].ToString());
                    }
                }
                else
                {
                    MgrID.Add("");
                    MgrUser.Add("");
                    MgrName.Add("");
                }
                int Mgr_Count = MgrID.Count;
                if (Mgr_Count == 0)
                {
                    return "Invalid Manager role setting for support";
                }
                readerMgr.Close();

                //BBL_K2_CUSTOMERS Cash TT
                string queryStrCustTT = "SELECT RM_CONTACT_TEL1," +
                "CONTACT_EMAIL2," +
                "NO_OF_USERS," +
                "RM_CONTACT_EMAIL2," +
                "CONTACT_ADDR," + //4
                "CONTACT_MOBILE1," +
                "CONTACT_MOBILE2," +
                "ACCT_MGR," +
                "COMP_ENG_NAME," +
                "COMP_TH_NAME," + //9
                "RM_CONTACT_TEL2," +
                "CONTACT_EMAIL1," +
                "CONTACT_TEL1," +
                "CONTACT_TEL2," +
                "MIGRATION_LOT," + //14
                "CONTACT_PERSON_NAME," +
                "RM_REGIS_EMAIL1," +
                "RM_REGIS_EMAIL2," +
                "RM_REGIS_TEL2," +
                "RM_CONTACT_EMAIL1," +
                "CASH_SALES_MGR," + //20
                "CASH_SALES_TEAM," +
                "TOP_TIER_FLAG," +
                "HOLDING_COMP," + //23
                "RM_REGIS_MOBILE1," +
                "RM_REGIS_MOBILE2," +
                "RM_CONTACT_ADDR," +
                "RM_CONTACT_MOBILE2," +
                "DEBIT_FEE_ACCT," +
                "RECORD_TYPE," +
                "COMP_CODE," + //30
                "RM_REGIS_TEL1," +
                "ID_NO," +
                "RM_REGIS_ADDR," +
                "RM_CONTACT_MOBILE1, " +
                "SYS_CODE," +
                "rec_ID FROM dbo.BBL_K2_CUSTOMERS WHERE TOP_TIER_FLAG = 'Y' AND MIGRATION_LOT = " + lot;

                SqlCommand commandCustTT = new SqlCommand(queryStrCustTT, oConn);
                commandCustTT.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                SqlDataReader readerCustTT = commandCustTT.ExecuteReader();
                //DataTable dt = new DataTable();

                string query = "";
                if (readerCustTT.HasRows)
                {
                    int rr = 0;
                    string customerTypeID = "0";
                    string customerType = "";
                    string accMgrID = "0";
                    string accMgrName = "";
                    string saleMgrID = "0";
                    string saleMgrName = "";
                    string saleTeamID = "0";
                    string CHName = "";
                    string CHID = "0";


                    //map data base 
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("custID");
                    dataTable.Columns.Add("customerTypeID");
                    dataTable.Columns.Add("customerType");
                    dataTable.Columns.Add("systemID");
                    dataTable.Columns.Add("systemName");
                    dataTable.Columns.Add("channelID");
                    dataTable.Columns.Add("channelName");
                    dataTable.Columns.Add("compCode");
                    dataTable.Columns.Add("batchNo");
                    dataTable.Columns.Add("juristicNo");
                    dataTable.Columns.Add("taxIDNo");
                    dataTable.Columns.Add("companyNamwEN");
                    dataTable.Columns.Add("companyNameTH");
                    dataTable.Columns.Add("holdingCompany");
                    dataTable.Columns.Add("NoOfUsers");
                    dataTable.Columns.Add("contactPersonName");
                    dataTable.Columns.Add("RMRegistedAddress");
                    dataTable.Columns.Add("RMRegistedEmail1");
                    dataTable.Columns.Add("RMRegistedEmail2");
                    dataTable.Columns.Add("RMRegistedTelephone1");
                    dataTable.Columns.Add("RMRegistedTelephone2");
                    dataTable.Columns.Add("RMRegistedMobile1");
                    dataTable.Columns.Add("RMRegistedMobile2");
                    dataTable.Columns.Add("RMContactAddress");
                    dataTable.Columns.Add("RMContactEmail1");
                    dataTable.Columns.Add("RMContactEmail2");
                    dataTable.Columns.Add("RMContactTelephone1");
                    dataTable.Columns.Add("RMContactTelephone2");
                    dataTable.Columns.Add("RMContactMobile1");
                    dataTable.Columns.Add("RMContactMobile2");
                    dataTable.Columns.Add("contactAddress");
                    dataTable.Columns.Add("contactEmail1");
                    dataTable.Columns.Add("contactEmail2");
                    dataTable.Columns.Add("contactTelephone1");
                    dataTable.Columns.Add("contactTelephone2");
                    dataTable.Columns.Add("contactMobile1");
                    dataTable.Columns.Add("contactMobile2");
                    dataTable.Columns.Add("accountMgrID");
                    dataTable.Columns.Add("accountMgr");
                    dataTable.Columns.Add("salesMgrID");
                    dataTable.Columns.Add("cashSalesMgr");
                    dataTable.Columns.Add("cashSalesTeam");
                    dataTable.Columns.Add("topTierFlag");
                    dataTable.Columns.Add("debitFeeAccount");
                    dataTable.Columns.Add("cmtOwnerID");
                    dataTable.Columns.Add("cmtOwner");
                    dataTable.Columns.Add("trainerOwnerID");
                    dataTable.Columns.Add("trainerOwner");
                    dataTable.Columns.Add("RMName");
                    dataTable.Columns.Add("customerStatus");
                    dataTable.Columns.Add("cmtManagerID");
                    dataTable.Columns.Add("cmtManager");
                    dataTable.Columns.Add("modifyBy");
                    dataTable.Columns.Add("modifyDate");
                    dataTable.Columns.Add("courseID");
                    dataTable.Columns.Add("CourseNo");
                    dataTable.Columns.Add("directEmail");
                    dataTable.Columns.Add("cashSalesTeamID");
                    dataTable.Columns.Add("by");
                    dataTable.Columns.Add("adminNameTH");
                    dataTable.Columns.Add("adminNameEN");
                    dataTable.Columns.Add("adminIDNo");
                    dataTable.Columns.Add("adminAddress");
                    dataTable.Columns.Add("adminEmail");
                    dataTable.Columns.Add("adminTelMobile");
                    dataTable.Columns.Add("adminFax");
                    dataTable.Columns.Add("adminWishService");
                    dataTable.Columns.Add("NoOfTrainingUsers");
                    dataTable.Columns.Add("trainingTypeID");
                    dataTable.Columns.Add("trainingTypeName");
                    dataTable.Columns.Add("personType");
                    dataTable.Columns.Add("faxNo");
                    dataTable.Columns.Add("boardAuthority");
                    dataTable.Columns.Add("boardName");
                    dataTable.Columns.Add("allocation");
                    dataTable.Columns.Add("currentAccount");
                    dataTable.Columns.Add("bcLocation");
                    dataTable.Columns.Add("bcName");
                    dataTable.Columns.Add("tradeSaleName");
                    dataTable.Columns.Add("tradeSaleEmail");
                    dataTable.Columns.Add("tradeSaleTel");
                    dataTable.Columns.Add("ROName");
                    dataTable.Columns.Add("ROEmail");
                    dataTable.Columns.Add("ROTel");
                    dataTable.Columns.Add("DuplicateCashCheck");
                    dataTable.Columns.Add("contactAddressTH");
                    dataTable.Columns.Add("RMContactName");

                    while (readerCustTT.Read())
                    {
                        customerTypeID = "1";
                        customerType = "Toptier";

                        if (rr == (Owner_Count))
                        {
                            rr = 0;
                        }
                        string noofuser = readerCustTT[2].ToString();
                        if (noofuser == "")
                        {
                            noofuser = "0";
                        }
                        if (readerCustTT[35].ToString() == "ICH")
                        {
                            CHName = "iCash";
                            CHID = "2";
                        }
                        else if (readerCustTT[35].ToString() == "BIZ")
                        {
                            CHName = "Biz iBanking";
                            CHID = "3";
                        }

                        // Assuming 'dataTable' is your DataTable object and 'readerCustTT' is your data reader

                        DataRow newRow = dataTable.NewRow();

                        newRow["customerTypeID"] = Convert.ToInt32(customerTypeID);
                        newRow["customerType"] = customerType;
                        newRow["systemID"] = 2;
                        newRow["systemName"] = "Cash";
                        newRow["channelID"] = Convert.ToInt32(CHID);
                        newRow["channelName"] = CHName;
                        newRow["compCode"] = readerCustTT[30].ToString().Replace("'", "''");
                        newRow["batchNo"] = Convert.ToInt32(lot);
                        newRow["juristicNo"] = readerCustTT[32].ToString().Replace("'", "''");
                        newRow["taxIDNo"] = "";
                        newRow["companyNamwEN"] = readerCustTT[8].ToString().Replace("'", "''");
                        newRow["companyNameTH"] = readerCustTT[9].ToString().Replace("'", "''");
                        newRow["holdingCompany"] = readerCustTT[23].ToString().Replace("'", "''");
                        newRow["NoOfUsers"] = Convert.ToInt32(noofuser);
                        newRow["contactPersonName"] = readerCustTT[15].ToString().Replace("'", "''");
                        newRow["RMRegistedAddress"] = readerCustTT[33].ToString().Replace("'", "''");
                        newRow["RMRegistedEmail1"] = readerCustTT[16].ToString().Replace("'", "''");
                        newRow["RMRegistedEmail2"] = readerCustTT[17].ToString().Replace("'", "''");
                        newRow["RMRegistedTelephone1"] = readerCustTT[31].ToString().Replace("'", "''");
                        newRow["RMRegistedTelephone2"] = readerCustTT[18].ToString().Replace("'", "''");
                        newRow["RMRegistedMobile1"] = readerCustTT[24].ToString().Replace("'", "''");
                        newRow["RMRegistedMobile2"] = readerCustTT[25].ToString().Replace("'", "''");
                        newRow["RMContactAddress"] = readerCustTT[26].ToString().Replace("'", "''");
                        newRow["RMContactEmail1"] = readerCustTT[19].ToString().Replace("'", "''");
                        newRow["RMContactEmail2"] = readerCustTT[3].ToString().Replace("'", "''");
                        newRow["RMContactTelephone1"] = readerCustTT[0].ToString().Replace("'", "''");
                        newRow["RMContactTelephone2"] = readerCustTT[10].ToString().Replace("'", "''");
                        newRow["RMContactMobile1"] = readerCustTT[34].ToString().Replace("'", "''");
                        newRow["RMContactMobile2"] = readerCustTT[27].ToString().Replace("'", "''");
                        newRow["contactAddress"] = readerCustTT[4].ToString().Replace("'", "''");
                        newRow["contactEmail1"] = readerCustTT[11].ToString().Replace("'", "''");
                        newRow["contactEmail2"] = readerCustTT[1].ToString().Replace("'", "''");
                        newRow["contactTelephone1"] = readerCustTT[12].ToString().Replace("'", "''");
                        newRow["contactTelephone2"] = readerCustTT[13].ToString().Replace("'", "''");
                        newRow["contactMobile1"] = readerCustTT[5].ToString().Replace("'", "''");
                        newRow["contactMobile2"] = readerCustTT[6].ToString().Replace("'", "''");
                        newRow["accountMgrID"] = Convert.ToInt32(accMgrID);
                        newRow["accountMgr"] = accMgrName;
                        newRow["salesMgrID"] = Convert.ToInt32(saleMgrID);
                        newRow["cashSalesMgr"] = saleMgrName;
                        newRow["by"] = readerCustTT[21].ToString().Replace("'", "''");
                        newRow["topTierFlag"] = 0;
                        newRow["debitFeeAccount"] = "";
                        newRow["cmtOwnerID"] = Convert.ToInt32(OwnerID_rr[rr]);
                        newRow["cmtOwner"] = OwnerUser_rr[rr];
                        newRow["trainerOwnerID"] = DBNull.Value;
                        newRow["trainerOwner"] = "";
                        newRow["RMName"] = "";
                        newRow["courseID"] = DBNull.Value;
                        newRow["CourseNo"] = "";
                        newRow["customerStatus"] = "Active";
                        newRow["cmtManagerID"] = Convert.ToInt32(MgrID[0]);
                        newRow["cmtManager"] = MgrUser[0];
                        newRow["modifyBy"] = MgrName[0];
                        newRow["modifyDate"] = DateTime.Now;
                        newRow["directEmail"] = DBNull.Value; 
                        newRow["cashSalesTeamID"] = Convert.ToInt32(saleTeamID);
                        newRow["NoOfTrainingUsers"] = 2;
                        newRow["trainingTypeID"] = 0;
                        newRow["trainingTypeName"] = "";

                        dataTable.Rows.Add(newRow);
                        rr++;
                        query = "ok";
                    }

                    // Mark all rows as Unchanged
                    dataTable.AcceptChanges();
                    readerCustTT.Close();

                    string res = "";


                    res = CopyData(oConn, "dbo.cmt_i_customer_profile", dataTable);
                    if (res == "Pass")
                    {
                        Console.WriteLine("cmt_i_customer_profile 1 SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to cmt_i_customer_profile[" + res + "]";
                    }
                }


                //BBL_K2_CUSTOMERS Cash Std
                string queryStrCustStd = "SELECT RM_CONTACT_TEL1," +
                "CONTACT_EMAIL2," +
                "NO_OF_USERS," +
                "RM_CONTACT_EMAIL2," +
                "CONTACT_ADDR," + //4
                "CONTACT_MOBILE1," +
                "CONTACT_MOBILE2," +
                "ACCT_MGR," +
                "COMP_ENG_NAME," +
                "COMP_TH_NAME," + //9
                "RM_CONTACT_TEL2," +
                "CONTACT_EMAIL1," +
                "CONTACT_TEL1," +
                "CONTACT_TEL2," +
                "MIGRATION_LOT," + //14
                "CONTACT_PERSON_NAME," +
                "RM_REGIS_EMAIL1," +
                "RM_REGIS_EMAIL2," +
                "RM_REGIS_TEL2," +
                "RM_CONTACT_EMAIL1," +
                "CASH_SALES_MGR," + //20
                "CASH_SALES_TEAM," +
                "TOP_TIER_FLAG," +
                "HOLDING_COMP," + //23
                "RM_REGIS_MOBILE1," +
                "RM_REGIS_MOBILE2," +
                "RM_CONTACT_ADDR," +
                "RM_CONTACT_MOBILE2," +
                "DEBIT_FEE_ACCT," +
                "RECORD_TYPE," +
                "COMP_CODE," + //30
                "RM_REGIS_TEL1," +
                "ID_NO," +
                "RM_REGIS_ADDR," +
                "RM_CONTACT_MOBILE1," +
                "SYS_CODE," +
                "rec_ID FROM dbo.BBL_K2_CUSTOMERS WHERE (TOP_TIER_FLAG = 'N' OR TOP_TIER_FLAG is NULL) AND MIGRATION_LOT = " + lot;

                SqlCommand commandCustStd = new SqlCommand(queryStrCustStd, oConn);
                commandCustStd.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                SqlDataReader readerCustStd = commandCustStd.ExecuteReader();

                if (readerCustStd.HasRows)
                {
                    int rr = 0;
                    int rrt = 0;
                    string customerTypeID = "0";
                    string customerType = "";
                    string accMgrID = "0";
                    string accMgrName = "";
                    string saleMgrID = "0";
                    string saleMgrName = "";
                    string saleTeamID = "0";
                    string CHName = "";
                    string CHID = "0";

                    // Assuming 'dataTable' is your DataTable object
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("custID");
                    dataTable.Columns.Add("customerTypeID");
                    dataTable.Columns.Add("customerType");
                    dataTable.Columns.Add("systemID");
                    dataTable.Columns.Add("systemName");
                    dataTable.Columns.Add("channelID");
                    dataTable.Columns.Add("channelName");
                    dataTable.Columns.Add("compCode");
                    dataTable.Columns.Add("batchNo");
                    dataTable.Columns.Add("juristicNo");
                    dataTable.Columns.Add("taxIDNo");
                    dataTable.Columns.Add("companyNamwEN");
                    dataTable.Columns.Add("companyNameTH");
                    dataTable.Columns.Add("holdingCompany");
                    dataTable.Columns.Add("NoOfUsers");
                    dataTable.Columns.Add("contactPersonName");
                    dataTable.Columns.Add("RMRegistedAddress");
                    dataTable.Columns.Add("RMRegistedEmail1");
                    dataTable.Columns.Add("RMRegistedEmail2");
                    dataTable.Columns.Add("RMRegistedTelephone1");
                    dataTable.Columns.Add("RMRegistedTelephone2");
                    dataTable.Columns.Add("RMRegistedMobile1");
                    dataTable.Columns.Add("RMRegistedMobile2");
                    dataTable.Columns.Add("RMContactAddress");
                    dataTable.Columns.Add("RMContactEmail1");
                    dataTable.Columns.Add("RMContactEmail2");
                    dataTable.Columns.Add("RMContactTelephone1");
                    dataTable.Columns.Add("RMContactTelephone2");
                    dataTable.Columns.Add("RMContactMobile1");
                    dataTable.Columns.Add("RMContactMobile2");
                    dataTable.Columns.Add("contactAddress");
                    dataTable.Columns.Add("contactEmail1");
                    dataTable.Columns.Add("contactEmail2");
                    dataTable.Columns.Add("contactTelephone1");
                    dataTable.Columns.Add("contactTelephone2");
                    dataTable.Columns.Add("contactMobile1");
                    dataTable.Columns.Add("contactMobile2");
                    dataTable.Columns.Add("accountMgrID");
                    dataTable.Columns.Add("accountMgr");
                    dataTable.Columns.Add("salesMgrID");
                    dataTable.Columns.Add("cashSalesMgr");
                    dataTable.Columns.Add("cashSalesTeam");
                    dataTable.Columns.Add("topTierFlag");
                    dataTable.Columns.Add("debitFeeAccount");
                    dataTable.Columns.Add("cmtOwnerID");
                    dataTable.Columns.Add("cmtOwner");
                    dataTable.Columns.Add("trainerOwnerID");
                    dataTable.Columns.Add("trainerOwner");
                    dataTable.Columns.Add("RMName");
                    dataTable.Columns.Add("customerStatus");
                    dataTable.Columns.Add("cmtManagerID");
                    dataTable.Columns.Add("cmtManager");
                    dataTable.Columns.Add("modifyBy");
                    dataTable.Columns.Add("modifyDate");
                    dataTable.Columns.Add("courseID");
                    dataTable.Columns.Add("CourseNo");
                    dataTable.Columns.Add("directEmail");
                    dataTable.Columns.Add("cashSalesTeamID");
                    dataTable.Columns.Add("by");
                    dataTable.Columns.Add("adminNameTH");
                    dataTable.Columns.Add("adminNameEN");
                    dataTable.Columns.Add("adminIDNo");
                    dataTable.Columns.Add("adminAddress");
                    dataTable.Columns.Add("adminEmail");
                    dataTable.Columns.Add("adminTelMobile");
                    dataTable.Columns.Add("adminFax");
                    dataTable.Columns.Add("adminWishService");
                    dataTable.Columns.Add("NoOfTrainingUsers");
                    dataTable.Columns.Add("trainingTypeID");
                    dataTable.Columns.Add("trainingTypeName");
                    dataTable.Columns.Add("personType");
                    dataTable.Columns.Add("faxNo");
                    dataTable.Columns.Add("boardAuthority");
                    dataTable.Columns.Add("boardName");
                    dataTable.Columns.Add("allocation");
                    dataTable.Columns.Add("currentAccount");
                    dataTable.Columns.Add("bcLocation");
                    dataTable.Columns.Add("bcName");
                    dataTable.Columns.Add("tradeSaleName");
                    dataTable.Columns.Add("tradeSaleEmail");
                    dataTable.Columns.Add("tradeSaleTel");
                    dataTable.Columns.Add("ROName");
                    dataTable.Columns.Add("ROEmail");
                    dataTable.Columns.Add("ROTel");
                    dataTable.Columns.Add("DuplicateCashCheck");
                    dataTable.Columns.Add("contactAddressTH");
                    dataTable.Columns.Add("RMContactName");

                    while (readerCustStd.Read())
                    {
                        customerTypeID = "2";
                        customerType = "Std";

                        if (rr == (CMT_Count))
                        {
                            rr = 0;
                        }
                        if (rrt == CMTT_Count)
                        {
                            rrt = 0;
                        }

                        string noofuser = readerCustStd[2].ToString();
                        if (noofuser == "")
                        {
                            noofuser = "NULL";
                        }
                        if (readerCustStd[35].ToString() == "ICH")
                        {
                            CHName = "iCash";
                            CHID = "2";
                        }
                        else if (readerCustStd[35].ToString() == "BIZ")
                        {
                            CHName = "Biz iBanking";
                            CHID = "3";
                        }

                        // Assuming 'dataTable' is your DataTable object and 'readerCustStd' is your data reader
                        DataRow newRow = dataTable.NewRow();

                        newRow["customerTypeID"] = Convert.ToInt32(customerTypeID);
                        newRow["customerType"] = customerType;
                        newRow["systemID"] = 2;
                        newRow["systemName"] = "Cash";
                        newRow["channelID"] = Convert.ToInt32(CHID);
                        newRow["channelName"] = CHName;
                        newRow["compCode"] = readerCustStd[30].ToString().Replace("'", "''");
                        newRow["batchNo"] = Convert.ToInt32(lot);
                        newRow["juristicNo"] = readerCustStd[32].ToString().Replace("'", "''");
                        newRow["taxIDNo"] = "";
                        newRow["companyNamwEN"] = readerCustStd[8].ToString().Replace("'", "''");
                        newRow["companyNameTH"] = readerCustStd[9].ToString().Replace("'", "''");
                        newRow["holdingCompany"] = readerCustStd[23].ToString().Replace("'", "''");
                        newRow["NoOfUsers"] = Convert.ToInt32(noofuser);
                        newRow["contactPersonName"] = readerCustStd[15].ToString().Replace("'", "''");
                        newRow["RMRegistedAddress"] = readerCustStd[33].ToString().Replace("'", "''");
                        newRow["RMRegistedEmail1"] = readerCustStd[16].ToString().Replace("'", "''");
                        newRow["RMRegistedEmail2"] = readerCustStd[17].ToString().Replace("'", "''");
                        newRow["RMRegistedTelephone1"] = readerCustStd[31].ToString().Replace("'", "''");
                        newRow["RMRegistedTelephone2"] = readerCustStd[18].ToString().Replace("'", "''");
                        newRow["RMRegistedMobile1"] = readerCustStd[24].ToString().Replace("'", "''");
                        newRow["RMRegistedMobile2"] = readerCustStd[25].ToString().Replace("'", "''");
                        newRow["RMContactAddress"] = readerCustStd[26].ToString().Replace("'", "''");
                        newRow["RMContactEmail1"] = readerCustStd[19].ToString().Replace("'", "''");
                        newRow["RMContactEmail2"] = readerCustStd[3].ToString().Replace("'", "''");
                        newRow["RMContactTelephone1"] = readerCustStd[0].ToString().Replace("'", "''");
                        newRow["RMContactTelephone2"] = readerCustStd[10].ToString().Replace("'", "''");
                        newRow["RMContactMobile1"] = readerCustStd[34].ToString().Replace("'", "''");
                        newRow["RMContactMobile2"] = readerCustStd[27].ToString().Replace("'", "''");
                        newRow["contactAddress"] = readerCustStd[4].ToString().Replace("'", "''");
                        newRow["contactEmail1"] = readerCustStd[11].ToString().Replace("'", "''");
                        newRow["contactEmail2"] = readerCustStd[1].ToString().Replace("'", "''");
                        newRow["contactTelephone1"] = readerCustStd[12].ToString().Replace("'", "''");
                        newRow["contactTelephone2"] = readerCustStd[13].ToString().Replace("'", "''");
                        newRow["contactMobile1"] = readerCustStd[5].ToString().Replace("'", "''");
                        newRow["contactMobile2"] = readerCustStd[6].ToString().Replace("'", "''");
                        newRow["accountMgrID"] = Convert.ToInt32(accMgrID);
                        newRow["accountMgr"] = accMgrName;
                        newRow["salesMgrID"] = Convert.ToInt32(saleMgrID);
                        newRow["cashSalesMgr"] = saleMgrName;
                        newRow["by"] = readerCustStd[21].ToString().Replace("'", "''");
                        newRow["topTierFlag"] = readerCustStd[22].ToString().Replace("'", "''");
                        newRow["debitFeeAccount"] = readerCustStd[28].ToString().Replace("'", "''");
                        newRow["cmtOwnerID"] = Convert.ToInt32(CMTID[rr]);
                        newRow["cmtOwner"] = CMTUser[rr];
                        newRow["trainerOwnerID"] = Convert.ToInt32(CMTTID[rrt]);
                        newRow["trainerOwner"] = CMTTUser[rrt];
                        newRow["RMName"] = "";
                        newRow["courseID"] = DBNull.Value;
                        newRow["CourseNo"] = DBNull.Value;
                        newRow["customerStatus"] = "Active";
                        newRow["cmtManagerID"] = Convert.ToInt32(MgrID[0]);
                        newRow["cmtManager"] = MgrUser[0].ToString();
                        newRow["modifyBy"] = MgrName[0].ToString();
                        newRow["modifyDate"] = DateTime.Now;
                        newRow["directEmail"] = true;
                        newRow["cashSalesTeamID"] = Convert.ToInt32(saleTeamID);
                        newRow["NoOfTrainingUsers"] = 2;
                        newRow["trainingTypeID"] = 5;
                        newRow["trainingTypeName"] = "Self learning";

                        // Add the new row to the DataTable
                        dataTable.Rows.Add(newRow);
                        rrt++;
                        rr++; ;

                        query = "ok";
                    }

                    // Mark all rows as Unchanged
                    dataTable.AcceptChanges();
                    readerCustStd.Close();

                    //[CMTK2] CR : Old Feature Defect2 \\START//
                    if (String.IsNullOrEmpty(query))
                    {
                        return "BBL_K2_CUSTOMERS data in the lot " + lot + " is not found . Please import Customer data for lot " + lot;

                    }
                    //[CMTK2] CR : Old Feature Defect2 //END\\


                    string res = "";

                    res = CopyData(oConn, "dbo.cmt_i_customer_profile", dataTable);
                    if (res == "Pass")
                    {
                        Console.WriteLine("cmt_i_customer_profile 2 SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to cmt_i_customer_profile[" + res + "]";
                    }
                }

                //BBL_K2_CUSTOMERS Cash All
                string queryStrCustAll = "SELECT a.custID " +
                 " , a.customerTypeID " +
                 " , a.customerType " +
                 " , a.systemID " +
                 " , a.systemName " +
                 " , a.channelID " + //5
                 " , a.channelName " +
                 " , a.compCode " +
                 " , a.batchNo " +
                 " , a.juristicNo " +
                 " , a.taxIDNo " + //10
                 " , a.companyNamwEN " +
                 " , a.companyNameTH " +
                 " , a.holdingCompany " +
                 " , a.NoOfUsers " +
                 " , a.contactPersonName " + //15
                 " , a.RMRegistedAddress " +
                 " , a.RMRegistedEmail1 " +
                 " , a.RMRegistedEmail2 " +
                 " , a.RMRegistedTelephone1 " +
                 " , a.RMRegistedTelephone2 " + //20
                 " , a.RMRegistedMobile1 " +
                 " , a.RMRegistedMobile2 " +
                 " , a.RMContactAddress " +
                 " , a.RMContactEmail1 " +
                 " , a.RMContactEmail2 " + //25
                  " , a.RMContactTelephone1 " +
                  " , a.RMContactTelephone2 " +
                  " , a.RMContactMobile1 " +
                 " , a.RMContactMobile2 " +
                 " , a.contactAddress " + //30
                 " , a.contactEmail1 " +
                 " , a.contactEmail2 " +
                 " , a.contactTelephone1 " +
                 " , a.contactTelephone2 " +
                 " , a.contactMobile1 " + //35
                 " , a.contactMobile2 " +
                 " , a.accountMgrID " +
                 " , a.accountMgr " +
                 " , a.salesMgrID " +
                 " , a.cashSalesMgr " + //40
                 " , a.cashSalesTeam " +
                 " , a.topTierFlag " +
                 " , a.debitFeeAccount " +
                 " , a.cmtOwnerID " +
                 " , a.cmtOwner " + //45
                 " , a.trainerOwnerID " +
                 " , a.trainerOwner " +
                 " , a.RMName " +
                 " , a.courseID " +
                 " , a.CourseNo " + //50
                 " , a.customerStatus " +
                 " , a.cmtManagerID " +
                 " , a.cmtManager " +
                 " , a.modifyBy " +
                 " , a.modifyDate " + //55
                 " , a.directEmail " +
                 " , a.cashSalesTeamID " +
                 " , a.NoOfTrainingUsers " +
                 " , reBatch.IsReassignBatch" +
                " FROM dbo.cmt_i_customer_profile as a " +
                " LEFT JOIN dbo.cmt_t_reassign_batch as reBatch ON reBatch.custID = a.custID AND reBatch.destinationBatchNo = " + lot +
                " WHERE (reBatch.IsReassignBatch is null OR reBatch.IsReassignBatch = 0) AND systemID = 2 AND batchNo = " + lot;

                SqlCommand commandCustAll = new SqlCommand(queryStrCustAll, oConn);
                commandCustAll.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                SqlDataReader readerCustAll = commandCustAll.ExecuteReader();

                if (readerCustAll.HasRows)
                {
                    string custsupport = "";

                    // Assuming 'dataTable' is your DataTable object
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("myTaskID");
                    dataTable.Columns.Add("batchNo");
                    dataTable.Columns.Add("custID");
                    dataTable.Columns.Add("customerType");
                    dataTable.Columns.Add("compCode");
                    dataTable.Columns.Add("companyNameTH");
                    dataTable.Columns.Add("companyNameEN");
                    dataTable.Columns.Add("companyGroupCode");
                    dataTable.Columns.Add("juristicNo");
                    dataTable.Columns.Add("custSupport");
                    dataTable.Columns.Add("stageID");
                    dataTable.Columns.Add("stageName");
                    dataTable.Columns.Add("channelID");
                    dataTable.Columns.Add("channelName");
                    dataTable.Columns.Add("systemID");
                    dataTable.Columns.Add("systemName");
                    dataTable.Columns.Add("statusID");
                    dataTable.Columns.Add("status");
                    dataTable.Columns.Add("assignedTo");
                    dataTable.Columns.Add("actionRequire");
                    dataTable.Columns.Add("currentFunction");
                    dataTable.Columns.Add("remark");
                    dataTable.Columns.Add("createBy");
                    dataTable.Columns.Add("createDate");
                    dataTable.Columns.Add("lastAction");
                    dataTable.Columns.Add("lastUpdateBy");
                    dataTable.Columns.Add("lastUpdateDate");
                    dataTable.Columns.Add("byPassProcess");
                    dataTable.Columns.Add("currentFunctionID");
                    dataTable.Columns.Add("currentActionStatusID");
                    dataTable.Columns.Add("currentActionStatusName");
                    dataTable.Columns.Add("roleID");
                    dataTable.Columns.Add("flagSendEmail");
                    dataTable.Columns.Add("tokenBooked");
                    dataTable.Columns.Add("registeredUAS");
                    dataTable.Columns.Add("validateDocument");
                    dataTable.Columns.Add("iTradeRegistered");
                    dataTable.Columns.Add("custTypeID");
                    dataTable.Columns.Add("taskSerialNo");
                    dataTable.Columns.Add("applyToAllCompany");
                    dataTable.Columns.Add("procInstId");
                    dataTable.Columns.Add("delegateTo");

                    DataTable dataTable2 = new DataTable();
                    dataTable2.Columns.Add("ID");
                    dataTable2.Columns.Add("custID");
                    dataTable2.Columns.Add("stage");
                    dataTable2.Columns.Add("stageName");
                    dataTable2.Columns.Add("cmt_category");
                    dataTable2.Columns.Add("cmtCategoryName");
                    dataTable2.Columns.Add("status");
                    dataTable2.Columns.Add("Note");
                    dataTable2.Columns.Add("attachment");
                    dataTable2.Columns.Add("attachmentURL");
                    dataTable2.Columns.Add("createBy");
                    dataTable2.Columns.Add("createDate");
                    dataTable2.Columns.Add("actFormType");
                    dataTable2.Columns.Add("valueDate1");
                    dataTable2.Columns.Add("valueDate2");
                    dataTable2.Columns.Add("valueCode");
                    dataTable2.Columns.Add("valueDetail");
                    dataTable2.Columns.Add("functionID");
                    dataTable2.Columns.Add("actionStatusID");
                    dataTable2.Columns.Add("valueDetail2");

                    while (readerCustAll.Read())
                    {
                        //MgrOwnerName = "(SELECT displayName FROM dbo.cmt_ctrl_users WHERE userName = '" + readerCustAll[53].ToString() + "' )";
                        custsupport = "(SELECT TOP(1) cmt_ctrl_users.displayName FROM dbo.cmt_ctrl_users WHERE cmt_ctrl_users.userName = '" + readerCustAll[45].ToString() + "' )";
                        if (readerCustAll[2].ToString().Replace("'", "''") == "Toptier")
                        {
                            custsupport = "";
                        }
                        else
                        {
                            SqlCommand commandCustSupport = new SqlCommand(custsupport, oConn);
                            commandCustSupport.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));
                            SqlDataReader readerCustSupport = commandCustSupport.ExecuteReader();

                            while (readerCustSupport.Read())
                            {
                                custsupport = readerCustAll[0].ToString().Replace("'", "''");
                            }
                            readerCustSupport.Close();
                        }

                        DataRow newRow = dataTable.NewRow();
                        newRow["batchNo"] = Convert.ToInt32(readerCustAll[8].ToString().Replace("'", "''"));
                        newRow["custID"] = Convert.ToInt32(readerCustAll[0].ToString().Replace("'", "''"));
                        newRow["customerType"] = readerCustAll[2].ToString().Replace("'", "''");
                        newRow["compCode"] = readerCustAll[7].ToString().Replace("'", "''");
                        newRow["companyNameTH"] = readerCustAll[12].ToString().Replace("'", "''");
                        newRow["companyNameEN"] = readerCustAll[11].ToString().Replace("'", "''");
                        newRow["companyGroupCode"] = readerCustAll[13].ToString().Replace("'", "''");
                        newRow["juristicNo"] = readerCustAll[9].ToString().Replace("'", "''");
                        newRow["custSupport"] = custsupport; // assuming custsupport is a valid integer
                        newRow["stageID"] = 1;
                        newRow["stageName"] = "Prerequisites";
                        newRow["channelID"] = Convert.ToInt32(readerCustAll[5].ToString().Replace("'", "''"));
                        newRow["channelName"] = readerCustAll[6].ToString().Replace("'", "''");
                        newRow["systemID"] = Convert.ToInt32(readerCustAll[3].ToString().Replace("'", "''"));
                        newRow["systemName"] = readerCustAll[4].ToString().Replace("'", "''");
                        newRow["statusID"] = 3;
                        newRow["status"] = "InProgress";
                        newRow["assignedTo"] = readerCustAll[53].ToString().Replace("'", "''");
                        newRow["actionRequire"] = "No action needed";
                        newRow["currentFunction"] = "Import Customer Data";
                        newRow["remark"] = "";
                        newRow["createBy"] = readerCustAll[54].ToString().Replace("'", "''");
                        newRow["createDate"] = DateTime.Now;
                        newRow["lastAction"] = "Import Data";
                        newRow["lastUpdateBy"] = readerCustAll[54].ToString().Replace("'", "''");
                        newRow["lastUpdateDate"] = DateTime.Now;
                        newRow["byPassProcess"] = DBNull.Value;
                        newRow["currentFunctionID"] = 15;
                        newRow["currentActionStatusID"] = 1;
                        newRow["currentActionStatusName"] = "Open";
                        newRow["roleID"] = 6;
                        newRow["flagSendEmail"] = DBNull.Value;
                        newRow["tokenBooked"] = DBNull.Value;
                        newRow["registeredUAS"] = DBNull.Value;
                        newRow["validateDocument"] = DBNull.Value;
                        newRow["iTradeRegistered"] = DBNull.Value;
                        newRow["custTypeID"] = Convert.ToInt32(readerCustAll[1].ToString());
                        newRow["taskSerialNo"] = "";
                        newRow["applyToAllCompany"] = DBNull.Value;
                        newRow["procInstId"] = DBNull.Value;
                        dataTable.Rows.Add(newRow);

                        DataRow newRow2 = dataTable2.NewRow();
                        newRow2["custID"] = Convert.ToInt32(readerCustAll[0].ToString());
                        newRow2["stage"] = 1;
                        newRow2["stageName"] = "Prerequisites";
                        newRow2["cmt_category"] = 40;
                        newRow2["cmtCategoryName"] = "Import Data";
                        newRow2["status"] = 3;
                        newRow2["createBy"] = readerCustAll[54].ToString().Replace("'", "''");
                        newRow2["createDate"] = DateTime.Now;
                        newRow2["actFormType"] = 1;
                        newRow2["valueDate1"] = DateTime.Now;
                        newRow2["functionID"] = 15;
                        newRow2["actionStatusID"] = 1;
                        dataTable2.Rows.Add(newRow2);

                    }

                    // Mark all rows as Unchanged
                    dataTable.AcceptChanges();
                    dataTable2.AcceptChanges();
                    readerCustAll.Close();

                    string res = "";

                    //bulkCopy dbo.cmt_t_mytask_request
                    res = CopyData(oConn, "dbo.cmt_t_mytask_request", dataTable);
                    if (res == "Pass")
                    {
                        Console.WriteLine("cmt_t_mytask_request QLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to cmt_t_mytask_request[" + res + "]";
                    }

                    //bulkCopy dbo.cmt_t_history_log
                    res = CopyData(oConn, "dbo.cmt_t_history_log", dataTable2);
                    if (res == "Pass")
                    {
                        Console.WriteLine("cmt_t_history_log SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to cmt_t_history_log[" + res + "]";
                    }
              
                }//[CMTK2] CR : Old Feature Defect2 \\START//
                else
                {
                    return "BBL_K2_CUSTOMERS data in the lot " + lot + " is not found. Please import Customer data for lot " + lot;
                }
                //[CMTK2] CR : Old Feature Defect2 //END\\

                //BBL_K2_ACCOUNT Cash
                string queryStrAcc = "SELECT RECORD_TYPE, COMP_CODE, ACCT_NO, rec_ID FROM dbo.BBL_K2_ACCOUNTS WHERE COMP_CODE IN " +
                "(SELECT BBL_K2_CUSTOMERS.COMP_CODE FROM dbo.BBL_K2_CUSTOMERS WHERE MIGRATION_LOT = " + lot + " )";

                SqlCommand commandAcc = new SqlCommand(queryStrAcc, oConn);
                commandAcc.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                SqlDataReader readerAcc = commandAcc.ExecuteReader();

                if (readerAcc.HasRows)
                {

                    // สร้าง DataTable ของข้อมูลที่ต้องการบันทึก
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("ID");
                    dataTable.Columns.Add("custID");
                    dataTable.Columns.Add("companyCode");
                    dataTable.Columns.Add("accountNo");
                    dataTable.Columns.Add("accountName");
                    dataTable.Columns.Add("branch");
                    dataTable.Columns.Add("purpose");
                    dataTable.Columns.Add("createdBy");
                    dataTable.Columns.Add("createdDate");
                    dataTable.Columns.Add("modifyBy");
                    dataTable.Columns.Add("modifyDate");
                    dataTable.Columns.Add("active");

                    while (readerAcc.Read())
                    {
                        DataRow newRow = dataTable.NewRow();
                        string custIDquery = "SELECT TOP 1 cmt_i_customer_profile.custID FROM dbo.cmt_i_customer_profile WHERE cmt_i_customer_profile.compCode = '" + readerAcc[1].ToString() + "'";

                        SqlCommand custIDqueryq = new SqlCommand(custIDquery, oConn);
                        SqlDataReader readerCustID = custIDqueryq.ExecuteReader();

                        while (readerCustID.Read())
                        {
                            if (readerCustID.HasRows)
                            {
                                newRow["custID"] = readerCustID["custID"];
                            }
                            else
                            {
                                newRow["custID"] = DBNull.Value;
                            }
                        }
                        readerCustID.Close();
                        // เพิ่มข้อมูลลงใน DataTable
                        newRow["companyCode"] = readerAcc[1].ToString();
                        newRow["accountNo"] = readerAcc[2].ToString();
                        newRow["createdBy"] = MgrName[0];
                        newRow["createdDate"] = DateTime.Now;
                        newRow["modifyBy"] = MgrName[0];
                        newRow["modifyDate"] = DateTime.Now;
                        newRow["active"] = true;
                        dataTable.Rows.Add(newRow);
                    }

                    // Mark all rows as Unchanged
                    dataTable.AcceptChanges();
                    readerAcc.Close();

                    string res = "";

                    res = CopyData(oConn, "dbo.cmt_i_customer_accounts", dataTable);
                    if (res == "Pass")
                    {
                        Console.WriteLine("cmt_i_customer_accounts SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to dbo.cmt_i_customer_accounts[" + res + "]";
                    }
                }


                //BBL_K2_AUTH Cash
                string queryStrAuth = "SELECT PRODUCT_DESC,PRODUCT_CODE,MIN_AMT,LEVEL_APPRV,RECORD_TYPE,COMP_CODE,AUTH_ORDER,MAX_AMT,RULE_DEFINITION,rec_ID FROM dbo.BBL_K2_AUTH WHERE COMP_CODE IN " +
                "(SELECT BBL_K2_CUSTOMERS.COMP_CODE FROM dbo.BBL_K2_CUSTOMERS WHERE MIGRATION_LOT = " + lot + " )";

                SqlCommand commandAuth = new SqlCommand(queryStrAuth, oConn);
                commandAuth.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));
                SqlDataReader readerAuth = commandAuth.ExecuteReader();

                if (readerAuth.HasRows)
                {

                    // สร้าง DataTable ที่มีโครงสร้างเหมือนกับตาราง dbo.cmt_i_customer_authentications
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("ID");
                    dataTable.Columns.Add("custID");
                    dataTable.Columns.Add("companyCode");
                    dataTable.Columns.Add("productCode");
                    dataTable.Columns.Add("productDescription");
                    dataTable.Columns.Add("authorizationOrder");
                    dataTable.Columns.Add("minAmount");
                    dataTable.Columns.Add("maxAmount");
                    dataTable.Columns.Add("levelApproval");
                    dataTable.Columns.Add("ruleDefinition");
                    dataTable.Columns.Add("createdBy");
                    dataTable.Columns.Add("createdDate");

                    while (readerAuth.Read())
                    {
                        DataRow newRow = dataTable.NewRow();
                        string custIDquery = "SELECT TOP 1 cmt_i_customer_profile.custID FROM dbo.cmt_i_customer_profile WHERE cmt_i_customer_profile.compCode = '" + readerAuth[5].ToString() + "'";

                        //MgrOwnerName = "(SELECT displayName FROM dbo.cmt_ctrl_users WHERE userName = '" + readerCustAll[53].ToString() + "' )";
                        SqlCommand custIDqueryq = new SqlCommand(custIDquery, oConn);
                        custIDqueryq.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                        SqlDataReader readerCustID = custIDqueryq.ExecuteReader();

                        while (readerCustID.Read())
                        {
                            if (readerCustID.HasRows)
                            {
                                newRow["custID"] = Convert.ToInt32(readerCustID["custID"]);
                            }
                            else
                            {
                                newRow["custID"] = DBNull.Value;
                            }

                        }
                        readerCustID.Close();

                        string minAmount = readerAuth[2].ToString();
                        newRow["minAmount"] = string.IsNullOrEmpty(minAmount) ? (object)DBNull.Value : decimal.Parse(minAmount);
                        string maxAmount = readerAuth[7].ToString();
                        newRow["maxAmount"] = string.IsNullOrEmpty(maxAmount) ? (object)DBNull.Value : decimal.Parse(maxAmount);

                        // เพิ่มข้อมูลลงใน DataTable
                        newRow["companyCode"] = readerAuth[5].ToString();
                        newRow["productCode"] = readerAuth[1].ToString();
                        newRow["productDescription"] = readerAuth[0].ToString();
                        newRow["authorizationOrder"] = readerAuth[6].ToString();
                        newRow["levelApproval"] = readerAuth[3].ToString();
                        newRow["ruleDefinition"] = readerAuth[8].ToString();
                        newRow["createdBy"] = MgrName[0];
                        newRow["createdDate"] = DateTime.Now;
                        dataTable.Rows.Add(newRow);
                    }

                    // Mark all rows as Unchanged
                    dataTable.AcceptChanges();
                    readerAuth.Close();

                    string res = "";

                    res = CopyData(oConn, "dbo.cmt_i_customer_authentications", dataTable);
                    if (res == "Pass")
                    {
                        Console.WriteLine("cmt_i_customer_authentications SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to dbo.cmt_i_customer_authentications[" + res + "]";
                    }
                }

                //BBL_K2_PRODUCTS Cash
                string queryStrProd = "SELECT ACCT_PREREGIS,WHT_DOC_SUPPORT,PRODUCT_DESC,PRODUCT_CODE,SIGBLIND,RECORD_TYPE,COMP_CODE,TXN_LIMIT_AMT,DAILY_LIMIT_AMT,rec_ID FROM dbo.BBL_K2_PRODUCTS WHERE COMP_CODE IN " +
                "(SELECT BBL_K2_CUSTOMERS.COMP_CODE FROM dbo.BBL_K2_CUSTOMERS WHERE MIGRATION_LOT = " + lot + " )";
                SqlCommand commandProd = new SqlCommand(queryStrProd, oConn);
                commandProd.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                SqlDataReader readerProd = commandProd.ExecuteReader();
                if (readerProd.HasRows)
                {
                    // สร้าง DataTable ที่มีโครงสร้างเหมือนกับตาราง dbo.cmt_i_customer_products
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("ID");
                    dataTable.Columns.Add("custID");
                    dataTable.Columns.Add("companyCode");
                    dataTable.Columns.Add("productCode");
                    dataTable.Columns.Add("productDescription");
                    dataTable.Columns.Add("signBlind");
                    dataTable.Columns.Add("accountPreRegister");
                    dataTable.Columns.Add("WHTDocumentSupport");
                    dataTable.Columns.Add("transactionLimitAmount");
                    dataTable.Columns.Add("dailyLimitAmount");
                    dataTable.Columns.Add("createdBy");
                    dataTable.Columns.Add("createdDate");

                    while (readerProd.Read())
                    {
                        DataRow newRow = dataTable.NewRow();
                        string custIDquery = "SELECT TOP 1 cmt_i_customer_profile.custID FROM dbo.cmt_i_customer_profile WHERE cmt_i_customer_profile.compCode = '" + readerProd[6].ToString() + "'";
                        SqlCommand custIDqueryq = new SqlCommand(custIDquery, oConn);
                        custIDqueryq.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                        SqlDataReader readerCustID = custIDqueryq.ExecuteReader();

                        while (readerCustID.Read())
                        {
                            if (readerCustID.HasRows)
                            {
                                newRow["custID"] = Convert.ToInt32(readerCustID["custID"]);
                            }
                            else
                            {
                                newRow["custID"] = DBNull.Value;
                            }
                        }
                        readerCustID.Close();

                        string transactionLimitAmount = readerProd[7].ToString();
                        newRow["transactionLimitAmount"] = string.IsNullOrEmpty(transactionLimitAmount) ? (object)DBNull.Value : Convert.ToInt32(transactionLimitAmount);

                        string dailyLimitAmount = readerProd[8].ToString();
                        newRow["dailyLimitAmount"] = string.IsNullOrEmpty(dailyLimitAmount) ? (object)DBNull.Value : Convert.ToInt32(dailyLimitAmount);

                        // เพิ่มข้อมูลลงใน DataTable
                        newRow["companyCode"] = readerProd[6].ToString();
                        newRow["productCode"] = readerProd[3].ToString();
                        newRow["productDescription"] = readerProd[2].ToString();
                        newRow["signBlind"] = readerProd[4].ToString();
                        newRow["accountPreRegister"] = readerProd[0].ToString();
                        newRow["WHTDocumentSupport"] = readerProd[1].ToString();
                        newRow["createdBy"] = MgrName[0];
                        newRow["createdDate"] = DateTime.Now;
                        dataTable.Rows.Add(newRow);
                    }

                    // Mark all rows as Unchanged
                    dataTable.AcceptChanges();
                    readerProd.Close();

                    string res = "";
                    res = CopyData(oConn, "dbo.cmt_i_customer_products", dataTable);
                    if (res == "Pass")
                    {
                        Console.WriteLine("cmt_i_customer_products SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to dbo.cmt_i_customer_products[" + res + "]";
                    }
                }

                //BBL_K2_REPORTS Cash
                string queryStrRep = "SELECT REPORT_NAME,PRODUCT_DESC,PRODUCT_CODE,RECORD_TYPE,COMP_CODE,GROUP_CODE,REPORT_DESC,rec_ID FROM dbo.BBL_K2_REPORTS WHERE COMP_CODE IN " +
                "(SELECT BBL_K2_CUSTOMERS.COMP_CODE FROM dbo.BBL_K2_CUSTOMERS WHERE MIGRATION_LOT = " + lot + " )";

                SqlCommand commandRep = new SqlCommand(queryStrRep, oConn);
                commandRep.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                SqlDataReader readerRep = commandRep.ExecuteReader();

                if (readerRep.HasRows)
                {
                    // สร้าง DataTable ที่มีโครงสร้างเหมือนกับตาราง dbo.cmt_i_customer_reports
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("ID");
                    dataTable.Columns.Add("custID");
                    dataTable.Columns.Add("companyCode");
                    dataTable.Columns.Add("productCode");
                    dataTable.Columns.Add("productDescription");
                    dataTable.Columns.Add("reportName");
                    dataTable.Columns.Add("reportType");
                    dataTable.Columns.Add("groupCode");
                    dataTable.Columns.Add("createdBy");
                    dataTable.Columns.Add("createdDate");
                  
                    while (readerRep.Read())
                    {
                        DataRow newRow = dataTable.NewRow();
                        string custIDquery = "SELECT TOP 1 cmt_i_customer_profile.custID FROM dbo.cmt_i_customer_profile WHERE cmt_i_customer_profile.compCode = '" + readerRep[4].ToString() + "'";
                        SqlCommand custIDqueryq = new SqlCommand(custIDquery, oConn);
                        custIDqueryq.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                        SqlDataReader readerCustID = custIDqueryq.ExecuteReader();

                        while (readerCustID.Read())
                        {
                            if (readerCustID.HasRows)
                            {
                                newRow["custID"] = Convert.ToInt32(readerCustID["custID"]);
                            }
                            else
                            {
                                newRow["custID"] = DBNull.Value;
                            }

                        }
                        readerCustID.Close();

                        newRow["companyCode"] = readerRep[4].ToString();
                        newRow["productCode"] = readerRep[2].ToString();
                        newRow["productDescription"] = readerRep[1].ToString();
                        newRow["reportName"] = readerRep[6].ToString();
                        newRow["createdBy"] = MgrName[0];
                        newRow["createdDate"] = DateTime.Now;
                        dataTable.Rows.Add(newRow);
                    }

                    // Mark all rows as Unchanged
                    dataTable.AcceptChanges();
                    readerRep.Close();

                    string res = "";

                    res = CopyData(oConn, "dbo.cmt_i_customer_reports", dataTable);
                    if (res == "Pass")
                    {
                        Console.WriteLine("cmt_i_customer_reports SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to dbo.cmt_i_customer_reports[" + res + "]";
                    }
                }

                //BBL_K2_USERS Cash
                string queryStrUsr = "SELECT USER_STAT,BLOCK_USER,TOKEN_SERL_NO,USER_CODE,USER_ROLE,USER_NAME,SIGNBLIND,RECORD_TYPE,COMP_CODE,LAST_LOGIN,DAILY_LIMIT_AMT,GROUP_CODE,rec_ID FROM dbo.BBL_K2_USERS WHERE COMP_CODE IN " +
                "(SELECT BBL_K2_CUSTOMERS.COMP_CODE FROM dbo.BBL_K2_CUSTOMERS WHERE MIGRATION_LOT = " + lot + " )";

                SqlCommand commandUsr = new SqlCommand(queryStrUsr, oConn);
                commandUsr.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));
                SqlDataReader readerUsr = commandUsr.ExecuteReader();
                if (readerUsr.HasRows)
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("ID");
                    dataTable.Columns.Add("custID");
                    dataTable.Columns.Add("companyCode");
                    dataTable.Columns.Add("IDNo");
                    dataTable.Columns.Add("userName");
                    dataTable.Columns.Add("userID");
                    dataTable.Columns.Add("role");
                    dataTable.Columns.Add("userStatus");
                    dataTable.Columns.Add("blockUser");
                    dataTable.Columns.Add("tokenSerialNo");
                    dataTable.Columns.Add("signBlind");
                    dataTable.Columns.Add("lastLogin");
                    dataTable.Columns.Add("dailyLimitAmount");
                    dataTable.Columns.Add("groupCode");
                    dataTable.Columns.Add("userEmail");
                    dataTable.Columns.Add("userMobileNo");
                    dataTable.Columns.Add("userDepatment");
                    dataTable.Columns.Add("createdBy");
                    dataTable.Columns.Add("createdDate");
                    dataTable.Columns.Add("modifyBy");
                    dataTable.Columns.Add("modifyDate");
                    dataTable.Columns.Add("active");
                    dataTable.Columns.Add("userRole");
                    dataTable.Columns.Add("userStat");

                    while (readerUsr.Read())
                    {
                        DataRow newRow = dataTable.NewRow();

                        string lastlogin = readerUsr[9].ToString();

                        string dailyLimitAmount = readerUsr["DAILY_LIMIT_AMT"].ToString();
                        newRow["dailyLimitAmount"] = string.IsNullOrEmpty(dailyLimitAmount) ? (object)DBNull.Value : decimal.Parse(dailyLimitAmount);

                        newRow["lastlogin"] = string.IsNullOrEmpty(lastlogin) ? (object)DBNull.Value : decimal.Parse(lastlogin);

                        // Added CR2023
                        newRow["lastlogin"] = DBNull.Value;

                        string usrCust = "SELECT TOP 1 cmt_i_customer_profile.custID FROM dbo.cmt_i_customer_profile WHERE " +
                                  "cmt_i_customer_profile.compCode = '" + readerUsr[8].ToString() + "'";
                        SqlCommand commandUsrCust = new SqlCommand(usrCust, oConn);
                        commandUsrCust.CommandTimeout = Int32.Parse(ConfigurationSettings.AppSettings.Get("commandTimeout"));

                        SqlDataReader readerUsrCust = commandUsrCust.ExecuteReader();

                        while (readerUsrCust.Read())
                        {
                            newRow["custID"] = readerUsrCust["custID"].ToString();
                        }
                        readerUsrCust.Close();

                        newRow["companyCode"] = readerUsr[8].ToString();
                        newRow["userStat"] = readerUsr[0].ToString();
                        newRow["userName"] = readerUsr[5].ToString();
                        newRow["userID"] = readerUsr[3].ToString();
                        newRow["userRole"] = readerUsr[4].ToString();
                        newRow["blockUser"] = readerUsr[1].ToString();
                        newRow["tokenSerialNo"] = readerUsr[2].ToString();
                        newRow["signBlind"] = readerUsr[6].ToString();
                        // newRow["lastLogin"] = DBNull.Value; // ถ้า lastLogin เป็น NULL
                        newRow["groupCode"] = readerUsr[11].ToString();
                        newRow["createdBy"] = MgrName[0];
                        newRow["createdDate"] = DateTime.Now;
                        newRow["modifyBy"] = MgrName[0];
                        newRow["modifyDate"] = DateTime.Now;
                        newRow["active"] = true;

                        dataTable.Rows.Add(newRow);
                    }

                    // Mark all rows as Unchanged
                    dataTable.AcceptChanges();
                    readerUsr.Close();

                    string res = "";

                    res = CopyData(oConn, "dbo.cmt_i_customer_users", dataTable);
                    if (res == "Pass")
                    {
                        Console.WriteLine("cmt_i_customer_users SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to dbo.cmt_i_customer_users[" + res + "]";
                    }
                }
                oConn.Close();

                return "Prerequiresite Stage has been started";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.ToString();
            }
        }

        private string CopyData(SqlConnection oConn, string desTable, DataTable dt)
        {

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn))
            {
                bulkCopy.DestinationTableName = desTable;
                bulkCopy.NotifyAfter = 10000;
                try
                {
                    // Write unchanged rows from the source to the destination.
                    bulkCopy.WriteToServer(dt, DataRowState.Unchanged);
                    bulkCopy.Close();
                    return "Pass";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return ex.ToString();
                }
            }

        }
    }
}
