using Maxi.Services.SouthSide.SouthsideFile;
using Maxi.Services.SouthSide.SouthsideFile.Types;
using Maxi.Services.SouthSide.Utils;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;
// 1 com.maxillc.checks.Integration.Persistence
namespace Maxi.Services.SouthSide.Repositories
{
    public interface IChecksRepository
    {
        IDbConnection GetDbConnection(bool isLog = false);
        List<BundleWf> GetChecks(DateTime currentDate);
        bool UpdateStatus(List<int> checks, List<int> credits, int idStatus, string fileName);
        XElement ToChecksXML(List<int> list);
        XElement ToCreditsXML(List<int> list);
        bool UpdateCheckBundles(List<int> bundles, string fileName);
        XElement ToBundlesXML(List<int> list);
        IDbDataParameter GetParameter(IDbCommand command, string parameterName, object value, DbType type);
    }

    public class ChecksRepository : IChecksRepository
    {
        private string _serviceCode;
        private IConfiguration _configuration;
        private ILogger<ChecksRepository> _logger;
        private IServiceUtils _serviceUtils;

        public ChecksRepository(IConfiguration configuration, ILogger<ChecksRepository> logger, IServiceUtils serviceUtils)
        {
            _configuration = configuration;
            _serviceCode = _configuration.GetValue<string>("ServiceCode");
            _logger = logger;
            _serviceUtils = serviceUtils;   
        }

        public IDbConnection GetDbConnection(bool isLog = false)
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public List<BundleWf> GetChecks(DateTime currentDate)
        {
            try
            {
                List<BundleWf> listBundles = new List<BundleWf>();

                IDbConnection dbConnection = GetDbConnection();
                IDbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandTimeout = 0;
                dbCommand.CommandText = "st_GetChecksToProcessSouthSide";
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbConnection.Open();

                #region get bundles
                _logger.LogInformation($"Service {_serviceCode}: Begins: get bundles from store procedure");
                IDataReader dataReader = dbCommand.ExecuteReader();
                int count = 1;

                while (dataReader.Read())
                {
                    string abaRouting = dataReader["AbaRouting"].ToString();
                    dataReader["Account"].ToString();
                    BundleWf bundle = new BundleWf()
                    {
                        BundleHeader = new BundleWf.Header(_logger, _serviceUtils)
                        {
                            BundleBusinessDate = currentDate.ToWfDateString(),
                            BundleCreationDate = currentDate.ToWfDateString(),
                            BundleId = dataReader["IdCheckBundle"].ToString(),
                            BundleSequenceNumber = Converters.BundleSequenceNumber(Convert.ToInt32(dataReader["BundleSequence"])),
                            DestinationRoutingNumber = abaRouting,
                            ECEInstitutionRoutingNumber = abaRouting
                        },
                        BundleControl = new BundleWf.Control(_logger, _serviceUtils)
                        {
                            ItemsWithinBundleCount = Convert.ToInt32(dataReader["ItemsWithinBundleCount"]).ToString(),
                            BundleTotalAmount = Convert.ToDecimal(dataReader["Amount"]).ToWfDecimalString(),
                            MICRValidTotalAmount = Convert.ToDecimal(dataReader["Amount"]).ToWfDecimalString(),
                            ImagesWithinBundleCount = Convert.ToInt32(dataReader["ImagesWithinBundleCount"]).ToString(),
                            BundleSequenceNumber = Converters.BundleSequenceNumber(Convert.ToInt32(dataReader["BundleSequence"]))
                        }
                    };
                    listBundles.Add(bundle);
                }
                #endregion

                #region get credits
                _logger.LogInformation($"Service {_serviceCode}: Begins: get credits from store procedure");
                dataReader.NextResult();
                while (dataReader.Read())
                {
                    string idBundleCredit = Converters.BundleSequenceNumber(Convert.ToInt32(dataReader["BundleSequence"]));
                    BundleWf bundleWfCredit1 = listBundles.FirstOrDefault(b => b.BundleHeader.BundleSequenceNumber == idBundleCredit);
                    string abaRoutingCredit = dataReader["AbaRouting"].ToString();
                    string accountCredit = dataReader["Account"].ToString();

                    bundleWfCredit1.Credit = new CreditWf(_logger,_serviceUtils)
                    {
                        AuxiliaryOnUs = dataReader["SubAccount"].ToString(),
                        PayorBankRoutingNumber = abaRoutingCredit,
                        PayorBankRoutingNumberCheckDigit = abaRoutingCredit[abaRoutingCredit.Length - 1].ToString(),
                        OnUs = dataReader["MicrOnUs"].ToString(),
                        ItemAmount = Convert.ToDecimal(dataReader["Amount"]).ToWfDecimalString(),
                        ItemSequenceNumber = Convert.ToInt32(dataReader["IdCheckCredit"]).ToString(),
                        ImageFrontDetail = new CreditWf.DetailAndData(ConstantsSendService.FrontCashLetterPath, CreditWf.ImageSide.Front, _serviceUtils)
                        {
                            ImageDataAgentName = dataReader["ImageDataAgentName"].ToString(),
                            ImageDataResume = dataReader["ImageDataResume"].ToString(),
                            ImageDataCheckDetail = dataReader["ImageDataCheckDetail"].ToString()
                        },
                        ImageRearDetail = new CreditWf.DetailAndData(ConstantsSendService.RearCashLetterPath, CreditWf.ImageSide.Rear, _serviceUtils)
                        {
                            ImageDataCheckDetail = dataReader["ImageDataCheckDetail"].ToString()
                        },
                    };

                    bundleWfCredit1.Credit.ImageFrontDetail.ImageDetail.ImageCreatorRoutingNumber = abaRoutingCredit;
                    bundleWfCredit1.Credit.ImageFrontDetail.ImageData.Data_ECEInstitutionRoutingNumber = accountCredit;
                    bundleWfCredit1.Credit.ImageFrontDetail.ImageDetail.ImageCreatorDate = currentDate.ToWfDateString();
                    bundleWfCredit1.Credit.ImageFrontDetail.ImageDetail.ViewSide = "0";
                    bundleWfCredit1.Credit.ImageFrontDetail.ImageData.Data_BundleBusinessDate = currentDate.ToWfDateString();
                    bundleWfCredit1.Credit.ImageFrontDetail.ImageData.Data_ItemSequenceNumber = Convert.ToInt32(dataReader["IdCheckCredit"]).ToString();

                    bundleWfCredit1.Credit.ImageRearDetail.ImageDetail.ImageCreatorDate = currentDate.ToWfDateString();
                    bundleWfCredit1.Credit.ImageRearDetail.ImageDetail.ViewSide = "1";
                    bundleWfCredit1.Credit.ImageRearDetail.ImageData.Data_BundleBusinessDate = currentDate.ToWfDateString();
                    bundleWfCredit1.Credit.ImageRearDetail.ImageData.Data_ItemSequenceNumber = Convert.ToInt32(dataReader["IdCheckCredit"]).ToString();
                    bundleWfCredit1.Credit.ImageRearDetail.ImageData.Data_ECEInstitutionRoutingNumber = accountCredit;
                    bundleWfCredit1.Credit.ImageRearDetail.ImageDetail.ImageCreatorRoutingNumber = abaRoutingCredit;

                }
                #endregion

                #region get checks
                _logger.LogInformation($"Service {_serviceCode}: Begins: get checks from store procedure");
                dataReader.NextResult();
                while (dataReader.Read())
                {
                    string idBundle = Converters.BundleSequenceNumber(Convert.ToInt32(dataReader["BundleSequence"]));
                    BundleWf bundle = listBundles.FirstOrDefault(b => b.BundleHeader.BundleSequenceNumber == idBundle);
                    if (bundle.Checks == null)
                        bundle.Checks = new List<CheckWf>();
                    string imageCreatorRoutingNumber = dataReader["AbaRouting"].ToString();
                    string data_ECEInstitutionRoutingNumber = dataReader["Account"].ToString();

                    CheckWf checkWf = new CheckWf(_logger, _serviceUtils)
                    {
                        AuxiliaryOnUs = dataReader["MicrAuxOnUs"].ToString(),
                        ExternalProcessingCode = dataReader["MicrExternalProcessingCode"].ToString(),
#if DEBUG
                        ImageFrontDetail = new CheckWf.DetailAndData(@"C:\chequesSouth\upload\front.tif", CheckWf.ImageSide.Front, Convert.ToBoolean(dataReader["IsReclear"]), dataReader["AgentCode"].ToString(), _serviceUtils),
                        ImageRearDetail = new CheckWf.DetailAndData(@"C:\chequesSouth\upload\rear.tif", CheckWf.ImageSide.Rear, false, dataReader["AgentCode"].ToString(), _serviceUtils),
#else
                            ImageFrontDetail = new CheckWf.DetailAndData(dataReader["FrontImagePath"].ToString(), CheckWf.ImageSide.Front, Convert.ToBoolean(dataReader["IsReclear"]), dataReader["AgentCode"].ToString()),
                            ImageRearDetail = new CheckWf.DetailAndData(dataReader["RearImagePath"].ToString(), CheckWf.ImageSide.Rear, false, dataReader["AgentCode"].ToString()),
#endif
                        ItemAmount = Convert.ToDecimal(dataReader["Amount"]).ToWfDecimalString(),
                        ItemSequenceNumber = Convert.ToInt32(dataReader["ItemSequenceNumber"]).ToString(),
                        OnUs = dataReader["MicrOnUs"].ToString(),
                        PayorBankRoutingNumber = dataReader["MicrRoutingTransitNumber"].ToString(),
                        PayorBankRoutingNumberCheckDigit = dataReader["MicrRoutingTransitNumberCheckDigit"].ToString(),

                        ChecksAdd = new CheckAddWf(_logger, _serviceUtils)
                        {
                            BOFDRoutingNumber = ConstantFields.DestinationRoutingNumber,
                            BOFDItemSequenceNumber = count.ToString(),
                            BOFDDepositAccountNumber = ConstantFields.OriginRoutingNumber
                        }
                    };

                    checkWf.ImageFrontDetail.ImageDetail.ViewSide = "0";
                    checkWf.ImageFrontDetail.ImageDetail.ImageCreatorDate = currentDate.ToWfDateString();
                    checkWf.ImageFrontDetail.ImageData.Data_BundleBusinessDate = currentDate.ToWfDateString();
                    checkWf.ImageFrontDetail.ImageData.Data_ItemSequenceNumber = Convert.ToInt32(dataReader["ItemSequenceNumber"]).ToString();
                    checkWf.ImageFrontDetail.ImageDetail.ImageCreatorRoutingNumber = imageCreatorRoutingNumber;
                    checkWf.ImageFrontDetail.ImageData.Data_ECEInstitutionRoutingNumber = data_ECEInstitutionRoutingNumber;

                    checkWf.ImageRearDetail.ImageDetail.ViewSide = "1";
                    checkWf.ImageRearDetail.ImageDetail.ImageCreatorDate = currentDate.ToWfDateString();
                    checkWf.ImageRearDetail.ImageData.Data_BundleBusinessDate = currentDate.ToWfDateString();
                    checkWf.ImageRearDetail.ImageData.Data_ItemSequenceNumber = Convert.ToInt32(dataReader["ItemSequenceNumber"]).ToString();
                    checkWf.ImageRearDetail.ImageDetail.ImageCreatorRoutingNumber = imageCreatorRoutingNumber;
                    checkWf.ImageRearDetail.ImageData.Data_ECEInstitutionRoutingNumber = data_ECEInstitutionRoutingNumber;

                    bundle.Checks.Add(checkWf);
                    count++;
                }
                #endregion

                dbConnection.Close();

                return listBundles;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ErrorUtils.FormatError("ChecksRepositorie.GetChecks", ex, _serviceCode));
                throw;
            }
        }

        public bool UpdateStatus(List<int> checks, List<int> credits, int idStatus, string fileName)
        {
            try
            {
                IDbConnection dbConnection = GetDbConnection();
                IDbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandTimeout = 0;
                dbCommand.CommandText = "st_UpdateCheckAndCreditCheckStatusSouthSide";
                dbCommand.Parameters.Add(GetParameter(dbCommand, "@IdStatus", idStatus, DbType.Int32));
                dbCommand.Parameters.Add(GetParameter(dbCommand, "@ChecksXml", ToChecksXML(checks).ToString(), DbType.Xml));
                dbCommand.Parameters.Add(GetParameter(dbCommand, "@CheckCreditXml", ToCreditsXML(credits).ToString(), DbType.Xml));
                dbCommand.Parameters.Add(GetParameter(dbCommand, "@fileName", fileName, DbType.String));
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbConnection.Open();
                _logger.LogInformation($"Service {_serviceCode}: Begins: UpdateStatus from store procedure");

                dbCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ErrorUtils.FormatError("ChecksRepositorie.GetChecks", ex, _serviceCode));
                throw;
            }
        }

        public XElement ToChecksXML(List<int> list)
        {
            XElement result = new XElement("Checks");
            list.ForEach(c => result.Add(new XElement("Check", new XElement("IdCheck", c))));
            return result;
        }

        public XElement ToCreditsXML(List<int> list)
        {
            XElement result = new XElement("Credits");
            list.ForEach(c => result.Add(new XElement("Credit", new XElement("IdCheckCredit", c))));
            return result;
        }

        public bool UpdateCheckBundles(List<int> bundles, string fileName)
        {
            try
            {
                IDbConnection dbConnection = GetDbConnection();
                IDbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandTimeout = 0;
                dbCommand.CommandText = "st_UpdateCheckBundlesSouthSide";
                dbCommand.Parameters.Add(GetParameter(dbCommand, "@BundlesXml", ToBundlesXML(bundles).ToString(), DbType.Xml));
                dbCommand.Parameters.Add(GetParameter(dbCommand, "@FileName", fileName, DbType.String));
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbConnection.Open();
                _logger.LogInformation($"Service {_serviceCode}: Begins: UpdateStatus from store procedure");

                dbCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ErrorUtils.FormatError("ChecksRepositorie.GetChecks", ex, _serviceCode));
                throw;
            }
        }

        public XElement ToBundlesXML(List<int> list)
        {
            XElement result = new XElement("Bundles");
            list.ForEach(c => result.Add(new XElement("Bundle", new XElement("IdCheckBundle", c))));
            return result;
        }

        public IDbDataParameter GetParameter(
          IDbCommand command,
          string parameterName,
          object value,
          DbType type)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = type;
            parameter.Value = value;
            return parameter;
        }
    }
}
