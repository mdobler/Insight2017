using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionAPIHelper
{
    public enum VisionInfoCenters
    {
        Projects,
        Clients,
        Contacts,
        Employees,
        EmployeesMC,
        Opportunities,
        Leads,
        MktCampaigns,
        Vendors,
        TextLibraries,
        Activities
    }

    public static class VisionInfoCentersExtensions
    {
        public static string GetValueName(this VisionInfoCenters infoCenter)
        {
            string _retval = Enum.GetName(typeof(VisionInfoCenters), infoCenter);
            return _retval;
        }

    }
}
