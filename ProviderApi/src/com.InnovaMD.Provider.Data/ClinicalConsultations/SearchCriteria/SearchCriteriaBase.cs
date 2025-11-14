using System;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria
{
    public abstract class SearchCriteriaBase
    {
        private int pageSize = 10;
        public virtual int PageSize
        {
            get { return pageSize; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"Invalid PageSize value of {value}.");
                }
                pageSize = value;
            }
        }

        private int page = 1;

        public virtual int Page
        {

            get { return page; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"Invalid Page value of {value}.");
                }
                page = value;
            }
        }

        public int Offset
        {
            get
            {
                return (Page - 1) * PageSize;
            }
        }


        public int Fetch
        {
            get
            {
                return PageSize;
            }
        }
    }
}
