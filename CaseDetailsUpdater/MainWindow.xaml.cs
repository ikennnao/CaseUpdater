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
using System.Threading;
using System.Net.Http;
using System.Security.Cryptography;
using System.IO;

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

        private bool connectToCRM()
        {
            try
            {
                
                
                string encryptedconnectionString = ConfigurationManager.ConnectionStrings["DevCrm"].ConnectionString;
                string connectionString = TripleDESDecrypt(encryptedconnectionString);

                if (!string.IsNullOrEmpty(connectionString))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    CrmServiceClient = new CrmServiceClient(connectionString);
                    if (!CrmServiceClient.IsReady)
                    {
                        
                        connectionState = false;
                    }
                    else
                    {
                        connectionState = true;
                       
                    }
                }
            }
            catch(Exception ex)
            {
                connectstatuslbl.Content = ex.Message;
            }
            return connectionState;
        }

        private async void crmConnectbtn_Click(object sender, RoutedEventArgs e)
        {
            crmConnectbtn.IsEnabled = false;
            if (!connectionState)
            {
                Task<bool> t = new Task<bool>(connectToCRM);
                t.Start();

                statuslbl.Content = "Connecting to CRM..";
               bool connState =  await t;
                if (!connState)
                {
                    connectstatuslbl.Content = "Unable to connect to CRM";
                    connectstatuslbl.Background = Brushes.Red;
                    connectstatuslbl.Foreground = Brushes.White;
                    statuslbl.Content = "Connection Failed. Check CRM Connection string";

                }
                else
                {
                    statuslbl.Content = "Connection successful";
                    connectstatuslbl.Content = $"Connected to {CrmServiceClient.ConnectedOrgFriendlyName}";
                    connectstatuslbl.Background = Brushes.Green;
                    connectstatuslbl.Foreground = Brushes.White;
                }
                
                //connectToCRM();
                
            }
            else
            {
                crmConnectbtn.IsEnabled = true;
            }
        }

        private async void executeqrybtn_Click(object sender, RoutedEventArgs e)
        {
            string query = fetchqry.Text; int recordBeignProcessed = 0; List<Entity> records = new List<Entity>();
            string cityId = cityGuid.Text; 
            Task<List<Entity>> GetRecordsTask = new Task<List<Entity>>(() =>
            {
                
                return GetRecords(query);
                
            });
            GetRecordsTask.Start();

            statuslbl.Content = "Retreiving records....";
             records =  await GetRecordsTask;
            int recordCount = records.Count();
            
            statuslbl.Content = $"Total number of records is {recordCount}";
           // Thread.Sleep(5000);
            statuslbl.Content = $"Processing of Case Records...";
            //Thread.Sleep(3000);
            //start new Task
            foreach (Entity record in records)
            {
                Task updatecaseTask = new Task(() =>
                {
                recordBeignProcessed++;
                    //Thread.Sleep(1000);
                    ExecuteWorkflowRequest executeWorkflowRequest = new ExecuteWorkflowRequest()
                    {
                        WorkflowId = new Guid("a452ed63-add0-4e07-bb52-7444ed568a5d"),
                        EntityId = record.Id
                    };
                    ExecuteWorkflowResponse res = (ExecuteWorkflowResponse) CrmServiceClient.Execute(executeWorkflowRequest);
                    
                   // ExecuteCaseCityUpdate(record,Guid.Parse(cityId));
                });
                updatecaseTask.Start();
                await updatecaseTask;
                statuslbl.Content = $"Processing {recordBeignProcessed} of {recordCount}";
            }

            statuslbl.Content = "Case Update Completed...";
        }

        private List<Entity> GetRecords(string fetchqry)
        {
            //executeqrybtn.IsEnabled = false;
            var pageNumber = 1;
            var pagingCookie = string.Empty;
            var result = new List<Entity>();
            EntityCollection entCasesToUpdate;
            //read the fetch xml
            fetchXML = fetchqry;
            FetchXmlToQueryExpressionRequest queryExpressionRequest = new FetchXmlToQueryExpressionRequest()
            {
                FetchXml = fetchXML
            };

            FetchXmlToQueryExpressionResponse resQuery = (FetchXmlToQueryExpressionResponse)CrmServiceClient.Execute(queryExpressionRequest);
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

               
                //executeqrybtn.IsEnabled = true;
                
            }
            return result;
        }


        private void ExecuteCaseCityUpdate(Entity updatecase, Guid cityId)
        {

            //get the case record and check if the case status is resolved
           
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
                    caseToUpdate.Attributes["tc_city"] = new EntityReference("cc_city", cityId);
                    CrmServiceClient.Update(caseToUpdate);

                    // Close case back to original state

                    //Close the case
                    var resolution = new Entity("incidentresolution");
                    resolution["subject"] = "Close Case";
                    resolution["incidentid"] = new EntityReference { Id = caseToUpdate.Id, LogicalName = "incident" };
                    var closeIncidentRequest = new CloseIncidentRequest
                    {
                        IncidentResolution = resolution,
                        Status = new OptionSetValue(1000),
                        
                    };
                    CrmServiceClient.Execute(closeIncidentRequest);
                }

                else
                {
                    //Update the case details to International City
                    updatecase.Attributes["tc_city"] = new EntityReference("cc_city", cityId);
                    CrmServiceClient.Update(updatecase);
                }

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

        private string TripleDESDecrypt(string strDecryptText)
        {
            string strDecryptedTxt = string.Empty;
            string strTripleDESSaltKey = "sta!2021@vc#crm$v9.0%sau";

            if (!string.IsNullOrWhiteSpace(strDecryptText) && !string.IsNullOrWhiteSpace(strTripleDESSaltKey))
            {
                byte[] rgbIv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                var rgbKey = Encoding.UTF8.GetBytes(strTripleDESSaltKey);
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

                byte[] inputByteArray = Convert.FromBase64String(strDecryptText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                strDecryptedTxt = encoding.GetString(ms.ToArray());
            }

            return strDecryptedTxt;
        }


    }
}
