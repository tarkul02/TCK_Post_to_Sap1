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
using System.Data.OracleClient;
using System.Configuration;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.Runtime.ExceptionServices;
using Antlr.Runtime.Tree;
using SourceCode.Workflow.Client;

namespace WSCMTBBL
{
    /// <summary>
    /// Summary description for cmtBBLWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class cmtBBLWS : System.Web.Services.WebService
    {
        [WebMethod]
        public string testK2Connection(string hostserver)
        {
            string m_strBPServer = hostserver;
            try
            {
                // open a K2 connection
                SourceCode.Workflow.Client.Connection oConn = new SourceCode.Workflow.Client.Connection();
                oConn.Open(m_strBPServer);
                // oConn.ImpersonateUser(taskOwner[0]);
                return "Connecting to K2, Completed";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.ToString();
            }
        }

        [WebMethod]
        public string taskEmailAckK2(string recUser)
        {
            string m_strBPServer = "IT-DHRAPP1601";
            
            try
            {
                SqlConnection Conn;
                string SQLConnectionStr = "MyConn";
                string SQLConString;
                SQLConString = ConfigurationManager.ConnectionStrings[SQLConnectionStr].ToString();
                //Console.WriteLine(SQLConString);
                Conn = new SqlConnection(SQLConString);
                Conn.Open();
                //Console.WriteLine("SQL Connection Opened, OK");

                //Get assigned person and procInstId
                string queryTaskStr = "SELECT assignedTo,procInstId FROM SmartBoxData.cmt_t_mytask_request WHERE cmt_t_mytask_request.statusID = 3 AND cmt_t_mytask_request.custID = " + recUser;
                SqlCommand commandtaskst = new SqlCommand(queryTaskStr, Conn);
                SqlDataReader taskreaders = commandtaskst.ExecuteReader();
                List<string> taskProdinst = new List<string>();
                List<string> taskOwner = new List<string>();
                if (taskreaders.HasRows)
                {
                    while (taskreaders.Read())
                    {
                        taskOwner.Add(taskreaders[0].ToString());
                        taskProdinst.Add(taskreaders[1].ToString());
                    }
                    Conn.Close();
                }
                else
                {
                    return "Not found task of:" + recUser;
                }

                //Console.WriteLine(SQLConString);
                Conn = new SqlConnection(SQLConString);
                Conn.Open();

                string queryStr = "SELECT ID,[User] FROM ServerLog.ActInstDest WHERE ActInstDest.[Status] = 0 AND ActInstDest.ProcInstID = " + taskProdinst[0]  + "; ";
                SqlCommand commandst = new SqlCommand(queryStr, Conn);
                SqlDataReader readers = commandst.ExecuteReader();
                List<string> ActID = new List<string>();
                List<string> Users = new List<string>();
                if (readers.HasRows)
                {
                    while (readers.Read())
                    {
                        ActID.Add(readers[0].ToString());
                        Users.Add(readers[1].ToString());
                    }
                }

                var sn = taskProdinst[0] + "_" + ActID[0];

                Conn.Close();

                // open a K2 connection
                SourceCode.Workflow.Client.Connection oConn = new SourceCode.Workflow.Client.Connection();
                oConn.Open(m_strBPServer);
              
                oConn.ImpersonateUser(Users[0]);

                // get this specific task
                SourceCode.Workflow.Client.WorklistItem oWli = oConn.OpenWorklistItem(sn);
                if (oWli != null)
                {
                    foreach (SourceCode.Workflow.Client.Action oAct in oWli.Actions)
                    {
                        if (oAct.Name == "acknowledge")
                        {
                            oAct.Execute();
                            break;
                        }
                    }
                }
                else
                {
                    return "Not found task of: Acknowledge";
                }

                return "CustID " + recUser + " has been Acknowledged";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.ToString();
            }
        }

        [WebMethod]
        public string taskEmailFailMBFullK2(string recUser)
        {
            string m_strBPServer = "IT-DHRAPP1601";
            

            try
            {
                string queryStr = "";
                SqlCommand commandst ;
                SqlDataReader readers ;
                List<string> ActID = new List<string>();
                List<string> Users = new List<string>();
                var sn = "";
                SourceCode.Workflow.Client.WorklistItem oWli;

                SqlConnection Conn;
                string SQLConnectionStr = "MyConn";
                string SQLConString;
                SQLConString = ConfigurationManager.ConnectionStrings[SQLConnectionStr].ToString();
                //Console.WriteLine(SQLConString);
                Conn = new SqlConnection(SQLConString);
                Conn.Open();
                //Console.WriteLine("SQL Connection Opened, OK");

                //Get assigned person and procInstId
                string queryTaskStr = "SELECT assignedTo,procInstId FROM SmartBoxData.cmt_t_mytask_request,SmartBoxData.cmt_i_customer_profile WHERE " +
                    "cmt_t_mytask_request.custID = cmt_i_customer_profile.custID AND cmt_t_mytask_request.statusID = 3 AND cmt_i_customer_profile.contactEmail1 = '" + recUser + "'; ";
                SqlCommand commandtaskst = new SqlCommand(queryTaskStr, Conn);
                SqlDataReader taskreaders = commandtaskst.ExecuteReader();
                List<string> taskProdinst = new List<string>();
                List<string> taskOwner = new List<string>();
                if (taskreaders.HasRows)
                {
                    // open a K2 connection
                    SourceCode.Workflow.Client.Connection oConn = new SourceCode.Workflow.Client.Connection();
                    oConn.Open(m_strBPServer);
                    var i = 0;
                    
                    while (taskreaders.Read())
                    {

                        taskOwner.Add(taskreaders[0].ToString());
                        taskProdinst.Add(taskreaders[1].ToString());

                        //Console.WriteLine(SQLConString);
                        SqlConnection Connn = new SqlConnection(SQLConString);
                        Connn.Open();

                        queryStr = "SELECT ID,[User] FROM ServerLog.ActInstDest WHERE ActInstDest.[Status] = 0 AND ActInstDest.ProcInstID = " + taskProdinst[i] + "; ";
                        commandst = new SqlCommand(queryStr, Connn);
                        readers = commandst.ExecuteReader();
                        
                        if (readers.HasRows)
                        {
                            var j = 0;
                            while (readers.Read())
                            {
                                ActID.Add(readers[0].ToString());
                                Users.Add(readers[1].ToString());


                                sn = taskProdinst[i] + "_" + ActID[j];

                                oConn.ImpersonateUser(Users[j]);

                                // get this specific task
                                oWli = oConn.OpenWorklistItem(sn);
                                if (oWli != null)
                                {
                                    foreach (SourceCode.Workflow.Client.Action oAct in oWli.Actions)
                                    {
                                        if (oAct.Name == "MBFull")
                                        {
                                            oAct.Execute();
                                            break;

                                        }
                                    }
                                }
                                oConn.RevertUser();
                                j++;
                            }

                        }
                        Connn.Close();
                        i++;
                    }
                }
                else
                {
                    return "Not found task of:" + recUser;
                }


                Conn.Close();

                return "Email " + recUser + " has been detected Fail status on Task acknowkedge";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.ToString();
            }
        }

        [WebMethod]
        public string taskEmailFailInvalidK2(string recUser)
        {
            string m_strBPServer = "IT-DHRAPP1601";


            try
            {
                string queryStr = "";
                SqlCommand commandst;
                SqlDataReader readers;
                List<string> ActID = new List<string>();
                List<string> Users = new List<string>();
                var sn = "";
                SourceCode.Workflow.Client.WorklistItem oWli;

                SqlConnection Conn;
                string SQLConnectionStr = "MyConn";
                string SQLConString;
                SQLConString = ConfigurationManager.ConnectionStrings[SQLConnectionStr].ToString();
                //Console.WriteLine(SQLConString);
                Conn = new SqlConnection(SQLConString);
                Conn.Open();
                //Console.WriteLine("SQL Connection Opened, OK");

                //Get assigned person and procInstId
                string queryTaskStr = "SELECT assignedTo,procInstId FROM SmartBoxData.cmt_t_mytask_request,SmartBoxData.cmt_i_customer_profile WHERE " +
                    "cmt_t_mytask_request.custID = cmt_i_customer_profile.custID AND cmt_t_mytask_request.statusID = 3 AND cmt_i_customer_profile.contactEmail1 = '" + recUser + "'; ";
                SqlCommand commandtaskst = new SqlCommand(queryTaskStr, Conn);
                SqlDataReader taskreaders = commandtaskst.ExecuteReader();
                List<string> taskProdinst = new List<string>();
                List<string> taskOwner = new List<string>();
                if (taskreaders.HasRows)
                {
                    // open a K2 connection
                    SourceCode.Workflow.Client.Connection oConn = new SourceCode.Workflow.Client.Connection();
                    oConn.Open(m_strBPServer);
                    var i = 0;

                    while (taskreaders.Read())
                    {

                        taskOwner.Add(taskreaders[0].ToString());
                        taskProdinst.Add(taskreaders[1].ToString());

                        //Console.WriteLine(SQLConString);
                        SqlConnection Connn = new SqlConnection(SQLConString);
                        Connn.Open();

                        queryStr = "SELECT ID,[User] FROM ServerLog.ActInstDest WHERE ActInstDest.[Status] = 0 AND ActInstDest.ProcInstID = " + taskProdinst[0] + "; ";
                        commandst = new SqlCommand(queryStr, Connn);
                        readers = commandst.ExecuteReader();

                        if (readers.HasRows)
                        {
                            var j = 0;
                            while (readers.Read())
                            {
                                ActID.Add(readers[0].ToString());
                                Users.Add(readers[1].ToString());

                                sn = taskProdinst[i] + "_" + ActID[j];
                                
                                oConn.ImpersonateUser(Users[0]);

                                // get this specific task
                                oWli = oConn.OpenWorklistItem(sn);
                                if (oWli != null)
                                {
                                    foreach (SourceCode.Workflow.Client.Action oAct in oWli.Actions)
                                    {
                                        if (oAct.Name == "Invalid")
                                        {
                                            oAct.Execute();
                                            break;

                                        }
                                    }
                                }
                                oConn.RevertUser();
                                j++;
                            }
                        }
                        Connn.Close();

                        i++;
                    }
                }
                else
                {
                    return "Not found task of:" + recUser;
                }


                Conn.Close();

                return "Email " + recUser + " has been detected Fail status on Task acknowkedge";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.ToString();
            }
        }

        [WebMethod]
        public string createGPGFile(string Type, string D, string F)
        {
            string configFile = @"C:\wsconfig\config.txt";
            List<string> pathStr = new List<string>();
            if (System.IO.File.Exists(configFile) == true)
            {
                System.IO.StreamReader objReader = new System.IO.StreamReader(configFile);
                while (objReader.Peek() != -1)
                    pathStr.Add(objReader.ReadLine());
            }
            else
                return "Not found config file";

            string configBatchPath = @"C:\wsconfig\configbatch.txt";
            List<string> pathBatchStr = new List<string>();

            if (System.IO.File.Exists(configBatchPath) == true)
            {
                System.IO.StreamReader objReader = new System.IO.StreamReader(configBatchPath);
                while (objReader.Peek() != -1)
                    pathBatchStr.Add(objReader.ReadLine());
            }
            else
                // MessageBox.Show("File Does Not Exist")
                return "Not found config Batch patch file";

            string thisDateTime;
            thisDateTime = DateTime.Today.Year + DateTime.Today.Month.ToString("#00") + DateTime.Today.Day.ToString("#00") + DateTime.Now.Hour.ToString("#00") + DateTime.Now.Minute.ToString("#00") + DateTime.Now.Second.ToString("#00");

            string thisDate;
            thisDate = DateTime.Today.Year + DateTime.Today.Month.ToString("#00") + DateTime.Today.Day.ToString("#00");

            string fileName = "";
            if (Type == "RequestMigrate")
                fileName = "REQMGT_" + thisDate;
            else if (Type == "RequestCustInfo")
                fileName = "REQCUST_" + thisDate;

            string filepath = pathStr[0] + @"\" + fileName + ".txt";
            string filepathOut = pathStr[0] + @"\out\" + fileName + ".txt.gpg";
            string filepathCopyTo = pathStr[1] + @"\" + fileName + ".txt.gpg";
            if (System.IO.File.Exists(filepath))
                System.IO.File.Delete(filepath);
            System.IO.File.Create(filepath).Dispose();
            System.IO.StreamWriter objWriter = new System.IO.StreamWriter(filepath, true);
            objWriter.WriteLine("H|" + thisDate);
            objWriter.WriteLine(D);
            objWriter.WriteLine("F|" + F);
            objWriter.Close();

            if (System.IO.Directory.Exists(pathBatchStr[0]))
            {
                ProcessStartInfo processinfo = new ProcessStartInfo();
                processinfo.WorkingDirectory = pathBatchStr[0]; // "C:\AdapterGPG"
                processinfo.FileName = pathBatchStr[1]; // "C:\AdapterGPG\jdk1.8.0_131\jre\bin\java.exe"
                processinfo.Arguments = pathBatchStr[2]; // "-jar GPGAdapter.jar"
                System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
                processinfo.UseShellExecute = false;
                processinfo.RedirectStandardOutput = true;
                myProcess.StartInfo = processinfo;
                myProcess.Start();
                StreamReader myStreamReader = myProcess.StandardOutput;
                // Read the standard output of the spawned process. 
                string myString = "";
                do
                    myString = myString + myStreamReader.ReadLine();
                while (!(myStreamReader.EndOfStream));
                myProcess.WaitForExit();
                myProcess.Close();

                Console.WriteLine(myString);
            }
            else
                return "Not found JAVA file Folder at " + pathBatchStr[0];

            if (System.IO.File.Exists(filepathOut))
            {
                //System.IO.File.Copy(filepathOut, filepathCopyTo, true);

                //RunCommandCom("move " & filepathOut & " " & filepathCopyTo, "", True)
                // FileSystem.CopyFile(filepathOut, filepathCopyTo, overwrite:=True)
                string strCmdText;
                strCmdText = "move " + filepathOut + " " + filepathCopyTo;
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                return "GPG File has been created " + filepathCopyTo;
            }
            else
                return "GPG File NOT YET created " + filepathOut;
        }

        [WebMethod]
        public string ImportFromExcel()
        {
            string lastmsg = "";
            try
            {
               
                //SqlConnection oConn;
                //string SQLConnectionStr = "MyConn";
                //string SQLConString;
                //SQLConString = ConfigurationManager.ConnectionStrings[SQLConnectionStr].ToString();
                //Console.WriteLine(SQLConString);
                //oConn = new SqlConnection(SQLConString);
                //oConn.Open();
                //Console.WriteLine("SQL Connection Opened, OK");
                string res = "";

                //------------------- Customer File --------------------
                string csv_customerfile_path = @"C:\FileUpload\FileCustomer.csv";
                string custQueryStr = "";
               // SqlCommand commandstr = new SqlCommand("DELETE FROM SmartBoxData.BBL_TRADE_CUSTOMER", oConn);
               // commandstr.ExecuteNonQuery();
                DataTable csvCustData = new DataTable();
                using (TextFieldParser csvReader = new TextFieldParser(csv_customerfile_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        double h = Convert.ToDouble(fieldData[6]);
                        decimal h2 = decimal.Parse(fieldData[6], System.Globalization.NumberStyles.Any);
                        custQueryStr = "INSERT [SmartBoxData].[BBL_TRADE_CUSTOMER] VALUES (NULL, N'" + fieldData[1] + "', N'" + fieldData[2] + "', N'" + fieldData[3] + "', N'" + fieldData[4] + "', N'" + fieldData[5] + "', N'" + decimal.Parse(fieldData[6], System.Globalization.NumberStyles.Any).ToString() + "', N'" + fieldData[7] + "', N'" + fieldData[8] + "', N'" + fieldData[9] + "', N'" + fieldData[10] + "', N'" + fieldData[11] + "', N'" + fieldData[12] + "', N'" + fieldData[13] + "', N'" + fieldData[14] + "', N'" + fieldData[15] + "', N'" + fieldData[16] + "', N'" + fieldData[17] + "', N'" + fieldData[18] + "', N'" + fieldData[19] + "', N'" + fieldData[20] + "', N'" + fieldData[21] + "', N'" + fieldData[22] + "', N'" + fieldData[23] + "', N'" + fieldData[24] + "', N'" + fieldData[25] + "', N'" + fieldData[26] + "', N'" + fieldData[27] + "', N'" + fieldData[28] + "', N'" + fieldData[29] + "', N'" + fieldData[31] + "', N'" + fieldData[32] + "', N'" + fieldData[33] + "', N'" + fieldData[34] + "', N'" + fieldData[35] + "', N'" + fieldData[36] + "', N'" + fieldData[37] + "', N'" + fieldData[38] + "');";
                        return custQueryStr;
                        //csvCustData.Rows.Add(fieldData);
                    }

                }

                if (csvCustData.Rows.Count > 0)
                {
                    //res = CopyData(oConn, "SmartBoxData.BBL_TRADE_CUSTOMER", csvCustData);
                    if (res == "Pass")
                    {
                        Console.WriteLine("Data copied to BBL_TRADE_CUSTOMER SQL Server using SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to BBL_TRADE_CUSTOMER[" + res + "]";
                    }
                }
                else
                {
                    lastmsg += ", BBL_TRADE_CUSTOMER No new data to Update";
                    return lastmsg;
                }

                //------------------- Account File --------------------
                string csv_accountfile_path = @"C:\FileUpload\FileAccount.csv";
                DataTable csvAccData = new DataTable();
                using (TextFieldParser csvReader = new TextFieldParser(csv_accountfile_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvAccData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvAccData.Rows.Add(fieldData);
                    }
                }
                if (csvAccData.Rows.Count > 0)
                {
                    //res = CopyData(oConn, "SmartBoxData.BBL_TRADE_ACCOUNT", csvAccData);
                    if (res == "Pass")
                    {
                        Console.WriteLine("Data copied to BBL_TRADE_ACCOUNT SQL Server using SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to BBL_TRADE_ACCOUNT[" + res + "]";
                    }
                }
                else
                {
                    lastmsg += ", BBL_TRADE_ACCOUNT No new data to Update";
                    return lastmsg;
                }

                //------------------- Authorizer File --------------------
                string csv_userfile_path = @"C:\FileUpload\FileUser.csv";
                DataTable csvUserData = new DataTable();
                using (TextFieldParser csvReader = new TextFieldParser(csv_userfile_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvUserData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvUserData.Rows.Add(fieldData);
                    }
                }
                if (csvUserData.Rows.Count > 0)
                {
                    //res = CopyData(oConn, "SmartBoxData.BBL_TRADE_USER", csvUserData);
                    if (res == "Pass")
                    {
                        Console.WriteLine("Data copied to BBL_TRADE_USER SQL Server using SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to BBL_TRADE_USER[" + res + "]";
                    }
                }
                else
                {
                    lastmsg += ", BBL_TRADE_USER No new data to Update";
                    return lastmsg;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "Success: Data interfaced to K2 " + lastmsg;
        }

        [WebMethod()]
        public string DisplayOracleData(int lot)
        {
            string OracleConnectionStr = "OracleConStr";
            string ConString;
            ConString = ConfigurationManager.ConnectionStrings[OracleConnectionStr].ToString();

            OracleConnection connection = new OracleConnection();

            connection.ConnectionString = ConString;
            connection.Open();
            Console.WriteLine("Oracle Connection Opened, OK");

            try
            {
                OracleCommand command = connection.CreateCommand();
                //BBL_K2_CUSTOMERS
                string queryStr = "SELECT * FROM BBL.BBL_K2_CUSTOMER WHERE MIGRATION_LOT = " + lot;
                command.CommandText = queryStr;
                OracleDataReader reader = command.ExecuteReader();
                Console.WriteLine("Data retrieved to reader from Oracle, OK " + reader.HasRows);
                DataTable dt = new DataTable();
                string lastmsg = "";
                string res = "";

                if (reader.HasRows)
                {
                    dt.Load(reader);
                    
                }
                string data = string.Empty;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                if (null != dt && null != dt.Rows)
                {
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        foreach (var item in dataRow.ItemArray)
                        {
                            sb.Append(item);
                            sb.Append(',');
                        }
                        sb.AppendLine();
                    }

                    data = sb.ToString();
                }
                return data;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod()]
        public string ImportOracleToSQLserver(int lot)
        {
            string OracleConnectionStr = "OracleConStr";
            string ConString;
            ConString = ConfigurationManager.ConnectionStrings[OracleConnectionStr].ToString();

            OracleConnection connection = new OracleConnection();
            
            try
            {
                SqlConnection oConn;
                string SQLConnectionStr = "MyConn";
                string SQLConString;
                SQLConString = ConfigurationManager.ConnectionStrings[SQLConnectionStr].ToString();
                Console.WriteLine(SQLConString);
                oConn = new SqlConnection(SQLConString);
                oConn.Open();
                Console.WriteLine("SQL Connection Opened, OK");

                connection.ConnectionString = ConString;
                connection.Open();
                Console.WriteLine("Oracle Connection Opened, OK");

                OracleCommand command = connection.CreateCommand();

                //BBL_K2_CUSTOMERS
                string queryStr = "SELECT RM_CONTACT_TEL1, CONTACT_EMAIL2, NO_OF_USERS, RM_CONTACT_EMAIL2, CONTACT_ADDR, CONTACT_MOBILE1, CONTACT_MOBILE2, ACCT_MGR, COMP_ENG_NAME, COMP_TH_NAME, RM_CONTACT_TEL2, CONTACT_EMAIL1, CONTACT_TEL1, CONTACT_TEL2, MIGRATION_LOT, CONTACT_PERSON_NAME, RM_REGIS_EMAIL1, RM_REGIS_EMAIL2, RM_REGIS_TEL2, RM_CONTACT_EMAIL1, CASH_SALES_MGR, CASH_SALES_TEAM, TOP_TIER_FLAG, HOLDING_COMP, RM_REGIS_MOBILE1, RM_REGIS_MOBILE2, RM_CONTACT_ADDR, RM_CONTACT_MOBILE2, DEBIT_FEE_ACCT, RECORD_TYPE, COMP_CODE, RM_REGIS_TEL1, ID_NO, RM_REGIS_ADDR, RM_CONTACT_MOBILE1,'' AS rec_ID, SYS_CODE FROM BBL.BBL_K2_CUSTOMER WHERE MIGRATION_LOT = " + lot;
                command.CommandText = queryStr;
                OracleDataReader reader = command.ExecuteReader();
                Console.WriteLine("Data retrieved to reader from Oracle, OK " + reader.HasRows);
                DataTable dt = new DataTable();
                string lastmsg = "";
                string res = "";

                if (reader.HasRows) 
                {
                    dt.Load(reader);
                    res = CopyData(oConn, "SmartBoxData.BBL_K2_CUSTOMERS", dt);
                    if (res == "Pass")
                    {
                        Console.WriteLine("Data copied to BBL_K2_CUSTOMERS SQL Server using SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to BBL_K2_CUSTOMERS[" + res + "]";
                    }
                }
                else
                {
                    lastmsg += ", BBL_K2_CUSTOMERS No new data to update";
                }
                

                //BBL_K2_USER
                queryStr = "SELECT USER_STAT, BLOCK_USER, TOKEN_SERL_NO, USER_CODE, USER_ROLE, USER_NAME, SIGNBLIND, RECORD_TYPE, COMP_CODE, LAST_LOGIN, DAILY_LIMIT_AMT, GROUP_CODE, '' AS rec_ID  FROM BBL.BBL_K2_USER WHERE COMP_CODE IN (SELECT COMP_CODE FROM BBL.BBL_K2_CUSTOMER WHERE MIGRATION_LOT = " + lot + ")";
                command.CommandText = queryStr;
                reader = command.ExecuteReader();
                Console.WriteLine("Data retrieved to reader from Oracle, OK " + reader.HasRows);
                dt = new DataTable();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                    res = CopyData(oConn, "SmartBoxData.BBL_K2_USERS", dt);
                    if (res == "Pass")
                    {
                        Console.WriteLine("Data copied to BBL_K2_USER SQL Server using SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to BBL_K2_USER[" + res + "]";
                    }
                }
                else
                {
                    lastmsg += ", BBL_K2_USER No new data to Update";
                }

                //BBL_K2_ACCOUNTS
                queryStr = "SELECT RECORD_TYPE, COMP_CODE, ACCT_NO, '' AS rec_ID  FROM BBL.BBL_K2_ACCOUNT WHERE COMP_CODE IN (SELECT COMP_CODE FROM BBL.BBL_K2_CUSTOMER WHERE MIGRATION_LOT = " + lot + ")";
                command.CommandText = queryStr;
                reader = command.ExecuteReader();
                Console.WriteLine("Data retrieved to reader from Oracle, OK " + reader.HasRows);
                dt = new DataTable();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                    res = CopyData(oConn, "SmartBoxData.BBL_K2_ACCOUNTS", dt);
                    if (res == "Pass")
                    {
                        Console.WriteLine("Data copied to BBL_K2_ACCOUNTS SQL Server using SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to BBL_K2_ACCOUNTS[" + res + "]";
                    }
                }
                else
                {
                    lastmsg += ", BBL_K2_ACCOUNTS No new data to Update";
                }

                //BBL_K2_AUTH
                queryStr = "SELECT PRODUCT_DESC, PRODUCT_CODE, MIN_AMT, LEVEL_APPRV, RECORD_TYPE, COMP_CODE, AUTH_ORDER, MAX_AMT, RULE_DEFINITION, '' AS rec_ID  FROM BBL.BBL_K2_AUTH_MATRIX WHERE COMP_CODE IN (SELECT COMP_CODE FROM BBL.BBL_K2_CUSTOMER WHERE MIGRATION_LOT = " + lot + ")";
                command.CommandText = queryStr;
                reader = command.ExecuteReader();
                Console.WriteLine("Data retrieved to reader from Oracle, OK " + reader.HasRows);
                dt = new DataTable();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                    res = CopyData(oConn, "SmartBoxData.BBL_K2_AUTH", dt);
                    if (res == "Pass")
                    {
                        Console.WriteLine("Data copied to BBL_K2_AUTH SQL Server using SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to BBL_K2_AUTH[" + res + "]";
                    }
                }
                else
                {
                    lastmsg += ", BBL_K2_AUTH No new data to Update";
                }

                //BBL_K2_PRODUCTS
                queryStr = "SELECT ACCT_PREREGIS, WHT_DOC_SUPPORT, PRODUCT_DESC, PRODUCT_CODE, SIGNBLIND, RECORD_TYPE, COMP_CODE, TXN_LIMIT_AMT, DAILY_LIMIT_AMT, '' AS rec_ID  FROM BBL.BBL_K2_PRODUCT WHERE COMP_CODE IN (SELECT COMP_CODE FROM BBL.BBL_K2_CUSTOMER WHERE MIGRATION_LOT = " + lot + ")";
                command.CommandText = queryStr;
                reader = command.ExecuteReader();
                Console.WriteLine("Data retrieved to reader from Oracle, OK " + reader.HasRows);
                dt = new DataTable();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                    res = CopyData(oConn, "SmartBoxData.BBL_K2_PRODUCTS", dt);
                    if (res == "Pass")
                    {
                        Console.WriteLine("Data copied to BBL_K2_PRODUCTS SQL Server using SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to BBL_K2_PRODUCTS[" + res + "]";
                    }
                }
                else
                {
                    lastmsg += ", BBL_K2_PRODUCTS No new data to Update";
                }

                //BBL_K2_REPORTS
                queryStr = "SELECT REPORT_NAME, PRODUCT_DESC, PRODUCT_CODE, RECORD_TYPE, COMP_CODE, GROUP_CODE, REPORT_DESC, '' AS rec_ID  FROM BBL.BBL_K2_REPORTS WHERE COMP_CODE IN (SELECT COMP_CODE FROM BBL.BBL_K2_CUSTOMER WHERE MIGRATION_LOT = " + lot + ")";
                command.CommandText = queryStr;
                reader = command.ExecuteReader();
                Console.WriteLine("Data retrieved to reader from Oracle, OK " + reader.HasRows);
                dt = new DataTable();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                    res = CopyData(oConn, "SmartBoxData.BBL_K2_REPORTS", dt);
                    if (res == "Pass")
                    {
                        Console.WriteLine("Data copied to BBL_K2_REPORTS SQL Server using SQLBulkCopy, -- FINISHED --");
                    }
                    else
                    {
                        return "Error: Fail to data interfaced to BBL_K2_REPORTS[" + res + "]";
                    }
                }
                else
                {
                    lastmsg += ", BBL_K2_REPORTS No new data to Update";
                }

                oConn.Close();

                return "Success: Data interfaced to K2 " + lastmsg;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            finally
            {

            }
            //return ConString;
        }

        [WebMethod()]
        public string ImportSQLToSQLserver(int lot, string souTable, string desTable)
        {
            
            try
            {
                SqlConnection oConn;
                string SQLConnectionStr = "MyConn";
                string SQLConString;
                SQLConString = ConfigurationManager.ConnectionStrings[SQLConnectionStr].ToString();
                Console.WriteLine(SQLConString);
                oConn = new SqlConnection(SQLConString);
                oConn.Open();
                Console.WriteLine("SQL Connection Opened, OK");

                string queryStr = "SELECT RM_CONTACT_TEL1, CONTACT_EMAIL2, NO_OF_USERS, RM_CONTACT_EMAIL2, CONTACT_ADDR, CONTACT_MOBILE1, CONTACT_MOBILE2, ACCT_MGR, COMP_ENG_NAME, COMP_TH_NAME, RM_CONTACT_TEL2, CONTACT_EMAIL1, CONTACT_TEL1, CONTACT_TEL2, MIGRATION_LOT, CONTACT_PERSON_NAME, RM_REGIS_EMAIL1, RM_REGIS_EMAIL2, RM_REGIS_TEL2, RM_CONTACT_EMAIL1, CASH_SALES_MGR, CASH_SALES_TEAM, TOP_TIER_FLAG, HOLDING_COMP, RM_REGIS_MOBILE1, RM_REGIS_MOBILE2, RM_CONTACT_ADDR, RM_CONTACT_MOBILE2, DEBIT_FEE_ACCT, RECORD_TYPE, COMP_CODE, RM_REGIS_TEL1, ID_NO, RM_REGIS_ADDR, RM_CONTACT_MOBILE1 FROM " + souTable;
                SqlCommand command = new SqlCommand(queryStr, oConn);
                SqlDataReader reader = command.ExecuteReader();
                DataTable dt = new DataTable();

                dt.Load(reader);
                if (CopyData(oConn, desTable, dt) == "Pass")
                {
                    Console.WriteLine("Data copied to " + desTable + " SQL Server using SQLBulkCopy, -- FINISHED --");
                }
                else
                {
                    return "Error: Fail to data interfaced to " + desTable;
                }


                oConn.Close();

                return "Data interface compleated";

                //(IDisposable)connection.Dispose();

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            finally
            {

            }
            //return ConString;
        }

        [WebMethod()]
        public string SQLqueryAction(string query)
        {

            try
            {
                SqlConnection oConn;
                string SQLConnectionStr = "MyConn";
                string SQLConString;
                SQLConString = ConfigurationManager.ConnectionStrings[SQLConnectionStr].ToString();
                Console.WriteLine(SQLConString);
                oConn = new SqlConnection(SQLConString);
                oConn.Open();
                Console.WriteLine("SQL Connection Opened, OK");

                SqlCommand commandstr = new SqlCommand(query, oConn);
                commandstr.ExecuteNonQuery();
                oConn.Close();
                return "Execute finished";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.ToString();
            }
        }

        [WebMethod()]
        public string AssignOwnerTrade()
        {
            try
            {
                SqlConnection oConn;
                string SQLConnectionStr = "MyConn";
                string SQLConString;
                SQLConString = ConfigurationManager.ConnectionStrings[SQLConnectionStr].ToString();
                Console.WriteLine(SQLConString);
                oConn = new SqlConnection(SQLConString);
                oConn.Open();
                Console.WriteLine("SQL Connection Opened, OK");

                string queryStr = "";
                string queryStrCMTTrain = "";
                string queryStrMgr = "";
                string queryStrCMT = "";
                string query = "";

                //------------ Fetch Implement Trade owner list -------------------
                queryStr = "SELECT cmt_ctrl_users.userID, userName, displayName, cmt_ctrl_user_roles.userRoleID " +
                "FROM SmartBoxData.cmt_ctrl_users, SmartBoxData.cmt_ctrl_user_roles " +
                "WHERE SmartBoxData.cmt_ctrl_users.userID = SmartBoxData.cmt_ctrl_user_roles.userID " +
                "AND cmt_ctrl_user_roles.RoleID = 2 AND cmt_ctrl_user_roles.systemID = 1";
                SqlCommand command = new SqlCommand(queryStr, oConn);
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
                "FROM SmartBoxData.cmt_ctrl_users, SmartBoxData.cmt_ctrl_user_roles " +
                "WHERE SmartBoxData.cmt_ctrl_users.userID = SmartBoxData.cmt_ctrl_user_roles.userID " +
                "AND cmt_ctrl_user_roles.RoleID = 1";
                SqlCommand CMTcommand = new SqlCommand(queryStrCMT, oConn);
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

                //------------ Fetch CMT Manager list -------------------
                queryStrMgr = "SELECT cmt_ctrl_users.userID, userName, displayName, cmt_ctrl_user_roles.userRoleID " +
                    "FROM SmartBoxData.cmt_ctrl_users, SmartBoxData.cmt_ctrl_user_roles " +
                    "WHERE SmartBoxData.cmt_ctrl_users.userID = SmartBoxData.cmt_ctrl_user_roles.userID " +
                    "AND cmt_ctrl_user_roles.RoleID = 6 AND cmt_ctrl_user_roles.systemID = 3";
                SqlCommand Mgrcommand = new SqlCommand(queryStrMgr, oConn);
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

                //------------ Fetch Trade Manager list -------------------
                queryStrMgr = "SELECT cmt_ctrl_users.userID, userName, displayName, cmt_ctrl_user_roles.userRoleID " +
                    "FROM SmartBoxData.cmt_ctrl_users, SmartBoxData.cmt_ctrl_user_roles " +
                    "WHERE SmartBoxData.cmt_ctrl_users.userID = SmartBoxData.cmt_ctrl_user_roles.userID " +
                    "AND cmt_ctrl_user_roles.RoleID = 6 AND cmt_ctrl_user_roles.systemID = 1";
                SqlCommand MgrTcommand = new SqlCommand(queryStrMgr, oConn);
                SqlDataReader readerMgrT = MgrTcommand.ExecuteReader();
                List<string> MgrTID = new List<string>();
                List<string> MgrTUser = new List<string>();
                List<string> MgrTName = new List<string>();
                if (readerMgrT.HasRows)
                {
                    while (readerMgrT.Read())
                    {
                        MgrTID.Add(readerMgrT[0].ToString());
                        MgrTUser.Add(readerMgrT[1].ToString());
                        MgrTName.Add(readerMgrT[2].ToString());
                    }
                }
                else
                {
                    MgrTID.Add("");
                    MgrTUser.Add("");
                    MgrTName.Add("");
                }
                int MgrT_Count = MgrID.Count;
                if (MgrT_Count == 0)
                {
                    return "Invalid Manager Trade role setting for support";
                }
                readerMgrT.Close();

                //------------ Fetch CMT trainer list -------------------
                queryStrCMTTrain = "SELECT cmt_ctrl_users.userID, userName, displayName, cmt_ctrl_user_roles.userRoleID " +
                "FROM SmartBoxData.cmt_ctrl_users, SmartBoxData.cmt_ctrl_user_roles " +
                "WHERE SmartBoxData.cmt_ctrl_users.userID = SmartBoxData.cmt_ctrl_user_roles.userID " +
                "AND cmt_ctrl_user_roles.RoleID = 3";
                SqlCommand CMTTcommand = new SqlCommand(queryStrCMTTrain, oConn);
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

                //BBL_K2_CUSTOMERS Trade ALL
                string queryStrCustTT = "SELECT No" +
                      ",CustCode" +
                      ",CustType" +
                      ",CustNameEN" +
                      ",CustNameTH" +
                      ",BusinessType" + //5
                      ",JRNo" +
                      ",TaxID" +
                      ",[By]" +
                      ",contactAddressTH" +
                      ",contactAddress" + //10
                      ",contactEmail" +
                      ",contactTelephone" +
                      ",contactFax" +
                      ",adminNameTH" +
                      ",adminNameEN" + //15
                      ",ID" +
                      ",adminAddress" +
                      ",adminEmail" +
                      ",adminTel" +
                      ",adminFax" + //20
                      ",Purpose" +
                      ",BoardCondition" +
                      ",BoardName" +
                      ",Allocation" +
                      ",currentAcc" + //25
                      ",BCLocation" +
                      ",BCName" +
                      ",SaleName" +
                      ",SaleEmail" +
                      ",SelTel" + //30
                      ",RMname" +
                      ",RMEmail" +
                      ",RMTel" +
                      ",ROName" +
                      ",ROEmail" + //35
                      ",ROTel" +
                      ",responsible" +
                      ",DupCashCheck" +
                  " FROM SmartBoxData.BBL_TRADE_CUSTOMER;";

                //return queryStrCustTT;

                SqlCommand commandCustTT = new SqlCommand(queryStrCustTT, oConn);
                SqlDataReader readerCustTT = commandCustTT.ExecuteReader();
                //DataTable dt = new DataTable();
                if (readerCustTT.HasRows)
                {
                    int rr = 0;
                    int rrT = 0;
                    int Trr = 0;
                    string customerTypeID = "0";
                    string customerType = "";
                    string accMgrID = "0";
                    string accMgrName = "";
                    string saleMgrID = "0";
                    string saleMgrName = "";
                    string saleTeamID = "0";
                    string cashSalesMgr = "";
                    string cashSalesTeam = "";
                    string ownerID = "0";
                    string ownerUser = "";
                    string trainerID = "0";
                    string trainerUser = "";
                    string ManagerID = "0";
                    string ManagerUser = "";
                    string ManagerName = "";

                    while (readerCustTT.Read())
                    {
                        if (Trr == Owner_Count)
                        {
                            Trr = 0;
                        }
                        if (rr == CMT_Count)
                        {
                            rr = 0;
                        }
                        if (rrT == CMTT_Count)
                        {
                            rrT = 0;
                        }

                        if (readerCustTT[2].ToString().Replace("'", "''") == "Toptier")
                        {
                            customerTypeID = "1";
                            customerType = "Toptier";
                            ownerID = OwnerID_rr[Trr];
                            ownerUser = OwnerUser_rr[Trr];
                            trainerID = OwnerID_rr[Trr];
                            trainerUser = OwnerUser_rr[Trr];
                            ManagerID = MgrTID[0];
                            ManagerUser = MgrTUser[0];
                            ManagerName = MgrTName[0];
                            Trr++;
                        }
                        else
                        {
                            customerTypeID = "2";
                            customerType = "Std";
                            ownerID = CMTID[rr];
                            ownerUser = CMTUser[rr];
                            trainerID = CMTTID[rrT];
                            trainerUser = CMTTUser[rrT];
                            ManagerID = MgrID[0];
                            ManagerUser = MgrUser[0];
                            ManagerName = MgrName[0];
                            rr++;
                            rrT++;
                        }

                        string noofuser = "(SELECT COUNT(*) FROM SmartBoxData.BBL_TRADE_USER WHERE BBL_TRADE_USER.JRNo = '" + readerCustTT[6].ToString().Replace("'", "''") + "')";
                        string RMRegistedEmail1 = ""; // readerCustTT[16].ToString().Replace("'", "''");
                        string RMRegistedEmail2 = ""; // readerCustTT[17].ToString().Replace("'", "''");
                        string RMRegistedTelephone1 = ""; // readerCustTT[31].ToString().Replace("'", "''");
                        string RMRegistedTelephone2 = ""; // readerCustTT[18].ToString().Replace("'", "''");
                        string RMRegistedMobile1 = ""; // readerCustTT[24].ToString().Replace("'", "''");
                        string RMRegistedMobile2 = ""; // readerCustTT[25].ToString().Replace("'", "''");
                        string RMContactAddress = ""; // readerCustTT[26].ToString().Replace("'", "''");
                        string RMContactEmail2 = ""; // readerCustTT[3].ToString().Replace("'", "''");
                        string RMContactTelephone2 = ""; //readerCustTT[10].ToString().Replace("'", "''");
                        string RMContactMobile1 = ""; //readerCustTT[34].ToString().Replace("'", "''");
                        string RMContactMobile2 = ""; //readerCustTT[27].ToString().Replace("'", "''");
                        string contactEmail2 = ""; // readerCustTT[1].ToString().Replace("'", "''");
                        string contactTelephone2 = ""; //
                        string contactMobile1 = ""; //
                        string contactMobile2 = "";
                        string debitFeeAccount = ""; //readerCustTT[0].ToString().Replace("'", "''");

                        query += "INSERT SmartBoxData.cmt_i_customer_profile (customerTypeID, customerType, systemID," +
                        " systemName, channelID, channelName, compCode, batchNo, juristicNo, taxIDNo, companyNamwEN," +
                        " companyNameTH, holdingCompany, NoOfUsers, contactPersonName, RMRegistedAddress, RMRegistedEmail1," +
                        " RMRegistedEmail2, RMRegistedTelephone1, RMRegistedTelephone2, RMRegistedMobile1, RMRegistedMobile2," +
                        " RMContactAddress, RMContactEmail1, RMContactEmail2, RMContactTelephone1, RMContactTelephone2," +
                        " RMContactMobile1, RMContactMobile2, contactAddress, contactEmail1, contactEmail2, contactTelephone1," +
                        " contactTelephone2, contactMobile1, contactMobile2, accountMgrID, accountMgr, salesMgrID," +
                        " cashSalesMgr, cashSalesTeam, topTierFlag, debitFeeAccount, cmtOwnerID, cmtOwner, trainerOwnerID," +
                        " trainerOwner, RMName, courseID, CourseNo, customerStatus, cmtManagerID, cmtManager, modifyBy," +
                        " modifyDate, directEmail, cashSalesTeamID, NoOfTrainingUsers, trainingTypeID, trainingTypeName, personType, faxNo, boardAuthority, " +
                        " boardName, allocation, currentAccount, bcLocation, bcName, tradeSaleName, tradeSaleEmail, tradeSaleTel, ROName, ROEmail, ROTel, DuplicateCashCheck, " +
                        " adminNameTH, adminNameEN, adminIDNo, adminAddress, adminEmail, adminTelMobile, adminFax, adminWishService, contactAddressTH, RMContactName)" +
                        " VALUES ( " + customerTypeID + ", N'" + customerType + "', 1, N'Trade', 1, N'iTrade', N'" + readerCustTT[1].ToString().Replace("'", "''") + "'," +
                        " 1 , N'" + readerCustTT[6].ToString().Replace("'", "''") + "', N'" + readerCustTT[7].ToString().Replace("'", "''") + "', N'" + readerCustTT[3].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustTT[4].ToString().Replace("'", "''") + "', N'" + readerCustTT[1].ToString().Replace("'", "''") + "', " + noofuser + "," +
                        " N'" + readerCustTT[8].ToString().Replace("'", "''") + "', N'" + readerCustTT[10].ToString().Replace("'", "''") + "', N'" + RMRegistedEmail1 + "', N'" + RMRegistedEmail2 + "'," +
                        " N'" + RMRegistedTelephone1 + "', N'" + RMRegistedTelephone2 + "', N'" + RMRegistedMobile1 + "', N'" + RMRegistedMobile2 + "'," +
                        " N'" + RMContactAddress + "', N'" + readerCustTT[32].ToString().Replace("'", "''") + "', N'" + RMContactEmail2 + "'," +
                        " N'" + readerCustTT[33].ToString().Replace("'", "''") + "', N'" + RMContactTelephone2 + "', N'" + RMContactMobile1 + "', N'" + RMContactMobile2 + "'," +
                        " N'" + readerCustTT[10].ToString().Replace("'", "''") + "', N'" + readerCustTT[11].ToString().Replace("'", "''") + "', N'" + contactEmail2 + "', N'" + readerCustTT[12].ToString().Replace("'", "''") + "'," +
                        " N'" + contactTelephone2 + "', N'" + contactMobile1 + "', N'" + contactMobile2 + "'," +
                        " " + accMgrID + ", N'" + accMgrName + "', " + saleMgrID + ", N'" + cashSalesMgr + "', N'" + cashSalesTeam + "'," +
                        " N'" + readerCustTT[2].ToString().Replace("'", "''") + "', N'" + debitFeeAccount + "', " + ownerID + ", N'" + ownerUser + "'," +
                        " " + trainerID + ", N'" + trainerUser + "'," +
                        " N'" + readerCustTT[31].ToString().Replace("'", "''") + "', NULL, N'', N'Active', " + ManagerID + ", N'" + ManagerUser + "', N'" + ManagerName + "'," +
                        " GETDATE(), 0, " + saleTeamID + ", " + noofuser + ", 0, '', N'" + readerCustTT[5].ToString().Replace("'", "''") + "', N'" + readerCustTT[13].ToString().Replace("'", "''") + "', " +
                        " N'" + readerCustTT[22].ToString().Replace("'", "''") + "', N'" + readerCustTT[23].ToString().Replace("'", "''") + "', N'" + readerCustTT[24].ToString().Replace("'", "''") + "', " +
                        " N'" + readerCustTT[25].ToString().Replace("'", "''") + "', N'" + readerCustTT[26].ToString().Replace("'", "''") + "', N'" + readerCustTT[27].ToString().Replace("'", "''") + "', " +
                        " N'" + readerCustTT[28].ToString().Replace("'", "''") + "', N'" + readerCustTT[29].ToString().Replace("'", "''") + "', N'" + readerCustTT[30].ToString().Replace("'", "''") + "', " +
                        " N'" + readerCustTT[34].ToString().Replace("'", "''") + "', N'" + readerCustTT[35].ToString().Replace("'", "''") + "', N'" + readerCustTT[36].ToString().Replace("'", "''") + "', N'" + readerCustTT[38].ToString().Replace("'", "''") + "', " +
                        " N'" + readerCustTT[14].ToString().Replace("'", "''") + "', N'" + readerCustTT[15].ToString().Replace("'", "''") + "', N'" + readerCustTT[16].ToString().Replace("'", "''") + "', " +
                        " N'" + readerCustTT[17].ToString().Replace("'", "''") + "', N'" + readerCustTT[18].ToString().Replace("'", "''") + "', N'" + readerCustTT[19].ToString().Replace("'", "''") + "', " +
                        " N'" + readerCustTT[20].ToString().Replace("'", "''") + "', N'" + readerCustTT[21].ToString().Replace("'", "''") + "', N'" + readerCustTT[9].ToString().Replace("'", "''") + "', N'" + readerCustTT[31].ToString().Replace("'", "''") + "'); ";

                       
                    }

                }
                else
                {
                    return "No data for Customer Importing.";
                }
                readerCustTT.Close();

                //return query;

                SqlCommand commandCustomers = new SqlCommand(query, oConn);
                commandCustomers.ExecuteNonQuery();
                string queryAll = "";

                //BBL_K2_CUSTOMERS Trade All
                string queryStrCustAll = "SELECT custID " +
                 " , customerTypeID " +
                 " , customerType " +
                 " , systemID " +
                 " , systemName " +
                 " , channelID " + //5
                 " , channelName " +
                 " , compCode " +
                 " , batchNo " +
                 " , juristicNo " +
                 " , taxIDNo " + //10
                 " , companyNamwEN " +
                 " , companyNameTH " +
                 " , holdingCompany " +
                 " , NoOfUsers " +
                 " , contactPersonName " + //15
                 " , RMRegistedAddress " +
                 " , RMRegistedEmail1 " +
                 " , RMRegistedEmail2 " +
                 " , RMRegistedTelephone1 " +
                 " , RMRegistedTelephone2 " + //20
                 " , RMRegistedMobile1 " +
                 " , RMRegistedMobile2 " +
                 " , RMContactAddress " +
                 " , RMContactEmail1 " +
                 " , RMContactEmail2 " + //25
                  " , RMContactTelephone1 " +
                  " , RMContactTelephone2 " +
                  " , RMContactMobile1 " +
                 " , RMContactMobile2 " +
                 " , contactAddress " + //30
                 " , contactEmail1 " +
                 " , contactEmail2 " +
                 " , contactTelephone1 " +
                 " , contactTelephone2 " +
                 " , contactMobile1 " + //35
                 " , contactMobile2 " +
                 " , accountMgrID " +
                 " , accountMgr " +
                 " , salesMgrID " +
                 " , cashSalesMgr " + //40
                 " , cashSalesTeam " +
                 " , topTierFlag " +
                 " , debitFeeAccount " +
                 " , cmtOwnerID " +
                 " , cmtOwner " + //45
                 " , trainerOwnerID " +
                 " , trainerOwner " +
                 " , RMName " +
                 " , courseID " +
                 " , CourseNo " + //50
                 " , customerStatus " +
                 " , cmtManagerID " +
                 " , cmtManager " +
                 " , modifyBy " +
                 " , modifyDate " + //55
                 " , directEmail " +
                 " , cashSalesTeamID " +
                 " , NoOfTrainingUsers " +
                " FROM SmartBoxData.cmt_i_customer_profile " +
                " WHERE cmt_i_customer_profile.systemID = 1 AND cmt_i_customer_profile.custID NOT IN (SELECT cmt_t_mytask_request.custID FROM SmartBoxData.cmt_t_mytask_request WHERE custID IS NOT NULL);";

                SqlCommand commandCustAll = new SqlCommand(queryStrCustAll, oConn);
                SqlDataReader readerCustAll = commandCustAll.ExecuteReader();
                //DataTable dt = new DataTable();
                if (readerCustAll.HasRows)
                {

                    while (readerCustAll.Read())
                    {
                        queryAll += "INSERT SmartBoxData.cmt_t_mytask_request (batchNo,custID,customerType,compCode,companyNameTH,companyNameEN," +
                             "companyGroupCode,juristicNo,custSupport,stageID,stageName,channelID,channelName,systemID,systemName,statusID,status," +
                             "assignedTo,actionRequire,currentFunction,remark,createBy,createDate,lastAction,lastUpdateBy,lastUpdateDate,byPassProcess," +
                             "currentFunctionID,currentActionStatusID,currentActionStatusName,roleID,flagSendEmail,tokenBooked,registeredUAS," +
                             "validateDocument,iTradeRegistered,custTypeID,taskSerialNo,applyToAllCompany,procInstId)" +
                         " VALUES (" + readerCustAll[8].ToString().Replace("'", "''") + ", " + readerCustAll[0].ToString().Replace("'", "''") + ", N'" + readerCustAll[2].ToString().Replace("'", "''") + "', N'" + readerCustAll[7].ToString().Replace("'", "''") + "', " +
                         "N'" + readerCustAll[12].ToString().Replace("'", "''") + "', N'" + readerCustAll[11].ToString().Replace("'", "''") + "', N'" + readerCustAll[13].ToString().Replace("'", "''") + "', N'" + readerCustAll[9].ToString().Replace("'", "''") + "', " +
                         "(SELECT displayName FROM SmartBoxData.cmt_ctrl_users WHERE userName = '" + readerCustAll[53].ToString() + "' ), 1, N'Prerequisites', " + readerCustAll[5].ToString().Replace("'", "''") + ", N'" + readerCustAll[6].ToString().Replace("'", "''") + "', " + readerCustAll[3].ToString().Replace("'", "''") + ", " +
                         "N'" + readerCustAll[4].ToString().Replace("'", "''") + "', 3, N'InProgress', N'" + readerCustAll[53].ToString().Replace("'", "''") + "', " +
                         "N'No action needed', N'Import Customer Data', N'', N'System', GETDATE(), N'System', N'System', GETDATE(), NULL, 15, 1, N'Open', 6, " +
                         "NULL, NULL, NULL, NULL, NULL, " + readerCustAll[1].ToString() + ", N'', NULL, NULL); ";
                    }

                }
                readerCustAll.Close();
                //return queryAll;
                if (queryAll != "")
                {
                    SqlCommand commandAlls = new SqlCommand(queryAll, oConn);
                    commandAlls.ExecuteNonQuery();

                }


                //BBL_K2_ACCOUNT Trade
                string queryStrAcc = "SELECT JRNo,CompName,DDA,AccName,Branch,Purpose FROM SmartBoxData.BBL_TRADE_ACCOUNT WHERE JRNo IN " +
                "(SELECT BBL_TRADE_CUSTOMER.JRNo FROM SmartBoxData.BBL_TRADE_CUSTOMER);";
                string queryAcc = "";
                SqlCommand commandAcc = new SqlCommand(queryStrAcc, oConn);
                SqlDataReader readerAcc = commandAcc.ExecuteReader();
                if (readerAcc.HasRows)
                {
                    while (readerAcc.Read())
                    {
                        queryAcc += "INSERT SmartBoxData.cmt_i_customer_accounts(custID, companyCode, accountNo, createdBy, createdDate, modifyBy, modifyDate, active) " +
                          "VALUES((SELECT TOP 1 cmt_i_customer_profile.custID FROM SmartBoxData.cmt_i_customer_profile WHERE cmt_i_customer_profile.juristicNo = '" + readerAcc[0].ToString() + "')," +
                          "(SELECT TOP 1 cmt_i_customer_profile.CompCode FROM SmartBoxData.cmt_i_customer_profile WHERE cmt_i_customer_profile.juristicNo = '" + readerAcc[0].ToString() + "')," +
                          "'" + readerAcc[2].ToString() + "', '" + MgrName[0] + "',GETDATE(),'" + MgrName[0] + "',GETDATE(),1); ";
                    }


                }
                readerAcc.Close();
                // //return queryAcc;
                if (queryAcc != "")
                {
                    SqlCommand commandAccs = new SqlCommand(queryAcc, oConn);
                    commandAccs.ExecuteNonQuery();

                }

                //BBL_K2_USERS Trade
                string queryStrUsr = "SELECT JRNo,CompName,IDNo,UserName,UserID,Email,MobileNo,Department,Role FROM SmartBoxData.BBL_TRADE_USER WHERE JRNo IN " +
            "(SELECT BBL_TRADE_CUSTOMER.JRNo FROM SmartBoxData.BBL_TRADE_CUSTOMER)";
                string queryUsr = "";
                SqlCommand commandUsr = new SqlCommand(queryStrUsr, oConn);
                SqlDataReader readerUsr = commandUsr.ExecuteReader();
                if (readerUsr.HasRows)
                {
                    while (readerUsr.Read())
                    {
                        

                        queryUsr += "INSERT SmartBoxData.cmt_i_customer_users(custID,companyCode,userName,userID,userRole," +
                            "createdBy,createdDate,modifyBy,modifyDate) " +
                          "VALUES((SELECT TOP 1 cmt_i_customer_profile.custID FROM SmartBoxData.cmt_i_customer_profile WHERE cmt_i_customer_profile.juristicNo = '" + readerUsr[0].ToString() + "')," +
                          "(SELECT TOP 1 cmt_i_customer_profile.compcode FROM SmartBoxData.cmt_i_customer_profile WHERE cmt_i_customer_profile.juristicNo = '" + readerUsr[0].ToString() + "'), " +
                          "'" + readerUsr[3].ToString() + "', '" + readerUsr[4].ToString() + "', '" + readerUsr[8].ToString() + "', " +
                          "'" + MgrName[0] + "',GETDATE(),'" + MgrName[0] + "',GETDATE()) ;";
                    }


                }
                readerUsr.Close();
                //return queryAcc;
                if (queryUsr != "")
                {
                    SqlCommand commandUsrs = new SqlCommand(queryUsr, oConn);
                    commandUsrs.ExecuteNonQuery();
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

        [WebMethod()]
        public string AssignOwnerCash(int lot)
        {

            try
            {
                SqlConnection oConn;
                string SQLConnectionStr = "MyConn";
                string SQLConString;
                SQLConString = ConfigurationManager.ConnectionStrings[SQLConnectionStr].ToString();
                Console.WriteLine(SQLConString);
                oConn = new SqlConnection(SQLConString);
                oConn.Open();
                Console.WriteLine("SQL Connection Opened, OK");

                string queryStr = "";
                string queryStrTradeMgr = "";
                string queryStrMgr = "";
                string queryStrCMT = "";
                string queryStrCMTTrain = "";
                string query = "";

                //------------ Fetch Implement Cash owner list -------------------
                queryStr = "SELECT cmt_ctrl_users.userID, userName, displayName, cmt_ctrl_user_roles.userRoleID " +
                "FROM SmartBoxData.cmt_ctrl_users, SmartBoxData.cmt_ctrl_user_roles " +
                "WHERE SmartBoxData.cmt_ctrl_users.userID = SmartBoxData.cmt_ctrl_user_roles.userID " +
                "AND cmt_ctrl_user_roles.RoleID = 2 AND cmt_ctrl_user_roles.systemID = 2";
                SqlCommand command = new SqlCommand(queryStr, oConn);
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
                if(Owner_Count == 0)
                {
                    return "Invalid Implement role setting for support";
                }
                reader.Close();

                //------------ Fetch CMT owner list -------------------
                queryStrCMT = "SELECT cmt_ctrl_users.userID, userName, displayName, cmt_ctrl_user_roles.userRoleID " +
                "FROM SmartBoxData.cmt_ctrl_users, SmartBoxData.cmt_ctrl_user_roles " +
                "WHERE SmartBoxData.cmt_ctrl_users.userID = SmartBoxData.cmt_ctrl_user_roles.userID " +
                "AND cmt_ctrl_user_roles.RoleID = 1";
                SqlCommand CMTcommand = new SqlCommand(queryStrCMT, oConn);
                SqlDataReader readerCMT = CMTcommand.ExecuteReader();
                List<string> CMTID = new List<string>();
                List<string> CMTUser = new List<string>();
                List<string> CMTName = new List<string>();
                if(readerCMT.HasRows)
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
                "FROM SmartBoxData.cmt_ctrl_users, SmartBoxData.cmt_ctrl_user_roles " +
                "WHERE SmartBoxData.cmt_ctrl_users.userID = SmartBoxData.cmt_ctrl_user_roles.userID " +
                "AND cmt_ctrl_user_roles.RoleID = 3";
                SqlCommand CMTTcommand = new SqlCommand(queryStrCMTTrain, oConn);
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
                    "FROM SmartBoxData.cmt_ctrl_users, SmartBoxData.cmt_ctrl_user_roles " +
                    "WHERE SmartBoxData.cmt_ctrl_users.userID = SmartBoxData.cmt_ctrl_user_roles.userID " +
                    "AND cmt_ctrl_user_roles.RoleID = 6 AND (cmt_ctrl_user_roles.systemID = 2 OR cmt_ctrl_user_roles.systemID = 3)";
                SqlCommand Mgrcommand = new SqlCommand(queryStrMgr, oConn);
                SqlDataReader readerMgr = Mgrcommand.ExecuteReader();
                List<string> MgrID = new List<string>();
                List<string> MgrUser = new List<string>();
                List<string> MgrName = new List<string>();
                if(readerMgr.HasRows)
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
                "rec_ID FROM SmartBoxData.BBL_K2_CUSTOMERS WHERE TOP_TIER_FLAG = 'Y' AND MIGRATION_LOT = " + lot;

                SqlCommand commandCustTT = new SqlCommand(queryStrCustTT, oConn);
                SqlDataReader readerCustTT = commandCustTT.ExecuteReader();
                //DataTable dt = new DataTable();
                if (readerCustTT.HasRows)
                {
                    int rr = 0;
                    int rrIndex = 0;
                    string customerTypeID = "0";
                    string customerType = "";
                    string accMgrID = "0";
                    string accMgrName = "";
                    string saleMgrID = "0";
                    string saleMgrName = "";
                    string saleTeamID = "0";
                    string CHName = "";
                    string CHID = "0";
                    while (readerCustTT.Read())
                    {
                        customerTypeID = "1";
                        customerType = "Toptier";
 
                        if(rr == (Owner_Count))
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
                        else
                        {
                            CHName = "BiziBangking";
                            CHID = "3";
                        }
                        query += "INSERT SmartBoxData.cmt_i_customer_profile (customerTypeID, customerType, systemID," +
                        " systemName, channelID, channelName, compCode, batchNo, juristicNo, taxIDNo, companyNamwEN," +
                        " companyNameTH, holdingCompany, NoOfUsers, contactPersonName, RMRegistedAddress, RMRegistedEmail1," +
                        " RMRegistedEmail2, RMRegistedTelephone1, RMRegistedTelephone2, RMRegistedMobile1, RMRegistedMobile2," +
                        " RMContactAddress, RMContactEmail1, RMContactEmail2, RMContactTelephone1, RMContactTelephone2," +
                        " RMContactMobile1, RMContactMobile2, contactAddress, contactEmail1, contactEmail2, contactTelephone1," +
                        " contactTelephone2, contactMobile1, contactMobile2, accountMgrID, accountMgr, salesMgrID," +
                        " cashSalesMgr, cashSalesTeam, topTierFlag, debitFeeAccount, cmtOwnerID, cmtOwner, trainerOwnerID," +
                        " trainerOwner, RMName, courseID, CourseNo, customerStatus, cmtManagerID, cmtManager, modifyBy," +
                        " modifyDate, directEmail, cashSalesTeamID, NoOfTrainingUsers, trainingTypeID, trainingTypeName)" +
                        " VALUES ( " + customerTypeID + ", N'" + customerType + "', 2, N'Cash', " + CHID + ", N'" + CHName + "', N'" + readerCustTT[30].ToString().Replace("'", "''") + "'," +
                        lot + " , N'" + readerCustTT[32].ToString().Replace("'", "''") + "', N'', N'" + readerCustTT[8].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustTT[9].ToString().Replace("'", "''") + "', N'" + readerCustTT[23].ToString().Replace("'", "''") + "', " + noofuser + "," +
                        " N'" + readerCustTT[15].ToString().Replace("'", "''") + "', N'" + readerCustTT[33].ToString().Replace("'", "''") + "', N'" + readerCustTT[16].ToString().Replace("'", "''") + "', N'" + readerCustTT[17].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustTT[31].ToString().Replace("'", "''") + "', N'" + readerCustTT[18].ToString().Replace("'", "''") + "', N'" + readerCustTT[24].ToString().Replace("'", "''") + "', N'" + readerCustTT[25].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustTT[26].ToString().Replace("'", "''") + "', N'" + readerCustTT[19].ToString().Replace("'", "''") + "', N'" + readerCustTT[3].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustTT[0].ToString().Replace("'", "''") + "', N'" + readerCustTT[10].ToString().Replace("'", "''") + "', N'" + readerCustTT[34].ToString().Replace("'", "''") + "', N'" + readerCustTT[27].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustTT[4].ToString().Replace("'", "''") + "', N'" + readerCustTT[11].ToString().Replace("'", "''") + "', N'" + readerCustTT[1].ToString().Replace("'", "''") + "', N'" + readerCustTT[12].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustTT[13].ToString().Replace("'", "''") + "', N'" + readerCustTT[5].ToString().Replace("'", "''") + "', N'" + readerCustTT[6].ToString().Replace("'", "''") + "'," +
                        " " + accMgrID + ", N'" + accMgrName + "', " + saleMgrID + ", N'" + saleMgrName + "', N'" + readerCustTT[21].ToString() + "'," +
                        " N'" + readerCustTT[22].ToString().Replace("'", "''") + "', N'" + readerCustTT[28].ToString().Replace("'", "''") + "', 0, N''," +
                        " " + OwnerID_rr[rr] + ", N'" + OwnerUser_rr[rr] + "'," +
                        " N'', NULL, N'', N'Active', " + MgrID[0] + ", N'" + MgrUser[0] + "', N'" + MgrName[0] + "'," +
                        " GETDATE(), 0, " + saleTeamID + ", 2, 0, ''); ";

                        rr++;
                    }
                    
                }
                
                readerCustTT.Close();

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
                "rec_ID FROM BBL_K2_CUSTOMERS WHERE TOP_TIER_FLAG = 'N' AND MIGRATION_LOT = " + lot;

                SqlCommand commandCustStd = new SqlCommand(queryStrCustStd, oConn);
                SqlDataReader readerCustStd = commandCustStd.ExecuteReader();
                //DataTable dt = new DataTable();
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
                    while (readerCustStd.Read())
                    {
                        customerTypeID = "2";
                        customerType = "Std";

                        if (rr == (CMT_Count))
                        {
                            rr = 0;
                        }
                        if(rrt == CMTT_Count)
                        {
                            rrt = 0;
                        }

                        string noofuser = readerCustStd[2].ToString();
                        if(noofuser == "")
                        {
                            noofuser = "NULL";
                        }
                        if (readerCustStd[35].ToString() == "ICH")
                        {
                            CHName = "iCash";
                            CHID = "2";
                        }
                        else
                        {
                            CHName = "BiziBangking";
                            CHID = "3";
                        }
                        query += "INSERT SmartBoxData.cmt_i_customer_profile (customerTypeID, customerType, systemID," +
                        " systemName, channelID, channelName, compCode, batchNo, juristicNo, taxIDNo, companyNamwEN," +
                        " companyNameTH, holdingCompany, NoOfUsers, contactPersonName, RMRegistedAddress, RMRegistedEmail1," +
                        " RMRegistedEmail2, RMRegistedTelephone1, RMRegistedTelephone2, RMRegistedMobile1, RMRegistedMobile2," +
                        " RMContactAddress, RMContactEmail1, RMContactEmail2, RMContactTelephone1, RMContactTelephone2," +
                        " RMContactMobile1, RMContactMobile2, contactAddress, contactEmail1, contactEmail2, contactTelephone1," +
                        " contactTelephone2, contactMobile1, contactMobile2, accountMgrID, accountMgr, salesMgrID," +
                        " cashSalesMgr, cashSalesTeam, topTierFlag, debitFeeAccount, cmtOwnerID, cmtOwner, trainerOwnerID," +
                        " trainerOwner, RMName, courseID, CourseNo, customerStatus, cmtManagerID, cmtManager, modifyBy," +
                        " modifyDate, directEmail, cashSalesTeamID, NoOfTrainingUsers, trainingTypeID, trainingTypeName)" +
                        " VALUES ( " + customerTypeID + ", N'" + customerType + "', 2, N'Cash', " + CHID + ", N'" + CHName + "', N'" + readerCustStd[30].ToString().Replace("'", "''") + "'," +
                        lot + " , N'" + readerCustStd[32].ToString().Replace("'","''") + "', N'', N'" + readerCustStd[8].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustStd[9].ToString().Replace("'", "''") + "', N'" + readerCustStd[23].ToString().Replace("'", "''") + "', " + noofuser + "," +
                        " N'" + readerCustStd[15].ToString().Replace("'", "''") + "', N'" + readerCustStd[33].ToString().Replace("'", "''") + "', N'" + readerCustStd[16].ToString().Replace("'", "''") + "', N'" + readerCustStd[17].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustStd[31].ToString().Replace("'", "''") + "', N'" + readerCustStd[18].ToString().Replace("'", "''") + "', N'" + readerCustStd[24].ToString().Replace("'", "''") + "', N'" + readerCustStd[25].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustStd[26].ToString().Replace("'", "''") + "', N'" + readerCustStd[19].ToString().Replace("'", "''") + "', N'" + readerCustStd[3].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustStd[0].ToString().Replace("'", "''") + "', N'" + readerCustStd[10].ToString().Replace("'", "''") + "', N'" + readerCustStd[34].ToString().Replace("'", "''") + "', N'" + readerCustStd[27].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustStd[4].ToString().Replace("'", "''") + "', N'" + readerCustStd[11].ToString().Replace("'", "''") + "', N'" + readerCustStd[1].ToString().Replace("'", "''") + "', N'" + readerCustStd[12].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustStd[13].ToString().Replace("'", "''") + "', N'" + readerCustStd[5].ToString().Replace("'", "''") + "', N'" + readerCustStd[6].ToString().Replace("'", "''") + "'," +
                        " " + accMgrID + ", N'" + accMgrName + "', " + saleMgrID + ", N'" + saleMgrName + "', N'" + readerCustStd[21].ToString().Replace("'", "''") + "'," +
                        " N'" + readerCustStd[22].ToString().Replace("'", "''") + "', N'" + readerCustStd[28].ToString().Replace("'", "''") + "', " + CMTID[rr] + ", N'" + CMTUser[rr] + "'," +
                        " " + CMTTID[rrt] + ", N'" + CMTTUser[rrt] + "'," +
                        " N'', NULL, N'', N'Active', " + MgrID[0] + ", N'" + MgrUser[0] + "', N'" + MgrName[0] + "'," +
                        " GETDATE(), 1, " + saleTeamID + ", 2, 5, 'Self learning'); ";
                        rrt++;
                        rr++;
                    }
                    
                }
                readerCustStd.Close();
                //if(debug == 1)
                //{
                //    return query;
                //}
                
                SqlCommand commandCustomers = new SqlCommand(query, oConn);
                commandCustomers.ExecuteNonQuery();
                string queryAll = "";

                //BBL_K2_CUSTOMERS Cash All
                string queryStrCustAll = "SELECT custID " +
                 " , customerTypeID " +
                 " , customerType " +
                 " , systemID " +
                 " , systemName " +
                 " , channelID " + //5
                 " , channelName " +
                 " , compCode " +
                 " , batchNo " +
                 " , juristicNo " +
                 " , taxIDNo " + //10
                 " , companyNamwEN " +
                 " , companyNameTH " +
                 " , holdingCompany " +
                 " , NoOfUsers " +
                 " , contactPersonName " + //15
                 " , RMRegistedAddress " +
                 " , RMRegistedEmail1 " +
                 " , RMRegistedEmail2 " +
                 " , RMRegistedTelephone1 " +
                 " , RMRegistedTelephone2 " + //20
                 " , RMRegistedMobile1 " +
                 " , RMRegistedMobile2 " +
                 " , RMContactAddress " +
                 " , RMContactEmail1 " +
                 " , RMContactEmail2 " + //25
                  " , RMContactTelephone1 " +
                  " , RMContactTelephone2 " +
                  " , RMContactMobile1 " +
                 " , RMContactMobile2 " +
                 " , contactAddress " + //30
                 " , contactEmail1 " +
                 " , contactEmail2 " +
                 " , contactTelephone1 " +
                 " , contactTelephone2 " +
                 " , contactMobile1 " + //35
                 " , contactMobile2 " +
                 " , accountMgrID " +
                 " , accountMgr " +
                 " , salesMgrID " +
                 " , cashSalesMgr " + //40
                 " , cashSalesTeam " +
                 " , topTierFlag " +
                 " , debitFeeAccount " +
                 " , cmtOwnerID " +
                 " , cmtOwner " + //45
                 " , trainerOwnerID " +
                 " , trainerOwner " +
                 " , RMName " +
                 " , courseID " +
                 " , CourseNo " + //50
                 " , customerStatus " +
                 " , cmtManagerID " +
                 " , cmtManager " +
                 " , modifyBy " +
                 " , modifyDate " + //55
                 " , directEmail " +
                 " , cashSalesTeamID " +
                 " , NoOfTrainingUsers " +
                " FROM SmartBoxData.cmt_i_customer_profile " +
                " WHERE systemID = 2 AND batchNo = " + lot;

                SqlCommand commandCustAll = new SqlCommand(queryStrCustAll, oConn);
                SqlDataReader readerCustAll = commandCustAll.ExecuteReader();
                //DataTable dt = new DataTable();
                if (readerCustAll.HasRows)
                {
                    string custsupport = "";
                    //string MgrOwnerName = "";
                    while (readerCustAll.Read())
                    {
                        //MgrOwnerName = "(SELECT displayName FROM SmartBoxData.cmt_ctrl_users WHERE userName = '" + readerCustAll[53].ToString() + "' )";
                        custsupport = "(SELECT cmt_ctrl_users.displayName FROM SmartBoxData.cmt_ctrl_users WHERE cmt_ctrl_users.userName = '" + readerCustAll[45].ToString() + "' )";
                        if (readerCustAll[2].ToString().Replace("'", "''") == "Toptier")
                        {
                            custsupport = "N''";
                        }
                        queryAll += "INSERT SmartBoxData.cmt_t_mytask_request (batchNo,custID,customerType,compCode,companyNameTH,companyNameEN," +
                             "companyGroupCode,juristicNo,custSupport,stageID,stageName,channelID,channelName,systemID,systemName,statusID,status," +
                             "assignedTo,actionRequire,currentFunction,remark,createBy,createDate,lastAction,lastUpdateBy,lastUpdateDate,byPassProcess," +
                             "currentFunctionID,currentActionStatusID,currentActionStatusName,roleID,flagSendEmail,tokenBooked,registeredUAS," +
                             "validateDocument,iTradeRegistered,custTypeID,taskSerialNo,applyToAllCompany,procInstId)" +
                         " VALUES (" + readerCustAll[8].ToString().Replace("'", "''") + ", " + readerCustAll[0].ToString().Replace("'", "''") + ", N'" + readerCustAll[2].ToString().Replace("'", "''") + "', N'" + readerCustAll[7].ToString().Replace("'", "''") + "', " +
                         "N'" + readerCustAll[12].ToString().Replace("'", "''") + "', N'" + readerCustAll[11].ToString().Replace("'", "''") + "', N'" + readerCustAll[13].ToString().Replace("'", "''") + "', N'" + readerCustAll[9].ToString().Replace("'", "''") + "', " +
                         custsupport + " , 1, N'Prerequisites', " + readerCustAll[5].ToString().Replace("'", "''") + ", N'" + readerCustAll[6].ToString().Replace("'", "''") + "', " + readerCustAll[3].ToString().Replace("'", "''") + ", " +
                         "N'" + readerCustAll[4].ToString().Replace("'", "''") + "', 3, N'InProgress', N'" + readerCustAll[53].ToString().Replace("'", "''") + "', " +
                         "N'No action needed', N'Import Customer Data', N'', N'" + readerCustAll[54].ToString().Replace("'", "''") + "', GETDATE(), N'Import Data', N'" + readerCustAll[54].ToString().Replace("'", "''") + "', GETDATE(), NULL, 15, 1, N'Open', 6, " +
                         "NULL, NULL, NULL, NULL, NULL, " + readerCustAll[1].ToString() + ", N'', NULL, NULL); ";
                        queryAll += "INSERT INTO SmartBoxData.cmt_t_history_log (custID,stage,stageName,cmt_category,cmtCategoryName,status,createBy," +
                            "createDate,actFormType,valueDate1,functionID,actionStatusID) VALUES (" + readerCustAll[0].ToString() + ", 1, 'Prerequisites', 40, 'Import Data', 3, " +
                            "N'" + readerCustAll[54].ToString().Replace("'", "''") + "', GETDATE(), 1,GETDATE(),15,1); ";
                    }

                }
                readerCustAll.Close();
                //return queryAll;
                if (queryAll != "")
                {
                    SqlCommand commandAlls = new SqlCommand(queryAll, oConn);
                    commandAlls.ExecuteNonQuery();

                }


                //BBL_K2_ACCOUNT Cash
                string queryStrAcc = "SELECT RECORD_TYPE, COMP_CODE, ACCT_NO, rec_ID FROM SmartBoxData.BBL_K2_ACCOUNTS WHERE COMP_CODE IN " +
                "(SELECT BBL_K2_CUSTOMERS.COMP_CODE FROM SmartBoxData.BBL_K2_CUSTOMERS WHERE MIGRATION_LOT = " + lot + " )";
                string queryAcc = "";
                SqlCommand commandAcc = new SqlCommand(queryStrAcc, oConn);
                SqlDataReader readerAcc = commandAcc.ExecuteReader();
                if (readerAcc.HasRows)
                {
                    while (readerAcc.Read())
                    {
                        queryAcc += "INSERT SmartBoxData.cmt_i_customer_accounts(custID, companyCode, accountNo, createdBy, createdDate, modifyBy, modifyDate, active) " +
                          "VALUES((SELECT TOP 1 cmt_i_customer_profile.custID FROM SmartBoxData.cmt_i_customer_profile WHERE " +
                          "cmt_i_customer_profile.compCode = '" + readerAcc[1].ToString() + "'),'" + readerAcc[1].ToString() + "','" + readerAcc[2].ToString() + "', '" + MgrName[0] + "',GETDATE(),'" + MgrName[0] + "',GETDATE(),1); ";
                    }

                    
                }
                readerAcc.Close();
                // //return queryAcc;
                if (queryAcc != "")
                {
                    SqlCommand commandAccs = new SqlCommand(queryAcc, oConn);
                    commandAccs.ExecuteNonQuery();

                }
                
                //BBL_K2_AUTH Cash
                string queryStrAuth = "SELECT PRODUCT_DESC,PRODUCT_CODE,MIN_AMT,LEVEL_APPRV,RECORD_TYPE,COMP_CODE,AUTH_ORDER,MAX_AMT,RULE_DEFINITION,rec_ID FROM SmartBoxData.BBL_K2_AUTH WHERE COMP_CODE IN " +
                "(SELECT BBL_K2_CUSTOMERS.COMP_CODE FROM SmartBoxData.BBL_K2_CUSTOMERS WHERE MIGRATION_LOT = " + lot + " )";
                string queryAuth = "";
                SqlCommand commandAuth = new SqlCommand(queryStrAuth, oConn);
                SqlDataReader readerAuth = commandAuth.ExecuteReader();
                if (readerAuth.HasRows)
                {
                    while (readerAuth.Read())
                    {
                        string minAmount  = readerAuth[2].ToString();
                        if (minAmount == "")
                        {
                            minAmount = "NULL";
                        }
                        string maxAmount = readerAuth[7].ToString();
                        if (maxAmount == "")
                        {
                            maxAmount = "NULL";
                        }
                        queryAuth += "INSERT SmartBoxData.cmt_i_customer_authentications(custID,companyCode,productCode,productDescription,authorizationOrder,minAmount,maxAmount,levelApproval,ruleDefinition,createdBy,createdDate) " +
                          "VALUES((SELECT TOP 1 cmt_i_customer_profile.custID FROM SmartBoxData.cmt_i_customer_profile WHERE " +
                          "cmt_i_customer_profile.compCode = '" + readerAuth[5].ToString() + "'),'" + readerAuth[5].ToString() + "','" + readerAuth[1].ToString() + "', " +
                          "'" + readerAuth[0].ToString() + "','" + readerAuth[6].ToString() + "'," + minAmount + "," + maxAmount + "," +
                          "'" + readerAuth[3].ToString() + "','" + readerAuth[8].ToString() + "','" + MgrName[0] + "',GETDATE()); ";
                    }

                    
                }
                readerAuth.Close();
                // //return queryAuth;
                if (queryAuth != "")
                {
                    SqlCommand commandAuths = new SqlCommand(queryAuth, oConn);
                    commandAuths.ExecuteNonQuery();
                }

                //BBL_K2_PRODUCTS Cash
                string queryStrProd = "SELECT ACCT_PREREGIS,WHT_DOC_SUPPORT,PRODUCT_DESC,PRODUCT_CODE,SIGBLIND,RECORD_TYPE,COMP_CODE,TXN_LIMIT_AMT,DAILY_LIMIT_AMT,rec_ID FROM SmartBoxData.BBL_K2_PRODUCTS WHERE COMP_CODE IN " +
                "(SELECT BBL_K2_CUSTOMERS.COMP_CODE FROM SmartBoxData.BBL_K2_CUSTOMERS WHERE MIGRATION_LOT = " + lot + " )";
                string queryProd = "";
                SqlCommand commandProd = new SqlCommand(queryStrProd, oConn);
                SqlDataReader readerProd = commandProd.ExecuteReader();
                if (readerProd.HasRows)
                {
                    while (readerProd.Read())
                    {
                        string transactionLimitAmount = readerProd[7].ToString();
                        if (transactionLimitAmount == "")
                        {
                            transactionLimitAmount = "NULL";
                        }
                        string dailyLimitAmount = readerProd[8].ToString();
                        if (dailyLimitAmount == "")
                        {
                            dailyLimitAmount = "NULL";
                        }
                        queryProd += "INSERT SmartBoxData.cmt_i_customer_products(custID,companyCode,productCode,productDescription,signBlind,accountPreRegister,WHTDocumentSupport,transactionLimitAmount,dailyLimitAmount,createdBy,createdDate) " +
                          "VALUES((SELECT TOP 1 cmt_i_customer_profile.custID FROM SmartBoxData.cmt_i_customer_profile WHERE " +
                          "cmt_i_customer_profile.compCode = '" + readerProd[6].ToString() + "'),'" + readerProd[6].ToString() + "','" + readerProd[3].ToString() + "', " +
                          "'" + readerProd[2].ToString() + "','" + readerProd[4].ToString() + "','" + readerProd[0].ToString() + "','" + readerProd[1].ToString() + "'," +
                          "" + transactionLimitAmount + "," + dailyLimitAmount + ",'" + MgrName[0] + "',GETDATE()) ;";
                    }

                    
                }
                readerProd.Close();
                // //return queryProd;
                if (queryProd != "")
                {
                    SqlCommand commandProds = new SqlCommand(queryProd, oConn);
                    commandProds.ExecuteNonQuery();
                }

                    //BBL_K2_REPORTS Cash
                    string queryStrRep = "SELECT REPORT_NAME,PRODUCT_DESC,PRODUCT_CODE,RECORD_TYPE,COMP_CODE,GROUP_CODE,REPORT_DESC,rec_ID FROM SmartBoxData.BBL_K2_REPORTS WHERE COMP_CODE IN " +
                "(SELECT BBL_K2_CUSTOMERS.COMP_CODE FROM SmartBoxData.BBL_K2_CUSTOMERS WHERE MIGRATION_LOT = " + lot + " )";
                string queryRep = "";
                SqlCommand commandRep = new SqlCommand(queryStrRep, oConn);
                SqlDataReader readerRep = commandRep.ExecuteReader();
                if (readerRep.HasRows)
                {
                    while (readerRep.Read())
                    {
                        queryRep += "INSERT SmartBoxData.cmt_i_customer_reports(custID,companyCode,productCode,productDescription,reportName,createdBy,createdDate) " +
                          "VALUES((SELECT TOP 1 cmt_i_customer_profile.custID FROM SmartBoxData.cmt_i_customer_profile WHERE " +
                          "cmt_i_customer_profile.compCode = '" + readerRep[4].ToString() + "'),'" + readerRep[4].ToString() + "','" + readerRep[2].ToString() + "', " +
                          "'" + readerRep[1].ToString() + "','" + readerRep[6].ToString() + "','" + MgrName[0] + "',GETDATE()); ";
                    }

                    
                }
                readerRep.Close();
                // //return queryRep;
                if (queryRep != "")
                {
                    SqlCommand commandReps = new SqlCommand(queryRep, oConn);
                    commandReps.ExecuteNonQuery();
                }

                    //BBL_K2_USERS Cash
                    string queryStrUsr = "SELECT USER_STAT,BLOCK_USER,TOKEN_SERL_NO,USER_CODE,USER_ROLE,USER_NAME,SIGNBLIND,RECORD_TYPE,COMP_CODE,LAST_LOGIN,DAILY_LIMIT_AMT,GROUP_CODE,rec_ID FROM SmartBoxData.BBL_K2_USERS WHERE COMP_CODE IN " +
                "(SELECT BBL_K2_CUSTOMERS.COMP_CODE FROM SmartBoxData.BBL_K2_CUSTOMERS WHERE MIGRATION_LOT = " + lot + " )";
                string queryUsr = "";
                SqlCommand commandUsr = new SqlCommand(queryStrUsr, oConn);
                SqlDataReader readerUsr = commandUsr.ExecuteReader();
                if (readerUsr.HasRows)
                {
                    while (readerUsr.Read())
                    {
                        string dailyLimitAmount = readerUsr[10].ToString();
                        if(dailyLimitAmount == "")
                        {
                            dailyLimitAmount = "NULL";
                        }

                        queryUsr += "INSERT SmartBoxData.cmt_i_customer_users(custID,companyCode,userStat,userName,userID,userRole,blockUser,tokenSerialNo,signBlind,lastLogin," +
                            "dailyLimitAmount,groupCode,createdBy,createdDate,modifyBy,modifyDate,active) " +
                          "VALUES((SELECT TOP 1 cmt_i_customer_profile.custID FROM SmartBoxData.cmt_i_customer_profile WHERE " +
                          "cmt_i_customer_profile.compCode = '" + readerUsr[8].ToString() + "'),'" + readerUsr[8].ToString() + "', '" + readerUsr[0].ToString() + "', " +
                          "'" + readerUsr[5].ToString() + "', '" + readerUsr[3].ToString() + "', '" + readerUsr[4].ToString() + "', " +
                          "'" + readerUsr[1].ToString() + "', '" + readerUsr[2].ToString() + "', '" + readerUsr[6].ToString() + "', " +
                          " NULL, " + dailyLimitAmount + ", '" + readerUsr[11].ToString() + "', " +
                          "'" + MgrName[0] + "',GETDATE(),'" + MgrName[0] + "',GETDATE(),1) ;";
                    }

                    
                }
                readerUsr.Close();
                //return queryAcc;
                if (queryUsr != "")
                {
                    SqlCommand commandUsrs = new SqlCommand(queryUsr, oConn);
                    commandUsrs.ExecuteNonQuery();
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
            SqlCommand commandstr = new SqlCommand("DELETE FROM " + desTable, oConn);
            commandstr.ExecuteNonQuery();

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
