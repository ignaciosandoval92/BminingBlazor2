using Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.SecurityReflection;
using Models.SecurityViewModel;
using static Data.TableConstants;
using SqlKata.Execution;

namespace BminingBlazor.Services
{
    public class SecurityReflectionDataService:ISecurityReflectionDataService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public SecurityReflectionDataService(IDataAccess dataAccess, IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("default");
        }
        public async Task<List<SecurityReflectionViewModel>> ReadSecurityReflection()
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var securityReflections = (await queryFactory
                .Query()
                .From(SecurityReflectionTable)
                .Select(SecurityReflectionConstants.SecurityId)
                .Select(SecurityReflectionConstants.Name)
                .Select(SecurityReflectionConstants.Date)
                .Select(SecurityReflectionConstants.Topic)                
                .GetAsync<SecurityReflectionModel>()).ToList();
            var securityViewModel = new List<SecurityReflectionViewModel>();
            foreach (var securityReflection in securityReflections)
            {
                securityViewModel.Add(new SecurityReflectionViewModel
                {
                    MyId = securityReflection.SecurityId,                   
                    MyName = securityReflection.Name,
                    MyDate = securityReflection.Date,
                    MyTopic = securityReflection.Topic
                });
            }
            return securityViewModel;
        }
    }
}
