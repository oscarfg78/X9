using Maxi.Services.SouthSide.Data;
using Maxi.Services.SouthSide.Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlTypes;


namespace Maxi.Services.SouthSide.Repositories
{
    public interface IServiceRepository
    {
        List<ServiceConfiguration> GetServiceConfiguration();
        List<GlobalAttributes> GlobalAttributes();
    }

    public class ServiceRepository: IServiceRepository
    {
        private List<ServiceConfiguration> _serviceConfigurations;
        private ILogger<ServiceRepository> _logger;
        private ApplicationDbContext _applicationDbContext;
        private IConfiguration _configuration;

        public ServiceRepository(ApplicationDbContext applicationDbContext, IConfiguration configuration, ILogger<ServiceRepository> logger)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
            _logger = logger;
        }

        public List<ServiceConfiguration> GetServiceConfiguration()
        {
            DateTime? nullable;
            DateTime? nullable1;
            if (_serviceConfigurations != null && _serviceConfigurations.Count > 0)
                return _serviceConfigurations;
            
            _serviceConfigurations ??= new List<ServiceConfiguration>();
            List<ServiceAttribute> serviceAttributes = new List<ServiceAttribute>();
            try
            {
                using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (IDbCommand dbCommand = dbConnection.CreateCommand())
                    {
                        dbCommand.CommandText = "[Services].[st_GetServiceConfigurations]";
                        dbCommand.CommandType = CommandType.StoredProcedure;
                        dbConnection.Open();
                        using (IDataReader dataReader = dbCommand.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                ServiceConfiguration serviceConfiguration = new ServiceConfiguration()
                                {
                                    Code = dataReader["Code"].ToString(),
                                    Description = dataReader["Description"].ToString()
                                };

                                if (dataReader["LastTick"] != DBNull.Value)
                                {
                                    nullable = new DateTime?(Convert.ToDateTime(dataReader["LastTick"]));
                                }
                                else
                                {
                                    nullable = null;
                                }
                                serviceConfiguration.LastTick = nullable;


                                if (dataReader["NextTick"] != DBNull.Value)
                                {
                                    nullable1 = new DateTime?(Convert.ToDateTime(dataReader["NextTick"]));
                                }
                                else
                                {
                                    nullable1 = null;
                                }
                                serviceConfiguration.NextTick = nullable1;
                                serviceConfiguration.IsEnabled = (dataReader["IsEnabled"] == DBNull.Value ? false : Convert.ToBoolean(dataReader["IsEnabled"]));
                                _serviceConfigurations.Add(serviceConfiguration);
                            }

                            dataReader.NextResult();
                            while (dataReader.Read())
                            {
                                ServiceAttribute serviceAttribute = new ServiceAttribute()
                                {
                                    Code = dataReader["Code"].ToString(),
                                    Key = dataReader["Key"].ToString(),
                                    Value = dataReader["Value"].ToString()
                                };
                                serviceAttributes.Add(serviceAttribute);
                            }

                            //datareader.nextresult();
                            //while (datareader.read())
                            //{
                            //    service configuration tick
                            //}

                            //datareader.nextresult();
                            //while (datareader.read())
                            //{
                            //    service schedules
                            //}
                        }
                    }
                }

                foreach (ServiceConfiguration list in _serviceConfigurations)
                {
                    list.Attributes = (
                        from a in serviceAttributes
                        where string.Compare(list.Code, a.Code, StringComparison.InvariantCultureIgnoreCase) == 0
                        select a).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("Error in ServiceRepository.GetServiceConfiguration, {0}", ex.Message));
            }
            return _serviceConfigurations;
        }

        public List<GlobalAttributes> GlobalAttributes()
        {
            return _applicationDbContext.GlobalAttributes.ToList();
        }
    }
}
