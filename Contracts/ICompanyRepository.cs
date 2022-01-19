
using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllCompanies(CompanyParameters companyParameters, bool trackchanges);
        Task<Company> GetCompany(Guid companyId, bool trackchanges);
        void CreateCompany(Company company);
        Task<IEnumerable<Company>>  GetByIds(IEnumerable<Guid> ids, bool trackchanges);
        void DeleteCompany(Company company);

        
    }
}
