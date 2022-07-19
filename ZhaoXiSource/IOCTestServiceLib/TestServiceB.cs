using IOCTestInterfaceLib;
using System;

namespace IOCTestServiceLib
{
    public class TestServiceB : ITestServiceB
    {
        private ITestServiceA _ITestServiceA = null;
        public TestServiceB(ITestServiceA iTestServiceA)
        {
            Console.WriteLine($"{this.GetType().Name}被构造。。。");
            this._ITestServiceA = iTestServiceA;
        }


        public void Show()
        {
            Console.WriteLine($"This is TestServiceB B123456");
        }
    }
}
