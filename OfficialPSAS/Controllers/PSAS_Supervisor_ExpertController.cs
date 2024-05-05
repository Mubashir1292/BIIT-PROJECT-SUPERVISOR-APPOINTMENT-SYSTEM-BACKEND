using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OfficialPSAS.Models;
using System.Web.Http.Cors;

namespace OfficialPSAS.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PSAS_Supervisor_ExpertController : ApiController
    {
        OfficialSASEntities30 db = new OfficialSASEntities30(); 
                /*-------------------====================   Get All Notifications of relative supervisor    ==========-----------------------*/
        [HttpGet]
        public HttpResponseMessage GetAllNotifications(int teacher_id)
        {
            try
            {
                // get requests for the Supervisor of the groups 
                var teacher = db.teacher.Where(s => s.tid == teacher_id).FirstOrDefault();
                if (teacher != null)
                {
                    // all Project Requests
                    var allRequests = db.projectRequests.Where(s => s.teacher.tid == teacher.tid && s.status==0).Select(request => new
                    {
                        requestDetails= new
                        {
                            request.req_id,
                            request.req_date,
                        },
                        projectDetails = new
                        {
                            request.group.gid,
                            request.Project.pid,
                            request.Project.title,
                            request.group.avgCgpa,
                        }
                    }).Distinct().ToList();
                    if (allRequests.Count > 0)
                    {
                        return Request.CreateResponse(allRequests);
                    }
                    else
                    {
                        return Request.CreateResponse("No Project Requests Founded");
                    }
                }
                else
                {
                    return Request.CreateResponse("Teacher Not Founded");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message+":"+cp.InnerException);
            }
        }
        /*-------------------====================   Get All Notifications of relative supervisor    ==========-----------------------*/
        [HttpGet]
        public HttpResponseMessage SingleSupervisingRequest(int req_id)
        {
            try
            {                
                var request = db.projectRequests.Where(s => s.req_id == req_id).FirstOrDefault();
                if (request != null)
                {
                            
                }
                else
                {
                    return Request.CreateResponse("");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message+":"+cp.InnerException);
            }
        }
    }
}
