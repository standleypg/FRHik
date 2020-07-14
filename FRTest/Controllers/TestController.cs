using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FRTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace FRTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {


        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_STDXMLConfig(int iUserID, ref CHCNetSDK.NET_DVR_XML_CONFIG_INPUT lpInputParam, ref CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT lpOutputParam);

        //private string IpAddress = "192.168.0.161"; //Home
        public string IpAddress = "10.17.103.133";//Office
        private string UserName = "admin";
        private string Password = "admin12345";
        private ushort Port = 8000;

       
        
        [Route("search")]
        [HttpPost]
        public ActionResult Ping()
        {

            InitiateDevice initDev = new InitiateDevice();
            initDev.Login();

            PersonManageTest personManageTest = new PersonManageTest();
             
            string str = personManageTest.SearchPerson(IpAddress, UserName, Password, Port);
           // string last = Convert.ToString(str);

            return Content(str);
            //initDev.Logout();
        }

        /**********************************************************Person Starts**********************************************************/
        
        [Route("add")]
        [HttpPost]
        public ActionResult<Person> AddPerson(Person todoItem)
        {
            InitiateDevice initDev = new InitiateDevice();
            initDev.Login();

            Dictionary<string, dynamic> Valid = new Dictionary<string, dynamic>();
            Valid.Add("enable", todoItem.userInfo.valid.enable);
            Valid.Add("beginTime", todoItem.userInfo.valid.beginTime);
            Valid.Add("endTime", todoItem.userInfo.valid.endTime);
            Valid.Add("timeType", todoItem.userInfo.valid.timeType);
            
            var userInfo = new
            {
                employeeNo = todoItem.userInfo.employeeNo,
                name = todoItem.userInfo.name,
                userType = todoItem.userInfo.userType,
                Valid = Valid,
                checkUser =  todoItem.userInfo.checkUser,
                addUser = todoItem.userInfo.addUser
            };

            var strJson = JsonConvert.SerializeObject(userInfo); //convert object -> string
            Console.WriteLine(strJson);

            PersonManageTest personManageTest = new PersonManageTest();
            personManageTest.AddPerson(IpAddress, UserName, Password, Port, userInfo);
            
            //personManageTest.AddPerson();

            return todoItem;
        }
        [Route("modify")]
        [HttpPut]
        public ActionResult<Person> EditPerson(Person todoItem)
        {
            InitiateDevice initDev = new InitiateDevice();
            initDev.Login();

            Dictionary<string, dynamic> Valid = new Dictionary<string, dynamic>();
            Valid.Add("enable", todoItem.userInfo.valid.enable);
            Valid.Add("beginTime", todoItem.userInfo.valid.beginTime);
            Valid.Add("endTime", todoItem.userInfo.valid.endTime);
            Valid.Add("timeType", todoItem.userInfo.valid.timeType);

            var userInfo = new
            {
                employeeNo = todoItem.userInfo.employeeNo,
                name = todoItem.userInfo.name,
                userType = todoItem.userInfo.userType,
                Valid = Valid,
                checkUser = todoItem.userInfo.checkUser,
                addUser = todoItem.userInfo.addUser
            };

            var strJson = JsonConvert.SerializeObject(userInfo); //convert object -> string
            Console.WriteLine(strJson);

            PersonManageTest personManageTest = new PersonManageTest();
            personManageTest.EditPerson(IpAddress, UserName, Password, Port, userInfo);
            //personManageTest.AddPerson();

            return todoItem;
        }

        /**********************************************************Person Ends**********************************************************/

        /**********************************************************Card Starts**********************************************************/


        [Route("card/search")]
        [HttpPost]
        public string SearchCard()
        {
            InitiateDevice initDev = new InitiateDevice();
            initDev.Login();

            CardManagement cardManagement = new CardManagement();
            string str = cardManagement.SearchCard(IpAddress, UserName, Password, Port);

            return str;
        }

        [Route("card/add")]
        [HttpPost]
        public ActionResult<Card> AddCard(Card card)
        {
            InitiateDevice initDev = new InitiateDevice();
            initDev.Login();

            var cardInfo = new
            {
                employeeNo = card.cardInfo.employeeNo,
                cardNo = card.cardInfo.cardNo,
                cardType = card.cardInfo.cardType
            };

            var strJson = JsonConvert.SerializeObject(cardInfo); //convert object -> string
            Console.WriteLine(strJson);


            CardManagement cardManagement = new CardManagement();
            string str = cardManagement.AddCard(IpAddress, UserName, Password, Port, cardInfo);

            //personManageTest.AddPerson();

            return card;


        }


        [Route("card/delete")]
        [HttpPost]
        public ActionResult<Card> DeleteCard(Card deleteCard)
        {
            InitiateDevice initDev = new InitiateDevice();
            initDev.Login();

            var cardInfo = new
            {
                employeeNo = deleteCard.cardInfo.employeeNo,
                cardNo = deleteCard.cardInfo.cardNo,
                cardType = deleteCard.cardInfo.cardType,
                deleteCard = deleteCard.cardInfo.deleteCard

            };

            var strJson = JsonConvert.SerializeObject(cardInfo); //convert object -> string
            Console.WriteLine(strJson);


            CardManagement cardManagement = new CardManagement();
            string str = cardManagement.DeleteCard(IpAddress, UserName, Password, Port, cardInfo);

            //personManageTest.AddPerson();

            return deleteCard;


        }
        /**********************************************************Card Ends**********************************************************/

        /**********************************************************Face Starts**********************************************************/

        [Route("face/ava")]
        [HttpPost]
        public void GetFaceAvailibity()
        {
            FaceManagement faceManagement = new FaceManagement();
            faceManagement.GetFaceAvailibility(IpAddress, UserName, Password, Port);
        }

        [Route("face/get_face")]
        [HttpPost]
        public void GetFace()
        {
            InitiateDevice initDev = new InitiateDevice();
            initDev.Login();

            FaceManagement faceManagement = new FaceManagement();
            faceManagement.GetFace(IpAddress, UserName, Password, Port);
        }

        [Route("face/set_face")]
        [HttpPost]
        public void SetFace()
        {
            InitiateDevice initDev = new InitiateDevice();
            initDev.Login();

            FaceManagement faceManagement = new FaceManagement();
            faceManagement.SetFace(IpAddress, UserName, Password, Port);
        }


        [Route("face/recog_mode")]
        [HttpPost]
        public void RecogMode()
        {
            InitiateDevice initDev = new InitiateDevice();
            initDev.Login();

            FaceManagement faceManagement = new FaceManagement();
            faceManagement.RecogMode(IpAddress, UserName, Password, Port);
        }




        /**********************************************************Face Ends**********************************************************/

    }
}
