using Microsoft.AspNetCore.Mvc;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.DataTransferObject;
using AutoMapper;
using Entities.Models;
using CompanyEmployees.ModelBinders;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
           
                var companies =  await _repository.Company.GetAllCompanies(trackchanges: false);
                var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);            
                return Ok(companiesDto);
          
        }
        [HttpGet("{Id}", Name = "CompanyById")]
        public async Task<ActionResult> GetCompany(Guid id)
        {
            var company = await _repository.Company.GetCompany(id, trackchanges: false);
            if(company== null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in teh DataBase");
                return NotFound();
            }
            else
            {
                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(companyDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody]CompanyForCreationDto company)
        {
            if(company == null)
            {
                _logger.LogError("CompanyForCreationDto object send from client is null.");
                return BadRequest("CompanyForCreationDto object is null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for CompanyCreationDto object");
                return UnprocessableEntity(ModelState);
            }
            var companyEntity = _mapper.Map<Company>(company);

            _repository.Company.CreateCompany(companyEntity);
            await _repository.SaveAsync();

            var companytoreturn = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtRoute("CompanyById", new { id = companytoreturn.Id },
                companytoreturn);

        }
        [HttpGet("collection/({ids})", Name ="CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }

            var companyEntities =  await _repository.Company.GetByIds(ids, trackchanges: false);
            if(ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }
        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody]
        IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _logger.LogError("Company collection send from client is null.");
                return BadRequest("Company collection is null.");
            }

            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
            await _repository.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = await _repository.Company.GetCompany(id, trackchanges: false);
            if(company == null)
            {
                _logger.LogInfo($"the company with id: {id} doesn´t exist in tyhe database");
                return NotFound();
            }
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();

            return NoContent();
        }
        // TODO: PATCH METHODS
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(Guid companyId,[FromBody] CompanyForUpdateDto company)
        {
            if (company == null)
            {
                _logger.LogError("CompanyForUpdateDto object send from client is null");
                return BadRequest("CompanyForUpdateDto object is null");
            }
            var companyEntity = await _repository.Company.GetCompany(companyId, trackchanges: true);
            if (companyEntity == null)
            {
                _logger.LogInfo($"the company with id : {companyId} doesn´t exist in the database.");
                return NotFound();
            }

            _mapper.Map(company, companyEntity);
            await _repository.SaveAsync();

            return NoContent();
        }
    }
}
