using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScheduler
{
    class Program
    {
        static void Main(string[] args){
            ServerManager serverManager = new ServerManager();
            Configuration config = serverManager.GetApplicationHostConfiguration();
            ConfigurationSection ipSecuritySection = config.GetSection("system.webServer/security/ipSecurity", "Rela");
            ConfigurationElementCollection ipSecurityCollection = ipSecuritySection.GetCollection();
            List<string> blockedIpByService = new List<string>();

            List<ConfigurationElement> listOfElements = new List<ConfigurationElement>();
            if (ipSecurityCollection.Count > 0)
            {
                foreach (var element in ipSecurityCollection)
                {
                    blockedIpByService.Add(element.Attributes[0].Value.ToString());
                    listOfElements.Add(element);
                }

                foreach (var element in listOfElements)
                    ipSecurityCollection.Remove(element);

                serverManager.CommitChanges();
            }
        }
    }
}
