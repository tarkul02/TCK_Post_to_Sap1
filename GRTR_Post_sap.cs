using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

using System.IO;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Xml.Linq;

namespace PostSap_GR_TR
{
    public partial class GRTR_Post_sap : Form
    {
        public GRTR_Post_sap()
        {
            InitializeComponent();
        }
        public async void GRTRPost_sap(object sender, EventArgs e)
        {
            Class.Blukcppy Blukcppy = new Class.Blukcppy();
            Blukcppy.AssignOwnerCash(43);
            
        }
       
        string start_Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
        int checkruntime = 1;
        int checkcatchend = 0;
        string DBconfig = ConfigurationManager.AppSettings["Databaseconfig"];
        string checkError;

        
        private async Task GetErrorAndNotify()
        {
            try
            {
               
                Console.WriteLine("Process Notify");
                Console.WriteLine("#################################################### \n");
                _ = new DataTable();
                string checkTime = DateTime.Now.ToString("HH:mm");
                Class.Condb Condb = new Class.Condb();
                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
                if (setting != null)
                {
                    _ = setting.ConnectionString;
                }
                string sqlemailGR = "select  RefDocNo as DocNo , EMessage from "+ DBconfig +".[v_get_dataNotify_gr] where 1 = 1";
                string sqlemailTR = "select  RefDocNo as DocNo , EMessage from "+ DBconfig +".[v_get_dataNotify_tr] where 1 = 1";
                string sqlemailGI = "select  RefDocNo as DocNo , EMessage from "+ DBconfig +".[v_get_dataNotify_gi] where 1 = 1";

                string sqllineGR = "select count(*) totalSum from "+ DBconfig +".[v_get_dataNotify_gr] where Action = 1";
                string sqllineTR = "select count(*) totalSum From (select count(*) TR_Re_NO, SLIPNO, Action from "+ DBconfig +".[v_get_dataNotify_tr] where Action = 1 GROUP BY SLIPNO, Action)D1 ";
                string sqllineGI = "select count(*) totalSum From (select count(*) TR_Re_NO, ORDERNO, Action from "+ DBconfig +".[v_get_dataNotify_gi] where Action = 1 GROUP BY ORDERNO, Action)D1 ";

                DataTable GetDataErrorGR = Condb.GetQuery(sqlemailGR);
                DataTable GetDataErrorTR = Condb.GetQuery(sqlemailTR);
                DataTable GetDataErrorGI = Condb.GetQuery(sqlemailGI);
                DataTable GetDataErrorGRrow = Condb.GetQuery(sqllineGR);
                DataTable GetDataErrorTRrow = Condb.GetQuery(sqllineTR);
                DataTable GetDataErrorGIrow = Condb.GetQuery(sqllineGI);
                string checkdata1 = GetDataErrorGRrow.Rows.Count > 0 ? GetDataErrorGRrow.Rows[0]["totalSum"].ToString() : "";
                string checkdata2 = GetDataErrorTRrow.Rows.Count > 0 ? GetDataErrorTRrow.Rows[0]["totalSum"].ToString() : "";
                string checkdata3 = GetDataErrorGIrow.Rows.Count > 0 ? GetDataErrorGIrow.Rows[0]["totalSum"].ToString() : "";
                string MessagelistGR = int.Parse(checkdata1) > 0 ? "GR Error : " + checkdata1 + " Item" : "";
                string MessagelistTR = int.Parse(checkdata2) > 0 ? "TR Error : " + checkdata2 + " Item" : "";
                string MessagelistGI = int.Parse(checkdata3) > 0 ? "GI Error : " + checkdata3 + " Item" : "";
                Console.WriteLine("Start sent LineNotify ");
                string ValidateMessage = "Error  \nrun time =  " + checkTime + "\n" + MessagelistGR + "\n" + MessagelistTR + "\n" + MessagelistGI;

                Console.WriteLine("Start sent LineNotify ");
                // start line notify 
                
                if (int.Parse(checkdata1) > 0 || int.Parse(checkdata2) > 0 || int.Parse(checkdata3) > 0)
                {
                    Class.LineNotify lineNotify = new Class.LineNotify();
                    lineNotify.FNLineNotify(ValidateMessage);
                }
                await Task.Delay(3000);
                // end line notify
                string checkruntime =  getTimeNotify();
                if (checkruntime == "Y")
                {
                    
                    // start cerate file and send mail
                    if (GetDataErrorGR.Rows.Count > 0 || GetDataErrorTR.Rows.Count > 0 || GetDataErrorGI.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        DateTime now = DateTime.Now;
                        string filename = now.ToString("yyyy-MM-dd HH:mm:ss:fff");

                        string[] words = filename.Split(' ');
                        string[] text1 = words[0].Split('-');
                        string[] text2 = words[1].Split(':');
                        string lastfilename = text1[0] + "_" + text1[1] + "_" + text1[2] + "_" + text2[0];
                        string Fordername = text1[0] + "_" + text1[1] + "_" + text1[2] + "_" + text2[0];
                        string folderPath = @"C:\testTKC\temp\" + Fordername;
                        using (var package = new ExcelPackage())
                        {

                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Data");

                            // Write column headers
                            if (GetDataErrorGR.Rows.Count > 0)
                            {
                                for (int i = 0; i < GetDataErrorGR.Columns.Count; i++)
                                {
                                    worksheet.Cells[1, i + 1].Value = GetDataErrorGR.Columns[i].ColumnName;
                                }

                                // Write data to Excel file
                                for (int row = 0; row < GetDataErrorGR.Rows.Count; row++)
                                {
                                    for (int column = 0; column < GetDataErrorGR.Columns.Count; column++)
                                    {
                                        worksheet.Cells[row + 2, column + 1].Value = GetDataErrorGR.Rows[row][column];
                                    }
                                }

                                //string folderPath = @"C:\TKC\TCK_Post_to_Sap\temp\" + Fordername;
                                Directory.CreateDirectory(folderPath);
                                FileInfo excelFileGR = new FileInfo(folderPath + "\\GR" + lastfilename + ".xlsx");
                                package.SaveAs(excelFileGR);
                            }

                            if (GetDataErrorTR.Rows.Count > 0)
                            {
                                for (int i = 0; i < GetDataErrorTR.Columns.Count; i++)
                                {
                                    worksheet.Cells[1, i + 1].Value = GetDataErrorTR.Columns[i].ColumnName;
                                }

                                // Write data to Excel file
                                for (int row = 0; row < GetDataErrorTR.Rows.Count; row++)
                                {
                                    for (int column = 0; column < GetDataErrorTR.Columns.Count; column++)
                                    {
                                        worksheet.Cells[row + 2, column + 1].Value = GetDataErrorTR.Rows[row][column];
                                    }
                                }


                                //string folderPath = @"C:\TKC\TCK_Post_to_Sap\temp\" + Fordername;
                                Directory.CreateDirectory(folderPath);
                                FileInfo excelFileGR = new FileInfo(folderPath + "\\TR" + lastfilename + ".xlsx");
                                package.SaveAs(excelFileGR);
                            }

                            if (GetDataErrorGI.Rows.Count > 0)
                            {
                                for (int i = 0; i < GetDataErrorGI.Columns.Count; i++)
                                {
                                    worksheet.Cells[1, i + 1].Value = GetDataErrorGI.Columns[i].ColumnName;
                                }

                                // Write data to Excel file
                                for (int row = 0; row < GetDataErrorGI.Rows.Count; row++)
                                {
                                    for (int column = 0; column < GetDataErrorGI.Columns.Count; column++)
                                    {
                                        worksheet.Cells[row + 2, column + 1].Value = GetDataErrorGI.Rows[row][column];
                                    }
                                }


                                //string folderPath = @"C:\TKC\TCK_Post_to_Sap\temp\" + Fordername;
                                Directory.CreateDirectory(folderPath);
                                FileInfo excelFileGR = new FileInfo(folderPath + "\\GI" + lastfilename + ".xlsx");
                                package.SaveAs(excelFileGR);
                            }
                        }
                        Console.WriteLine("Start sent Email \n");
                        ////// Email settings
                        ///
                        //string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
                        //string receiverEmail = ConfigurationManager.AppSettings["mailTO"];

                        //string subject = "Excel File Attachment Error";
                        //string body = "Please Check your data in Excel file attached. \n" + ValidateMessage;

                        // Email configuration

                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(ConfigurationManager.AppSettings["SenderEmail"]);
                        mail.To.Add(ConfigurationManager.AppSettings["mailTO"]);
                        mail.Subject = "Excel File Attachment Error";
                        mail.Body = "Please Check your data in Excel file attached. \n" + ValidateMessage;

                        SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["SmtpClient"]);
                        client.Port = 25; // Set the port according to your email provider
                        client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["CredentialsUser"], ConfigurationManager.AppSettings["CredentialsPass"]);
                        client.EnableSsl = false;


                        // Enable SSL
                        //Attach the Excel file
                        // Attachment attachment1 = new Attachment(@"C:\TKC\TCK_Post_to_Sap\temp\GR" + lastfilename + ".xlsx");
                        // Attachment attachment2 = new Attachment(@"C:\TKC\TCK_Post_to_Sap\temp\TR" + lastfilename + ".xlsx");
                        if (GetDataErrorGR.Rows.Count > 0)
                        {
                            Attachment attachment1 = new Attachment(folderPath + "\\GR" + lastfilename + ".xlsx");
                            mail.Attachments.Add(attachment1);
                        }
                        if (GetDataErrorTR.Rows.Count > 0)
                        {
                            Attachment attachment2 = new Attachment(folderPath + "\\TR" + lastfilename + ".xlsx");
                            mail.Attachments.Add(attachment2);
                        }
                        if (GetDataErrorGI.Rows.Count > 0)
                        {
                            Attachment attachment3 = new Attachment(folderPath + "\\GI" + lastfilename + ".xlsx");
                            mail.Attachments.Add(attachment3);
                        }
                        // Send the email
                        try
                        {
                            client.Send(mail);
                            Console.WriteLine("Email sent successfully!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                    }
                    // end cerate file and send mail
                }
            }
            catch (Exception ex)
            {
                string Message = "Unexpected error GetErrorAndNotify : " + ex.Message; ;
            }

        }
        public string getTimeNotify()
        {
            string checkTime = DateTime.Now.ToString("HH:mm");
            string[] timenow = checkTime.Split(':');
            string hour = timenow[0];
            int minute = Convert.ToInt32(timenow[1]);
            string flag = "Y";
            if ((hour == "08"  && minute < 30) || (hour == "13" && minute < 30)) {
                flag = "Y";
            }
            else {
                flag = "N";
            }
            return flag;

        }

    }
}
