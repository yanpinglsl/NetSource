using System;
using IOCTestInterfaceLib;

namespace IOCTestServiceLib
{
    public class TestServiceAV2 : ITestServiceA
    {
        public TestServiceAV2()
        {
            Console.WriteLine($"{this.GetType().Name} V2被构造。。。");
        }

        public void Show()
        {
            Console.WriteLine("A123456");
        }
    }
}
