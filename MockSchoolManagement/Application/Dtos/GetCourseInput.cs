using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Dtos
{
    public class GetCourseInput: PagedSortedAndFilterInput
    {
        public GetCourseInput()
        {
            Sorting = "CourseID";
            MaxResultCount = 3;
        }
    }
}
