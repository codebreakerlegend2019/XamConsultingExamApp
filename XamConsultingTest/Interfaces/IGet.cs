using System;
using System.Collections.Generic;
using System.Text;

namespace XamConsultingTest.Interfaces
{
    public interface IGet<T>
    {
        T Get(int id);
    }
}
