using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Entities;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpoyeeController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public EmpoyeeController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllemployees()
        {
            var AllEmp = dbContext.Employees.ToList();
            return Ok(AllEmp);
        }

        [HttpPost]
        public IActionResult PostEmpoyee(AddEmpDto employee)
        {
            var employeeEntity = new Employee()
            {
                Name = employee.Name,
                Email = employee.Email,
                Phone = employee.Phone,
                Salary = employee.Salary
            };
            dbContext.Employees.Add(employeeEntity);
            dbContext.SaveChanges();
            return Ok(employeeEntity); // Returns 200 OK with the created entity

        }

        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetEmpByID(Guid id)
        {
            var emp = dbContext.Employees.Find(id);
            if (emp == null)
            {
                return NotFound();
            }
            return Ok(emp);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public IActionResult UpdateEmployee(Guid id, UpdateEmpDto updateEmp)
        {
            var emp = dbContext.Employees.Find(id);
            if (emp == null)
            {
                return NotFound();
            }
            emp.Name = updateEmp.Name;
            emp.Email = updateEmp.Email;
            emp.Phone = updateEmp.Phone;
            emp.Salary = updateEmp.Salary;
            dbContext.SaveChanges();
            return Ok(emp);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public IActionResult DeletEmp(Guid id)
        {
            var emp = dbContext.Employees.Find(id);
            if (emp == null)
            {
                return NotFound();
            }
            dbContext.Employees.Remove(emp);
            dbContext.SaveChanges();
            return Ok();
        }
    }
}
