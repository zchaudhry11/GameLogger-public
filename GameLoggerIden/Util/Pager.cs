using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameLoggerIden.Util
{
    public class Pager
    {
        public int NumElements { get; set; }
        public int CurrPage { get; set; }
        public int NumResults { get; set; }
        public int TotalPages { get; set; }
        public int FirstPage { get; set; }
        public int LastPage { get; set; }

        public Pager(int numElements, int? pageNum, int numResults)
        {
            // calculate total, start and end pages
            var totalPages = (int)Math.Ceiling((decimal)numElements / (decimal)numResults);
            var currentPage = pageNum != null ? (int)pageNum : 1;
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            NumElements = numElements;
            CurrPage = currentPage;
            NumResults = numResults;
            TotalPages = totalPages;
            FirstPage = startPage;
            LastPage = endPage;
        }
    }
}