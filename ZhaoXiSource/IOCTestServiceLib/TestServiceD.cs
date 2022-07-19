using IOCTestInterfaceLib;
using System;

namespace IOCTestServiceLib
{
    public class TestServiceD : ITestServiceD
    {
        public TestServiceD()
        {
            Console.WriteLine($"{this.GetType().Name}被构造。。。");
        }
        public void Show()
        {
            Console.WriteLine("D123456");
        }
    }
}
