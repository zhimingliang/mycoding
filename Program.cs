using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//想法 :在 MyStringBuilder类里面创建一个 能装下 500个内存块 的 内存池  内存池用静态数组存储 内存块类型为嵌套类 每个内存块初始能存100个字符
        // MyStringBuilder 类里面两个静态方法 MyNew MyDelete   用来管理内存池的资源 
        // 申请空间的时候 就调用MyNew 从内存池 数组中分出来资源给 对象 判断依据是 mem(内存块类)中的IsUse 是不是false 
        // 当不用这块 资源的时候要手动释放   MyDelete给 内存池 
// Append  Insert   Replace   ToString 这几个函数 只考虑 是否 越界 越界要释放资源到内存池中(考虑到一般用字符超过100的几率很小 所以并没有在内存池中给他分配资源而是StringBuilder类自动扩容 )。
//其他都调用 StringBuilder自有的 相应函数
public  class MyStringBuilder 
{
    static int memCapcity;
    static mem[] m_mem ;           //初始化 有500个可被分配的结构体
    static MyStringBuilder()
    {
        memCapcity = 500;
        m_mem = new mem[500];
        for(int i=0;i<memCapcity;i++)
        {
            m_mem[i] = new mem();
        }
    }

    public StringBuilder chars;
    public int length;
    public int capcity = 100;

    //内存块 结构体
    class mem   
    {
        public StringBuilder m_block;          //每个 结构体里面的内存块可以装100个字符
        public  bool IsUsed;                                    //初始化没被使用
        public  MyStringBuilder m_str;                                  // 字段
        public  mem()
        {
            m_block = new StringBuilder(100);  
            IsUsed = false; 
            m_str=null;
        }
    }

       
   
//无参构造
public MyStringBuilder()
{
    MyNew(chars);
    length = 0;
}
  
//添加
public MyStringBuilder Append(string value)
{
	if (value == null)
		return this;
        //新申请 一块 空间 把原来的空间释放掉 (但是执行到这步的 几率很小很少有超过 100大小的字符串)
	if (length + value.Length > capcity)
	{
		while (length + value.Length > capcity) 
		{
			capcity *= 2 ;
		}
		   
        StringBuilder m_new = new StringBuilder(capcity);
        m_new.Append( chars.ToString());
        MyDelete(chars);                    //释放掉在池中的空间
        chars = m_new;
	}
    chars.Append(value);
	return this;
}
//插入
public MyStringBuilder Insert(int index, string value)
{
	if (index < 0 || index > length || value == null)
		return this;
    //这个越界的 几率也比较小 (空间 定容100)
	if (length + value.Length > capcity)
	{
		while (length + value.Length > capcity) 
		{
			capcity *= 2 ;
		}
        StringBuilder m_new = new StringBuilder(capcity);
        m_new.Append(chars.ToString());
        MyDelete(chars);                    //释放掉在池中的空间
        chars = m_new;
	}
    chars.Insert(index, value);
	return this;
}
//replace 
public MyStringBuilder Replace(string oldValue, string newValue)
{
    if (newValue == null || oldValue == null)
		return this;
	//看是不是需要扩容 
    StringBuilder m_temp = chars;                   //标记一下 chars
    //发生扩容 的几率比较小 
    if (chars.Replace(oldValue, newValue).Length > 100)
    {
        MyDelete(m_temp);               //如果扩容了就删除原来的空间 
    }
	return this;
}
public override string ToString()
{
	//创建一个string类型的 变量 然后返回

    return chars.ToString();
}

//从池中申请空间 
public static void MyNew( StringBuilder other)
{
    for (int i = 0; i < memCapcity; i++)
    {
        if (!m_mem[i].IsUsed)
        { 
            //没有被使用可以分配 就分配下空间 
            other = m_mem[i].m_block;
            m_mem[i].IsUsed = true;
            return;
        }
    }
    //没有能 分配的空间了 内存大小要扩容(或者一开始就开了足够的空间)
            
}
//从池中删除空间
public static void MyDelete(StringBuilder other)
{
    if (other==null)
        return;
    for (int i = 0; i < memCapcity; i++)
    {
        if ( StringBuilder.Equals( other,  m_mem[i].m_block) )
        {

            m_mem[i].m_block = null;
            m_mem[i].IsUsed = false;                    //将这块内存标记成 可用内存
            return;
        }
    }
}

}
class Program
{
    static void Main(string[] args)
    {

        MyStringBuilder mmmmm = new MyStringBuilder();
        mmmmm.Append("Hello World");
            
    }
}