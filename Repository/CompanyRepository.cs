using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Company>> GetAllCompanies(CompanyParameters companyParameters, bool trackchanges)
        { 
            var companies = await FindAll(trackchanges)           
            .Search(companyParameters.SearchTerm)
            .Sort((companyParameters.OrderBy))
            .ToListAsync();

            return companies;
            }

        public async Task<Company> GetCompany(Guid companyId, bool trackchanges) =>
            await FindByCondition(c => c.Id.Equals(companyId), trackchanges)
            .SingleOrDefaultAsync();

        public void CreateCompany(Company company) => Create(company);

        public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> ids, bool trackchanges) =>
             await FindByCondition(x => ids.Contains(x.Id), trackchanges)
            .ToListAsync();

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }
    }
}
