using System;
using System.Collections.Generic;
using System.Text;

namespace XamConsultingTest.Interfaces
{
    public interface IPost<T>
    {
        void Post(T entity);
    }
}
