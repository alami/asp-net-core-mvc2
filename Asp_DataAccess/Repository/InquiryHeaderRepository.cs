using Asp_DataAccess.Data;
using Asp_Models;
using Asp_Utility;
using Asp_DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp_DataAccess.Repository
{
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public InquiryHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(InquiryHeader obj)
        {
            _db.InquiryHeader.Update(obj);
        }      
    }
}
