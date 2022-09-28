using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseDetailsUpdater.Resources
{
    public class CrmQuery
    {
        internal string GetQueryForApplicableInternationalCities()
        {
            string strXML = string.Empty;

            strXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='incident'>
    <attribute name='createdon' />
    <attribute name='incidentid' />
    <attribute name='customerid' />
    <attribute name='ownerid' />
    <attribute name='statuscode' />
    <attribute name='statecode' />
    <attribute name='tc_subcategory' />
    <attribute name='tc_category' />
    <attribute name='tc_casetype' />
    <attribute name='firstresponseslastatus' />
    <attribute name='tc_caserefno' />
    <order attribute='createdon' descending='true' />
    <filter type='and'>
      <condition attribute='tc_city' operator='in'>
        <value uiname='All Cities' uitype='cc_city'>{6586E025-3DD3-EB11-B808-0050560B5C4A}</value>
        <value uiname='International departure' uitype='cc_city'>{4EC788CE-D78C-EC11-B807-0050560B5C4B}</value>
      </condition>
      <condition attribute='tc_subcategory' operator='in'>
        <value uiname='Issue with VisitSaudi.com/App' uitype='tc_casesubcategory'>{14816C84-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='International Provider - Fake offer' uitype='tc_casesubcategory'>{2F77B080-7EDD-EB11-B808-0050560B5C4A}</value>
        <value uiname='International Provider - Contract terms between the visitor and the provider' uitype='tc_casesubcategory'>{E63FD6D1-82DD-EB11-B808-0050560B5C4A}</value>
        <value uiname='International Provider - Refund Issue' uitype='tc_casesubcategory'>{E86CC5F1-82DD-EB11-B808-0050560B5C4A}</value>
        <value uiname='International Provider -  Cancellation of reservation without request' uitype='tc_casesubcategory'>{3DA2DC06-83DD-EB11-B808-0050560B5C4A}</value>
        <value uiname='MT website' uitype='tc_casesubcategory'>{6F62958A-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='MT platform' uitype='tc_casesubcategory'>{7162958A-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='Booking an experience via VisitSaudi.com' uitype='tc_casesubcategory'>{9D87169D-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='E-Visa: Technical Issue with Health Insurance' uitype='tc_casesubcategory'>{367C69A9-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='E-Visa: Technical Issue with Application' uitype='tc_casesubcategory'>{387C69A9-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='E-Visa: Payment Issue' uitype='tc_casesubcategory'>{3A7C69A9-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='Visa Cancellation Issue' uitype='tc_casesubcategory'>{406DA71C-0A93-EC11-B807-0050560B5C4B}</value>
        <value uiname='License application' uitype='tc_casesubcategory'>{8587169D-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='Renewal of license' uitype='tc_casesubcategory'>{9187169D-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='Incorrect payment processed by MT' uitype='tc_casesubcategory'>{C5CB52A3-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='Issue with MT employee' uitype='tc_casesubcategory'>{E0928D92-3ABA-EC11-B80A-0050560B5C4A}</value>
        <value uiname='No response to raised complaint' uitype='tc_casesubcategory'>{318098AD-3ABA-EC11-B80A-0050560B5C4A}</value>
        <value uiname='Issue with fine from MT' uitype='tc_casesubcategory'>{9787169D-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='Visa on Arrival Issue' uitype='tc_casesubcategory'>{18816C84-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='Issue with boarding the plane to KSA' uitype='tc_casesubcategory'>{9F87169D-AFD1-EB11-B808-0050560B5C4A}</value>
        <value uiname='Entry Point Issue' uitype='tc_casesubcategory'>{3C7C69A9-AFD1-EB11-B808-0050560B5C4A}</value>
      </condition>
    </filter>
    <link-entity name='contact' from='contactid' to='customerid' visible='false' link-type='outer' alias='a_f5be541c782ceb11a813000d3a6540e8'>
      <attribute name='tc_preferredcommunicationlanguage' />
    </link-entity>
  </entity>
</fetch>";

            return strXML;
        }

        internal string GetQueryForNotApplicableInternationalCities()
        {
            string strXML = string.Empty;
            //            strXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
            //  <entity name='incident'>
            //    <attribute name='createdon' />
            //    <attribute name='incidentid' />
            //    <attribute name='customerid' />
            //    <attribute name='ownerid' />
            //    <attribute name='statuscode' />
            //    <attribute name='statecode' />
            //    <attribute name='tc_subcategory' />
            //    <attribute name='tc_category' />
            //    <attribute name='tc_casetype' />
            //    <attribute name='firstresponseslastatus' />
            //    <attribute name='tc_caserefno' />
            //    <attribute name='tc_city' />
            //    <order attribute='createdon' descending='true' />
            //    <filter type='and'>
            //      <condition attribute='tc_city' operator='eq' uiname='Riyadh' uitype='cc_city' value='{6A4EDFC3-8343-EB11-BB23-000D3A22C1F6}' />
            //      <condition attribute='tc_subcategory' operator='in'>
            //        <value uiname='VisitSaudi Kiosk/Dispenser' uitype='tc_casesubcategory'>{8B87169D-AFD1-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='Domestic Provider - Poor service' uitype='tc_casesubcategory'>{1E799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='Domestic Provider - Fake offer' uitype='tc_casesubcategory'>{20799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='Domestic Provider - Contract terms between the visitor and the provider' uitype='tc_casesubcategory'>{22799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='Domestic Provider - Cancellation of reservation without request' uitype='tc_casesubcategory'>{24799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='Domestic Provider - Refund Issue' uitype='tc_casesubcategory'>{26799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='Domestic Provider - Non-adherence to COVID Guidelines' uitype='tc_casesubcategory'>{28799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='Domestic Provider -  Mistreatment / refusal of entry' uitype='tc_casesubcategory'>{1DE1F7B5-AFD1-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='Domestic Provider - No booking upon arrival at accommodation' uitype='tc_casesubcategory'>{31C27D10-7EDD-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='International Provider - Poor service' uitype='tc_casesubcategory'>{33CD8240-7EDD-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='International Provider - Non-adherence to COVID Guidelines' uitype='tc_casesubcategory'>{4FFB2E1C-83DD-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='International Provider - Mistreatment / refusal of entry' uitype='tc_casesubcategory'>{7818274D-83DD-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='International Provider - No booking upon arrival at accommodation' uitype='tc_casesubcategory'>{0F975063-83DD-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='Poor facilities or health &amp; Safety concerns' uitype='tc_casesubcategory'>{6C06CA84-F9DE-EB11-B808-0050560B5C4A}</value>
            //        <value uiname='Changing Tawakalna Status' uitype='tc_casesubcategory'>{58648A09-A381-EC11-B80A-0050560B5C4A}</value>
            //      </condition>
            //      <condition attribute='modifiedby' operator='eq-userid' />
            //      <condition attribute='modifiedon' operator='today' />
            //      <condition attribute='statecode' operator='eq' value='0' />
            //    </filter>
            //    <link-entity name='contact' from='contactid' to='customerid' visible='false' link-type='outer' alias='a_f5be541c782ceb11a813000d3a6540e8'>
            //      <attribute name='tc_preferredcommunicationlanguage' />
            //    </link-entity>
            //  </entity>
            //</fetch>";

            strXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='incident'>
    <attribute name='createdon' />
    <attribute name='incidentid' />
    <attribute name='customerid' />
    <attribute name='ownerid' />
    <attribute name='statuscode' />
    <attribute name='statecode' />
    <attribute name='tc_subcategory' />
    <attribute name='tc_category' />
    <attribute name='tc_casetype' />
    <attribute name='firstresponseslastatus' />
    <attribute name='tc_caserefno' />
    <attribute name='tc_city' />
    <order attribute='createdon' descending='true' />
    <filter type='and'>
      <condition attribute='tc_city' operator='in'>
        <value uiname='All Cities' uitype='cc_city'>{6586E025-3DD3-EB11-B808-0050560B5C4A}</value>
        <value uiname='International departure' uitype='cc_city'>{4EC788CE-D78C-EC11-B807-0050560B5C4B}</value>
      </condition>

                  <condition attribute='tc_subcategory' operator='in'>
                    <value uiname='VisitSaudi Kiosk/Dispenser' uitype='tc_casesubcategory'>{8B87169D-AFD1-EB11-B808-0050560B5C4A}</value>
                    <value uiname='Domestic Provider - Poor service' uitype='tc_casesubcategory'>{1E799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
                    <value uiname='Domestic Provider - Fake offer' uitype='tc_casesubcategory'>{20799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
                    <value uiname='Domestic Provider - Contract terms between the visitor and the provider' uitype='tc_casesubcategory'>{22799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
                    <value uiname='Domestic Provider - Cancellation of reservation without request' uitype='tc_casesubcategory'>{24799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
                    <value uiname='Domestic Provider - Refund Issue' uitype='tc_casesubcategory'>{26799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
                    <value uiname='Domestic Provider - Non-adherence to COVID Guidelines' uitype='tc_casesubcategory'>{28799BAF-AFD1-EB11-B808-0050560B5C4A}</value>
                    <value uiname='Domestic Provider -  Mistreatment / refusal of entry' uitype='tc_casesubcategory'>{1DE1F7B5-AFD1-EB11-B808-0050560B5C4A}</value>
                    <value uiname='Domestic Provider - No booking upon arrival at accommodation' uitype='tc_casesubcategory'>{31C27D10-7EDD-EB11-B808-0050560B5C4A}</value>
                    <value uiname='International Provider - Poor service' uitype='tc_casesubcategory'>{33CD8240-7EDD-EB11-B808-0050560B5C4A}</value>
                    <value uiname='International Provider - Non-adherence to COVID Guidelines' uitype='tc_casesubcategory'>{4FFB2E1C-83DD-EB11-B808-0050560B5C4A}</value>
                    <value uiname='International Provider - Mistreatment / refusal of entry' uitype='tc_casesubcategory'>{7818274D-83DD-EB11-B808-0050560B5C4A}</value>
                    <value uiname='International Provider - No booking upon arrival at accommodation' uitype='tc_casesubcategory'>{0F975063-83DD-EB11-B808-0050560B5C4A}</value>
                    <value uiname='Poor facilities or health &amp; Safety concerns' uitype='tc_casesubcategory'>{6C06CA84-F9DE-EB11-B808-0050560B5C4A}</value>
                    <value uiname='Changing Tawakalna Status' uitype='tc_casesubcategory'>{58648A09-A381-EC11-B80A-0050560B5C4A}</value>
                  </condition>
                </filter>
                <link-entity name='contact' from='contactid' to='customerid' visible='false' link-type='outer' alias='a_f5be541c782ceb11a813000d3a6540e8'>
                  <attribute name='tc_preferredcommunicationlanguage' />
                </link-entity>
              </entity>
            </fetch>";

            return strXML;
        }


      internal string GetDevResolvedCases()
        {
            string strXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' top='30'>
  <entity name='incident'>
    <attribute name='createdon' />
    <attribute name='customerid' />
    <attribute name='ownerid' />
    <attribute name='statuscode' />
    <attribute name='statecode' />
    <attribute name='tc_subcategory' />
    <attribute name='tc_category' />
    <attribute name='tc_casetype' />
    <attribute name='incidentid' />
    <attribute name='firstresponseslastatus' />
    <attribute name='tc_caserefno' />
    <attribute name='tc_city' />
    <order attribute='createdon' descending='true' />
    
    <link-entity name='contact' from='contactid' to='customerid' visible='false' link-type='outer' alias='a_f5be541c782ceb11a813000d3a6540e8'>
      <attribute name='tc_preferredcommunicationlanguage' />
    </link-entity>
  </entity>
</fetch>";
            return strXml;
        }
    }
}
