using System;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using OfficialPSAS.Models;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace OfficialPSAS.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PSAS_Supervisor_ExpertController : ApiController
    {
        OfficialSASEntities31 db = new OfficialSASEntities31(); 
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
                Dictionary<string, object> allDetails = new Dictionary<string, object>();
                List<groupMembersDetails> allGroupMembers = new List<groupMembersDetails>();
                var request = db.projectRequests.Where(s => s.req_id == req_id && s.status==0).FirstOrDefault();
                if (request != null)
                {
                    // project details 
                    var project = db.Project.Where(s => s.pid == request.Project.pid && s.status == 0).Select(s => new
                    {
                        s.title,
                        s.teacher.tid,
                        s.teacher.users.username,                        
                    }).FirstOrDefault();
                    if (project != null)
                    {
                        allDetails["projectDetails"] = project;
                        var group = db.group.Where(s => s.gid == request.group.gid).FirstOrDefault();
                        if (group != null)
                        {
                            var groupDetails = new
                            {
                                group.gid,
                                group.avgCgpa,
                                group.session,
                                group.creatingDate,                                
                            };
                            allDetails["groupDetails"] = groupDetails;
                            var groupMembers = db.GroupMember.Where(s => s.group.gid == group.gid).Distinct().ToList();
                            if (groupMembers != null)
                            {                                
                                foreach(var member in groupMembers)
                                {
                                    groupMembersDetails gm = new groupMembersDetails();
                                    gm.st_id = member.st_id;
                                    gm.name = member.Student.users.username;
                                    gm.cgpa = (double)member.Student.cgpa;
                                    gm.technology = member.Technology.name;
                                    gm.grade = member.Student.Grade;
                                    gm.semester = member.Student.semester;
                                    gm.section = member.Student.section;
                                    allGroupMembers.Add(gm);
                                }
                                allDetails["groupMembers"] = allGroupMembers;
                                return Request.CreateResponse(allDetails);
                            }
                            else
                            {
                                return Request.CreateResponse("Group Members not founded");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse("group not founded");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("Project not Allocated");
                    }
                }
                else
                {
                    return Request.CreateResponse("Project Request not founded");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message+":"+cp.InnerException);
            }
        }
        /*----------------------------====================   Responding to the project Request      ========--------------------------------------------------*/
        [HttpPost]
        public HttpResponseMessage RespondingToRequest(int req_id,int status)
        {
            try
            {
                var request = db.projectRequests.Where(s => s.req_id == req_id && s.status==0).FirstOrDefault();
                if (request != null)
                {
                    // project details 
                    var project = db.Project.Where(s => s.pid == request.Project.pid && s.status == 0 && s.group.gid.Equals(null)).FirstOrDefault();
                    if (project != null)
                    {
                        var group = db.group.Where(s => s.gid == request.group.gid && (s.pid.Equals(null) || s.pid == 0 || s.tid.Equals(null) || s.tid == 0)).FirstOrDefault();
                        if (group != null)
                        {
                            // when the supevisor approves the request the project request status will change and the commettiee panel will decide 
                            // whether the project assign to the group or not...
                            if (status == 1)
                            {
                                // approving the status of project Request and then the commettiee panel will approve the project
                                request.status = 1;
                                db.SaveChanges();
                                return Request.CreateResponse("Project Request Approved");
                            }
                            else
                            {
                                request.status = -1;
                                db.SaveChanges();
                                return Request.CreateResponse("Project Request Rejected");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse("group not founded");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("project not founded");
                    }
                }
                else
                {
                    return Request.CreateResponse("Project Request not founded");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message+":"+cp.InnerException);
            }
        }
        /*-------------------------------------=========        Task Management    =========------------------------------------*/
        

        /*-------------------------------------=========      Allocated Projects Dropdown     =========------------------------------------*/
        [HttpGet]
        public HttpResponseMessage AllocatedGroups(int teacher_id)
        {
            try
            {
                var supervisor = db.teacher.Where(s => s.tid == teacher_id).FirstOrDefault();
                if (supervisor != null)
                {
                    var AllocatedGroups = db.SupervisorGroupConnection.Where(s => s.teacher.tid == supervisor.tid).Select(s => new
                    {
                        label = s.group.gid,
                        value = s.group.Project.Select(t => t.title).FirstOrDefault()
                    }).Distinct().ToList();
                    if (AllocatedGroups.Count > 0)
                    {
                        return Request.CreateResponse(AllocatedGroups);
                    }
                    else
                    {
                        return Request.CreateResponse("");
                    }
                }
                else
                {
                    return Request.CreateResponse("Supervisor Not Founded");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        /*-------------------------------===============   Assign new Task   =================--------------------------*/
        [HttpPost]
        public HttpResponseMessage AssignNewTask()
        {
            try
            {
                Task task = new Task();
                var formData = HttpContext.Current.Request;
                string title = formData.Form["title"];
                var dueDate = formData.Form["dueDate"];
                var postedFile = formData.Files["file"];
                string description = formData.Form["description"];
                // teacher , group finding 
                string group_id = formData.Form["group_id"];
                int gid = int.Parse(group_id);
                string teacher_id = formData.Form["teacher_id"];
                int teacherId = int.Parse(teacher_id);
                var teacher = db.teacher.Where(s => s.tid == teacherId).FirstOrDefault();
                if (teacher != null)
                {
                    // group finding
                    var group = db.group.Where(s => s.gid == gid).FirstOrDefault();
                    // checking group's supervisor
                    var checkingSupervisorOfGroup = db.SupervisorGroupConnection.Where(s => s.group.gid == group.gid && s.teacher.tid == teacher.tid).FirstOrDefault();
                    if (checkingSupervisorOfGroup != null)
                    {
                        if (group != null)
                        {
                            if (postedFile != null || postedFile.ContentLength != 0)
                            {
                                var filePath = HttpContext.Current.Server.MapPath("~/Content/Images/" + postedFile.FileName.Split('.')[0]);
                                postedFile.SaveAs(filePath);
                                task.filePath = postedFile.FileName.Split('.')[0];
                            }
                            else
                            {
                                task.filePath = null;
                                return Request.CreateResponse("No file uploaded");
                            }
                            if (title != null && dueDate != null && description != null)
                            {
                                task.Title = title;
                                task.DueDate = DateTime.Parse(dueDate);
                                task.description = description;
                                task.group = group;
                                db.Task.Add(task);
                                int RowsEffected = db.SaveChanges();

                                return Request.CreateResponse("New Task Added  " + RowsEffected);
                            }
                            else
                            {
                                return Request.CreateResponse("Fields must be filled");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse("group Not Founded");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("Group and teacher not Matched");
                    }
                }
                else
                {
                    return Request.CreateResponse("Teacher Not Founded");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }

    }
}
