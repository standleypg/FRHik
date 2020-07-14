using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FRTest
{

    public class FaceManagement
    {
        public CHCNetSDK.NET_DVR_USER_LOGIN_INFO nET_DVR_USER_LOGIN_INFO = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
        public CHCNetSDK.NET_DVR_DEVICEINFO_V40 nET_DVR_DEVICEINFO_V40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
       
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_STDXMLConfig(int iUserID, ref CHCNetSDK.NET_DVR_XML_CONFIG_INPUT lpInputParam, ref CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT lpOutputParam);
        [DllImportAttribute(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetNextRemoteConfig(int lHandle, ref NET_DVR_CAPTURE_FACE_CFG lpOutBuff, int dwOutBuffSize);
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDeviceAbility(int lUserID, uint dwAbilityType, IntPtr pInBuf, uint dwInLength, IntPtr pOutBuf, uint dwOutLengt);

        public delegate void RemoteConfigCallback(uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData);
        [DllImportAttribute(@"HCNetSDK.dll")]
        public static extern int NET_DVR_StartRemoteConfig(int lUserID, uint dwCommand, IntPtr lpInBuffer, Int32 dwInBufferLen, RemoteConfigCallback cbStateCallback, IntPtr pUserData);


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CAPTURE_FACE_CFG
        {
            public int dwSize;
            public int dwFaceTemplate1Size;
            public IntPtr pFaceTemplate1Buffer;
            public int dwFaceTemplate2Size;
            public IntPtr pFaceTemplate2Buffer; 
            public int dwFacePicSize;
            public IntPtr pFacePicBuffer;
            public byte byFaceQuality1;
            public byte byFaceQuality2;
            public byte byCaptureProgress;    
            public byte byRes1;
            public int dwInfraredFacePicSize;   
            public IntPtr pInfraredFacePicBuffer;     
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 116, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
         
        }

        public void Auth(string IpAddress, string UserName, string Password, ushort Port)
        {

            nET_DVR_USER_LOGIN_INFO.sDeviceAddress = IpAddress;
            nET_DVR_USER_LOGIN_INFO.wPort = Port;
            nET_DVR_USER_LOGIN_INFO.sUserName = UserName;
            nET_DVR_USER_LOGIN_INFO.sPassword = Password;
            nET_DVR_USER_LOGIN_INFO.bUseAsynLogin = false;
        }
        public void GetFaceAvailibility(string IpAddress, string UserName, string Password, ushort Port)
        {
            
            Auth(IpAddress, UserName, Password, Port)
;
            nET_DVR_DEVICEINFO_V40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];
            int userID = CHCNetSDK.NET_DVR_Login_V40(ref nET_DVR_USER_LOGIN_INFO, ref nET_DVR_DEVICEINFO_V40);

            string XML_Desc_AcsAbility = new string(new char[2048]);
            XML_Desc_AcsAbility = "<AcsAbility version=\"2.0\">	<!--opt, specify child nodes about access control capabilities to be returned-->	< / AcsAbility>";
            IntPtr pInBuf = (IntPtr)Marshal.StringToHGlobalAnsi(XML_Desc_AcsAbility);
            uint dwInLength = 2048;

            string XML_AcsAbility = new string(new char[65536]);
            IntPtr pOutBuf = (IntPtr)Marshal.StringToHGlobalAnsi(XML_AcsAbility);
            uint dwOutLength = 65536;


            if (NET_DVR_GetDeviceAbility(userID, CHCNetSDK.ACS_ABILITY, pInBuf, dwInLength, pOutBuf, dwOutLength) == true)
            {
                Console.WriteLine("success");
                string stringB = Marshal.PtrToStringAnsi(pOutBuf);
                Console.WriteLine(stringB);
            }
            else
            {
                //call NET_DVR_GetLastError to get the error code
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string strErr = "NET_DVR_STDXMLConfig failed, error code= " + iLastErr;
                Console.WriteLine(strErr);
            }
           
            Marshal.FreeHGlobal(pInBuf);
            Marshal.FreeHGlobal(pOutBuf);
           

        }

        public void GetFaceLib(string IpAddress, string UserName, string Password, ushort Port)
        {
            Auth(IpAddress, UserName, Password, Port);

            nET_DVR_DEVICEINFO_V40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            int userID = CHCNetSDK.NET_DVR_Login_V40(ref nET_DVR_USER_LOGIN_INFO, ref nET_DVR_DEVICEINFO_V40);

            string url = "GET /ISAPI/Intelligent/FDLib?format=json";

            CHCNetSDK.NET_DVR_XML_CONFIG_INPUT nET_DVR_XML_CONFIG_INPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();

            Int32 nInSize = Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_INPUT.dwSize = (uint)nInSize;
            nET_DVR_XML_CONFIG_INPUT.lpRequestUrl = Marshal.StringToHGlobalAnsi(url);
            nET_DVR_XML_CONFIG_INPUT.dwRequestUrlLen = (uint)url.Length;

            /*      var cardInfoSearchCond = new { searchID = "0", searchResultPosition = 0, maxResults = 50 };
                  var json = new { CardInfoSearchCond = cardInfoSearchCond };
                  var strJson = JsonConvert.SerializeObject(json);

                  nET_DVR_XML_CONFIG_INPUT.lpInBuffer = Marshal.StringToHGlobalAnsi(strJson);
                  nET_DVR_XML_CONFIG_INPUT.dwInBufferSize = (uint)strJson.Length;*/

            // reserve space for return data
            CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT nET_DVR_XML_CONFIG_OUTPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();
            nET_DVR_XML_CONFIG_OUTPUT.dwSize = (uint)Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer = Marshal.AllocHGlobal(3 * 1024 * 1024);
            nET_DVR_XML_CONFIG_OUTPUT.dwOutBufferSize = 3 * 1024 * 1024;
            nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer = Marshal.AllocHGlobal(4096 * 4);
            nET_DVR_XML_CONFIG_OUTPUT.dwStatusSize = 4096 * 4;

            if (!NET_DVR_STDXMLConfig(userID, ref nET_DVR_XML_CONFIG_INPUT, ref nET_DVR_XML_CONFIG_OUTPUT))
            {
                uint getLastError = CHCNetSDK.NET_DVR_GetLastError();
                string errorStr = "NET_DVR_STDXMLConfig failed :" + getLastError;
                Console.WriteLine(errorStr);
            }

            string result = Marshal.PtrToStringAnsi(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);

            Console.WriteLine(result);


            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_INPUT.lpRequestUrl);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer);
        }

        public List<uint> GetFace(string IpAddress, string UserName, string Password, ushort Port)
        {

            List<uint> errorCode = new List<uint>();
            Auth(IpAddress, UserName, Password, Port);
            nET_DVR_DEVICEINFO_V40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];
            int userID = CHCNetSDK.NET_DVR_Login_V40(ref nET_DVR_USER_LOGIN_INFO, ref nET_DVR_DEVICEINFO_V40);

            CHCNetSDK.NET_DVR_CAPTURE_FACE_COND nET_DVR_CAPTURE_FACE_COND = new CHCNetSDK.NET_DVR_CAPTURE_FACE_COND();
           
            nET_DVR_CAPTURE_FACE_COND.dwSize = (uint)Marshal.SizeOf(nET_DVR_CAPTURE_FACE_COND);
            //nET_DVR_CAPTURE_FACE_COND.byRes = new byte[128];

            int dwInBufferLen = (int)nET_DVR_CAPTURE_FACE_COND.dwSize;
            IntPtr lpInBuffer = Marshal.AllocHGlobal(dwInBufferLen);
            Marshal.StructureToPtr(nET_DVR_CAPTURE_FACE_COND, lpInBuffer, false);

            //RemoteConfigCallback remoteConfigCallback = new RemoteConfigCallback(remoteConfig);

            int returnValue = NET_DVR_StartRemoteConfig(userID, CHCNetSDK.NET_DVR_CAPTURE_FACE_INFO, lpInBuffer, dwInBufferLen, remoteConfig, IntPtr.Zero);
            if (returnValue == -1)
            {
                Console.WriteLine("\n");
                //call NET_DVR_GetLastError to get the error code
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string strErr = "NET_DVR_StartRemoteConfig failed, error code= " + iLastErr;
                Console.WriteLine(strErr);
            }
           
                Console.WriteLine("NET_DVR_StartRemoteConfig Success");

            
            Marshal.FreeHGlobal(lpInBuffer);
            return errorCode;

        }

        public void SetFace(string IpAddress, string UserName, string Password, ushort Port)
        {
            Auth(IpAddress, UserName, Password, Port);

            nET_DVR_DEVICEINFO_V40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            int userID = CHCNetSDK.NET_DVR_Login_V40(ref nET_DVR_USER_LOGIN_INFO, ref nET_DVR_DEVICEINFO_V40);

            string url = "PUT /ISAPI/Intelligent/FDLib/FDSetUp?format=json";

            CHCNetSDK.NET_DVR_XML_CONFIG_INPUT nET_DVR_XML_CONFIG_INPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();

            Int32 nInSize = Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_INPUT.dwSize = (uint)nInSize;
            nET_DVR_XML_CONFIG_INPUT.lpRequestUrl = Marshal.StringToHGlobalAnsi(url);
            nET_DVR_XML_CONFIG_INPUT.dwRequestUrlLen = (uint)url.Length;

            string strpath = null;
            strpath = string.Format("capture.jpg", Environment.CurrentDirectory);
            if (!File.Exists(strpath))
            {
                Console.WriteLine("file does not exist!");

            }
            else
            {
                Console.WriteLine("file exist!");
            }
          
            var json = new { 
                faceURL = "http://10.17.105.138/capture.jpg",
                faceLibType = "blackFD",
                FDID = "1",
                FPID = "1",
                //deleteFP = true,
                name = "jakcun",
                bornTime = "2004-05-03"
                
            };
            var strJson = JsonConvert.SerializeObject(json);

            nET_DVR_XML_CONFIG_INPUT.lpInBuffer = Marshal.StringToHGlobalAnsi(strJson);
            nET_DVR_XML_CONFIG_INPUT.dwInBufferSize = (uint)strJson.Length;

            // reserve space for return data
            CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT nET_DVR_XML_CONFIG_OUTPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();
            nET_DVR_XML_CONFIG_OUTPUT.dwSize = (uint)Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer = Marshal.AllocHGlobal(3 * 1024 * 1024);
            nET_DVR_XML_CONFIG_OUTPUT.dwOutBufferSize = 3 * 1024 * 1024;
            nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer = Marshal.AllocHGlobal(4096 * 4);
            nET_DVR_XML_CONFIG_OUTPUT.dwStatusSize = 4096 * 4;

            if (!NET_DVR_STDXMLConfig(userID, ref nET_DVR_XML_CONFIG_INPUT, ref nET_DVR_XML_CONFIG_OUTPUT))
            {
                uint getLastError = CHCNetSDK.NET_DVR_GetLastError();
                string errorStr = "NET_DVR_STDXMLConfig failed :" + getLastError;
                Console.WriteLine(errorStr);
            }


            string result = Marshal.PtrToStringAnsi(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);
            string outXML = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(result));
            Console.WriteLine(outXML);
            string outStatus = Marshal.PtrToStringAnsi(nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer);
            Console.WriteLine("Status Output:\n" + outStatus);


            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_INPUT.lpRequestUrl);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer);
        }

        private void ProcessCapFaceData(IntPtr pFacePicBuffer, int dwFacePicSize)
        {
            if (0 == dwFacePicSize)
            {
                return;
            }
            string strpath = null;
            DateTime dt = DateTime.Now;
            strpath = string.Format("capture.jpeg", Environment.CurrentDirectory);
            try
            {
                using (FileStream fs = new FileStream(strpath, FileMode.OpenOrCreate))
                {
                    int FaceLen = dwFacePicSize;
                    byte[] by = new byte[FaceLen];
                    Marshal.Copy(pFacePicBuffer, by, 0, FaceLen);
                    fs.Write(by, 0, FaceLen);
                    fs.Close();
                }

                Console.WriteLine("Picture is saved at: " + string.Format("{0}\\{1}", Environment.CurrentDirectory, strpath));
                Console.WriteLine("Capture succeed", "SUCCESSFUL");
            }
            catch
            {
                //flag = false;
                Console.WriteLine("CaptureFaceData failed");
            }
        }

        public void remoteConfig(uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData)
        {
            //Console.WriteLine("received data");


            if (dwType == (uint)CHCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_DATA)
            {
                NET_DVR_CAPTURE_FACE_CFG captureFace = (NET_DVR_CAPTURE_FACE_CFG)Marshal.PtrToStructure(lpBuffer,typeof(NET_DVR_CAPTURE_FACE_CFG));
                IntPtr pFacePicBuffer = captureFace.pFacePicBuffer;
                int dwFacePicSize = captureFace.dwFacePicSize;
                ProcessCapFaceData(pFacePicBuffer, dwFacePicSize);
                //Console.WriteLine("datas received are correct");

            }
        }

        //Face Recognize Mode check/set

        public void RecogMode(string IpAddress, string UserName, string Password, ushort Port)
        {
            Auth(IpAddress, UserName, Password, Port);

            nET_DVR_DEVICEINFO_V40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            int userID = CHCNetSDK.NET_DVR_Login_V40(ref nET_DVR_USER_LOGIN_INFO, ref nET_DVR_DEVICEINFO_V40);

            string url = "PUT /ISAPI/AccessControl/FaceRecognizeMode?format=json";

            CHCNetSDK.NET_DVR_XML_CONFIG_INPUT nET_DVR_XML_CONFIG_INPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();

            Int32 nInSize = Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_INPUT.dwSize = (uint)nInSize;
            nET_DVR_XML_CONFIG_INPUT.lpRequestUrl = Marshal.StringToHGlobalAnsi(url);
            nET_DVR_XML_CONFIG_INPUT.dwRequestUrlLen = (uint)url.Length;

            var faceRecognizeMode = new
            {
                mode = "normalMode"

            };
            var json = new { FaceRecognizeMode = faceRecognizeMode };
            var strJson = JsonConvert.SerializeObject(json);

            nET_DVR_XML_CONFIG_INPUT.lpInBuffer = Marshal.StringToHGlobalAnsi(strJson);
            nET_DVR_XML_CONFIG_INPUT.dwInBufferSize = (uint)strJson.Length;

            // reserve space for return data
            CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT nET_DVR_XML_CONFIG_OUTPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();
            nET_DVR_XML_CONFIG_OUTPUT.dwSize = (uint)Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer = Marshal.AllocHGlobal(3 * 1024 * 1024);
            nET_DVR_XML_CONFIG_OUTPUT.dwOutBufferSize = 3 * 1024 * 1024;
            nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer = Marshal.AllocHGlobal(4096 * 4);
            nET_DVR_XML_CONFIG_OUTPUT.dwStatusSize = 4096 * 4;

            if (!NET_DVR_STDXMLConfig(userID, ref nET_DVR_XML_CONFIG_INPUT, ref nET_DVR_XML_CONFIG_OUTPUT))
            {
                uint getLastError = CHCNetSDK.NET_DVR_GetLastError();
                string errorStr = "NET_DVR_STDXMLConfig failed :" + getLastError;
                Console.WriteLine(errorStr);
            }


            string result = Marshal.PtrToStringAnsi(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);
            string outXML = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(result));
            Console.WriteLine(outXML);
            string outStatus = Marshal.PtrToStringAnsi(nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer);
            Console.WriteLine("Status Output:\n" + outStatus);


            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_INPUT.lpRequestUrl);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer);
        }
    }
}
