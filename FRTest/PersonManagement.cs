using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace FRTest
{
    public class PersonManagement
    {
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_STDXMLConfig(int iUserID, ref CHCNetSDK.NET_DVR_XML_CONFIG_INPUT lpInputParam, ref CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT lpOutputParam);

        public string XMLTransparent(string IpAddress, string UserName, string Password, ushort Port)
        {
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            struLoginInfo.bUseAsynLogin = false;
            struLoginInfo.sDeviceAddress = IpAddress;
            struLoginInfo.sUserName = UserName;
            struLoginInfo.sPassword = Password;
            struLoginInfo.wPort = Port;

            //string url = "GET /ISAPI/System/time";
            string url = "POST /ISAPI/AccessControl/UserInfo/Search?format=json";
            int userID = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);

            CHCNetSDK.NET_DVR_XML_CONFIG_INPUT pInputXml = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();
            Int32 nInSize = Marshal.SizeOf(pInputXml);
            pInputXml.dwSize = (uint)nInSize;

            // add request url
            string strRequestUrl = url;
            uint dwRequestUrlLen = (uint)strRequestUrl.Length;
            pInputXml.lpRequestUrl = Marshal.StringToHGlobalAnsi(strRequestUrl);
            pInputXml.dwRequestUrlLen = dwRequestUrlLen;
            //

            var userInfoSearchCond = new { searchID = "1", searchResultPosition = 0, maxResults = 100 };

            //string userInfo1 = JsonConvert.SerializeObject(userInfoSearchCond);
            var json = new { UserInfoSearchCond = userInfoSearchCond };
            string jsonStr = JsonConvert.SerializeObject(json); //convert object -> string

            Console.WriteLine(jsonStr);

            // add input parameters
            string strInputParam = jsonStr;

            pInputXml.lpInBuffer = Marshal.StringToHGlobalAnsi(strInputParam);
            pInputXml.dwInBufferSize = (uint)strInputParam.Length;
            //

            // reserve space for return data
            CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT pOutputXml = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();
            pOutputXml.dwSize = (uint)Marshal.SizeOf(pInputXml);
            pOutputXml.lpOutBuffer = Marshal.AllocHGlobal(3 * 1024 * 1024);
            pOutputXml.dwOutBufferSize = 3 * 1024 * 1024;
            pOutputXml.lpStatusBuffer = Marshal.AllocHGlobal(4096 * 4);
            //pOutputXml.dwStatusSize = 4096 * 4;
            //

            if (!NET_DVR_STDXMLConfig(userID, ref pInputXml, ref pOutputXml))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string strErr = "NET_DVR_STDXMLConfig failed, error code= " + iLastErr;
                // Failed to send XML data and output the error code
                Console.WriteLine(strErr);
            }

            string strOutputParam = Marshal.PtrToStringAnsi(pOutputXml.lpOutBuffer);
            //Console.WriteLine("Output param: {0}", strOutputParam); // display in json format in console
            string outXML = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(strOutputParam));
            //Console.WriteLine("XML Output:\n" + outXML);
            string outStatus = Marshal.PtrToStringAnsi(pOutputXml.lpStatusBuffer);
            //Console.WriteLine("Status Output:\n" + outStatus);

            dynamic json2 = JsonConvert.DeserializeObject(outXML);//conver string -> object
            //Console.WriteLine("response status str: {0}", json2.UserInfoSearch.responseStatusStrg);
            JArray UserInfoList = json2.UserInfoSearch.UserInfo;
            

            


            foreach (dynamic userInfo in UserInfoList)
            {
                //Console.WriteLine(userInfo);
                dynamic param = userInfo;
                Console.WriteLine("employeeNo: {0}", param.employeeNo);
                Console.WriteLine("employeeName: {0}", param.name);
                Console.WriteLine("employeeType: {0}", param.userType);
                dynamic param2 = param.Valid;
                Console.WriteLine("begiTime: {0}", param2.beginTime);

            
                Console.WriteLine("password: {0}", param.password);

            }

            Marshal.FreeHGlobal(pInputXml.lpRequestUrl);
            Marshal.FreeHGlobal(pOutputXml.lpOutBuffer);
            Marshal.FreeHGlobal(pOutputXml.lpStatusBuffer);

            return strOutputParam;

           

           
        }
    }
}
