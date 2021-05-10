using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemoryShare
{
   public class MemoryShareClass
    {
        private Semaphore m_Write;  //可写的信号
        private Semaphore m_Read;   //可读的信号
        private IntPtr m_handle;    //文件句柄
        private IntPtr m_addr;      //共享内存地址
        uint mapLength;             //共享内存长

        Thread threadRed;           //线程用来读取数据


        #region 内存操作的函数

        const int INVALID_HANDLE_VALUE = -1;
        const int PAGE_READWRITE = 0x04;
        const int FILE_MAP_ALL_ACCESS = 0x0002;
        const int FILE_MAP_WRITE = 0x0002;

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        //共享内存
        [DllImport("Kernel32.dll", EntryPoint = "CreateFileMapping")]
        private static extern IntPtr CreateFileMapping(IntPtr hFile, //HANDLE hFile,
         UInt32 lpAttributes,                                        //LPSECURITY_ATTRIBUTES lpAttributes,  //0
         UInt32 flProtect,                                           //DWORD flProtect
         UInt32 dwMaximumSizeHigh,                                   //DWORD dwMaximumSizeHigh,
         UInt32 dwMaximumSizeLow,                                    //DWORD dwMaximumSizeLow,
         string lpName                                               //LPCTSTR lpName
         );

        [DllImport("Kernel32.dll", EntryPoint = "OpenFileMapping")]
        private static extern IntPtr OpenFileMapping(
         UInt32 dwDesiredAccess,        //DWORD dwDesiredAccess,
         int bInheritHandle,            //BOOL bInheritHandle,
         string lpName                  //LPCTSTR lpName
         );


        [DllImport("Kernel32.dll", EntryPoint = "MapViewOfFile")]
        private static extern IntPtr MapViewOfFile(
         IntPtr hFileMappingObject,     //HANDLE hFileMappingObject,
         UInt32 dwDesiredAccess,        //DWORD dwDesiredAccess
         UInt32 dwFileOffsetHight,      //DWORD dwFileOffsetHigh,
         UInt32 dwFileOffsetLow,        //DWORD dwFileOffsetLow,
         UInt32 dwNumberOfBytesToMap    //SIZE_T dwNumberOfBytesToMap
         );

        [DllImport("Kernel32.dll", EntryPoint = "UnmapViewOfFile")]
        private static extern int UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("Kernel32.dll", EntryPoint = "CloseHandle")]
        private static extern int CloseHandle(IntPtr hObject);

        #endregion


        public MemoryShareClass()
        {
            mapLength = 1024;
        }



        ///<summary>
        /// 初始化共享内存数据 创建一个共享内存
        ///</summary>
        public void init()
        {
            IntPtr hFile;

            m_Write = new Semaphore(1, 1, "WriteMap");  //开始的时候有一个可以写
            m_Read = new Semaphore(0, 1, "ReadMap");    //没有数据可读

            hFile = new IntPtr(INVALID_HANDLE_VALUE);
            m_handle = CreateFileMapping(hFile, 0, PAGE_READWRITE, 0, mapLength, "shareMemory");
            m_addr = MapViewOfFile(m_handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);
            Console.WriteLine("Memory Address Operating:" + m_addr);

            //handle = OpenFileMapping(0x0002, 0, "shareMemory");
            //addr = MapViewOfFile(handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);

            threadRed = new Thread(new ThreadStart(ThreadReceive));
            threadRed.Start();
        }

        /// <summary>
        /// 线程启动从共享内存中获取数据信息 
        /// </summary>
        private void ThreadReceive()
        {
            byte[] byteStr;
            string str;

            while (true)
            {
                try
                {
                    //m_Write = Semaphore.OpenExisting("WriteMap");
                    //m_Read = Semaphore.OpenExisting("ReadMap");
                    //handle = OpenFileMapping(FILE_MAP_WRITE, 0, "shareMemory");

                    //读取共享内存中的数据：
                    m_Read.WaitOne();//是否有数据写过来
                    Console.WriteLine("接收到发送过来的数据:");

                    //IntPtr m_Sender = MapViewOfFile(handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);
                    byteStr = new byte[mapLength];
                    ByteCopy(byteStr, m_addr);
                    str = Encoding.Default.GetString(byteStr, 0, byteStr.Length);
                    if (str[0] != '\0')
                        Console.WriteLine(str.Contains('\0') ? str.Substring(0, str.IndexOf('\0')) : str);
                    else
                        Console.WriteLine("Data Emptry, Error!");

                    //调用数据处理方法 处理读取到的数据
                    m_Write.Release();

                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    continue;
                    //Thread.Sleep(0);
                }

            }

        }


        //不安全的代码在项目生成的选项中选中允许不安全代码
        static unsafe void ByteCopy(byte[] dst, IntPtr src)
        {
            fixed (byte* pDst = dst)
            {
                byte* pdst = pDst;
                byte* psrc = (byte*)src;
                while ((*pdst++ = *psrc++) != '\0')
                    ;
            }

        }



        #region 向进程共享内存中写入数据（B模式运行的进程）

        /// <summary>
        /// B进程：向共享内存中写入的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void WriteData2SharedMemory()
        {
            string readedBuffer;
            byte[] sendStr;


            try
            {
                m_Write = Semaphore.OpenExisting("WriteMap");
                m_Read = Semaphore.OpenExisting("ReadMap");
                m_handle = OpenFileMapping(FILE_MAP_WRITE, 0, "shareMemory");
                m_addr = MapViewOfFile(m_handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);
                Console.WriteLine("B Process Memory Address Operating:" + m_addr);


                while (true)
                {

                    // 等待写的信号量
                    m_Write.WaitOne();

                    // 获取写入数据的访问权限，开始对共享资源的访问。
                    Console.WriteLine("退出请输入[E]:");
                    readedBuffer = Console.ReadLine();
                    if (readedBuffer.Contains("E"))
                    {
                        m_Write.Release();
                        break;
                    }

                    Console.WriteLine("请输入一行数据，将数据写入共享内存[不大于1024个字符]：");
                    readedBuffer = Console.ReadLine();
                    sendStr = Encoding.Default.GetBytes(readedBuffer + '\0');
                    //如果要是超长的话，应另外处理，最好是分配足够的内存
                    if (sendStr.Length < mapLength)
                        CopyByte2Memory(sendStr, m_addr);

                    m_Read.Release();
                }

            }
            catch (WaitHandleCannotBeOpenedException)
            {
                Console.WriteLine("不存在系统信号量!");
                return;
            }
        }


        static unsafe void CopyByte2Memory(byte[] byteSrc, IntPtr dst)
        {
            fixed (byte* pSrc = byteSrc)
            {
                byte* pDst = (byte*)dst;
                byte* psrc = pSrc;
                for (int i = 0; i < byteSrc.Length; i++)
                {
                    *pDst = *psrc;
                    pDst++;
                    psrc++;
                }
            }
        }


        #endregion
    }
}
