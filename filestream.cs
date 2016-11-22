using System;
using System.IO;
using System.Collections.Generic;

//定义文件信息  
class MyFileInfo 
{
    public string FileFullName;     //文件全名
    public long Position;            //当前文件所在位置
    public long Size;               //文件 所占大小
    public MyFileInfo()
    {
        FileFullName = null;
        Position = 0;
        Size = 0;
    }
    public MyFileInfo(string InFullName, long InPosition, long InSize)
    {
        FileFullName = InFullName;
        Position = InPosition;
        Size = InSize;
    }
}

//自己定义 文件流  把文件流再封装一下 主要用于查找指定文件在大文件里面的位置
class MyFileStream
{
    public FileStream file;
    public string Allpath;
    public string FileName;
    public long Position;            //当前文件所在位置
    public long Size;               //文件 所占大小    public MyFileStream(MyFileInfo info)
    public MyFileStream(MyFileInfo info, FileMode mode, FileAccess access)
    {
        Allpath = @"D:\C#Project\11-21\11-21\bin\Debug\MyAllLua";
        FileName = info.FileFullName;
        Position = info.Position;
        Size = info.Size;
        file = new FileStream(Allpath, mode, access);
    }
    public int Read(byte[] array, int offset, int count)
    {
        file.Position= file.Seek(Position, SeekOrigin.Begin);
        return file.Read(array,offset,count);
    }
    public void Write(byte[] array, int offset, int count)
    {
        file.Position = file.Seek(Position, SeekOrigin.Begin);
        file.Write(array, offset, count);
    }

}



 
class MyText
{
    public  static List< MyFileInfo> m_fileList=new  List< MyFileInfo>();          //创建 文件信息类 对象 储存结构
    

    static void Main(string[] args)
    {
        long sum = 0;
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
            //文件第一行 1>文件的大小  2>文件头行数   3>文件头偏移量
            //文件头 后几行 文件的名称 文件的虚拟位置 文件的大小 
            //通过 传入 子文件的名称 在文件头中逐行查找 找到之后 用文件的虚拟位置+文件头偏移量获得子文件在大文件中的位置  
            //通过调用 read（) 读取子文件 并加载到内存 
           // int hangNum = 0;        //文件头行数 
            //从文件夹例遍历文件     -----------------------------------------  计算文件头 写法 -------------------------------------
            //foreach (FileInfo NextFile in TheFolder.GetFiles())
            //{
            //    //只有正确后缀名 才能 写入到大文件当中
            //    if (Equals(Path.GetExtension(NextFile.Name), ".lua"))
            //    {
            //        //读取 当前 文件 
            //        using (FileStream currentRead = new FileStream(NextFile.FullName, FileMode.Open, FileAccess.Read))
            //        {
            //            //写入到 大文件
            //            using (FileStream fileWrite = new FileStream("MyAllLua", FileMode.Append, FileAccess.Write))
            //            {
            //               //获得 文件的 大小 
            //               long nLen = NextFile.Length;
            //               sum += nLen;
            //               hangNum++;
            //               fileWrite.Seek(0,SeekOrigin.Begin);//第二个参数将 开始位置结束位置 或者 当前位置当做给定点 
            //                fileWrite.w
            //            }
            //        }
            //    }
            //}


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
                            currentRead.Read(buff, 0, (int)nLen);
                            //将文件 写入大文件中 
                            fileWrite.Write(buff, 0, (int)nLen);
                            //然后 记录信息
                            MyFileInfo m_tempInfo = new MyFileInfo(NextFile.FullName, sum, nLen);
                            m_fileList.Add(m_tempInfo);
                            sum += nLen;
                            //sw.Write(buff);
                        }
                    }
                }
            }
            //using (FileStream fileWrite = new FileStream("MyAllLua", FileMode.Open, FileAccess.ReadWrite))
            //{
               
            //    fileWrite.Seek(0, SeekOrigin.Begin);             
            //   j
            //    fileWrite.Write( System.Text.Encoding.Default.GetBytes("AAAA"), 0, 4);
            //}
           



        }
       
        Console.WriteLine(sum);
    }
      



}
