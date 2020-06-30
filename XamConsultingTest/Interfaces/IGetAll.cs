using System;
using System.Collections.Generic;
using System.Text;

namespace XamConsultingTest.Interfaces
{
    public interface IGetAll<T>
    {
        List<T> GetAll();
    }
}
