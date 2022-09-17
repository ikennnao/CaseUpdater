using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using System.Net;

namespace CaseDetailsUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CrmServiceClient CrmServiceClient = null;
        bool connectionState = false;
        Resources.CrmQuery crmQuery = new Resources.CrmQuery();
        string fetchXML;
        public MainWindow()
        {
            
            InitializeComponent();
         
        }

        private void connectToCRM()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Xrm"].ConnectionString;
                if (!string.IsNullOrEmpty(connectionString))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    CrmServiceClient = new CrmServiceClient(connectionString);
                    if (!CrmServiceClient.IsReady)
                    {
                        connectstatuslbl.Content = "Unable to connect to CRM";
                        connectstatuslbl.Background = Brushes.Red;
                        connectstatuslbl.Foreground = Brushes.White;
                    }
                    else
                    {
                        connectionState = true;
                        connectstatuslbl.Content = $"Connected to {CrmServiceClient.ConnectedOrgFriendlyName}";
                        connectstatuslbl.Background = Brushes.Green;
                        connectstatuslbl.Foreground = Brushes.White;
                    }
                }
            }
            catch(Exception ex)
            {
                connectstatuslbl.Content = ex.Message;
            }
        }

        private void crmConnectbtn_Click(object sender, RoutedEventArgs e)
        {
            crmConnectbtn.IsEnabled = false;
            if (!connectionState)
            {
                connectToCRM();
                
            }
            else
            {
                crmConnectbtn.IsEnabled = true;
            }
        }

        private void executeqrybtn_Click(object sender, RoutedEventArgs e)
        {
            statuslbl.Content = "Retrieving record count.....";
            executeqrybtn.IsEnabled = false;
             var pageNumber = 1;
            var pagingCookie = string.Empty;
            var result = new List<Entity>();
            EntityCollection entCasesToUpdate;
            //read the fetch xml
            fetchXML = fetchqry.Text;
            FetchXmlToQueryExpressionRequest queryExpressionRequest = new FetchXmlToQueryExpressionRequest()
            {
                FetchXml = fetchXML
            };

           FetchXmlToQueryExpressionResponse resQuery =  (FetchXmlToQueryExpressionResponse)CrmServiceClient.Execute(queryExpressionRequest);
           QueryExpression queryExpression = resQuery.Query;
            if (!string.IsNullOrEmpty(fetchXML))
            {
                
                do
                {
                    if (pageNumber != 1)
                    {
                        queryExpression.PageInfo.PageNumber = pageNumber;
                        queryExpression.PageInfo.PagingCookie = pagingCookie;
                    }

                    entCasesToUpdate = CrmServiceClient.RetrieveMultiple(queryExpression);
                    //Exeute query and fetch the results

                    if (entCasesToUpdate != null && entCasesToUpdate.Entities.Count > 0)
                    {
                        if (entCasesToUpdate.MoreRecords)
                        {
                            pageNumber++;
                            pagingCookie = entCasesToUpdate.PagingCookie;
                        }
                        result.AddRange(entCasesToUpdate.Entities);
                        
                    }
                    
                }
                while (entCasesToUpdate.MoreRecords);
                statuslbl.Content = $"Total number of Records is {result.Count}";
                executeqrybtn.IsEnabled = true;
                //update case records 
               ExecuteUpdateCase(result);
            }
        }

        private void ExecuteUpdateCase(List<Entity> entities)
        {
          
            //get the case record and check if the case status is resolved
            statuslbl.Content = "Updating cases....";
                foreach (Entity updatecase in entities)
                {
                int intialStateCode = updatecase.GetAttributeValue<OptionSetValue>("statecode").Value;
                int intialStatusCode = updatecase.GetAttributeValue<OptionSetValue>("statuscode").Value;
                if (intialStateCode != 0) // case is resolved or cancelled
                {
                    //reactivate the case
                    SetStateRequest setStateRequest = new SetStateRequest()
                    {
                        // Set the Request Object's Properties
                        State = new OptionSetValue(0), // Active
                        Status = new OptionSetValue(1), // Inprogress


                        // Point the Request to the case whose state is being changed
                        EntityMoniker = updatecase.ToEntityReference()
                    };

                     CrmServiceClient.Execute(setStateRequest);

                    //Update the case details to International City
                    string[] cols = { "tc_city" };
                    Entity caseToUpdate = CrmServiceClient.Retrieve("incident", updatecase.Id, new ColumnSet(cols)) ;
                    caseToUpdate.Attributes["tc_city"] = new EntityReference("cc_city", Guid.Parse(cityGuid.Text));
                    CrmServiceClient.Update(caseToUpdate);

                    // Close case back to original state

                    //reactivate the case
                    var resolution = new Entity("incidentresolution");
                    resolution["subject"] = "Close Case";
                    resolution["incidentid"] = new EntityReference { Id = caseToUpdate.Id, LogicalName = "incident" };
                    var closeIncidentRequest = new CloseIncidentRequest
                    {
                        IncidentResolution = resolution,
                        Status = new OptionSetValue(intialStatusCode),
                        
                    };
                    CrmServiceClient.Execute(closeIncidentRequest);
                }

                else
                {
                    //Update the case details to International City
                    updatecase.Attributes["tc_city"] = new EntityReference("cc_city", Guid.Parse(cityGuid.Text));
                    CrmServiceClient.Update(updatecase);
                }
                }

            statuslbl.Content = "Case Update Completed...";





        }

        private void qryselectorbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            executeqrybtn.IsEnabled = false;
            cityGuid.Text = string.Empty;
            int selectedItem = qryselectorbox.SelectedIndex;
            if (selectedItem == 0) //applicable to international cities
            {
                fetchqry.Text = crmQuery.GetQueryForApplicableInternationalCities();
                cityGuid.Text = "4CB11407-FBDF-EB11-B808-0050560B5C4A"; //International
                
            }

            else if (selectedItem == 1) //not applicable to international cities
            {
                fetchqry.Text = crmQuery.GetQueryForNotApplicableInternationalCities();
                cityGuid.Text = "6A4EDFC3-8343-EB11-BB23-000D3A22C1F6";//riyard
            }

            else if (selectedItem == 2) //Test
            {
                fetchqry.Text = crmQuery.GetDevResolvedCases();
                cityGuid.Text = "2D986ACB-4270-EB11-B804-0050560B5C49";//riyard
            }

            if (!string.IsNullOrEmpty(fetchqry.Text))
            {
                executeqrybtn.IsEnabled = true;
            }
        }

        private void qryselectorbox_Initialized(object sender, EventArgs e)
        {
            int selectedItem = qryselectorbox.SelectedIndex;
            if (selectedItem == 0) //applicable to international cities
            {
                fetchqry.Text = crmQuery.GetQueryForApplicableInternationalCities();
            }

            else if (selectedItem == 1)
            {
                fetchqry.Text = crmQuery.GetQueryForNotApplicableInternationalCities();
            }
        }

        private void GetEmailTemplate()
        {
            Entity[] entAryFromPartyList = null, entAryToPartyList = null;
            EntityReference entrefRegardingObject = new EntityReference("incident", new Guid("7656623E-C9A8-EB11-B806-0050560B5C48"));
            InstantiateTemplateRequest rqtInstantiateTemplate = new InstantiateTemplateRequest
            {
                TemplateId = Guid.Parse("04E18245-5C33-ED11-B815-0050560B5C48"),//emailObjInputParams.entTemplate.Id,
                ObjectId = Guid.Parse("7656623E-C9A8-EB11-B806-0050560B5C48"),
                ObjectType = "incident"
             };

                InstantiateTemplateResponse rspInstantiateTemplate = (InstantiateTemplateResponse)CrmServiceClient.Execute(rqtInstantiateTemplate);

                if (rspInstantiateTemplate != null && rspInstantiateTemplate.EntityCollection != null && rspInstantiateTemplate.EntityCollection.Entities != null
                    && rspInstantiateTemplate.EntityCollection.Entities.Count > 0)
                {
                string newBody;
                Entity entEmailTemplate = rspInstantiateTemplate.EntityCollection.Entities.FirstOrDefault();
                   string strEmailSubject = entEmailTemplate.GetAttributeValue<string>("subject");
                   string strEmailBody = entEmailTemplate.GetAttributeValue<string>("description");

                newBody = strEmailBody.Contains("Case:Record GUID")
                    ? strEmailBody.Replace("Case:Record%20GUID", "7656623E-C9A8-EB11-B806-0050560B5C48")
                    : strEmailBody;

                Entity entFromAP = new Entity("activityparty");
                entFromAP.Attributes["partyid"] = new EntityReference("systemuser", Guid.Parse("3FB054CD-E66A-EB11-B804-0050560B5C48"));
                entAryFromPartyList = new Entity[] { entFromAP };

                Entity entToAP = new Entity("activityparty");
                entToAP.Attributes["partyid"] =new EntityReference("systemuser", Guid.Parse("3FB054CD-E66A-EB11-B804-0050560B5C48"));
                entAryToPartyList = new Entity[] { entToAP };


                Entity entCreateEmail = new Entity("email");

                if (entAryFromPartyList != null && entAryFromPartyList.Length > 0)
                {
                    entCreateEmail.Attributes["from"] = entAryFromPartyList;
                }
               
                if (entAryToPartyList != null && entAryToPartyList.Length > 0)
                {
                    entCreateEmail.Attributes["to"] = entAryToPartyList;
                }
                
                if (!string.IsNullOrWhiteSpace(strEmailSubject))
                {
                    entCreateEmail.Attributes["subject"] = strEmailSubject;
                }
                
                if (!string.IsNullOrWhiteSpace(newBody))
                {
                    entCreateEmail.Attributes["description"] = newBody;
                }
                
                if (entrefRegardingObject != null)
                {
                    entCreateEmail.Attributes["regardingobjectid"] = entrefRegardingObject;
                }

                entCreateEmail.Id = CrmServiceClient.Create(entCreateEmail);

                if (entCreateEmail.Id != Guid.Empty)
                {
                    SendEmailRequest rqtSendEmail = new SendEmailRequest
                    {
                        IssueSend = true,
                        EmailId = entCreateEmail.Id
                    };

                    SendEmailResponse rspSendEmail = (SendEmailResponse)CrmServiceClient.Execute(rqtSendEmail);
                }

            }
            
        }

        
    }
}
