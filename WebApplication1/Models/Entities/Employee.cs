namespace WebApplication1.Models.Entities
{
    //---------------------------------------------------------
    // EXISTING CLASS - KEEP AS IT IS
    //---------------------------------------------------------

    public class Employee
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public string? Phone { get; set; }

        public decimal Salary { get; set; }
    }

    public class ModelEmpEducationList

    {
        public ModelResponse Response { get; set; }
        public List<ModelEmployeeData> MasterEducationData { get; set; }  = new List<ModelEmployeeData>();

    }

    public class ModelEmployeeData
    {
        public string EMPGUID { get; set; }
        public string EMPName { get; set; }
        public string DepartmentID { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public string JoinDate { get; set; }
        public string Salary { get; set; }
        public int Status { get; set; }
        public List<ModelEmployeeducationDetails> EducationDetails { get; set; } = new List<ModelEmployeeducationDetails>();
    }

    public class ModelEmployeeducationDetails
    {
        public string EDUGUID { get; set; }
        public string EMPGUID { get; set; }
        public string Qualification { get; set; }
        public string InstitutionName { get; set; }
        public string YearOfPassing { get; set; }
        public string Percentage { get; set; }
        public string Remarks { get; set; }
        public int Status { get; set; }
    }

    //---------------------------------------------------------
    // MAIN EMPLOYEE MODEL
    //---------------------------------------------------------

    public class ModelEmployee
    {
        public string EMPGUID { get; set; }

        public string EMPName { get; set; }

        public string DepartmentID { get; set; }

        public string MobileNo { get; set; }

        public string EmailID { get; set; }

        public string JoinDate { get; set; }

        public string Salary { get; set; }

        public int Status { get; set; }

        public List<ModelEducation> Education { get; set; }
            = new List<ModelEducation>();
    }

    //---------------------------------------------------------
    // CHILD EDUCATION MODEL
    //---------------------------------------------------------

    public class ModelEducation
    {
        public string EDUGUID { get; set; }

        public string Qualification { get; set; }

        public string InstitutionName { get; set; }

        public string YearOfPassing { get; set; }

        public string Percentage { get; set; }

        public string Remarks { get; set; }

        public int Status { get; set; }
    }

    //---------------------------------------------------------
    // RESPONSE WRAPPER
    //---------------------------------------------------------

    public class ModelEmployeeResponse
    {
        public ModelResponse Response { get; set; }

        public List<ModelEmployee> EmployeeList { get; set; }
            = new List<ModelEmployee>();
    }

    //---------------------------------------------------------
    // COMMON RESPONSE MODEL
    //---------------------------------------------------------

    public class ModelResponse
    {
        public string _status { get; set; }

        public string _code { get; set; }

        public string _Source { get; set; }

        public string _Description { get; set; }

        public string _Details { get; set; }

        public string _TransID { get; set; }

        public ModelResponse(string code,
                             string status,
                             string source,
                             string Description,
                             string Details)
        {
            _code = code;
            _status = status;
            _Source = source;
            _Description = Description;
            _Details = Details;
        }

        public ModelResponse(string code,
                             string status,
                             string source,
                             string Description,
                             string Details,
                             string TransID)
        {
            _code = code;
            _status = status;
            _Source = source;
            _Description = Description;
            _Details = Details;
            _TransID = TransID;
        }

        public ModelResponse()
        {

        }
    }
}