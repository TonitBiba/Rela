using Microsoft.Web.Administration;
using RelaService.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RelaService
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer = new Timer();
        private RelaEntities Rela = new RelaEntities();
        private GhostProtocol ghostProtocol = new GhostProtocol("SqlInstance","DbName");


        private const int MAXNUMBEROFREQUESTS = 200;
        private const int MAXCAPTCHAERROR = 40;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer.Enabled = true;
            timer.Interval = 5000;
            timer.Elapsed += new ElapsedEventHandler(OnTimerEvent);
        }

        protected override void OnStop()
        {
        }

        private void OnTimerEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection("data source=SqLInstance;initial catalog=DbNAme;user id=Username;password=Password");
                sqlConnection.Open();
                sqlConnection.Close();
                DateTime captchaRequests = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                DateTime fiveSecondBack = DateTime.Now.Subtract(new TimeSpan(0, 0, 5));
                var listOfIpAddresses = Rela.Logs.Where(log => log.time_accessed > fiveSecondBack).Select(log => log.ip).ToList();
                var listCaptchaErrors = Rela.Logs.Where(log => log.time_accessed > captchaRequests && log.success == false).Select(log => log.ip).ToList();
                var distinctIpAddresses = listOfIpAddresses.Distinct().ToList();
                if (distinctIpAddresses.Count > 0)
                {
                    foreach (var ipAddress in distinctIpAddresses)
                    {
                        //Caktohet numri i kerkesave qe ka bere perdoruesi gjate 5 sekondave te kaluar.
                        int numberOfRequests = listOfIpAddresses.Where(ip => ip == ipAddress).Count();

                        //Caktohet numri i kerkesave qe jane bere ne Captcha dhe kane rezultuar si jo te suksesshme gjate 60 minutave te kaluar.
                        int numberOfCaptchaErrors = listCaptchaErrors.Where(captcha => captcha == ipAddress).Count();

                        if (numberOfRequests > MAXNUMBEROFREQUESTS || numberOfCaptchaErrors >= MAXCAPTCHAERROR)
                        {
                            blockIp(ipAddress, false);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ghostProtocol.tryToRepairSQL();
            }
        }

        public void blockIp(string ipAddress, bool allowedOrDeny)
        {
            try
            {
                ServerManager serverManager = new ServerManager();
                Configuration config = serverManager.GetApplicationHostConfiguration();
                ConfigurationSection ipSecuritySection = config.GetSection("system.webServer/security/ipSecurity", "Rela");
                ConfigurationElementCollection ipSecurityCollection = ipSecuritySection.GetCollection();
                ConfigurationElement addElement = ipSecurityCollection.CreateElement("add");
                addElement["ipAddress"] = ipAddress;
                addElement["allowed"] = allowedOrDeny;
                ipSecurityCollection.Add(addElement);
                serverManager.CommitChanges();
            }
            catch (Exception ex)
            {}
        }
    }
}
