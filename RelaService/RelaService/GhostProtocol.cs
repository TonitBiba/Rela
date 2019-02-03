using Microsoft.Web.Administration;
using RelaService.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RelaService
{
    internal class GhostProtocol
    {
        private const string linuxIp = "35.237.167.116";

        private static string sqlServerInstance { get; set; }
        private static string iisWebSite { get; set; }
        private static int countSqlFails { get; set; }

        private EmailService emailService;
        private ServerManager serverManager = new ServerManager();

        public GhostProtocol(string sqlServerInstance1, string iisWebSite1)
        {
            sqlServerInstance = sqlServerInstance1;
            iisWebSite = iisWebSite1;
        }

        public void tryToRepairSQL()
        {
            try
            {
                ServiceController serviceController = new ServiceController("MSSQLFDLauncher$SQLEXPRESS");
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                    serviceController.Start();

                serviceController.ServiceName = "MSSQL$SQLEXPRESS";
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                    serviceController.Start();

                serviceController.ServiceName = "SQLBrowser";
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                    serviceController.Start();

                serviceController.ServiceName = "SQLTELEMETRY$SQLEXPRESS";
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                    serviceController.Start();

                serviceController.ServiceName = "MSSQLLaunchpad$SQLEXPRESS";
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                    serviceController.Start();

                serviceController.ServiceName = "SQLWriter";
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                    serviceController.Start();
                countSqlFails = 0;

            }
            catch (Exception ex)
            {
                emailService = new EmailService("Error in Rela Project while connecting to SQL server. Error: " + ex, "User.biba@hotmail.com", "Sql server error!");
                countSqlFails++;
                emailService.sendMail();
                //Ne rast se deshton startimi i sherbimeve te SQL serverit pas 3 tentimeve, transferohet lidhja e databazen ne severin e Linux-it(Centoes).
                if (countSqlFails > 3)
                {
                    try
                    {
                        var xmlSerializer = new XmlSerializer(typeof(configuration));
                        var streamReader = new StreamReader(@"C:\Users\User_test_mail\Desktop\Host Rela\Web.config");
                        var result = (configuration)xmlSerializer.Deserialize(streamReader);
                        result.connectionStrings[0].connectionString = "Data Source=" + linuxIp + "\\SQLEXPRESS;Initial Catalog=Rela;User ID=SA;Password=Password";
                        result.connectionStrings[1].connectionString = "metadata=res://*/Models.RelaDb.csdl|res://*/Models.RelaDb.ssdl|res://*/Models.RelaDb.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source=" + linuxIp + "\\SQLEXPRESS;Initial Catalog=Rela;User ID=User;Password=Password&quot;";
                        streamReader.Close();

                        var streamWriter = new StreamWriter(@"C:\Users\User_test_mail\Desktop\Host Rela\Web.config");
                        xmlSerializer.Serialize(streamWriter, result);
                        streamWriter.Close();

                        //Nese qdo gje eshte ne rregull ristartohet edhe web serveri(IIS 10)
                        Process iisProcess = new Process();
                        iisProcess.StartInfo.FileName = "iisreset.exe";
                        iisProcess.StartInfo.RedirectStandardOutput = true;
                        iisProcess.StartInfo.UseShellExecute = true;
                        iisProcess.Start();

                        //Dhe krejt ne fund Ghost protokolli e shkatrron vetveten. Me kete rast eshte permbushur misioni i ketij sherbimi.
                        ServiceController serviceController = new ServiceController("Service1");
                        serviceController.Stop();
                    }
                    catch (Exception iisExeptions)
                    {
                        emailService = new EmailService("Rela server has restarted because of to many failed times to start SQL server. " + iisExeptions, "User.biba@hotmail.com", "Rela server has restarted");

                        //Ne rast se deshton ristartimi i web serverit apo ndryshimi i fajllit konfigurues të web aplikacionit ristartohet komplet Windows Serveri.
                        Process.Start("shutdown", "/r /t 0");
                        var processStartInfo = new ProcessStartInfo("shutdown", "/r /t 0");
                        processStartInfo.CreateNoWindow = true;
                        processStartInfo.UseShellExecute = false;
                        Process.Start(processStartInfo);
                    }
                }
            }
        }
    }
}
