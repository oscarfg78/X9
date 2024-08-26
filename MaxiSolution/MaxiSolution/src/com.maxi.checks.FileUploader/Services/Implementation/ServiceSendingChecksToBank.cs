using Boz.Services.Contracts;
using Maxi.Services.SouthSide.Data;
using Maxi.Services.SouthSide.Data.Entities;
using Maxi.Services.SouthSide.FtpServices;
using Maxi.Services.SouthSide.Repositories;
using Maxi.Services.SouthSide.Services.Interface;
using Maxi.Services.SouthSide.SouthsideFile;
using Maxi.Services.SouthSide.SouthsideFile.Types;
using Maxi.Services.SouthSide.Utils;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Maxi.Services.SouthSide.Services.Implementation
{
    public class ServiceSendingChecksToBank : BackgroundService
    {
        private readonly ILogger<ServiceSendingChecksToBank> _logger;
        //private readonly IServiceScopeFactory _serviceScopeFactory;
        private string _serviceName { get; set; }
        private string _serviceCode { get; set; }
        private ServiceConfiguration Configuration { get; set; }
        private List<GlobalAttributes> GlobalAttributes { get; set; }
        private List<FileWf> fileWfList { get; set; }
        //private FileWf fileWf { get; set; }
        private IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private IServiceUtils _serviceUtils;
        private IServiceRepository _serviceRepository;
        private IChecksRepository _checksRepositorie;

        public ServiceSendingChecksToBank(
            ILogger<ServiceSendingChecksToBank> logger,
            //ApplicationDbContext applicationDbContext,
            //IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration,
            //IServiceUtils serviceUtils,
            IServiceProvider serviceProvider
            //IServiceRepository serviceRepository,
            //IChecksRepository checksRepository
            )
        {
            _logger = logger;
            //_applicationDbContext = applicationDbContext;
            //_serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            //_serviceUtils = serviceUtils;


            fileWfList = new List<FileWf>();
            _serviceName = _configuration.GetValue<string>("ServiceName");
            _serviceCode = _configuration.GetValue<string>("ServiceCode");
            Configuration = new ServiceConfiguration();
            GlobalAttributes = new List<GlobalAttributes>();

            //_serviceRepository = serviceRepository;
            //_checksRepositorie = checksRepository;
        }

        private void SetConfigurations()
        {
            _logger.LogInformation($"Service {_serviceCode}: Begins Set configurations");
            List<ServiceConfiguration> serviceConfiguration = _serviceRepository.GetServiceConfiguration();
            GlobalAttributes = _serviceRepository.GlobalAttributes();
            var configuration = serviceConfiguration.FirstOrDefault(c => c.Code == _serviceCode);
            Configuration = configuration ?? Configuration;
        }

        public void SetConstantFields()
        {
            try
            {
                _logger.LogInformation($"Service {_serviceCode}: Begins Set constansts fields");
                ConstantFields.DestinationRoutingNumber = Configuration.Attributes.FirstOrDefault(c => c.Key == "Field_DestinationRoutingNumber").Value;
                ConstantFields.DestinationName = Configuration.Attributes.FirstOrDefault(c => c.Key == "Field_DestinationName").Value;
                ConstantFields.OriginRoutingNumber = Configuration.Attributes.FirstOrDefault(c => c.Key == "Field_OriginRoutingNumber").Value;
                ConstantFields.OrigiName = Configuration.Attributes.FirstOrDefault(c => c.Key == "Field_OrigiName").Value;
                ConstantFields.OriginatorContactName = Configuration.Attributes.FirstOrDefault(c => c.Key == "Field_OriginatorContactName").Value;
                ConstantFields.OriginatorContactPhoneNumber = Configuration.Attributes.FirstOrDefault(c => c.Key == "Field_OriginatorContactPhoneNumber").Value;
                ConstantFields.TestFileIndicator = Configuration.Attributes.FirstOrDefault(c => c.Key == "Field_TestFileIndicator").Value;
                _logger.LogInformation(string.Format("Service {0}: Ends: Set constants fields", _serviceCode));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("ServiceSendingChecksToBank.SetConstantFields", ex, _serviceCode));
                throw new Exception("Exception in SetConstantFields");
            }
        }

        public void SetConstant()
        {
            try
            {
                _logger.LogInformation(string.Format("Service {0}: Begins: Set constants", _serviceCode));
                ConstantsSendService.FtpUrl = GlobalAttributes.FirstOrDefault(c => c.Name == "SOUTHSIDE_FtpUrl").Value;
                ConstantsSendService.FtpPort = Convert.ToInt32(GlobalAttributes.FirstOrDefault(c => c.Name == "SOUTHSIDE_FtpPort").Value);
                ConstantsSendService.FtpUser = GlobalAttributes.FirstOrDefault(c => c.Name == "SOUTHSIDE_FtpUser").Value;
                ConstantsSendService.FtpPassword = GlobalAttributes.FirstOrDefault(c => c.Name == "SOUTHSIDE_FtpPassword").Value;
                ConstantsSendService.CreatedFilePath = Configuration.Attributes.FirstOrDefault(c => c.Key == "CreatedFilePath").Value;
                ConstantsSendService.FtpUploadTempPath = Configuration.Attributes.FirstOrDefault(c => c.Key == "FtpUploadPathTemp").Value;
                ConstantsSendService.FileName = Configuration.Attributes.FirstOrDefault(c => c.Key == "FileName").Value;
                ConstantsSendService.FtpUploadPath = Configuration.Attributes.FirstOrDefault(c => c.Key == "FtpUploadPath").Value;
                ConstantsSendService.UploadedFileBackup = Configuration.Attributes.FirstOrDefault(c => c.Key == "UploadedFileBackup").Value;
                ConstantsSendService.FrontCashLetterPath = Configuration.Attributes.FirstOrDefault(c => c.Key == "FrontCashLetterPath").Value;
                ConstantsSendService.RearCashLetterPath = Configuration.Attributes.FirstOrDefault(c => c.Key == "RearCashLetterPath").Value;
                ConstantsSendService.FontSizeReclear = float.Parse(Configuration.Attributes.FirstOrDefault(c => c.Key == "FontSizeReclear").Value);
#if DEBUG
                //ConstantsSendService.FtpUser = "xxxxxx";//Comentar linea para deploy
                //ConstantsSendService.FtpPassword = "xxxxxxx";//Comentar linea para deploy

                //ConstantsSendService.FtpUrl = "maxi.files.com";//Comentar linea para deploy
                //ConstantsSendService.FtpUploadPath = "/SouthsideBank_DevTesting/TEST/";//Comentar linea para deploy
                //ConstantsSendService.FtpPort = 22;//Comentar linea para deploy
                //ConstantsSendService.FtpUser = "Devtesting";//Comentar linea para deploy
                //ConstantsSendService.FtpPassword = "%phM82b8";//Comentar linea para deploy

                ConstantsSendService.CreatedFilePath = @"C:\\chequesSouth\";
                ConstantsSendService.UploadedFileBackup = @"C:\\chequesSouth\UploadedFileBackup\MaxitransfersSED{0}.937"; // path donde se realiza un backup del archivo escrito
                ConstantsSendService.FrontCashLetterPath = @"C:\\chequesSouth\Upload\front.jpg";
                ConstantsSendService.RearCashLetterPath = @"C:\\chequesSouth\Upload\rear.jpg";
                //ConstantsSendService.CreatedFilePathAndName = string.Format("{0}{1}", (object)ConstantsSendService.CreatedFilePath, (object)string.Format(ConstantsSendService.FileName, (object)string.Empty));
                //ConstantsSendService.CreatedFilePathAndName = $@"C:\\chequesSouth\MaxitransfersSEDDEMO{0}.937";
#endif
                _logger.LogInformation(string.Format("Service {0}: Ends: Set constants", _serviceCode));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("ServiceSendingChecksToBank.SetConstant", ex, _serviceCode));
                throw new Exception("Exception in SetConstant");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Service {_serviceCode}: Begins process execution general");
            Console.WriteLine($"Service {_serviceCode}: Begins process execution general");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Service {_serviceCode}: BEGINS iteration worker running at:  {DateTimeOffset.Now}");
                Console.WriteLine($"Service {_serviceCode}: BEGINS iteration worker running at:  {DateTimeOffset.Now}");

                try
                {
                    #region [   implementación en un scope unico    ]
                    using var scope = _serviceProvider.CreateScope();
                    _serviceUtils = scope.ServiceProvider.GetRequiredService<IServiceUtils>();
                    _serviceRepository = scope.ServiceProvider.GetRequiredService<IServiceRepository>();
                    _checksRepositorie = scope.ServiceProvider.GetRequiredService<IChecksRepository>();
                    //_applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    //_serviceRepository = new ServiceRepository(_applicationDbContext, _configuration, _logger);
                    //_checksRepositorie = new ChecksRepository(_configuration, _logger);
                    #endregion

                    // ejemplos para obtener el GetRequiredService
                    //_serviceRepository = scope.ServiceProvider.GetRequiredService<ServiceRepository>();
                    //_checksRepositorie = scope.ServiceProvider.GetRequiredService<ChecksRepository>();

                    //using (var scope = _serviceScopeFactory.CreateScope())
                    //{
                    //    _serviceUtils = scope.ServiceProvider.GetRequiredService<ServiceUtils>();
                    //    _serviceRepository = scope.ServiceProvider.GetRequiredService<ServiceRepository>();
                    //    _checksRepositorie = scope.ServiceProvider.GetRequiredService<ChecksRepository>();
                    //}

                    SetConfigurations();
                    SetConstantFields();
                    SetConstant();
                    StartProcess();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ErrorUtils.FormatError($"Error in worker running at: {DateTimeOffset.Now}. ServiceSendingChecksToBank.ExecuteAsync;", ex, _serviceCode));
                    Console.WriteLine(ErrorUtils.FormatError($"Error in worker running at: {DateTimeOffset.Now}. ServiceSendingChecksToBank.ExecuteAsync;", ex, _serviceCode));
                }

                // TODO definir el tiempo de ejecución del servicio
                await Task.Delay(10000, stoppingToken);

                _logger.LogInformation($"Service {_serviceCode}: END iteration worker running at:  {DateTimeOffset.Now}");
                Console.WriteLine($"Service {_serviceCode}: END iteration worker running at:  {DateTimeOffset.Now}");
            }
        }

        public void StartProcess()
        {
            try
            {
                fileWfList = new List<FileWf>();
                _logger.LogInformation(string.Format("Service {0}: Begins: get data", _serviceCode));
                GetData();
                foreach (FileWf lisfile in fileWfList)
                {
                    if (lisfile != null && lisfile.CashLetter != null && lisfile.CashLetter.Bundles != null && lisfile.CashLetter.Bundles.Count > 0)
                    {
                        _logger.LogInformation(string.Format("Service {0}: Begins: create file", _serviceCode));
                        string fileName = CreateFile(lisfile);
                        _logger.LogInformation(string.Format("Service {0}: Begins: upload file", _serviceCode));
                        UploadFile(fileName);
                        _logger.LogInformation(string.Format("Service {0}: Begins: update status", _serviceCode));
                        UpdateStatus(lisfile, fileName);
                        _logger.LogInformation(string.Format("Service {0}: Begins: update bundles", _serviceCode));
                        UpdateBunbles("-NoConfirmationFile-", lisfile);
                    }
                    else
                    {
                        _logger.LogInformation(string.Format("Service {0}: Ends: End process. No checks found", _serviceCode));
                    }

                }
                _logger.LogInformation(string.Format("Service {0}: Ends: End process", _serviceCode));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("ServiceSendingChecksToBank.StartProcess", ex, _serviceCode));
            }
            finally
            {
                fileWfList = null;
            }
        }

        public void GetData()
        {
            try
            {
                DateTime now = DateTime.Now;
                List<BundleWf> bundleWfList = new List<BundleWf>();
                _logger.LogInformation(string.Format("Service {0}: Begins: get bundles", _serviceCode));
                List<BundleWf> checks = _checksRepositorie.GetChecks(now);
                if (checks.Count <= 0)
                    return;
                _logger.LogInformation($"Service {_serviceCode}: To process bundles: {checks.Count}, checks: {checks.Sum(c => c.Checks.Count())}");
                Console.WriteLine($"Service {_serviceCode}: To process bundles: {checks.Count}, checks: {checks.Sum(c => c.Checks.Count())}");
                foreach (BundleWf bundleWf1 in checks)
                {
                    string routing = bundleWf1.BundleHeader.DestinationRoutingNumber == null ? "" : bundleWf1.BundleHeader.DestinationRoutingNumber;
                    FileWf fileWf;

                    if (fileWfList != null && fileWfList.Count > 0)
                        fileWf = fileWfList.Where(o => o.FileHeader.ImmediateDestinationRoutingNumber == routing).FirstOrDefault();
                    else fileWf = null;

                    if (fileWf != null)
                    {
                        fileWf.CashLetter.Bundles.Add(bundleWf1);
                        //_logger.LogInformation(string.Format("Service {0}: Begins: count bundles if file exists", _serviceCode));
                        CashLetterWf tempCashLetter = fileWf.CashLetter;
                        CashLetterWf.Control tempControl = new CashLetterWf.Control(_logger, _serviceUtils)
                        {
                            BundleCount = fileWf.CashLetter.Bundles.Count.ToString(),
                            ImagesWithinCashLetterCount = fileWf.CashLetter.Bundles.Sum(b => Convert.ToInt32(b.BundleControl.ImagesWithinBundleCount)).ToString(),
                            ItemsWithinCashLetterCount = fileWf.CashLetter.Bundles.Sum(b => Convert.ToInt32(b.BundleControl.ItemsWithinBundleCount)).ToString(),
                            CashLetterTotalAmount = fileWf.CashLetter.Bundles.Sum(b => Convert.ToInt32(b.BundleControl.BundleTotalAmount)).ToString()
                        };
                        tempCashLetter.CashLetterControl = tempControl;
                        FileWf.Control tempFileControl = new FileWf.Control(_logger, _serviceUtils)
                        {
                            CashLetterCount = "1",
                            FileTotalAmount = fileWf.CashLetter.CashLetterControl.CashLetterTotalAmount,
                            TotalItemCount = fileWf.CashLetter.CashLetterControl.ItemsWithinCashLetterCount,
                            TotalRecordCount = (
                            fileWf.CashLetter.Bundles.Sum(b => b.Checks.Count * 5 + (b.Credit == null ? 0 : 1)) +
                            fileWf.CashLetter.Bundles.Count * 2 + 4).ToString()
                        };
                        fileWf.FileControl = tempFileControl;
                        continue;
                    }

                    FileWf fileWf2 = new FileWf
                    {
                        FileHeader = new FileWf.Header(_logger, _serviceUtils)
                        {
                            FileCreationDate = now.ToWfDateString(),
                            FileCreationTime = now.ToWfTimeString(),
                            ImmediateDestinationRoutingNumber = bundleWf1.BundleHeader.DestinationRoutingNumber,
                            ImmediateOriginRoutingNumber = bundleWf1.BundleHeader.DestinationRoutingNumber
                        },
                        CashLetter = new CashLetterWf()
                        {
                            CashLetterHeader = new CashLetterWf.Header(_logger, _serviceUtils)
                            {
                                CashLetterBusinessDate = now.ToWfDateString(),
                                CashLetterCreationDate = now.ToWfDateString(),
                                CashLetterCreationTime = now.ToWfTimeString(),
                                CashLetterId = Converters.CashLetterId(now),
                                DestinationRoutingNumber = bundleWf1.BundleHeader.DestinationRoutingNumber,
                                ECEInstitutionRoutingNumber = bundleWf1.BundleHeader.DestinationRoutingNumber
                            }
                        }
                    };

                    fileWf2.CashLetter.Bundles = new List<BundleWf>();
                    fileWf2.CashLetter.Bundles.Add(bundleWf1);

                    _logger.LogInformation(string.Format("Service {0}: Begins: count bundles", _serviceCode));
                    CashLetterWf cashLetter = fileWf2.CashLetter;
                    CashLetterWf.Control control = new CashLetterWf.Control(_logger, _serviceUtils)
                    {
                        BundleCount = fileWf2.CashLetter.Bundles.Count.ToString(),
                        ImagesWithinCashLetterCount = fileWf2.CashLetter.Bundles.Sum(b => Convert.ToInt32(b.BundleControl.ImagesWithinBundleCount)).ToString(),
                        ItemsWithinCashLetterCount = fileWf2.CashLetter.Bundles.Sum(b => Convert.ToInt32(b.BundleControl.ItemsWithinBundleCount)).ToString(),
                        CashLetterTotalAmount = fileWf2.CashLetter.Bundles.Sum(b => Convert.ToInt32(b.BundleControl.BundleTotalAmount)).ToString()
                    };
                    cashLetter.CashLetterControl = control;

                    FileWf.Control fileControl = new FileWf.Control(_logger, _serviceUtils)
                    {
                        CashLetterCount = "1",
                        FileTotalAmount = fileWf2.CashLetter.CashLetterControl.CashLetterTotalAmount,
                        TotalItemCount = fileWf2.CashLetter.CashLetterControl.ItemsWithinCashLetterCount,
                        TotalRecordCount = (
                        fileWf2.CashLetter.Bundles.Sum(b => b.Checks.Count * 5 + (b.Credit == null ? 0 : 1)) +
                        fileWf2.CashLetter.Bundles.Count * 2 + 4).ToString()
                    };

                    fileWf2.FileControl = fileControl;

                    fileWfList.Add(fileWf2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("ServiceSendingChecksToBank.GetData", ex, _serviceCode));
                throw;
            }
        }

        public string CreateFile(FileWf objFile)
        {
            try
            {
                string dateTimePart = DateTime.Now.ToString("MMddyyHmmssfff");
                // ConstantsSendService.FileName has a format like the following: MaxitransfersSED-NV-{0}.937
                string finalFileName = string.Format(ConstantsSendService.FileName, dateTimePart);
                // ConstantsSendService.CreatedFilePath has a format like the following: \\MAXI-SERVER-008\Upload\SouthsideNVCheckFiles\
                string pathFinalFile = string.Format("{0}{1}", ConstantsSendService.CreatedFilePath, finalFileName);
                if (File.Exists(pathFinalFile))
                {
                    dateTimePart = DateTime.Now.AddMinutes(1.0).ToString("MMddyyHmmssfff");
                    finalFileName = string.Format(ConstantsSendService.FileName, dateTimePart);
                    _logger.LogInformation(string.Format("Service {0}: Begins: Se Modifica  Nombre Archivo A:" + finalFileName, _serviceCode));
                    Console.WriteLine(string.Format("Service {0}: Begins: Se Modifica  Nombre Archivo A:" + finalFileName, _serviceCode));
                    pathFinalFile = string.Format("{0}{1}", ConstantsSendService.CreatedFilePath, finalFileName);
                }
                else
                {
                    _logger.LogInformation(string.Format("Service {0}: Begins: Se agrega Nombre Archivo" + finalFileName, _serviceCode));
                    Console.WriteLine(string.Format("Service {0}: Begins: Se agrega Nombre Archivo" + finalFileName, _serviceCode));
                }

                _serviceUtils.CreateFile(objFile, pathFinalFile);
                // ConstantsSendService.UploadedFileBackup has a format like the following: \\MAXI-SERVER-008\FileStorage\Upload\SouthsideNVCheckFiles\UploadedFiles\MaxitransfersSED-NV-{0}.937
                File.Copy(pathFinalFile, string.Format(ConstantsSendService.UploadedFileBackup, dateTimePart));
                return finalFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("ServiceSendingChecksToBank.CreateFile", ex, _serviceCode));
                throw;
            }
        }

        public void UpdateBunbles(string fileName, FileWf objFile)
        {
            try
            {
                List<int> bundles = new List<int>();
                objFile.CashLetter.Bundles.ForEach(c => bundles.Add(Convert.ToInt32(c.BundleHeader.BundleId)));
                _checksRepositorie.UpdateCheckBundles(bundles, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("SouthsideChecksReceiveService.ReadFile", ex, _serviceCode));
                throw;
            }
        }

        public void UpdateStatus(FileWf objFile, string fileName)
        {
            try
            {
                int idStatus = 40;
                List<int> checks = new List<int>();
                List<int> credits = new List<int>();
                objFile.CashLetter.Bundles.ForEach(c =>
                {
                    if (c.Credit == null)
                        return;
                    credits.Add(Convert.ToInt32(c.Credit.ItemSequenceNumber));
                });
                objFile.CashLetter.Bundles.ForEach(c =>
                {
                    if (c.Checks == null)
                        return;
                    checks.AddRange(c.Checks.Select(ci => Convert.ToInt32(ci.ItemSequenceNumber)));
                });
                _checksRepositorie.UpdateStatus(checks, credits, idStatus, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("ServiceSendingChecksToBank.UpdateStatus", ex, _serviceCode));
                throw;
            }
        }

        public void UploadFile(string fileName)
        {
            try
            {
                new SftpProtocol().Upload(
                    ConstantsSendService.FtpUrl,
                    ConstantsSendService.FtpPort,
                    ConstantsSendService.FtpUser,
                    ConstantsSendService.FtpPassword,
                    ConstantsSendService.FtpUploadPath,
                    fileName,
                    ConstantsSendService.CreatedFilePath, // path where the file is created
                    fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("ServiceSendingChecksToBank.UploadFile", ex, _serviceCode));
                throw;
            }
        }
    }
}
