using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; //추가
using System.Data; //추가
using System.Runtime.InteropServices; //추가
using System.Threading;
using static System.Windows.Forms.AxHost;


namespace CIFX_50RE
{
    public class CIFX
    {
        #region properties

        public static UInt32 _hDriver;
        public static UInt32 hDriver
        { get { return _hDriver; } }

        private static UInt32 _hSysdevice;
        public static UInt32 hSysDevice
        { get { return _hSysdevice; } }

        private static UInt32 _hChannel;
        public static UInt32 hChannel
        { get { return _hChannel; } }

        private static string _ActiveBoard;
        public static string ActiveBoard
        { get { return _ActiveBoard; } set { _ActiveBoard = value; } }

        private static int _ActivChannel;
        public static int ActiveChannel
        { get { return _ActivChannel; } set { _ActivChannel = value; } }

        private static DRIVER_INFORMATIONtag _DriverInformation;
        public static DRIVER_INFORMATIONtag DriverInformation
        { get { return _DriverInformation; } }

        private static CIFX_DIRECTORYENTRYtag _CifXDirectoryEntry;
        public static CIFX_DIRECTORYENTRYtag CifXdirectoryEntry
        { get { return _CifXDirectoryEntry; } }

        private static SYSTEM_CHANNEL_INFORMATIONtag _SystemChannelInformation;
        public static SYSTEM_CHANNEL_INFORMATIONtag SystemChannelInformation
        { get { return _SystemChannelInformation; } }

        private static SYSTEM_CHANNEL_INFO_BLOCKtag _SystemChannelInfoBlock;
        public static SYSTEM_CHANNEL_INFO_BLOCKtag SystemchannelSystemInfoBlock
        { get { return _SystemChannelInfoBlock; } }

        private static SYSTEM_CHANNEL_SYSTEM_CONTROL_BLOCKtag _SystemChannelSystemControlBlock;
        public static SYSTEM_CHANNEL_SYSTEM_CONTROL_BLOCKtag SystemChannelSystemControlBlock
        { get { return _SystemChannelSystemControlBlock; } }

        private static SYSTEM_CHANNEL_SYSTEM_STATUS_BLOCKtag _SystemChannelSystemStatusBlock;
        public static SYSTEM_CHANNEL_SYSTEM_STATUS_BLOCKtag SystemChannelSystemStatusBlock
        { get { return _SystemChannelSystemStatusBlock; } }

        private static BOARD_INFORMATIONtag _BoardInformation;
        public static BOARD_INFORMATIONtag BoardInformation
        { get { return _BoardInformation; } }

        private static CHANNEL_INFORMATIONtag _ChannelInformation;
        public static CHANNEL_INFORMATIONtag ChannelInformation
        { get { return _ChannelInformation; } }

        private static CHANNEL_IO_INFORMATIONtag _ChannelIOInformation;
        public static CHANNEL_IO_INFORMATIONtag ChannelIOInformation
        { get { return _ChannelIOInformation; } }

        private static NETX_MASTER_STATUStag _NetxMasterStatus;
        public static NETX_MASTER_STATUStag NetxMasterStatus
        { get { return _NetxMasterStatus; } }

        private static NETX_COMMON_STATUS_BLOCKtag _NetxCommonStatusBlock;
        public static NETX_COMMON_STATUS_BLOCKtag NetxCommonStatusBlock
        { get { return _NetxCommonStatusBlock; } }

        private static CIFX_PACKET_HEADERtag _CifXPacketHeader;
        public static CIFX_PACKET_HEADERtag CifXPacketHeader
        { get { return _CifXPacketHeader; } set { } }

        private static CIFX_PACKETtag _CifXPacket;
        public static CIFX_PACKETtag CifXPacket
        { get { return _CifXPacket; } set { _CifXPacket = value; } }

        #endregion


        #region global definitions

        public UInt32 CIFXHANDLE;
        public UInt32 CHANNELHANDLE;

        /* DPM memory validation */
        public ulong CIFX_DPM_NO_MEMORY_ASSIGNED = 0x0BAD0BADUL;
        public ulong CIFX_DPM_INVALID_CONTENT = 0xFFFFFFFFUL;

        /* CIFx global timeouts in milliseconds */
        public ulong CIFX_TO_WAIT_HW_RESET_ACTIVE = 2000UL;
        public ulong CIFX_TO_WAIT_HW = 2000UL;
        public ulong CIFX_TO_WAIT_COS_CMD = 10UL;
        public ulong CIFX_TO_WAIT_COS_ACK = 10UL;
        public ulong CIFX_TO_SEND_PACKET = 5000UL;
        public ulong CIFX_TO_1ST_PACKET = 1000UL;
        public ulong CIFX_TO_CONT_PACKET = 1000UL;
        public ulong CIFX_TO_LAST_PACKET = 1000UL;
        public ulong CIFX_TO_FIRMWARE_START = 10000UL;

        /* Maximum channel number */
        public const int CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION = 8;
        public const UInt32 CIFX_MAX_NUMBER_OF_CHANNELS = 6;
        public const UInt32 CIFX_NO_CHANNEL = 0xFFFFFFFF;

        /* Maximum file name length */
        public UInt32 CIFX_MAX_FILE_NAME_LENGTH = 260;
        public UInt32 CIFX_MIN_FILE_NAME_LENGTH = 5;

        /* The system device port number */
        public UInt32 CIFX_SYSTEM_DEVICE = 0xFFFFFFFF;

        /* Information commands */
        public const UInt32 CIFX_INFO_CMD_SYSTEM_INFORMATION = 1;
        public const UInt32 CIFX_INFO_CMD_SYSTEM_INFO_BLOCK = 2;
        public const UInt32 CIFX_INFO_CMD_SYSTEM_CHANNEL_BLOCK = 3;
        public const UInt32 CIFX_INFO_CMD_SYSTEM_CONTROL_BLOCK = 4;
        public const UInt32 CIFX_INFO_CMD_SYSTEM_STATUS_BLOCK = 5;

        /* General commands */
        public UInt32 CIFX_CMD_READ_DATA = 1;
        public UInt32 CIFX_CMD_WRITE_DATA = 2;

        /* HOST mode definition */
        public const UInt32 CIFX_HOST_STATE_NOT_READY = 0;
        public const UInt32 CIFX_HOST_STATE_READY = 1;
        public const UInt32 CIFX_HOST_STATE_READ = 2;

        /* WATCHDOG commands*/
        public UInt32 CIFX_WATCHDOG_STOP = 0;
        public UInt32 CIFX_WATCHDOG_START = 1;

        /* Configuration Lock commands*/
        public UInt32 CIFX_CONFIGURATION_UNLOCK = 0;
        public UInt32 CIFX_CONFIGURATION_LOCK = 1;
        public UInt32 CIFX_CONFIGURATION_GETLOCKSTATE = 2;

        /* BUS state commands*/
        public UInt32 CIFX_BUS_STATE_OFF = 0;
        public UInt32 CIFX_BUS_STATE_ON = 1;
        public UInt32 CIFX_BUS_STATE_GETSTATE = 2;

        /* Memory pointer commands*/
        public UInt32 CIFX_MEM_PTR_OPEN = 1;
        public UInt32 CIFX_MEM_PTR_OPEN_USR = 2;
        public UInt32 CIFX_MEM_PTR_CLOSE = 3;

        /* I/O area definition */
        public UInt32 CIFX_IO_INPUT_AREA = 1;
        public UInt32 CIFX_IO_OUTPUT_AREA = 2;

        /* Reset definitions */
        public UInt32 CIFX_SYSTEMSTART = 1;
        public UInt32 CIFX_CHANNELINIT = 2;

        #endregion


        #region CIFX Device Driver data exchange structure definitions


        /*****************************************************************************/
        /*! Structure definitions                                                    */
        /*****************************************************************************/
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DRIVER_INFORMATIONtag
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] abDriverVersion;                              /*!< Driver version                 */
            public UInt32 ulBoardCnt;                                   /*!< Number of available Boards     */
        }

        public const int CIFx_MAX_INFO_NAME_LENTH = 16;

        /*****************************************************************************/
        /*! Directory Information structure for enumerating directories              */
        /*****************************************************************************/
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CIFX_DIRECTORYENTRYtag
        {
            public UInt32 hList;                                /*!< Handle from Enumeration function, do not touch */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CIFx_MAX_INFO_NAME_LENTH)]
            public byte[] szFilename;                           /*!< Returned file name. */
            public byte bFiletype;                            /*!< Returned file type. */
            public UInt32 ulFilesize;                           /*!< Returned file size. */
        }

        /*****************************************************************************/
        /*! System Channel Information structure                                     */
        /*****************************************************************************/
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SYSTEM_CHANNEL_INFORMATIONtag
        {
            public UInt32 ulSystemError;                                /*!< Global system error            */
            public UInt32 ulDpmTotalSize;                               /*!< Total size dual-port memory in bytes */
            public UInt32 ulMBXSize;                                    /*!< System mailbox data size [Byte]*/
            public UInt32 ulDeviceNumber;                               /*!< Global device number           */
            public UInt32 ulSerialNumber;                               /*!< Global serial number           */
            public UInt32 ulOpenCnt;                                    /*!< Channel open counter           */
        }
        /* System Channel: System Information Block */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SYSTEM_CHANNEL_INFO_BLOCKtag
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] abCookie;                                      /*!< 0x00 "netX" cookie */
            public UInt32 ulDpmTotalSize;                                /*!< 0x04 Total Size of the whole dual-port memory in bytes */
            public UInt32 ulDeviceNumber;                                /*!< 0x08 Device number */
            public UInt32 ulSerialNumber;                                /*!< 0x0C Serial number */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public UInt16[] ausHwOptions;                                  /*!< 0x10 Hardware options, xC port 0..3 */
            public UInt16 usManufacturer;                                /*!< 0x18 Manufacturer Location */
            public UInt16 usProductionDate;                              /*!< 0x1A Date of production */
            public UInt32 ulLicenseFlags1;                               /*!< 0x1C License code flags 1 */
            public UInt32 ulLicenseFlags2;                               /*!< 0x20 License code flags 2 */
            public UInt16 usNetxLicenseID;                               /*!< 0x24 netX license identification */
            public UInt16 usNetxLicenseFlags;                            /*!< 0x26 netX license flags */
            public UInt16 usDeviceClass;                                 /*!< 0x28 netX device class */
            public byte bHwRevision;                                   /*!< 0x2A Hardware revision index */
            public byte bHwCompatibility;                              /*!< 0x2B Hardware compatibility index */
            public byte bDevIdNumber;                                  /*!< Device Identification number (rotary switch) */
            public byte bReserved;                                     /*!< unused/reserved */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public UInt16[] ausReserved;                                   /*!< 0x2C:0x2F Reserved */
        } /* System Channel: Channel Information Block */
        public const int CIFX_SYSTEM_CHANNEL_DEFAULT_INFO_BLOCK_SIZE = 16;
        public byte[,] abInfoBlock = new byte[CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION, CIFX_SYSTEM_CHANNEL_DEFAULT_INFO_BLOCK_SIZE];


        /* System Channel: System Control Block */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SYSTEM_CHANNEL_SYSTEM_CONTROL_BLOCKtag
        {
            public UInt32 ulSystemCommandCOS;                            /*!< 0x00 System channel change of state command */
            public UInt32 ulReserved;                                    /*!< 0x04 Reserved */
        }

        /* System Channel: System Status Block */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SYSTEM_CHANNEL_SYSTEM_STATUS_BLOCKtag
        {
            public UInt32 ulSystemCOS;                                   /*!< 0x00 System channel change of state acknowledge */
            public UInt32 ulSystemStatus;                                /*!< 0x04 Actual system state */
            public UInt32 ulSystemError;                                 /*!< 0x08 Actual system error */
            public UInt32 ulReserved1;                                   /*!< 0x0C reserved */
            public UInt32 ulTimeSinceStart;                              /*!< 0x10 time since start in seconds */
            public UInt16 usCpuLoad;                                     /*!< 0x14 cpu load in 0,01% units (10000 => 100%) */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 42)]
            public byte[] abReserved;                                    /*!< 0x16:3F Reserved */
        }
        /*****************************************************************************/
        /*! Board Information structure                                              */
        /*****************************************************************************/
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BOARD_INFORMATIONtag
        {
            public UInt32 lBoardError;                                /*!< Global Board error. Set when device specific data must not be used */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CIFx_MAX_INFO_NAME_LENTH)]
            public byte[] abBoardName;                                /*!< Global board name              */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CIFx_MAX_INFO_NAME_LENTH)]
            public byte[] abBoardAlias;                               /*!< Global board alias name        */
            public UInt32 ulBoardID;                                  /*!< Unique board ID, driver created*/
            public UInt32 ulSystemError;                              /*!< System error                   */
            public UInt32 ulPhysicalAddress;                          /*!< Physical memory address        */
            public UInt32 ulIrqNumber;                                /*!< Hardware interrupt number      */
            public byte bIrqEnabled;                                /*!< Hardware interrupt enable flag */
            public UInt32 ulChannelCnt;                               /*!< Number of available channels   */
            public UInt32 ulDpmTotalSize;                             /*!< Dual-Port memory size in bytes */
            public SYSTEM_CHANNEL_INFO_BLOCKtag tSystemInfo;               /*!< System information             */
        }
        /*****************************************************************************/
        /*! Channel Information structure                                            */
        /*****************************************************************************/
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CHANNEL_INFORMATIONtag
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] abBoardName;        /*!< Global board name              */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] abBoardAlias;       /*!< Global board alias name        */
            public UInt32 ulDeviceNumber;                               /*!< Global board device number     */
            public UInt32 ulSerialNumber;                               /*!< Global board serial number     */

            public UInt16 usFWMajor;                                    /*!< Major Version of Channel Firmware  */
            public UInt16 usFWMinor;                                    /*!< Minor Version of Channel Firmware  */
            public UInt16 usFWBuild;                                    /*!< Build number of Channel Firmware   */
            public UInt16 usFWRevision;                                 /*!< Revision of Channel Firmware       */
            public byte bFWNameLength;                                /*!< Length  of FW Name                 */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 63)]
            public byte[] abFWName;                                 /*!< Firmware Name                      */
            public UInt16 usFWYear;                                     /*!< Build Year of Firmware             */
            public byte bFWMonth;                                     /*!< Build Month of Firmware (1..12)    */
            public byte bFWDay;                                       /*!< Build Day of Firmware (1..31)      */

            public UInt32 ulChannelError;                               /*!< Channel error                  */
            public UInt32 ulOpenCnt;                                    /*!< Channel open counter           */
            public UInt32 ulPutPacketCnt;                               /*!< Number of put packet commands  */
            public UInt32 ulGetPacketCnt;                               /*!< Number of get packet commands  */
            public UInt32 ulMailboxSize;                                /*!< Mailbox packet size in bytes   */
            public UInt32 ulIOInAreaCnt;                                /*!< Number of IO IN areas          */
            public UInt32 ulIOOutAreaCnt;                               /*!< Number of IO OUT areas         */
            public UInt32 ulHskSize;                                    /*!< Size of the handshake cells    */
            public UInt32 ulNetxFlags;                                  /*!< Actual netX state flags        */
            public UInt32 ulHostFlags;                                  /*!< Actual Host flags              */
            public UInt32 ulHostCOSFlags;                               /*!< Actual Host COS flags          */
            public UInt32 ulDeviceCOSFlags;                             /*!< Actual Device COS flags        */
        }
        /*****************************************************************************/
        /*! IO Area Information structure                                            */
        /*****************************************************************************/
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CHANNEL_IO_INFORMATIONtag
        {
            public UInt32 ulTotalSize;                  /*!< Total IO area size in byte */
            public UInt32 ulUsedSize;                   /*!< Used IO area size in byte */
            public UInt32 ulIOMode;                     /*!< Exchange mode */
        }
        /***************************************************************************/
        /* Driver dependent information */

        public const int CIFX_MAX_PACKET_SIZE = 1600;                  /*!< Maximum size of the RCX packet in bytes */
        public const int CIFX_PACKET_HEADER_SIZE = 40;                    /*!< Maximum size of the RCX packet header in bytes */
        public const int CIFX_MAX_DATA_SIZE = CIFX_MAX_PACKET_SIZE - CIFX_PACKET_HEADER_SIZE; /*!< Maximum RCX packet data size */
        public UInt32 CIFX_MSK_PACKET_ANSWER = 0x00000001;            /*!< Packet answer bit */

        /*****************************************************************************/
        /*! Packet header                                                            */
        /*****************************************************************************/
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CIFX_PACKET_HEADERtag
        {
            public UInt32 ulDest;   /*!< destination of packet, process queue */
            public UInt32 ulSrc;    /*!< source of packet, process queue */
            public UInt32 ulDestId; /*!< destination reference of packet */
            public UInt32 ulSrcId;  /*!< source reference of packet */
            public UInt32 ulLen;    /*!< length of packet data without header */
            public UInt32 ulId;     /*!< identification handle of sender */
            public UInt32 ulState;  /*!< status code of operation */
            public UInt32 ulCmd;    /*!< packet command defined in TLR_Commands.h */
            public UInt32 ulExt;    /*!< extension */
            public UInt32 ulRout;   /*!< router */
        }

        /*****************************************************************************/
        /*! Definition of the rcX Packet                                             */
        /*****************************************************************************/
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CIFX_PACKETtag
        {
            CIFX_PACKET_HEADERtag tHeader;                   /**! */
            [MarshalAs(UnmanagedType.LPArray, SizeConst = CIFX_MAX_DATA_SIZE)]
            public byte[] abData;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PFN_PROGRESS_CALLBACK
        {
            UInt32 ulStep;
            UInt32 ulMaxStep;
            UInt32 pvUser;
            char bFinished;
            long lError;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PFN_RECV_PKT_CALLBACK
        {
            [MarshalAs(UnmanagedType.U4)]
            CIFX_PACKETtag ptRecvPkt;
            UInt32 pvUser;
        }

        public UInt32 CIFX_CALLBACK_ACTIVE = 0;
        public UInt32 CIFX_CALLBACK_FINISHED = 1;

        public UInt32 DOWNLOAD_MODE_FIRMWARE = 1;
        public UInt32 DOWNLOAD_MODE_CONFIG = 2;
        public UInt32 DOWNLOAD_MODE_FILE = 3;
        public UInt32 DOWNLOAD_MODE_BOOTLOADER = 4; /*!< Download bootloader update to target. */
        public UInt32 DOWNLOAD_MODE_LICENSECODE = 5; /*!< License update code.                  */


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NETX_MASTER_STATUStag
        {
            public UInt32 ulSlaveState;
            public UInt32 ulSlaveErrLogInd;
            public UInt32 ulNumOfConfigSlavs;
            public UInt32 ulNumOfActiveSlaves;
            public UInt32 ulNumOfDiagSlaves;
            public UInt32 ulReserved;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NETX_COMMON_STATUS_BLOCKtag
        {
            public UInt32 ulCommunicationCOS;                               /*!< Global board device number     */
            public UInt32 ulCommunicationState;                               /*!< Global board serial number     */
            public UInt32 ulCommunicationError;

            public UInt16 usVersion;                                    /*!< Major Version of Channel Firmware  */
            public UInt16 usWatchdogTime;                                    /*!< Minor Version of Channel Firmware  */

            public byte bPDInHskMode;
            public byte bPDInSource;                                 /*!< Revision of Channel Firmware       */
            public byte bPDOutHskMode;                                    /*!< Build number of Channel Firmware   */
            public byte bPDOutSource;

            public UInt32 ulHostWatchdog;                               /*!< Global board serial number     */
            public UInt32 ulErrorCount;


            public byte bErrorLogInd;                                    /*!< Build number of Channel Firmware   */
            public byte bErrorPDInCnt;                                 /*!< Revision of Channel Firmware       */
            public byte bErrorPDOutCnt;                                    /*!< Build number of Channel Firmware   */
            public byte bErrorSyncCnt;
            public byte bSyncHskMode;                                    /*!< Build number of Channel Firmware   */
            public byte bSyncSource;


            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public UInt16[] ausReserved;     /*!< 0x10 Hardware options, xC port 0..3 */

            public NETX_MASTER_STATUStag tMasterStatus;               /*!< System information             */


        }

        #endregion


        #region CIFX32DLL.DLL imports

        [DllImport("cifx32dll.dll", EntryPoint = "xDriverOpen")]
        private static extern UInt32 _xDriverOpen([MarshalAs(UnmanagedType.U4)]
                                                     ref UInt32 CIFXHANDLE);

        [DllImport("cifx32dll.dll", EntryPoint = "xDriverClose")]
        private static extern UInt32 _xDriverClose(UInt32 CIFXHANDLE);

        [DllImport("cifx32dll.dll", EntryPoint = "xDriverGetInformation")]
        private static extern UInt32 _xDriverGetInformation(UInt32 CIFXHANDLE,
                                                                UInt32 ulSize,
                                                                ref DRIVER_INFORMATIONtag pvDriverInfo);

        [DllImport("cifx32dll.dll", EntryPoint = "xDriverGetErrorDescription")]
        private static extern UInt32 _xDriverGetErrorDescription(UInt32 lError,
                                                                    [Out, MarshalAs(UnmanagedType.LPArray)] byte[] szBuffer,
                                                                    UInt32 ulBufferLen);

        [DllImport("cifx32dll.dll", EntryPoint = "xDriverEnumBoards")]
        private static extern UInt32 _xDriverEnumBoards(UInt32 CIFXHANDLE,
                                                            UInt32 ulBoard,
                                                            UInt32 ulSize,
                                                            ref BOARD_INFORMATIONtag pvBoardInfo);

        [DllImport("cifx32dll.dll", EntryPoint = "xDriverEnumChannels")]
        private static extern UInt32 _xDriverEnumChannels(UInt32 CIFXHANDLE,
                                                            UInt32 ulBoard,
                                                            UInt32 ulChannel,
                                                            UInt32 ulSize,
                                                            ref CHANNEL_INFORMATIONtag pvChannelInfo);

        /* System device depending functions*/
        [DllImport("cifx32dll.dll", EntryPoint = "xSysdeviceOpen")]
        private static extern UInt32 _xSysdeviceOpen(UInt32 hDriver,
                                                        string szBoard,
                                                        [MarshalAs(UnmanagedType.U4)] ref UInt32 phSysdevice);

        [DllImport("cifx32dll.dll", EntryPoint = "xSysdeviceClose")]
        private static extern UInt32 _xSysdeviceClose(UInt32 hSysdevice);

        [DllImport("cifx32dll.dll", EntryPoint = "xSysdeviceGetMBXState")]
        private static extern UInt32 _xSysdeviceGetMBXState(UInt32 hSysdevice,
                                                      ref UInt32 pulRecvPktCount,
                                                      ref UInt32 pulSendPktCount);

        [DllImport("cifx32dll.dll", EntryPoint = "xSysdeviceInfo")]
        private static extern UInt32 _xSysdeviceInfo(UInt32 hSysdevice,
                                                      UInt32 ulCmd,
                                                      UInt32 ulSize,
                                                      [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pvzData);

        [DllImport("cifx32dll.dll", EntryPoint = "xSysdeviceFindFirstFile")]
        private static extern UInt32 _xSysdeviceFindFirstFile(UInt32 hSysdevice,
                                                        UInt32 ulChannel,
                                                        ref CIFX_DIRECTORYENTRYtag ptDirectoryInfo,
                                                        string PFN1,
                                                        ref UInt32 pvUser);

        [DllImport("cifx32dll.dll", EntryPoint = "xSysdeviceFindNextFile")]
        private static extern UInt32 _xSysdeviceFindNextFile(UInt32 hSysdevice,
                                                              UInt32 ulChannel,
                                                              ref CIFX_DIRECTORYENTRYtag ptDirectoryInfo,
                                                              string PFN1,
                                                              ref UInt32 pvUser);

        [DllImport("cifx32dll.dll", EntryPoint = "xSysdeviceDownload")]
        private static extern UInt32 _xSysdeviceDownload(UInt32 hSysdevice,
                                                          UInt32 ulChannel,
                                                          UInt32 ulMode,
                                                          [MarshalAs(UnmanagedType.LPStr)] string pszFileName,
                                                          byte[] pabFileData,
                                                          UInt32 ulFileSize,
                                                          string PFN1,
                                                          string PFN2,
                                                          ref UInt32 pvUser);

        [DllImport("cifx32dll.dll", EntryPoint = "xSysdeviceUpload")]
        private static extern UInt32 _xSysdeviceUpload(UInt32 hSysdevice,
                                                        UInt32 ulChannel,
                                                        UInt32 ulMode,
                                                        [MarshalAs(UnmanagedType.LPStr)] string pszFileName,
                                                        Byte[] pabFileData,
                                                        UInt32 pulFileSize,
                                                        string PFN1,
                                                        string PFN2,
                                                        ref UInt32 pvUser);

        [DllImport("cifx32dll.dll", EntryPoint = "xSysdeviceReset")]
        private static extern UInt32 _xSysdeviceReset(UInt32 hSysdevice, UInt32 ulTimeout);

        /* Channel depending functions */
        [DllImport("cifx32dll.dll", EntryPoint = "xChannelOpen")]
        private static extern UInt32 _xChannelOpen(UInt32 CIFXHANDLE,
                                                    [MarshalAs(UnmanagedType.LPStr)] string szBoard,
                                                    UInt32 ulChannel,
                                                    [MarshalAs(UnmanagedType.U4)] ref UInt32 phChannel);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelClose")]
        private static extern UInt32 _xChannelClose(UInt32 hChannel);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelFindFirstFile")]
        private static extern UInt32 _xChannelFindFirstFile(UInt32 hChannel,
                                                             ref CIFX_DIRECTORYENTRYtag ptDirectoryInfo,
                                                             string PFN1,
                                                             ref UInt32 pvUser);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelFindNextFile")]
        private static extern UInt32 _xChannelFindNextFile(UInt32 hChannel,
                                                            ref CIFX_DIRECTORYENTRYtag ptDirectoryInfo,
                                                            string PFN1,
                                                            ref UInt32 pvUser);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelDownload")]
        private static extern UInt32 _xChannelDownload(UInt32 hChannel,
                                                        UInt32 ulMode,
                                                        [MarshalAs(UnmanagedType.LPStr)] string sFileName,
                                                        Byte[] pabFileData,
                                                        UInt32 ulFileSize,
                                                        string PFN1,
                                                        string PFN2,
                                                        ref UInt32 pvUser);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelUpload")]
        private static extern UInt32 _xChannelUpload(UInt32 hChannel,
                                                      UInt32 ulMode,
                                                      [MarshalAs(UnmanagedType.LPStr)] string pszFileName,
                                                      Byte[] pabFileData,
                                                      UInt32 pulFileSize,
                                                      string PFN1,
                                                      string PFN2,
                                                      ref UInt32 pvUser);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelConfigLock")]
        private static extern UInt32 _xChannelConfigLock(UInt32 hChannel,
                                                          UInt32 ulCmd,
                                                          ref UInt32 pulState,
                                                          UInt32 ulTimeout);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelReset")]
        private static extern UInt32 _xChannelReset(UInt32 hChannel,
                                                        UInt32 ulResetMode,
                                                        UInt32 ulTimeout);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelInfo")]
        private static extern UInt32 _xChannelInfo(UInt32 CHANNELHANDLE,
                                                    UInt32 ulSize,
                                                    ref CHANNEL_INFORMATIONtag pvChannelInfo);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelWatchdog")]
        private static extern UInt32 _xChannelWatchdog(UInt32 hChannel,
                                                        UInt32 ulCmd,
                                                        ref UInt32 pulTrigger);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelHostState")]
        private static extern UInt32 _xChannelHostState(UInt32 hChannel,
                                                    UInt32 ulCmd,
                                                    ref UInt32 pulState,
                                                    UInt32 ulTimeout);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelBusState")]
        private static extern UInt32 _xChannelBusState(UInt32 hChannel,
                                                        UInt32 ulCmd,
                                                        ref UInt32 pulState,
                                                        UInt32 ulTimeout);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelIOInfo")]
        private static extern UInt32 _xChannelIOInfo(UInt32 hChannel,
                                                      UInt32 ulCmd,
                                                      UInt32 ulAreaNumber,
                                                      UInt32 ulSize,
                                                      ref CHANNEL_IO_INFORMATIONtag pvData);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelIORead")]
        private static extern UInt32 _xChannelIORead(UInt32 hChannel,
                                               UInt32 ulAreaNumber,
                                               UInt32 ulOffset,
                                               UInt32 ulDataLen,
                                               [In, MarshalAs(UnmanagedType.LPArray)] byte[] pvData,
                                               UInt32 ulTimeout);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelIOWrite")]
        private static extern UInt32 _xChannelIOWrite(UInt32 hChannel,
                                                UInt32 ulAreaNumber,
                                                UInt32 ulOffset,
                                                UInt32 ulDataLen,
                                                [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pvData,
                                                UInt32 ulTimeout);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelCommonStatusBlock")]
        private static extern UInt32 _xChannelCommonStatusBlock(UInt32 hChannel,
                                                         UInt32 ulCmd,
                                                         UInt32 ulOffset,
                                                         UInt32 ulSize,
                                                      ref NETX_COMMON_STATUS_BLOCKtag pvData);

        #endregion


        #region CIFX Device Driver Definitions and Error Codes

        public const ulong CIFX_NO_ERROR = 0x00000000L;
        public const ulong CIFX_INVALID_POINTER = 0x800A0001L;
        public const ulong CIFX_INVALID_BOARD = 0x800A0002L;
        public const ulong CIFX_INVALID_CHANNEL = 0x800A0003L;
        public const ulong CIFX_INVALID_HANDLE = 0x800A0004L;
        public const ulong CIFX_INVALID_PARAMETER = 0x800A0005L;
        public const ulong CIFX_INVALID_COMMAND = 0x800A0006L;
        public const ulong CIFX_INVALID_BUFFERSIZE = 0x800A0007L;
        public const ulong CIFX_INVALID_ACCESS_SIZE = 0x800A0008L;
        public const ulong CIFX_FUNCTION_FAILED = 0x800A0009L;
        public const ulong CIFX_FILE_OPEN_FAILED = 0x800A000AL;
        public const ulong CIFX_FILE_SIZE_ZERO = 0x800A000BL;
        public const ulong CIFX_FILE_LOAD_INSUFF_MEM = 0x800A000CL;
        public const ulong CIFX_FILE_CHECKSUM_ERROR = 0x800A000DL;
        public const ulong CIFX_FILE_READ_ERROR = 0x800A000EL;
        public const ulong CIFX_FILE_TYPE_INVALID = 0x800A000FL;
        public const ulong CIFX_FILE_NAME_INVALID = 0x800A0010L;
        public const ulong CIFX_FUNCTION_NOT_AVAILABLE = 0x800A0011L;
        public const ulong CIFX_BUFFER_TOO_SHORT = 0x800A0012L;
        public const ulong CIFX_MEMORY_MAPPING_FAILED = 0x800A0013L;
        public const ulong CIFX_NO_MORE_ENTRIES = 0x800A0014L;
        public const ulong CIFX_DRV_NOT_INITIALIZED = 0x800B0001L;
        public const ulong CIFX_DRV_INIT_STATE_ERROR = 0x800B0002L;
        public const ulong CIFX_DRV_READ_STATE_ERROR = 0x800B0003L;
        public const ulong CIFX_DRV_CMD_ACTIVE = 0x800B0004L;
        public const ulong CIFX_DRV_DOWNLOAD_FAILED = 0x800B0005L;
        public const ulong CIFX_DRV_WRONG_DRIVER_VERSION = 0x800B0006L;
        public const ulong CIFX_DRV_DRIVER_NOT_LOADED = 0x800B0030L;
        public const ulong CIFX_DRV_INIT_ERROR = 0x800B0031L;
        public const ulong CIFX_DRV_CHANNEL_NOT_INITIALIZED = 0x800B0032L;
        public const ulong CIFX_DRV_IO_CONTROL_FAILED = 0x800B0033L;
        public const ulong CIFX_DRV_NOT_OPENED = 0x800B0034L;
        public const ulong CIFX_DEV_DPM_ACCESS_ERROR = 0x800C0010L;
        public const ulong CIFX_DEV_NOT_READY = 0x800C0011L;
        public const ulong CIFX_DEV_NOT_RUNNING = 0x800C0012L;
        public const ulong CIFX_DEV_WATCHDOG_FAILED = 0x800C0013L;
        public const ulong CIFX_DEV_SYSERR = 0x800C0015L;
        public const ulong CIFX_DEV_MAILBOX_FULL = 0x800C0016L;
        public const ulong CIFX_DEV_PUT_TIMEOUT = 0x800C0017L;
        public const ulong CIFX_DEV_GET_TIMEOUT = 0x800C0018L;
        public const ulong CIFX_DEV_GET_NO_PACKET = 0x800C0019L;
        public const ulong CIFX_DEV_MAILBOX_TOO_SHORT = 0x800C001AL;
        public const ulong CIFX_DEV_RESET_TIMEOUT = 0x800C0020L;
        public const ulong CIFX_DEV_NO_COM_FLAG = 0x800C0021L;
        public const ulong CIFX_DEV_EXCHANGE_FAILED = 0x800C0022L;
        public const ulong CIFX_DEV_EXCHANGE_TIMEOUT = 0x800C0023L;
        public const ulong CIFX_DEV_COM_MODE_UNKNOWN = 0x800C0024L;
        public const ulong CIFX_DEV_FUNCTION_FAILED = 0x800C0025L;
        public const ulong CIFX_DEV_DPMSIZE_MISMATCH = 0x800C0026L;
        public const ulong CIFX_DEV_STATE_MODE_UNKNOWN = 0x800C0027L;
        public const ulong CIFX_DEV_HW_PORT_IS_USED = 0x800C0028L;
        public const ulong CIFX_DEV_CONFIG_LOCK_TIMEOUT = 0x800C0029L;
        public const ulong CIFX_DEV_CONFIG_UNLOCK_TIMEOUT = 0x800C002AL;
        public const ulong CIFX_DEV_HOST_STATE_SET_TIMEOUT = 0x800C002BL;
        public const ulong CIFX_DEV_HOST_STATE_CLEAR_TIMEOUT = 0x800C002CL;
        public const ulong CIFX_DEV_INITIALIZATION_TIMEOUT = 0x800C002DL;
        public const ulong CIFX_DEV_BUS_STATE_ON_TIMEOUT = 0x800C002EL;
        public const ulong CIFX_DEV_BUS_STATE_OFF_TIMEOUT = 0x800C002FL;

        #endregion


        //PC 기반 모션제어를 위한 함수 생성//
        public static uint DriveConnect()
        {
            /* HOST mode definition */            
            UInt32 CIFX_HOST_STATE_READY = 1;            

            /* BUS state commands*/            
            UInt32 CIFX_BUS_STATE_ON = 1;           

            UInt32 ulState = 0;            
            UInt32 ulState_Timeout = 5000;


            // DRIVER OPEN
            _xDriverOpen(ref _hDriver);

            // CHANNEL OPEN
            // CIFX Driver Setup Utility에서 설정한 Devicedls cifx0와 firmware를 download한 channel 번호인 0을 2, 3번째 parameter로 설정
            _xChannelOpen(hDriver, "cifx0", 0, ref _hChannel);

            // Driver 정보 display
            _xDriverGetInformation(hDriver, (UInt32)Marshal.SizeOf(_DriverInformation), ref _DriverInformation);

            string str1 = Encoding.Default.GetString(DriverInformation.abDriverVersion);

            // HOST STATE를 READY로 설정
            _xChannelHostState(hChannel, CIFX_HOST_STATE_READY, ref ulState, ulState_Timeout);

            // BUS STATE를 ON으로 설정
            _xChannelBusState(hChannel, CIFX_BUS_STATE_ON, ref ulState, ulState_Timeout);

            return hDriver;
        }

        public static byte[] xChannelRead()
        {
            UInt32 ulData_Length = 100; // 4byte data
            UInt32 ulArea = 0;
            UInt32 ulReadOffset = 0;            

            // 4byte IN/OUT data
            byte[] ReadData = new byte[96];
			UInt32 ulTimeout = 100;
                        
            // 입력 데이터 read
            _xChannelIORead(hChannel, ulArea, ulReadOffset, ulData_Length, ReadData, ulTimeout);
                        
            return ReadData;
        }

		public static void xChannelWrite(byte[] data)
		{			
			UInt32 ulArea = 0;			
			UInt32 ulWriteOffset = 0;

			UInt32 ulTimeout = 100;

			// 입력 데이터 write			
			_xChannelIOWrite(hChannel, ulArea, ulWriteOffset, (uint)data.Length, data, ulTimeout);

		}

		public static void close()
        {
			UInt32 CIFX_BUS_STATE_OFF = 0;
			UInt32 CIFX_HOST_STATE_NOT_READY = 0;
			UInt32 ulState = 0;
			UInt32 ulTimeout = 100;			

			_xChannelBusState(hChannel, CIFX_BUS_STATE_OFF, ref ulState, ulTimeout);

			// HOST STATE를 NOT READY로 설정
			_xChannelHostState(hChannel, CIFX_HOST_STATE_NOT_READY, ref ulState, ulTimeout);

			// CHANNEL CLOSE
			_xChannelClose(hChannel);

			// DRIVER CLOSE
			_xDriverClose(hDriver);
		}
	}
}
