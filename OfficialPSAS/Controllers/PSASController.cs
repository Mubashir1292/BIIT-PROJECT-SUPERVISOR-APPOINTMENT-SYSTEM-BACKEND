using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OfficialPSAS.Models;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http.Cors;
using System.Text;

namespace OfficialPSAS.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PSASController : ApiController
    {

        OfficialSASEntities28 db = new OfficialSASEntities28();
        /*public string  CheckForTechnicalExpert(string tid)
        {
            var teacherAndTechnicalExpertSame = from t in db.teacher
                                                join tx in db.TechnologyExpert
                                                on t.tid equals tx.teacher.tid
                                                select new { teacher = t, TechnologyExpert = tx };
            if (teacherAndTechnicalExpertSame != null)
            {
                var responseForTechnicalExpert = teacherAndTechnicalExpertSame.Select(s => s.TechnologyExpert).FirstOrDefault();
                if (responseForTechnicalExpert != null)
                {
                    return Request.CreateResponse("TecnicalExpert:Teacher");
                }
                else
                {
                    return Request.CreateResponse("just " + findingUser.role + findingUser.uid);
                }
            }
            else
            {
                return Request.CreateResponse("just " + findingUser.role + findingUser.uid);
            }
            return "";
        }*/
        [HttpGet]
        public HttpResponseMessage Login(string id,string password)
        {
            try
            {
                var findingUser = db.users.Where((u) => u.uid == id && u.password == password).Select(s => new
                {
                    s.uid,
                    s.username,
                    s.role
                }).FirstOrDefault();
                if (findingUser != null)
                {
                    if (findingUser.role == "student")
                    {
                        var findingStudent = db.Student.Where(s => s.st_id == findingUser.uid).FirstOrDefault();
                        var findingTechnology = db.GroupMember.Where(s => s.st_id == findingUser.uid).FirstOrDefault();
                        if (findingTechnology != null)
                        {
                            var response = new
                            {
                                findingTechnology.Student.users.uid,
                                findingTechnology.Student.cgpa,
                                findingTechnology.Technology.name,
                                findingTechnology.group.gid,
                                findingTechnology.Student.users.username,
                                findingUser.role,

                            };
                        return Request.CreateResponse(response);
                        }
                        else
                        {
                            var response = new
                            {
                                findingUser.uid,
                                findingStudent.cgpa,
                                findingUser.username,
                                findingUser.role,
                               
                            };
                            return Request.CreateResponse(response);
                        }
                    }
                    else if (findingUser.role == "teacher")
                    {
                        //var isTechnicalExpert = db.TechnologyExpert.Where(s => s.teacher==findingUser.teacher).FirstOrDefault();
                        // need a join here for checking is the same person is the technical Expert or not..
                        var isTeacherFounded = db.teacher.Where(s => s.users.uid == findingUser.uid).FirstOrDefault();
                        if(isTeacherFounded != null)
                        {
                            // set this material also..
                            var isTechnicalExpertFounded = db.TechnologyExpert.Where(s => s.teacher.tid == isTeacherFounded.tid).FirstOrDefault();
                            if (isTeacherFounded != null)
                            {
                                return Request.CreateResponse(findingUser);
                            }
                            else
                            {
                                return Request.CreateResponse(findingUser);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(findingUser);
                        }                       
                    }
                    else
                    {
                        return Request.CreateResponse(findingUser);
                    }
                }else
                {
                    return Request.CreateResponse("User Not Found "+id +","+password);
                }
            }
            catch (Exception cp) {
                return Request.CreateResponse("Login Failed. "+cp.Message +" "+cp.InnerException);
            }
        }
        /// Dashboard-----------------------------------------------------------
        [HttpGet]
        public HttpResponseMessage GetAllDuration()
        {
            try
            {
                    var options = new[]
                    {
                        new { label = "Today", value = "Today" },
                        new { label = "Last Week", value = "Last Week" },
                        new { label = "Last Month", value = "Last Month" },
                        new { label = "Last Six Month", value = "Last Six Month" }
                    }.ToList();
                return Request.CreateResponse(options);
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(cp.Message);
            }
        }
        /*------------------------------------Dashboard Api`s--------------------------------*/
        [HttpGet]
        public HttpResponseMessage getAllRequests(string Id)
        {
            try
            {
                var messages = (from message in db.groupRequests
                                join senderUser in db.users on message.users1.uid equals senderUser.uid
                                join receiverUser in db.users on message.users.uid equals receiverUser.uid
                                where senderUser.uid == Id
                                select new
                                {
                                    message.message_id,
                                    message.message_body,
                                    SenderId = new { 
                                        senderUser.uid ,
                                        senderUser.username
                                    },
                                    receiver = new{
                                    receiverUser.uid,
                                    receiverUser.username
                                    },
                                    message.datetime,
                                    message.status,
                                    message.Technology.name
                                }).Take(1000).ToList();




                return Request.CreateResponse(messages);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }



        /*-------------------------X-----------Dashboard Api`s--------------X------------------*/



        // Fetching the name and arid number of student for insertion screen for Personal Info Screen
        [HttpGet]
        public HttpResponseMessage FetchingTheNameAridNumber(string regNo)
        {
            try
            {
                var isStudentFinding = from u in db.users
                                       join st in db.Student on u.uid equals st.st_id
                                       where u.uid == st.st_id
                                       select new { Student = st, users = u };                         
                if (isStudentFinding != null)
                {                    
                    return Request.CreateResponse(isStudentFinding);
                }
                return Request.CreateResponse("id Did'nt Found "+regNo);
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        // techniloies getting for the dropdown 
        [HttpGet]
        public HttpResponseMessage FillingDropDown()
        {
            try
            {
                var Technologies = db.Technology.Select((t) =>new { 
                label=t.id,
                value=t.name}).ToList();
                if (Technologies==null)
                {
                    return Request.CreateResponse("Technologies not Founded");
                }
                return Request.CreateResponse(Technologies);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
/*        // Inserting the Group-Member Data
        [HttpPost]
        public HttpResponseMessage InsertingGroupMemberData()
        {
            try
            {
                var data = HttpContext.Current.Request;
                string id = data["id"];
                var isStudentFinding = from u in db.users
                                       join st in db.Student on u.uid equals st.st_id
                                       where u.uid == st.st_id
                                       select new { Student = st, users = u };
                if (isStudentFinding != null)
                {
                    return Request.CreateResponse(isStudentFinding);
                    string Semester = data["Semester"];
                    string Section = data["Section"];
                    string tech = data["technology"];
                }

                return Request.CreateResponse("");
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }*/
   
        /*Personal checking*/
    [HttpPost]
    public HttpResponseMessage PersonalDataInserting()
        {
            try
            {
                var data = HttpContext.Current.Request;
                Student student = new Student();
                string id = data["id"];
                string semester = data["semester"];
                string section = data["section"];
                string cgpa = data["cgpa"];
                string grade = data["grade"];
                var photo = data.Files["image"];
                string pName = photo.FileName.Split('.')[0];
                string fname = pName + "." + photo.FileName.Split('.')[1];
                photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/Images/" + fname));                
                var userFinding = db.users.Where((s) => s.uid == id).Select(s => s).FirstOrDefault();
                var studentFinding = db.Student.Where(s => s.st_id == id).Select(s => s).FirstOrDefault();
                if (studentFinding == null)
                {
                    if (userFinding != null)
                    {
                        student.st_id = userFinding.uid;
                        student.semester = semester;
                        student.section = section;
                        student.cgpa = float.Parse(cgpa);
                        student.Grade = grade;
                        student.users = userFinding;
                        student.image = photo.FileName;
                        db.Student.Add(student);
                    }
                    else
                    {
                        return Request.CreateResponse("User can't founded");
                    }
                    int RowsEffected = db.SaveChanges();
                    return Request.CreateResponse("Student data Saved " + RowsEffected);
                }
                else
                {
                    return Request.CreateResponse("Student Already Exists on Id "+id);
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message+":"+cp.InnerException);
            }
        }
        /*---------------------------------------------------------Checking the group Existense---------------------------*/
        [HttpGet]
        public HttpResponseMessage ChekingGroupExistence(string id)
        {
            try
            {
                var alreadyGroupOrMember = db.GroupMember.Where(s => s.st_id == id).Select(s => new
                {
                    s.st_id,
                    s.group.gid,
                    s.Technology,
                    s.group.avgCgpa,                    
                }).FirstOrDefault();
                if (alreadyGroupOrMember == null)
                {
                    return Request.CreateResponse(0);
                }
                return Request.CreateResponse("1");
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        /*-----------------------------------Creating new Group with one groupMember---------------------------*/
        [HttpPost]
        public HttpResponseMessage CreateNewGroup(string regNo,string title,string desc,string creatorTechnology)
        {
            try
            {
                {
                    group grp = new group();
                    GroupMember groupMem = new GroupMember();
                    var FindingUser = db.users.Where((s) => s.uid == regNo).FirstOrDefault();
                    if (FindingUser != null)
                    {
                        var studentFinding = db.Student.Where(s => s.st_id == regNo).FirstOrDefault();
                        if (studentFinding != null)
                        {
                            if (title != null && desc != null)
                            {
                                var isAlreadyCreatedGroup = db.group.Where(s => s.users.uid == FindingUser.uid).FirstOrDefault();
                                if (isAlreadyCreatedGroup == null)
                                {
                                    grp.users = FindingUser;
                                    grp.pid = null;
                                    grp.creatingDate = DateTime.Now;
                                    int month = DateTime.Now.Month;
                                    /*Checking the Spring and Fall for th group*/
                                    if (month >= 3 && month <= 5)
                                    {
                                        grp.session = "spring";
                                    }
                                    else if (month >= 6 && month <= 8)
                                    {
                                        grp.session = "summer";
                                    }
                                    else if (month >= 9 && month <= 11)
                                    {
                                        grp.session = "fall";
                                    }
                                    grp.tid = null;
                                    grp.title = title;
                                    grp.desc = desc;
                                    grp.avgCgpa = studentFinding.cgpa;
                                    db.group.Add(grp);
                                }
                                else
                                {
                                    return Request.CreateResponse("group already Exists");
                                }
                                /*finding technology from its table */

                                var technologyFinding = db.Technology.Where(s => s.name == creatorTechnology).FirstOrDefault();
                                if (technologyFinding != null)
                                {
                                    // insering the data for the creator group member 
                                    groupMem.Student = studentFinding;
                                    groupMem.group = grp;
                                    groupMem.Technology = technologyFinding;
                                    db.GroupMember.Add(groupMem);
                                }
                                else
                                {
                                    return Request.CreateResponse("technology not Exists");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse("Fields cannot be empty");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse("Student not Exists on id " + regNo);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("User not Exists on id " +regNo);
                    }
                }
                int RowsEffected = db.SaveChanges();
                return Request.CreateResponse("Group Created "+RowsEffected +" by "+regNo);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }

        }
        // inserting new group data and the group member data...

        
        /*checking the Student's Group Status*/
        [HttpGet]
        public HttpResponseMessage CheckingStudentGroupStatus(string regNo)
        {
            try
            {
                string statusOfSingleStudent="";                
                // finding user
                var findingUser = db.users.Where(s => s.uid == regNo).FirstOrDefault();
                if (findingUser != null)
                {
                    // checking if the user is student also
                    var studentChecking = db.Student.Where(s => s.st_id == findingUser.uid).FirstOrDefault();
                    if (studentChecking != null)
                    {
                        // find the student from the group member table                    
                        var groupMemberFinding = db.GroupMember.Where(s => s.st_id == studentChecking.st_id).FirstOrDefault();
                        if (groupMemberFinding == null)
                        {
                            statusOfSingleStudent = "0:"+studentChecking.users.username;
                        }
                        else
                        {
                            statusOfSingleStudent = "1:"+studentChecking.users.username;
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("Student not Exists");
                    }
                }
                else
                {
                    return Request.CreateResponse("User Not Exists");
                }
                return Request.CreateResponse(statusOfSingleStudent);
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        /*------------------------------Getting the Technologies other then the Creator Tehnology*/
        [HttpGet]
        public HttpResponseMessage GettingTechnolgiesOtherThenCreatorTechnology(string regNo)
        {
            try
            {
                var IsUser = db.users.Where(s => s.uid == regNo).FirstOrDefault();
                if (IsUser != null)
                {
                    var isStudentTechnology = db.Student.Where(s => s.st_id == regNo).Select(s => new { 
                        s.GroupMember.st_id,
                        s.GroupMember.Technology
                    }).FirstOrDefault();
                    if (isStudentTechnology != null)
                    {
                        var findingGroupId = db.GroupMember.Where(s => s.st_id == isStudentTechnology.st_id).Select(s => s.group.gid).FirstOrDefault();
                        if (findingGroupId != 0)
                        {
                            var allTechnologiesOfSameGroupMembers = db.GroupMember.Where(s=>s.group.gid == findingGroupId).Select(s => s.Technology.name ).ToList();
                            if (allTechnologiesOfSameGroupMembers.Count > 0)
                            {
                                var otherTechnologies = db.Technology
                                .Where(t => !allTechnologiesOfSameGroupMembers.Contains(t.name)).Select(t => new
                                {
                                    label = t.name,
                                    value = t.name,
                                })
                                .ToList();
                                if (otherTechnologies.Any())
                                {
                                    return Request.CreateResponse(otherTechnologies.Select(s => new { s.label, s.value }));
                                }
                                else
                                {
                                    return Request.CreateResponse("");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse("");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse("");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("");
                    }
                    }
                    else
                    {
                        return Request.CreateResponse("");
                    }
               
           }
            catch (Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }




/*        [HttpPost]
        public HttpResponseMessage Checkingtechnology()
        {
            try
            {
                var data = HttpContext.Current.Request;
                string senderid = data["senderId"];
                var remainingTechnologies = new object();
                var senderUser = db.users.Where((s) => s.uid == senderid).FirstOrDefault();
                if (senderUser != null)
                {
                   var AllTechnologies = db.Technology.Select(s => s).ToList();
                    if (AllTechnologies.Count != 0)
                    {
                        var senderGroup = db.group.Where(s => s.users.uid == senderUser.uid).FirstOrDefault();
                        if (senderGroup != null)
                        {
                            var checkingTechnologiesOfSendersGroupMember = db.GroupMember.Where(s => s.group == senderGroup).Select(s => s.Technology).ToList();
                            AllTechnologies.RemoveAll(tech => checkingTechnologiesOfSendersGroupMember.Contains(tech));
                            remainingTechnologies = AllTechnologies;
                        }
                    }
                }
                return Request.CreateResponse(remainingTechnologies);
            }
            catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
*/       
                                    /*Sending Request for the group Member*/
        [HttpPost]
        public HttpResponseMessage SendingRequestToMember(string senderid,string receiverId,string technology,string Message)
        {
            try
            {                
                groupRequests msg = new groupRequests();                
                var AllTechnologies = db.Technology.Select(s => s).ToList();
                DateTime currentDateTime = DateTime.Now;
                // Sender Student
                var senderUser = db.users.Where((s) => s.uid == senderid).FirstOrDefault();
                // Receiver Student
                var receiverUser = db.users.Where(s => s.uid == receiverId).FirstOrDefault();
                // now finding the groupMmeber
                if (receiverUser != null && senderUser != null)
                {
                    //checking sender's group 
                    var senderGroup = db.group.Where(s => s.users.uid == senderUser.uid).FirstOrDefault();
                    if (senderGroup != null)
                    {
                        // finding group of the sender 
                        var groupfindingOfSender = db.GroupMember.Where(s => s.st_id == senderUser.uid).Select(s => s.group.gid).FirstOrDefault();
                        if (groupfindingOfSender != 0)
                        {
                            var technologiesListOfSameGroupMembers = db.GroupMember.Where(s => s.group.gid == groupfindingOfSender).Select(s => s.Technology).ToList();
                            if (technologiesListOfSameGroupMembers.Count > 0)
                            {
                                var technologyFound = db.Technology.Where((s) => s.name == technology).FirstOrDefault();
                                if (!technologiesListOfSameGroupMembers.Contains(technologyFound))
                                {
                                    msg.Technology = technologyFound;
                                }
                                else
                                {
                                    return Request.CreateResponse("Choose another technology ");
                                }
                            }
                        }
                        if (Message != null)
                        {
                            // finding the duplication of the Request
                            var AlreadyfindingRequestStatus = db.groupRequests.Where(s => s.users.uid == receiverUser.uid && s.users1.uid == senderUser.uid).Select(s => s.status).FirstOrDefault();
                            if (AlreadyfindingRequestStatus == 2 || AlreadyfindingRequestStatus ==null)
                            {
                               /* Message body filling */
                                msg.users1 = senderUser;    
                                msg.users = receiverUser;
                                msg.datetime = currentDateTime;
                                msg.status = 0;
                                msg.message_body = Message;
                                db.groupRequests.Add(msg);
                            }
                            else if (AlreadyfindingRequestStatus == 0)
                            {
                                return Request.CreateResponse("Request is Already Posted <Pending>...");
                            }
                            else
                            {
                                return Request.CreateResponse("Can'nt Send Request to this member");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse("Message is not Null");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("Technology Already Taken choose Another one.");
                    }
                }
                else
                {
                    return Request.CreateResponse("not Founded Any Student");
                }
                int RowsEffected = db.SaveChanges();
                return Request.CreateResponse("Message Sent "+RowsEffected);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        //-----------------Sent Requests-----------------------------------------
        [HttpGet]
        public HttpResponseMessage GettingAllGroupRequests(string regNo)
        {
            try
            {
                var findingGroup = db.GroupMember.Where(s => s.st_id == regNo).FirstOrDefault();
                if (findingGroup != null)
                {
                    var gettingAllRequests = db.groupRequests
                        .Where(s => s.users1.uid == regNo)
                        .Select(s => new
                        {
                            s.message_id,
                            s.message_body,
                            sender = new
                            {
                                s.users1.uid,
                                s.users1.username
                            },
                            receiver = new
                            {
                                s.users.uid,
                                s.users.username
                            },
                            s.datetime,
                            s.status,
                            s.Technology.name,
                        })
                        .Distinct() // Ensure distinct records
                        .ToList();

                    if (gettingAllRequests.Count > 0)
                    {
                        return Request.CreateResponse(gettingAllRequests + ":" + findingGroup.group.gid);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Nothing in Messages");
                    }
                }
                else
                {
                    return Request.CreateResponse("Group not founded");
                }
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, cp.Message + ":" + cp.InnerException);
            }
        }


        // ------------------------Receiver Side---------------------------------------------------
        [HttpGet]
        public HttpResponseMessage ReceiverGettingAllRequests(string id)
        {
            try
            {
                var receiverStudent = db.Student.Where((s) => s.st_id == id).Select(s => s).FirstOrDefault();
                if (receiverStudent != null)
                {
                    var gettingAllRequests = db.groupRequests.Where(s => s.users.uid == receiverStudent.users.uid).Select(s => new
                    {
                        s.message_id,
                        s.message_body,
                        sender = new {
                            s.users1.uid,
                        },
                        receiver = new {
                            s.users.uid,                        
                        },
                        s.datetime,
                        s.status,
                        s.Technology.name,
                    }).ToList();
                    if(gettingAllRequests.Count > 0)
                    {
                        var parsingList = JsonConvert.SerializeObject(gettingAllRequests);
                        return Request.CreateResponse(HttpStatusCode.OK, parsingList);
                    }
                    else
                    {
                        return Request.CreateResponse("Nothing in Messages");
                    }
                }
                return Request.CreateResponse("not found the Message");
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        //-----------Just Message table getting------Receiver Side Single Request---------------------

        [HttpGet]
        public HttpResponseMessage ReceiverGettingSingleRequest(int rId)
        {
            try
            {
                var singleGroupRequest = db.groupRequests.Where(s => s.message_id == rId).Select(s => new
                {
                    message = new
                    {
                        s.message_id,
                        s.message_body,
                    },
                    s.status,
                    s.Technology.name,
                    sender = s.users1.uid,
                    receiver = s.users.uid,
                    dateTime=s.datetime
                }).FirstOrDefault();
                if(singleGroupRequest == null)
                {
                    return Request.CreateResponse("");
                }
                var parsing = JsonConvert.SerializeObject(singleGroupRequest);
                return Request.CreateResponse(parsing);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }

        //-----------------Receiver Side Single Request---------------------
        [HttpGet]
        public HttpResponseMessage FetchingSingleGroupRequest()
        {
            try
            {
                var data = HttpContext.Current.Request;                
                string receiverId = data["receiverId"];
                string messageId = data["messageId"];
                int msgId = int.Parse(messageId);
                var FetchingMessages = db.groupRequests.Where(s => s.users.uid == receiverId && s.message_id==msgId).Select(s => new
                {
                    sender = new
                    {
                        s.users1.uid,
                        s.users1.username
                    },
                    message = new
                    {
                        s.message_body,
                        s.Technology.id
                    },
                }).FirstOrDefault();
                if (FetchingMessages != null)
                {
                    var UserAndStudentFinding = from u in db.users
                                                join st in db.Student
                                                on u.uid equals st.users.uid
                                                where st.users.uid == FetchingMessages.sender.uid
                                                join g in db.@group on u.uid equals g.users.uid
                                                join tch in db.Technology on FetchingMessages.message.id equals tch.id
                                                select new
                                                {
                                                    Student = new
                                                    {
                                                        st.semester,
                                                        st.section,
                                                    },
                                                    Sender = new
                                                    {
                                                        u.uid,
                                                        u.username,
                                                    },
                                                    groupDetails = new
                                                    {
                                                        g.title,
                                                        g.gid,
                                                        g.session,
                                                    },
                                                    technologyOffered=new
                                                    {
                                                        tch.name,
                                                    }
                                                };
                      
                    return Request.CreateResponse(UserAndStudentFinding);
                }
                else
                {
                return Request.CreateResponse("not founded");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message +":"+cp.InnerException);
            }
        }
        [HttpPost]
        public HttpResponseMessage RespondingBackToRequest()
        {
            try
            {
                var data = HttpContext.Current.Request;
                string msgId = data["mId"];
                string response = data["response"];
                int intMsgId = int.Parse(msgId);
                var isMessageFound = db.groupRequests.Where(s => s.message_id == intMsgId).FirstOrDefault();
                if (isMessageFound != null)
                {
                    if (response == "Accept" || response=="accept")
                    {
                        isMessageFound.status = 1;
                        CheckingResponseOfRequest(isMessageFound.users1.uid,isMessageFound.message_id,isMessageFound.users.uid,1);
                    }
                    else if(response == "Reject")
                    {
                        isMessageFound.status = 2;
                    }
                }
                else
                {
                    return Request.CreateResponse("Not Found any Message with id :" + intMsgId);
                }
                int RowsEffected = db.SaveChanges();
                return Request.CreateResponse("Status Updated " + RowsEffected);
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        [HttpPost]
        public HttpResponseMessage CheckingResponseOfRequest(string senderId,int msgId,string receiverId,int status)
        {
            try
            {                
                float sum = 0, avg = 0;
                var data = HttpContext.Current.Request;
                if (data != null)
                {
                    GroupMember gm = new GroupMember();
                    var messageFinding = db.groupRequests.Where(s => s.users.uid == receiverId && s.users1.uid == senderId && s.message_id==msgId).Select(s => new
                    {
                        response = new
                        {
                            status
                        },
                        message = new
                        {
                            s.message_id,
                            s.message_body,
                            s.Technology.id
                        },
                        dateTime = new
                        {
                            sentTime = s.datetime,
                            responseTime = DateTime.Now
                        },
                        sender = new
                        {
                            s.users1.uid,
                            s.users1.username
                        },
                        receiver = new
                        {
                            s.users.uid,
                            s.users.username,                           
                        }
                    }).FirstOrDefault();
                    if (messageFinding != null)
                    {
                        // finding the groupmember
                        var GroupFinding = db.group.Where((s) => s.users.uid == messageFinding.sender.uid).Select(s =>s).FirstOrDefault();
                        if (GroupFinding != null)
                        {
                            var listOfGroupMembersOfSameGroup = db.GroupMember.Where(s => s.group.gid == GroupFinding.gid).ToList();                            
                            if (listOfGroupMembersOfSameGroup.Count < 5)
                            {
                                
                                if (messageFinding.receiver.uid != null && messageFinding.message.id != 0)
                                {
                                    var TechnologyItem = db.Technology.Where((s) => s.id == messageFinding.message.id).FirstOrDefault();
                                    if (TechnologyItem != null)
                                    {
                                        if (status == 1)
                                        {
                                            var findingStudent = db.Student.Where(s => s.st_id == messageFinding.receiver.uid).FirstOrDefault();
                                            //gm.st_id = findingStudent.st_id;
                                            gm.Student = findingStudent;
                                            gm.group = GroupFinding;
                                            gm.Technology = TechnologyItem;
                                            db.GroupMember.Add(gm);                                           
                                            listOfGroupMembersOfSameGroup.Add(gm);
                                            //-------Calculating the avgCGPA
                                            foreach (var i in listOfGroupMembersOfSameGroup)
                                            {
                                                sum += (float)i.Student.cgpa;
                                                avg = sum / listOfGroupMembersOfSameGroup.Count;
                                            }
                                            GroupFinding.avgCgpa = avg;
                                            int RowsEffected = db.SaveChanges();
                                            var responseToReturn = new
                                            {
                                                message = "Success",
                                                avgCgpa = avg,
                                                group=GroupFinding.gid,
                                                newMember=messageFinding.receiver.uid,
                                                technologyAdded=messageFinding.message.id,
                                            };
                                            return Request.CreateResponse(responseToReturn);
                                        }
                                        else if(messageFinding.response.status==2)
                                        {
                                            return Request.CreateResponse("Group Member not Agree");
                                        }else
                                        {
                                            return Request.CreateResponse("Request Pending...");
                                        }
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse("Null Error");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse("maximum 5 members are allowed in a group");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse("Group Member not found");
                        }                        
                    }
                }
                return Request.CreateResponse("Not Added");
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.InnerException + ":" + cp.Message);
            }
        }
            // now to check the group member length 
        [HttpGet]
        public HttpResponseMessage MyGroup(string regNo)
        {
            try
            {
                var checkingStudent = db.Student.Where(s => s.st_id == regNo).FirstOrDefault();
                if(checkingStudent != null)
                {
                    var findingGroup = db.GroupMember.Where(s => s.st_id == regNo).Select(s => s.group.gid).FirstOrDefault();
                    if (findingGroup != 0)
                    {
                        var findingGroupMembers = db.GroupMember.Where(s => s.group.gid == findingGroup).Select(s => new
                        {
                            member = new
                            {
                                s.st_id,
                                s.Student.users.username,
                                s.Technology.name,
                                s.group.title,
                                s.group.gid,
                                s.group.avgCgpa,
                            },
                        }).Distinct().ToList();
                        if(findingGroupMembers.Count > 0)
                        {
                            //var parsing = JsonConvert.SerializeObject(findingGroupMembers);
                            return Request.CreateResponse(findingGroupMembers);
                        }
                        else
                        {
                            return Request.CreateResponse("Cannot found any group-Member");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("Cannot found any group");
                    }
                }
                else
                {
                    return Request.CreateResponse("Cannot found any Student");
                }

            }
            catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        // fetching the group cgpa by providing the cgpa
        [HttpGet]
        public HttpResponseMessage GroupCgpaByRegNo(string regNo)
        {
            try
            {
                var findingAvgCgpaOfGroup = db.group.Where(s => s.users.uid == regNo).Select(s => s.avgCgpa).FirstOrDefault();
                if (findingAvgCgpaOfGroup != 0)
                {
                    return Request.CreateResponse(findingAvgCgpaOfGroup);
                }
                else
                    return Request.CreateResponse("");                
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }

        /*[HttpGet]
        public HttpResponseMessage CheckingTheGroupMembersQuantityAndAverageCgpa()
            {
                try
                {
                    var data = HttpContext.Current.Request;
                    string senderId = data["senderId"];
                    float avg=0, sum = 0;
                    var isUser = db.users.Where(s => s.uid == senderId).FirstOrDefault();
                    if (isUser != null)
                    {
                        var groupFinding = db.group.Where(s => s.users.uid == isUser.uid).FirstOrDefault();
                        if (groupFinding != null)
                        {
                            var groupMembersFinding = db.GroupMember.Where(s => s.group.gid == groupFinding.gid).Select(s=>s.st_id).ToList();
                            if (groupMembersFinding.Count >= 3)
                            {
                            // Calculating the Average of the cgpa
                            var AllCgpas = db.Student.Where(s => groupMembersFinding.Contains(s.st_id)).Select(s => s.cgpa).ToList();
                            if(AllCgpas.Count > 0)
                            {
                                foreach(float i in AllCgpas)
                                {
                                    sum += i;
                                }
                                avg = sum / AllCgpas.Count;
                            }                                                                               
                                return Request.CreateResponse("Navigate to Dashboard with Avg Cgpa is "+avg);
                            }
                            else
                            {
                                return Request.CreateResponse("Group Member must be 3 or more");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse("group not found");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("User not founded");
                    }
                }catch(Exception cp)
                {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
                }
            }
*/
                /* Filling the Project domains */
        [HttpGet]
        public HttpResponseMessage AllDomains()
        {
            try
            {
                var AllDomains = db.projectDomain.Select(s => new { 
                label=s.pd_Id,
                value=s.name}).ToList();
                if (AllDomains.Count != 0)
                {
                    return Request.CreateResponse(AllDomains);
                }
                else
                {
                    return Request.CreateResponse("Domains not found");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        /*Filter Projects using the cgpa list*/
        [HttpGet]
        public HttpResponseMessage ProjectsByDomain(string Domain,string regNo)
        {
            try
            {
                // finding group and groupMembers Quantity
                var findingGroup = db.GroupMember.Where(s => s.st_id == regNo).Select(s => s.group.gid).FirstOrDefault();
                if (findingGroup != 0)
                {
                    var groupfinding = db.GroupMember.Where(s => s.group.gid == findingGroup).ToList();
                    if (groupfinding.Count >= 3)
                    {
                        var group = db.group.Where(s => s.gid == findingGroup).FirstOrDefault();
                        var findingIdOfDomain = db.projectDomain.Where(s => s.name == Domain).Select(s => s.pd_Id).FirstOrDefault();
                            if (findingIdOfDomain != 0)
                            {
                                var ProjectsList = db.Project.Where(s => s.projectDomain.pd_Id == findingIdOfDomain && s.thresholdCgpa<=group.avgCgpa && s.status==0).Select(s => new
                                {
                                    s.pid,
                                    s.title,
                                    s.description,
                                    s.teacher.users.username,
                                    findingGroup
                                }).ToList();
                                if (ProjectsList != null)
                                {
                                    return Request.CreateResponse(ProjectsList);
                                }
                                else
                                {
                                    return Request.CreateResponse("Not Founded");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse("not founded any project on this domain");
                            }
                    }
                    else
                    {
                        return Request.CreateResponse("Minimum 3 group Members");
                    }
                }
                else
                {
                    return Request.CreateResponse("Group not found");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        //----- Requesting Projects--- 
        [HttpGet]
        public HttpResponseMessage DetailsProjectSupervisorGroup(int projectId,string regNo)
        {
            try
            {
                List<groupDetailsFor> singleGroupDetails = new List<groupDetailsFor>();
                var findingGroup = db.GroupMember.Where(s => s.st_id == regNo).Select(s => s.group.gid).FirstOrDefault();
                if (findingGroup != 0)
                {
                    var groupfinding = db.GroupMember.Where(s => s.group.gid == findingGroup).ToList();
                    if (groupfinding.Count >= 3)
                    {
                        foreach(var i in groupfinding)
                        {
                            groupDetailsFor gdf = new groupDetailsFor();
                            gdf.name = i.Student.users.username;
                            gdf.regNo = i.st_id;
                            gdf.technology = i.Technology.name;
                            singleGroupDetails.Add(gdf);
                        }
                        var projectFinding = db.Project.Where(s => s.pid == projectId).Select(s => new
                        {
                            s.title,
                            teacher = new
                            {
                                s.teacher.tid,
                                s.teacher.users.username,
                            },
                        }).FirstOrDefault();
                        if (projectFinding != null)
                        {
                            var response = new
                            {
                                projectDetails=new
                                {
                                    projectFinding.title,
                                    projectFinding.teacher
                                },
                                singleGroupDetails
                            };
                            return Request.CreateResponse(response);
                        }
                        else
                        {
                            return Request.CreateResponse("project not founded");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("Group Must have 3  Members");
                    }
                }
                else
                {
                    return Request.CreateResponse("Group not Founded");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
            
        
                        /// <summary>
                        /// Requesting to the Supervisor For Project
                        /// </summary>
                        /// <returns> This Api will get the group_id,project-id and the teacher-id</returns>
        [HttpPost]
        public HttpResponseMessage PostingRequesttoSupervisor(int projectId,string regNo)
        {
            try
            {
                projectRequests pr = new projectRequests();
                var findingProject = db.Project.Where(s => s.pid == projectId).FirstOrDefault();
                if (findingProject != null)
                {
                    var findingGroup = db.GroupMember.Where(s => s.st_id == regNo).Select(s => s.group).FirstOrDefault();
                    if (findingGroup != null)
                    {
                        var groupMembersList = db.GroupMember.Where(s => s.group.gid == findingGroup.gid).ToList();
                        if (groupMembersList.Count >= 3)
                        {
                            pr.group = findingGroup;
                            pr.teacher = findingProject.teacher;
                            pr.Project = findingProject;
                            pr.status = 0;
                            pr.req_date = DateTime.Now;
                            db.projectRequests.Add(pr);
                            db.SaveChanges();
                            return Request.CreateResponse("Posted the Request for the project");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound,"Group Must abe 3 members");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound,"group not founded");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,"Project not founded");
                }
            }
            catch(Exception cp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,cp.Message + ":" + cp.InnerException);
            }
        }

        // ------------ help Request------------------
        [HttpGet]
        public HttpResponseMessage GetAllDays()
        {
            try
            {
                List<string> DaysOfWeek = new List<string>()
                     {
                        "Monday",
                        "Tuesday",
                        "Wednesday",
                        "Thursday",
                        "Friday",
                      };
                return Request.CreateResponse(DaysOfWeek);
            }catch(Exception cp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,cp.Message + ":" + cp.InnerException);
            }
        }
        //Fetching the Technology on the arid Number
        [HttpGet]
        public HttpResponseMessage GetTechnology(string regNo)
        {
            try
            {
                var findingTechnology = db.Student.Where(s => s.st_id == regNo).Select(s => new
                {
                    s.GroupMember.Technology.id,
                    s.GroupMember.Technology.name,                    
                }).FirstOrDefault();
                if (findingTechnology != null)
                {
                    return Request.CreateResponse(findingTechnology);
                }
                else
                    return Request.CreateResponse("");
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        // Fetching the technicalExperts from the TechnicalExpertTechnology table with multiple technologies
        [HttpGet]
        public HttpResponseMessage FetchingRelativeTechnicalExpert(string regNo)
        {
            try
            {
                var studentFinding = db.Student.Where(s => s.st_id == regNo).FirstOrDefault();
                if(studentFinding != null)
                {
                    // groupMember
                    var technologyOfStudent = db.GroupMember.Where(s => s.st_id == regNo).Select(s => s.Technology).FirstOrDefault();
                    if (technologyOfStudent != null)
                    {
                        var findingTechnicalExperts = db.TechnicalExpertTechnology.Where(s => s.Technology.id == technologyOfStudent.id).Select(s => new
                        {
                           studentTechnology= technologyOfStudent.name,
                            s.id,
                            teacher = new
                            {
                                s.TechnologyExpert.teacher.tid,                                
                                s.Technology.name,
                                s.TechnologyExpert.users.username,
                            },
                        }).ToList();
                        if (findingTechnicalExperts.Count > 0)
                        {
                            return Request.CreateResponse(findingTechnicalExperts);
                        }
                        else
                        {
                            return Request.CreateResponse("");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("technology not found");
                    }
                }
                else
                {
                    return Request.CreateResponse("not found any student record");
                }
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, cp.Message + ":" + cp.InnerException);
            }
        }
        // fetching the timeSlots on related day from the dropdown
        [HttpGet]
        public HttpResponseMessage GetTheTimeSlots(string day,int teacher_id)
        {
            try
            {
                var findingTechnicalExpert = db.TechnicalExpertTechnology.Where(s => s.TechnologyExpert.teacher.tid == teacher_id).Select(s => s.TechnologyExpert.teacher.tid).FirstOrDefault();
                var findingTheDaysAndTimeSlots = db.Schedule.Where(s => s.teacher.tid == teacher_id && s.Day == day && s.status == 0).Select(s => new
                {
                    s.TimeSlots.id,
                    s.TimeSlots.start_time,
                    s.TimeSlots.end_time,
                    s.Sch_id
                }).ToList();
                if(findingTheDaysAndTimeSlots.Count > 0)
                {
                    return Request.CreateResponse(findingTheDaysAndTimeSlots);
                }
                else
                {
                    return Request.CreateResponse("");
                }
            }
            catch (Exception cp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, cp.Message + ":" + cp.InnerException);
            }
        }
        // posting the Request
        [HttpPost]
        public HttpResponseMessage PostingHelpRequest(int teacher_id,string regNo,int sch_Id,string message)
        {
            try
            {
                AppointmentRequests ar = new AppointmentRequests();
                var findingStudent = db.Student.Where(s => s.st_id == regNo).FirstOrDefault();
                var teacherFinding = db.teacher.Where(s => s.tid == teacher_id).Select(s => s.users).FirstOrDefault();
                if (teacherFinding != null)
                {
                    var ScheduleFinding = db.Schedule.Where(s => s.Sch_id == sch_Id).FirstOrDefault();
                    if(ScheduleFinding != null)
                    {
                        ar.users = teacherFinding;
                        ar.RequestedBy = findingStudent.st_id;
                        ar.Schedule = ScheduleFinding;
                        ar.status = 0;
                        ar.message = message;
                        db.AppointmentRequests.Add(ar);
                    }
                    else
                    {
                        return Request.CreateResponse("");
                    }
                }
                db.SaveChanges();
                return Request.CreateResponse("set the Appointment");
            }
            catch(Exception cp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,cp.Message+":"+cp.InnerException);
            }
        }

        //------------------------------------------------- Joining Group Request ---------------------------------------------
        // finding groups which dont have that particular technology
        [HttpGet]
        public HttpResponseMessage GroupsFetching(string techName,string regNo)
        {
            try
            {
                List<ProjectList> plist = new List<ProjectList>();
                var technologyFinding = db.Technology.Where(s => s.name == techName).FirstOrDefault();
                if (technologyFinding != null)
                {
                    // finding Student's cgpa 
                    var studentCgpa = db.GroupMember.Where(s => s.Student.st_id == regNo).Select(s => s.Student.cgpa).FirstOrDefault();
                    var findingProjectsWithSimilarCgpa = db.Project.Where(s => s.thresholdCgpa <= studentCgpa && s.status == 1 && s.group.gid != 0).Select(s => new
                    {
                        s.teacher.tid,
                        s.teacher.users.username,
                        s.title,
                        s.pid,
                        s.description,
                        s.projectDomain.name,
                        s.group.gid
                    }).ToList();
                    if (findingProjectsWithSimilarCgpa.Count > 0)
                    {

                        foreach (var i in findingProjectsWithSimilarCgpa)
                        {
                            var findingGroupMemberTechnologies = db.GroupMember.Where(s => s.group.gid == i.gid).Select(s => s.Technology.name).ToList();
                            if (!findingGroupMemberTechnologies.Contains(technologyFinding.name))
                            {
                                ProjectList pl = new ProjectList();
                                pl.tid = i.tid;
                                pl.username = i.username;
                                pl.title = i.title;
                                pl.pid = i.pid;
                                pl.description = i.description;
                                pl.projectName = i.name;
                                pl.gid = i.gid;
                                plist.Add(pl);
                            }                            
                        }
                        if (plist.Count > 0)
                        {
                           return Request.CreateResponse(plist);
                        }
                        else
                        {
                            return Request.CreateResponse("not Founded");
                        }
                    }
                    else
                        return Request.CreateResponse("");
                }
                else
                    return Request.CreateResponse("");
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }
        //Fetching the Group and supervisor and title of project by using the ids 
        [HttpGet]
        public HttpResponseMessage GetTheDetails(int group_id,int project_id)
        {
            try
            {
                List<groupDetailsFor> groupDetails = new List<groupDetailsFor>();
                var findingRequriedDetails = db.GroupMember.Where(s => s.group.gid == group_id).ToList();
                if (findingRequriedDetails.Count > 0)
                {
                    foreach(var i in findingRequriedDetails)
                    {
                        groupDetailsFor grf = new groupDetailsFor();
                        grf.name = i.Student.users.username;
                        grf.regNo = i.st_id;
                        grf.technology = i.Technology.name;
                        groupDetails.Add(grf);
                    }
                    var findingProject = db.Project.Where(s => s.pid == project_id).Select(s => new
                    {
                        project = new
                        {
                            s.pid,
                            s.title,
                        },
                        teacher = new
                        {
                            s.teacher.users.username,
                            s.teacher.tid,
                        },
                    }).FirstOrDefault();
                    if (findingProject != null)
                    {
                        var response = new
                        {
                            findingProject,
                            groupDetails
                        };
                         return Request.CreateResponse(response);
                    }
                    else
                    {
                        return Request.CreateResponse("");
                    }
                }
                else
                {
                    return Request.CreateResponse("");
                }

            }
            catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message+":"+cp.InnerException);
            }
        }
       
        // posting the Request for joining 
        [HttpPost]
        public HttpResponseMessage PostingRequestForGroupJoining(string regNo,int gid,int tecId,string message)
        {
            try
            {
                groupRequests gr = new groupRequests();
                var findingStudent = db.Student.Where(s => s.st_id == regNo).Select(s => s.users).FirstOrDefault();
                if(findingStudent != null)
                {
                    var technologyFinding = db.Technology.Where(s => s.id == tecId).FirstOrDefault();
                    var findingAdmin = db.users.Where(s => s.uid == "8").FirstOrDefault();
                    var checkingGroupExists = db.GroupMember.Where(s => s.st_id == regNo).FirstOrDefault();
                    if (checkingGroupExists == null)
                    {
                       var groupFinding = db.group.Where(s => s.gid == gid).FirstOrDefault();
                        if (groupFinding != null)
                        {
                            gr.group = groupFinding;
                            gr.message_body = message;
                            gr.users = findingStudent;
                            gr.users1 = findingAdmin;
                            gr.status = 0;
                            gr.Technology = technologyFinding;
                            gr.datetime = DateTime.Now;
                            db.groupRequests.Add(gr);
                            db.SaveChanges();
                            return Request.CreateResponse("Requested to join group");
                        }
                        else
                        {
                            return Request.CreateResponse("Group Not Founded");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse("You are joined with a group");
                    }
                }
                else
                {
                    return Request.CreateResponse("Student not founded");
                }
            }catch(Exception cp)
            {
                return Request.CreateResponse(cp.Message + ":" + cp.InnerException);
            }
        }






        //--------------------------Commettiee Panel
        [HttpGet]
        public IHttpActionResult GetNotification()
        {

            db.Configuration.ProxyCreationEnabled = false;

            try
            {

                var notification = db.Project.Where(item => item.status == 0 && !String.IsNullOrEmpty(item.users.uid)).Select(s=>new {
                 s.users.uid,
                 s.users.username,
                 s.title,                
                }).ToList();                               
                return Ok(notification);
            }
            catch (Exception cp)
            {
                return Ok(cp.Message);
            }
        }



    }
}



