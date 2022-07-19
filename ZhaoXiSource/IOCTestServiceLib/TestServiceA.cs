using IOCTestInterfaceLib;
using System;

namespace IOCTestServiceLib
{
    public class TestServiceA : ITestServiceA
    {
        public TestServiceA()
        {
            Console.WriteLine($"{this.GetType().Name}被构造。。。");
        }

        public void Show()
        {
            Console.WriteLine("A123456");
        }
    }
}
