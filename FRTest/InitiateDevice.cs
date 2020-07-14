using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FRTest
{
    public class InitiateDevice
    {
       
        private string IpAddress_ = "10.17.103.133"; //Office
        //private string IpAddress_ = "192.168.0.161"; //Home
        private string Username_ = "admin";
        private string Password_ = "admin12345";
        private ushort Port_ = 8000;
        private int UserID_;


        public static bool Init()
        {
            if (CHCNetSDK.NET_DVR_Init())
            {
                Console.WriteLine("Initiate Successful");
                return true;
            }
            else
            {
                Console.WriteLine("Initiate Failed");
                return false;
            }
        }
        public string DeviceIP { 
            get { return IpAddress_; }
            set { IpAddress_ = value; } 
        }
        public string LoginUsername
        {
            get { return Username_; }
            set { Username_ = value; }
        }

        public string LoginPassword
        {
            get { return Password_; }
            set { Password_ = value; }
        }

        public ushort Port
        {
            get { return Port_; }
            set { Port_ = value; }
        }

        public uint Login()
        {
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            struLoginInfo.bUseAsynLogin = false;
            struLoginInfo.sDeviceAddress = DeviceIP;
            struLoginInfo.sUserName = LoginUsername;
            struLoginInfo.sPassword = LoginPassword;
            struLoginInfo.wPort = Port;

            UserID_ = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);

            uint errorCode = 0;
            if (UserID_ >= 0)
            {
                errorCode = 0;
                Console.WriteLine("Login Successul");
            }
            else
            {
                errorCode = CHCNetSDK.NET_DVR_GetLastError();
                Console.WriteLine("Login failed. Error: " + errorCode);              
            }

            return errorCode;

        }


     /*   public int Logout()
        {
            if (UserID_ >= 0)
            {
                CHCNetSDK.NET_DVR_Logout_V30(UserID_);
                Console.WriteLine("Loged Out");
            }
            CHCNetSDK.NET_DVR_Cleanup();
            return 0;
        }*/

    }
}
