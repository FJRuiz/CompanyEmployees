using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
using Entities.DataTransferObject;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]

    public class EmployeeController : Controller
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        public EmployeeController(IRepositoryManager repository, ILoggerManager logger, 
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid companyId, [FromQuery]
        EmployeeParameters employeeParameters)
        {
            var company = await  _repository.Company.GetCompany(companyId, trackchanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the DataBase");
                return NotFound();
            }
            _logger.LogInfo("compañia encontrada");
            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

            return Ok(employeesDto);
        }

        [HttpGet("{Id}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, [FromQuery] 
        EmployeeParameters employeeParameters)
        {
            if(!employeeParameters.ValidAgeRange)
            {
                return BadRequest("Max age can't be less than min age");
            }
            var company = await _repository.Company.GetCompany(companyId, trackchanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the DataBase");
                return NotFound();
            }
            
            var employeeFromDb = await _repository.Employee.GetEmployeesAsync(companyId, 
                employeeParameters, trackChanges: false);

            Response.Headers.Add("x-pagination",
                JsonConvert.SerializeObject(employeeFromDb.MetaData));
                        
            var employeesDto = _mapper.Map<EmployeeDto>(employeeFromDb);
            return Ok(employeesDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            var company = await _repository.Company.GetCompany(companyId, trackchanges: false);
            if(company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn´t exist in the database.");
                return NotFound();
            }
            var employeeEntity = _mapper.Map<Employee>(employee);
            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }
        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            
            var employeeForCompany = HttpContext.Items["employee"] as Employee;
            
            _repository.Employee.DeleteEmployee(employeeForCompany);
            await _repository.SaveAsync();

            return NoContent();

        }
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody]
        EmployeeForUpdateDto employee)
        {
            var employeeEntity = HttpContext.Items["employee"] as Employee;
            
            _mapper.Map(employee, employeeEntity);
            await _repository.SaveAsync();

            return NoContent();
        }
        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> PartialUpdateEmployeeForCompany(Guid companyId, Guid id, 
            [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            var employeeEntity =HttpContext.Items["employee"] as Employee;
           

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

            patchDoc.ApplyTo(employeeToPatch, ModelState);
            TryValidateModel(employeeToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(employeeToPatch, employeeEntity);
            
            await _repository.SaveAsync();

            return NoContent();
        }
    }
}
