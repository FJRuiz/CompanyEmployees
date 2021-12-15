using AutoMapper;
using Contracts;
using Entities.DataTransferObject;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index(Guid companyId)
        {
            var company = _repository.Company.GetCompany(companyId, trackchanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the DataBase");
                return NotFound();
            }
            _logger.LogInfo("compañia encontrada");
            var employeesFromDb = _repository.Employee.GetEmployees(companyId, trackChanges: false);

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

            return Ok(employeesDto);
        }

        [HttpGet("{Id}")]
        public IActionResult GetEmployeeForCompany(Guid companyId, Guid Id)
        {
            var company = _repository.Company.GetCompany(companyId, trackchanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the DataBase");
                return NotFound();
            }
            
            var employeeDb = _repository.Employee.GetEmployee(companyId, Id, trackChanges: false);
            if(employeeDb == null)
            {
                _logger.LogInfo($"employee with id: {Id} doesn't exist in the DataBase");
                return NotFound();
            }
            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return Ok(employee);
        }
    }
}
