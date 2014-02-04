using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;  //api

namespace DL580_9130
{
   

    class readerDLL
    {
        //public struct TagIds
        //{
        //    public byte[] TagID;  //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)] 
        //    byte Tagatype;
        //    byte AntNum;
        //    private TagIds(byte A, byte B, byte[] C)
        //    {
        //        this.Tagatype = A;
        //        this.AntNum = B;
        //        this.TagID =C;
        //    }
        //}
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TagIds
        {
            public byte TagType;
            public byte AntNum;
            public byte Length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
            public byte[] Ids;

            private TagIds(byte A, byte B, byte C, byte[] D)
            {
                
                    this.TagType = A;
                   this.AntNum = B; 
                  this.Length = C;
                 this.Ids = D;
           }
        };


        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        
        //public static IntPtr TagIds;
        public static IntPtr hComm;

        [DllImport("Reader.dll")]
        public static extern short OpenReader(ref IntPtr hCom, byte LinkType, string com_port);   //Open communication port
        [DllImport("Reader.dll")]
        public static extern short CloseReader(IntPtr hCom);        //Close communication port
        [DllImport("Reader.dll")]
        public static extern short SetBaudRate(IntPtr hCom, int BaudRate);     //Set the baud rate
        [DllImport("Reader.dll")]
        public static extern short ResetReader(IntPtr hCom); // Reset reader
        [DllImport("Reader.dll")]
        public static extern short GetFirmwareVersion(IntPtr hCom, ref byte flag, ref byte major, ref byte minor);      //Reader version information
        [DllImport("Reader.dll")]
        public static extern short SingleTagIdentifyEX(IntPtr hCom, ref byte TagType, byte[] value);    //Reader tag ID, each read only one
        [DllImport("Reader.dll")]

        public static extern short SingleTagIdentify(IntPtr hCom, ref byte TagType,  byte[] value);    //Reader tag ID, each read only one
        [DllImport("Reader.dll")]

        public static extern short MultipleTagIdentify(IntPtr hCom, uint TagType, ref int Count, ref TagIds value);//Reader tag ID, each read more than one
        //      public static extern short MultipleTagIdentify(IntPtr hCom, byte TagType, ref int Count,ref TagIds value);//Reader tag ID, each read more than one

        [DllImport("Reader.dll")]
        public static extern short MultipleTagIdentifyEX(IntPtr hCom, uint TagType, ref int Count, ref TagIds value);//Reader tag ID, each read more than one

        [DllImport("Reader.dll")]
        public static extern short ReadTagUserWithID(IntPtr hCom, byte count, int Addr, byte[] Id, byte[] value);            //Function: read the stored contents of the specified label specified address
        [DllImport("Reader.dll")]
        public static extern short WriteTagUserWithID(IntPtr hCom, byte count, int Addr, byte[] Id, byte[] value, byte Result);  //To write the specified tag specifies the address of the storage area
        
        

        //try functions unknown
        //try functions
        
        [DllImport("Reader.dll")]
        public static extern short StopRFwork(IntPtr hCom);  //Stop RF work of Reader (no RF power sent from reader)

        [DllImport("Reader.dll")]
        public static extern short BeepControl(IntPtr hCom, byte ControlType);  //ControlType— controlling type of buzzer, defined as: 1 start buzzer; 2, stop the buzzer; 3. start the buzzer and automatically stop the buzzer
        

        [DllImport("Reader.dll")]
        public static extern short GetParameter(IntPtr hCom, int Addr, byte ParaCount, byte[] value);  //To get parameter values.

        [DllImport("Reader.dll")]
        public static extern short SetParameter(IntPtr hCom, int Addr, byte ParaCount, byte[] Parameters);  //To get parameter values.

   
    
    }
}
