using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionAPIHelper
{
    public enum PickList
    {
        CFGActivitySubject,
        CFGActivityType,
        CFGChargeType,
        CFGClientCurrentStatus,
        CFGClientRole,
        CFGClientStatus,
        CFGClientType,
        CFGContactRole,
        CFGContactTitle,
        CFGContactType,
        CFGCountry,
        CFGEmployeeRelationship,
        CFGEmployeeRole,
        CFGEmployeeStatus,
        CFGOpportunitySource,
        CFGOpportunityStage,
        CFGOpportunityStatus,
        CFGOpportunityType,
        CFGPhoneFormat,
        CFGPrimarySpecialty,
        CFGProbability,
        CFGProjectStatus,
        CFGStates,
        CFGVendorRole,
        CFGVendorStatus,
        CFGVendorType,
        ContactStatus,
        Organization,
        Payterms,
        SEUser,
        CFGOrgCodes,
    }

    public static class PickListExtensions
    {
        public static string GetValueName(this PickList pickList)
        {
            string _retval = Enum.GetName(typeof(PickList), pickList);
            return _retval;
        }

    }
}
