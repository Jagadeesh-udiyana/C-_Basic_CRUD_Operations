using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Xml;
using WebApplication1.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeEducationController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public EmployeeEducationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllEmployees")]

        public ModelEmpEducationList GetAllEmployees()
        {

            ModelEmpEducationList objResult = new ModelEmpEducationList();

            DataSet DS = new DataSet();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {

                SqlCommand cmd = new SqlCommand("USP_SEL_ALL_Employees", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //-------------------------------------------------
                // DATA ADAPTER
                //-------------------------------------------------

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(DS);

                //-------------------------------------------------
                // CHECK MASTER TABLE
                //-------------------------------------------------

                if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
                {

                    //-------------------------------------------------
                    // LOOP EMPLOYEES
                    //-------------------------------------------------

                    foreach (DataRow dr in DS.Tables[0].Rows)
                    {

                        ModelEmployeeData employee = new ModelEmployeeData();

                        employee.EMPGUID = dr["EMPGUID"].ToString();

                        employee.EMPName = dr["EMPName"].ToString();

                        employee.DepartmentID = dr["DepartmentID"].ToString();

                        employee.MobileNo = dr["MobileNo"].ToString();

                        employee.EmailID = dr["EmailID"].ToString();

                        employee.JoinDate = dr["JoinDate"].ToString();

                        employee.Salary = dr["Salary"].ToString();

                        employee.Status = Convert.ToInt32(dr["Status"]);

                        //-------------------------------------------------
                        // FIND MATCHING EDUCATION ROWS
                        //-------------------------------------------------

                        if (DS.Tables.Count > 1 && DS.Tables[1].Rows.Count > 0)
                        {

                            foreach (DataRow drEdu in DS.Tables[1].Rows)
                            {

                                //-------------------------------------------------
                                // MATCH USING EMPGUID
                                //-------------------------------------------------

                                if (drEdu["EMPGUID"].ToString() == employee.EMPGUID)
                                {

                                    ModelEmployeeducationDetails education = new ModelEmployeeducationDetails();

                                    education.EDUGUID = drEdu["EDU_GUID"].ToString();

                                    education.EMPGUID = drEdu["EMPGUID"].ToString();

                                    education.Qualification = drEdu["Qualification"].ToString();

                                    education.InstitutionName = drEdu["InstitutionName"].ToString();

                                    education.YearOfPassing = drEdu["YearOfPassing"].ToString();

                                    education.Percentage = drEdu["Percentage"].ToString();

                                    education.Remarks = drEdu["Remarks"].ToString();

                                    //-------------------------------------------------
                                    // ADD CHILD
                                    //-------------------------------------------------

                                    employee.EducationDetails.Add(education);

                                }

                            }

                        }

                        //-------------------------------------------------
                        // ADD EMPLOYEE
                        //-------------------------------------------------

                        objResult.MasterEducationData.Add(employee);

                    }

                    //-------------------------------------------------
                    // SUCCESS RESPONSE
                    //-------------------------------------------------

                    objResult.Response = new ModelResponse("111", "Success", "Employee", "Data Retrieved Successfully", "");

                }
                else
                {

                    objResult.Response = new ModelResponse("222", "No Data", "Employee", "No Records Found", "");

                }

            }

            return objResult;

        }

        [HttpGet]
        [Route("GetEmployee")]

        public ModelEmpEducationList GetEmployee(string EMPGUID)
        {

            ModelEmpEducationList objResult = new ModelEmpEducationList();

            DataSet DS = new DataSet();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {

                SqlCommand cmd = new SqlCommand("USP_SEL_Employee", con);

                cmd.CommandType =  CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue( "@guidEMPGUID", EMPGUID);

                //-------------------------------------------------
                // DATA ADAPTER
                //-------------------------------------------------

                SqlDataAdapter da =  new SqlDataAdapter(cmd);

                da.Fill(DS);

                //-------------------------------------------------
                // CHECK MASTER TABLE DATA
                //-------------------------------------------------

                if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
                {

                    //-------------------------------------------------
                    // LOOP MASTER TABLE
                    //-------------------------------------------------

                    foreach (DataRow dr in DS.Tables[0].Rows) {

                        ModelEmployeeData employee = new ModelEmployeeData();

                        employee.EMPGUID = dr["EMPGUID"].ToString();

                        employee.EMPName = dr["EMPName"].ToString();

                        employee.DepartmentID = dr["DepartmentID"].ToString();

                        employee.MobileNo = dr["MobileNo"].ToString();

                        employee.EmailID = dr["EmailID"].ToString();

                        employee.JoinDate = dr["JoinDate"].ToString();

                        employee.Salary = dr["Salary"].ToString();

                        employee.Status = Convert.ToInt32(dr["Status"]);

                        //-------------------------------------------------
                        // CHILD TABLE
                        //-------------------------------------------------

                        if (DS.Tables.Count > 1 && DS.Tables[1].Rows.Count > 0)
                        {

                            foreach (DataRow drEdu in DS.Tables[1].Rows)
                            {

                                ModelEmployeeducationDetails education = new ModelEmployeeducationDetails();

                                education.EDUGUID = drEdu["EDU_GUID"].ToString();

                                education.EMPGUID = drEdu["EMPGUID"].ToString();

                                education.Qualification = drEdu["Qualification"].ToString();

                                education.InstitutionName = drEdu["InstitutionName"].ToString();

                                education.YearOfPassing = drEdu["YearOfPassing"].ToString();

                                education.Percentage = drEdu["Percentage"].ToString();

                                education.Remarks = drEdu["Remarks"].ToString();

                                employee.EducationDetails.Add(education);

                            }

                        }

                        //-------------------------------------------------
                        // ADD TO FINAL LIST
                        //-------------------------------------------------

                        objResult.MasterEducationData.Add(employee);

                    }

                    //-------------------------------------------------
                    // SUCCESS RESPONSE
                    //-------------------------------------------------

                    objResult.Response = new ModelResponse("111", "Success", "Employee", "Data Retrieved Successfully", "");

                }

                else
                {

                    objResult.Response = new ModelResponse("222", "No Data", "Employee", "No Records Found", "");

                }

            }

            return objResult;

        }

        [HttpDelete]
        [Route("DeleteEmployee")]

        public ModelResponse DeleteEmployee(string EMPGUID)
        {

            ModelResponse objResponse =new ModelResponse();

            //-------------------------------------------------
            // JWT CLAIMS
            //-------------------------------------------------

            string userGuid = User.FindFirst("UserGUID")?.Value;
            string userName = User.FindFirst("UserName")?.Value;
            string roleName = User.FindFirst("RoleName")?.Value;

            //-------------------------------------------------
            // ROLE CHECK
            //-------------------------------------------------

            if (roleName != "Admin")
            {
                return new ModelResponse("401","Unauthorized","Employee", "Only Admin can delete employees", "");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {

                SqlCommand cmd =  new SqlCommand("USP_DEL_Employee", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //-------------------------------------------------
                // PARAMETER
                //-------------------------------------------------

                cmd.Parameters.AddWithValue( "@guidEMPGUID", EMPGUID);

                //-------------------------------------------------
                // OPEN CONNECTION
                //-------------------------------------------------

                con.Open();

                //-------------------------------------------------
                // EXECUTE
                //-------------------------------------------------

                int result = cmd.ExecuteNonQuery();

                //-------------------------------------------------
                // RESPONSE
                //-------------------------------------------------

                if (result > 0)
                {

                    objResponse = new ModelResponse("111", "Success", "Employee", "Deleted Successfully","");

                }
                else
                {

                    objResponse = new ModelResponse("222", "Failed", "Employee", "No Records Deleted", "");

                }

            }

            return objResponse;

        }


        [HttpPost]
        [Route("PostEmployee")]

        public ModelEmployeeResponse PostEmployee(ModelEmployee employee) {

            ModelEmployeeResponse objResult = new ModelEmployeeResponse();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString)) {

                SqlCommand cmd = new SqlCommand("USP_IOU_Employee", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //-------------------------------------------------
                // MASTER PARAMETERS
                //-------------------------------------------------

                cmd.Parameters.AddWithValue("@guidEMPGUID", employee.EMPGUID);

                cmd.Parameters.AddWithValue("@varEMPName", employee.EMPName);

                cmd.Parameters.AddWithValue("@guidDepartmentID", employee.DepartmentID);

                cmd.Parameters.AddWithValue("@varMobileNo", employee.MobileNo);

                cmd.Parameters.AddWithValue("@varEmailID", employee.EmailID);

                cmd.Parameters.AddWithValue("@dtmJoinDate", employee.JoinDate);
 
                cmd.Parameters.AddWithValue("@decSalary", employee.Salary);

                cmd.Parameters.AddWithValue("@tintStatus", employee.Status);

                //-------------------------------------------------
                // SAMPLE CREATED BY
                //-------------------------------------------------

                cmd.Parameters.AddWithValue("@guidCreatedBy", Guid.NewGuid());

                //-------------------------------------------------
                // XML CREATION
                //-------------------------------------------------

                string xmlEducation = "";

                if (employee.Education.Count > 0)
                {

                    XmlDocument doc = new XmlDocument();

                    XmlNode rootNode = doc.CreateElement("Table");

                    doc.AppendChild(rootNode);

                    for (int i = 0; i < employee.Education.Count; i++)
                    {

                        XmlNode rowNode = doc.CreateElement("Rows");

                        XmlElement rowElement = (XmlElement)rowNode;

                        rowElement.SetAttribute("EDU_GUID", employee.Education[i].EDUGUID);

                        rowElement.SetAttribute("Qualification", employee.Education[i].Qualification);

                        rowElement.SetAttribute("InstitutionName", employee.Education[i].InstitutionName);

                        rowElement.SetAttribute("YearOfPassing", employee.Education[i].YearOfPassing);

                        rowElement.SetAttribute("Percentage", employee.Education[i].Percentage);

                        rowElement.SetAttribute("Remarks", employee.Education[i].Remarks);

                        rootNode.AppendChild(rowNode);

                    }

                    xmlEducation = doc.InnerXml;

                }

                //-------------------------------------------------
                // XML PARAMETER
                //-------------------------------------------------

                cmd.Parameters.AddWithValue( "@xmlEmployeeEducation", xmlEducation);

                //-------------------------------------------------
                // OUTPUT PARAMETER
                //-------------------------------------------------

                cmd.Parameters.Add("@chnOutputParameter", SqlDbType.NVarChar, 500);

                cmd.Parameters["@chnOutputParameter"].Direction = ParameterDirection.Output;

                //-------------------------------------------------
                // EXECUTE
                //-------------------------------------------------

                con.Open();

                cmd.ExecuteNonQuery();

                string message = cmd.Parameters["@chnOutputParameter"].Value.ToString();

                //-------------------------------------------------
                // RESPONSE
                //-------------------------------------------------

                if (message == "Inserted")
                {
                    objResult.Response = new ModelResponse("111", "Success", "Employee", "Inserted Successfully", "");
                }
                else if (message == "Updated")
                {
                    objResult.Response = new ModelResponse("111","Success", "Employee", "Updated Successfully", "");
                }
                else if (message == "Exists")
                {
                    objResult.Response = new ModelResponse("222", "Exists", "Employee", "Employee Already Exists", "");
                }
                else
                {
                    objResult.Response = new ModelResponse("333", "Failed", "Employee", message, "" );
                }

            }

            return objResult;

        }


        [HttpGet]
        [Route("SearchEmployee")]

        public ModelEmpEducationList SearchEmployee(string? EMPName, string? MobileNo)
        {

            ModelEmpEducationList objResult = new ModelEmpEducationList();

            DataSet DS = new DataSet();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {

                SqlCommand cmd = new SqlCommand("USP_SRC_Employee", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@varEMPName", string.IsNullOrEmpty(EMPName) ? DBNull.Value: EMPName);
                cmd.Parameters.AddWithValue("@varMobileNo", string.IsNullOrEmpty(MobileNo) ? DBNull.Value : MobileNo);

                //-------------------------------------------------
                // DATA ADAPTER
                //-------------------------------------------------

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(DS);

                //-------------------------------------------------
                // CHECK MASTER TABLE DATA
                //-------------------------------------------------

                if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
                {

                    //-------------------------------------------------
                    // LOOP MASTER TABLE
                    //-------------------------------------------------

                    foreach (DataRow dr in DS.Tables[0].Rows)
                    {

                        ModelEmployeeData employee = new ModelEmployeeData();

                        employee.EMPGUID = dr["EMPGUID"].ToString();

                        employee.EMPName = dr["EMPName"].ToString();

                        employee.DepartmentID = dr["DepartmentID"].ToString();

                        employee.MobileNo = dr["MobileNo"].ToString();

                        employee.EmailID = dr["EmailID"].ToString();

                        employee.JoinDate = dr["JoinDate"].ToString();

                        employee.Salary = dr["Salary"].ToString();

                        employee.Status = Convert.ToInt32(dr["Status"]);

                        //-------------------------------------------------
                        // CHILD TABLE
                        //-------------------------------------------------

                        if (DS.Tables.Count > 1 && DS.Tables[1].Rows.Count > 0)
                        {

                            foreach (DataRow drEdu in DS.Tables[1].Rows)
                            {

                                if (drEdu["EMPGUID"].ToString() == employee.EMPGUID)
                                {

                                    ModelEmployeeducationDetails education = new ModelEmployeeducationDetails();

                                    education.EDUGUID = drEdu["EDU_GUID"].ToString();

                                    education.EMPGUID = drEdu["EMPGUID"].ToString();

                                    education.Qualification = drEdu["Qualification"].ToString();

                                    education.InstitutionName = drEdu["InstitutionName"].ToString();

                                    education.YearOfPassing = drEdu["YearOfPassing"].ToString();

                                    education.Percentage = drEdu["Percentage"].ToString();

                                    education.Remarks = drEdu["Remarks"].ToString();

                                    employee.EducationDetails.Add(education);
                                }

                            }

                        }

                        //-------------------------------------------------
                        // ADD TO FINAL LIST
                        //-------------------------------------------------

                        objResult.MasterEducationData.Add(employee);

                    }

                    //-------------------------------------------------
                    // SUCCESS RESPONSE
                    //-------------------------------------------------

                    objResult.Response = new ModelResponse("111", "Success", "Employee", "Data Retrieved Successfully", "");

                }

                else
                {

                    objResult.Response = new ModelResponse("222", "No Data", "Employee", "No Records Found", "");

                }

            }

            return objResult;

        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected");
            }

            Guid fileGuid = Guid.NewGuid();

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(),"Uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string savedFileName = fileGuid.ToString() + "_" + file.FileName;

            string fullPath = Path.Combine(uploadsFolder, savedFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //-------------------------------------------------
            // SAVE TO DATABASE
            //-------------------------------------------------

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO File_Master (FileGUID,FileName,RelativePath,FileType,FileSizeInBytes,CreatedOn,CreatedBy)
                             VALUES (@FileGUID,@FileName,@RelativePath,@FileType,@FileSizeInBytes,GETDATE(),'ADMIN')";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@FileGUID", fileGuid);
                cmd.Parameters.AddWithValue("@FileName", file.FileName);
                cmd.Parameters.AddWithValue("@RelativePath", savedFileName);
                cmd.Parameters.AddWithValue("@FileType", file.ContentType);
                cmd.Parameters.AddWithValue("@FileSizeInBytes", file.Length);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            FileUploadResponse response =
                new FileUploadResponse
                {
                    FileGUID = fileGuid,
                    FileName = file.FileName,
                    RelativePath = savedFileName,
                    FileType = file.ContentType,
                    FileSizeInBytes = file.Length,
                    CreatedOn = DateTime.Now
                };

            return Ok(response);
        }

        [HttpGet]
        [Route("GetFiles")]
        public IActionResult GetFiles()
        {
            List<FileUploadResponse> files = new List<FileUploadResponse>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con =  new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM File_Master", con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    files.Add(new FileUploadResponse
                    {
                        FileGUID = Guid.Parse(dr["FileGUID"].ToString()),

                        FileName = dr["FileName"].ToString(),

                        RelativePath = dr["RelativePath"].ToString(),

                        FileType = dr["FileType"].ToString(),

                        FileSizeInBytes = Convert.ToInt64(dr["FileSizeInBytes"]),

                        CreatedOn = Convert.ToDateTime(dr["CreatedOn"])
                    });
                }
            }

            return Ok(files);
        }

        [HttpGet]
        [Route("DownloadFile/{fileGuid}")]
        public IActionResult DownloadFile(Guid fileGuid)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            string fileName = "";
            string fileType = "";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM File_Master WHERE FileGUID=@FileGUID", con);

                cmd.Parameters.AddWithValue("@FileGUID", fileGuid);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    fileName = dr["RelativePath"].ToString();

                    fileType = dr["FileType"].ToString();
                }
            }

            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, fileType, fileName);
        }

        [HttpGet]
        [Route("GetFileDetails/{fileGuid}")]
        public IActionResult GetFileDetails(Guid fileGuid)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con =new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM File_Master WHERE FileGUID=@FileGUID",con);

                cmd.Parameters.AddWithValue("@FileGUID", fileGuid);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return Ok(new
                    {
                        FileGUID = dr["FileGUID"].ToString(),
                        FileName = dr["FileName"].ToString(),
                        RelativePath = dr["RelativePath"].ToString(),
                        FileType = dr["FileType"].ToString(),
                        FileSizeInBytes = dr["FileSizeInBytes"].ToString(),

                        DownloadUrl =
                            $"{Request.Scheme}://{Request.Host}/api/EmployeeEducation/DownloadFile/{fileGuid}"
                    });
                }
            }

            return NotFound("File not found");
        }

        [HttpDelete]
        [Route("DeleteFile/{fileGuid}")]
        public IActionResult DeleteFile(Guid fileGuid)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            string fileName = "";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                //-------------------------------------------------
                // GET FILE NAME
                //-------------------------------------------------

                SqlCommand getCmd = new SqlCommand("SELECT RelativePath FROM File_Master WHERE FileGUID=@FileGUID",con);

                getCmd.Parameters.AddWithValue("@FileGUID", fileGuid);

                object result = getCmd.ExecuteScalar();

                if (result == null)
                {
                    return NotFound();
                }

                fileName = result.ToString();

                //-------------------------------------------------
                // DELETE DB RECORD
                //-------------------------------------------------

                SqlCommand deleteCmd = new SqlCommand("DELETE FROM File_Master WHERE FileGUID=@FileGUID",con);

                deleteCmd.Parameters.AddWithValue("@FileGUID", fileGuid);

                deleteCmd.ExecuteNonQuery();
            }

            //-------------------------------------------------
            // DELETE PHYSICAL FILE
            //-------------------------------------------------

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads",fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return Ok("File Deleted Successfully");
        }


    }
}