using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonManager.WebAPIs
{
    class Result<T>
    {
        public bool IsSuccess;
        public T SuccessData;
        public FailData FailData;

        public Result(bool isSuccess, T successData, FailData failData)
        {
            IsSuccess = isSuccess;
            SuccessData = successData;
            FailData = failData;
        }

    }
}
