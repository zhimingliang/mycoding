using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;

//思路：  1>子文件全部读出到中间文件(记录每个文件的信息)
        //2>创建最终文件的文件头(  8字节-文件个数        文件个数*[ 8字节文件索引 80字节-文件名  8字节-文件大小 ]   )
        //3>将中间文件的内容 全部存入到最终文件里(加上文件头偏移量)
        //4>创建文件管理类对象 进行 文件操作 

        //文件信息类 存放 文件名称 文件大小 文件索引
        //文件处理类 文件信息类的list 读文件到内存的操作 
        

//定义文件信息类
class MyFileInfo
{
    public int index;
    public string FileFullName;     //文件全名
    public long Size;               //文件 所占大小
    public MyFileInfo()
    {
        FileFullName = null;
        index = 0;
        Size = 0;
    }
    public MyFileInfo(int InIndex,string InFullName, long InSize)
    {
        FileFullName = InFullName;              //文件名称
        index = InIndex;                        //标记索引         
        Size = InSize;                          //文件大小
    }
}

//文件处理类 
class MyFileManage
{
    public int filecount;                   //文件的个数
    public List<MyFileInfo> m_fileinfoList;         //文件数组

    public MyFileManage()
    {
        m_fileinfoList = new List<MyFileInfo>();
        //通过解析 传入的文件头数据 来构建 字段 
        GetFileInfo();
    }

    //获得文件名称  文件大小  index 赋值 
    public int GetFileInfo()
    {

        //前8字节 转成int 是文件的数量
        Byte[] a1 = new Byte[8];
        Byte[] a2 = new Byte[8];


        using (FileStream fileRead = new FileStream("MyFinelFile", FileMode.Open, FileAccess.ReadWrite))
        {

            fileRead.Read(a1, 0, 8);               //读取文件头 
        }
        for (int i = 0; i < 8; i++)
        {
            a2[i] = a1[i];
        }
        string ret = System.Text.Encoding.Default.GetString(a2);
        //将字符串转成 数字
        filecount = Convert.ToInt32(ret, 16);

        //文件头的大小 8+96*count
        Byte[] InData = new Byte[8 + filecount * 96];
        //将文件头信息读取到 InData 中
        using (FileStream fileRead = new FileStream("MyFinelFile", FileMode.Open, FileAccess.ReadWrite))
        {
            //fileRead.Seek(8,SeekOrigin.Begin);
            fileRead.Read(InData, 0, 8 + filecount * 96);               //读取文件头 
        }

        for (int i = 0; i < filecount; i++)
        {
            //将count个数的 文件信息 加入到 文件数组中
            //1> 文件的索引 8+96*i+j
            Byte[] fileIndex = new Byte[8];
            for (int j = 0; j < 8; j++)
            {
                fileIndex[j] = InData[8 + i * 96 + j];
            }
            //2>文件 的 文本信息 8+96*i+8+j
            Byte[] fileData = new Byte[80];
            for (int j = 0; j < 80; j++)
            {
                fileData[j] = InData[8 + i * 96 + 8 + j];
            }
            //3>文件 的 大小 8+96*i+8+80
            Byte[] fileSize = new Byte[8];
            for (int j = 0; j < 8; j++)
            {
                fileSize[j] = InData[8 + i * 96 + 8 + 80 + j];
            }
            //将内容copy到 fileinfo中
            ret = System.Text.Encoding.UTF8.GetString(fileIndex);
            int m_index = 0;
            m_index = Convert.ToInt32(ret, 16);
            ret = System.Text.Encoding.Default.GetString(fileSize);
            int m_size = Convert.ToInt32(ret, 16);
            ret = System.Text.Encoding.Default.GetString(fileData);
            //将文本的空格去掉    
            ret = ret.Trim();
            MyFileInfo m_file = new MyFileInfo(m_index, ret, m_size);            //索引 内容 大小 
            m_fileinfoList.Add(m_file);                  //添加到 链表容器中
        }


        return 0;
    }

    //当串文件名称近来的时候 要确定他的位置 从index开始找 
    public MyFileInfo Find(string str)
    {

        foreach (MyFileInfo m_fileInfo in m_fileinfoList)
        {
            if (m_fileInfo.FileFullName.Equals(str))
                return m_fileInfo;
        }
        return null;           //没找到 
    }
    //读子文件
    public Byte[] Read(string DescFileName)
    {
        using (FileStream fileRead = new FileStream("MyFinelFile", FileMode.Open, FileAccess.ReadWrite))
        {
            //fileRead.Seek(8,SeekOrigin.Begin);
            //读多少信息 
            MyFileInfo m_fileInfo = Find(DescFileName);
            if (m_fileInfo != null)
            {
                //定位 文件在其中的位置 
                long offset = 8 + filecount * 96;                //定义文件头 偏移量
                for (int i = 0; i < m_fileInfo.index; i++)
                {
                    offset += m_fileinfoList[i].Size;
                }
                //定位到偏移量的位置 
                fileRead.Seek(offset, SeekOrigin.Begin);
                //创建 缓冲区
                Byte[] temp = new Byte[m_fileInfo.Size];
                fileRead.Read(temp, 0, temp.Length);            //读出数据 然后返回 
                return temp;
            }

        }

        return null;
    }
    //写子文件 
    public void Write(byte[] array, int offset, int count)
    {

    }

}

//写入大文件 
class MyText
{
    public static List<MyFileInfo> m_fileList = new List<MyFileInfo>();          //创建 文件信息类 对象 储存结构


    static void Main(string[] args)
    {
        
        int FileIndex = -1;
        Queue<string> m_que = new Queue<string>();
        using (FileStream fileWrite = new FileStream("MyAllLua", FileMode.Create, FileAccess.Write))
        {
            //创建一个空的 
        }
        m_que.Enqueue(@"E:\game\");
        while (m_que.Count != 0)
        {
            //获得当前 目录的 信息 
            DirectoryInfo TheFolder = new DirectoryInfo(m_que.Dequeue());
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                //将文件夹 加入到 队列
                m_que.Enqueue(NextFolder.FullName);

            }
            //-------------------------------------------------------将子文件写入到大文件当中-------------------------------------------------------
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                //只有正确后缀名 才能 写入到大文件当中
                if (Equals(Path.GetExtension(NextFile.Name), ".lua"))
                {
                    //读取 当前 文件 
                    using (FileStream currentRead = new FileStream(NextFile.FullName, FileMode.Open, FileAccess.Read))
                    {
                        //写入到 大文件
                        using (FileStream fileWrite = new FileStream("MyAllLua", FileMode.Append, FileAccess.Write))
                        {
                            Console.WriteLine(NextFile.FullName);

                            //获得 文件的 大小 
                            long nLen = NextFile.Length;
                            Byte[] buff = new Byte[nLen];

                            //读取 子文件 
                            currentRead.Read(buff, 0, (int)nLen);
                            FileIndex++;                                                                        //文件的索引++
                            MyFileInfo m_tempInfo = new MyFileInfo(FileIndex, NextFile.FullName, nLen);         //生成 文件信息 
                            m_fileList.Add(m_tempInfo);                                                         //文件链表中添加 这个文件               
                            fileWrite.Write(buff, 0, (int)nLen);                                                //将文件 写入大文件中

                        }
                    }
                }
            }


        }


        //这个MyAllLua就是中间文件
        //--------------------------------------------------写文件头------------------------------------------------------------------------
        //计算 文件头的大小   4+FileCount*(48)               8字节是 多少个文件          8字节是文件索引 80字节是文件名 8字节是文件大小
        int FileSize = 8 + m_fileList.Count * 96;           //文件头大小
        //将文件头 写进 最终文件 
        using (FileStream fileWrite = new FileStream("MyFinelFile", FileMode.Create, FileAccess.Write))
        {
            //byte[] shi = System.BitConverter.GetBytes(s);
            String strA = m_fileList.Count.ToString("x8");
            fileWrite.Write(System.Text.Encoding.Default.GetBytes(strA), 0, 8);          //写入前八个字节
            //顺次写入48字节单文件信息
            int index = 0;
            foreach (MyFileInfo m_file in m_fileList)
            {
                strA = index.ToString("x8");
                fileWrite.Write(System.Text.Encoding.Default.GetBytes(strA), 0, 8);                                       //写入文件索引

                m_file.FileFullName=m_file.FileFullName.PadRight(80, ' ');

                fileWrite.Write(System.Text.Encoding.Default.GetBytes(m_file.FileFullName), 0, 80);                       //写入文件名 
                strA = m_file.Size.ToString("x8");
                fileWrite.Write(System.Text.Encoding.Default.GetBytes(strA), 0, 8);                                       //写入文件大小
                index++;
            }        
        }

        //-----------------------------------------------------写文件主体-------------------------------------------------------------------------
        //从中间文件将内容全部 打入到最终文件中

        using (FileStream fileRead = new FileStream("MyAllLua", FileMode.Open, FileAccess.ReadWrite))
        { 
            Byte[] buff = new Byte[fileRead.Length];            //创建 缓冲区

            fileRead.Read(buff, 0, buff.Length);

           
            //将内容 写入到最终文件中
            using (FileStream fileWrite = new FileStream("MyFinelFile", FileMode.Append, FileAccess.Write))
            {
                fileWrite.Write(buff, 0, buff.Length);
            }

        }

        //-----------------------------------------------------文件写完-----------------------------------------------------



        //--------------------------------------------------------测试--------------------------------------------------

        MyFileManage textfile = new MyFileManage();

        Byte[] strbuff = textfile.Read(@"E:\game\RewardsMod\View\RewardsView.lua");
        string m_textStr=System.Text.Encoding.UTF8.GetString(strbuff);


        //--------------------------------------------------------测试--------------------------------------------------
       
    }
}

