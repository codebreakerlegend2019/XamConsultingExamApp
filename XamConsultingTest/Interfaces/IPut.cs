using System;
using System.Collections.Generic;
using System.Text;

namespace XamConsultingTest.Interfaces
{
    public interface IPut<T>
    {
        void Put(T entity);
    }
}
