namespace CooperativeAppAPI.Helpers
{
    public class UserAuditClass
    {
        private readonly AppDBContext context;
        public UserAuditClass(AppDBContext context)
        {
            this.context = context;
            //
            // TODO: Add constructor logic here
            //
        }

        public void inserUserAppraisal(int sid, int acaid, int ap)
        {

        }

        public async void insertAtrail(string tbname, string op, int userid, string fname, string oldv, string newv)
        {
            var u = (from h in context.Staff
                     where h.StaffID == userid
                     select h).ToList().SingleOrDefault();
            if (u != null)
            {
                string mytime = (DateTime.Now).ToString("hh:mm tt");
                tblaudittrail a = new tblaudittrail();
                a.tablename = tbname;
                a.operation = op;
                a.fieldname = fname;
                a.occurreddate =Convert.ToDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
                a.timeoccurred = mytime;
                a.performedbyname = u.FirstName + " " + u.LastName;
                a.performedbyid = userid;
                a.oldvalue = oldv;
                a.newvalue = newv;
              await  context.tblaudittrail.AddRangeAsync(a);
              await  context.SaveChangesAsync();
            }
        }

        public async void insertAtrailMod(string tbname, string op, int userid, string fname, string oldv, string newv, int tbid)
        {
            var u = (from h in context.Staff
                     where h.StaffID == userid
                     select h).ToList().SingleOrDefault();
            if (u != null)
            {
                string mytime = (DateTime.Now).ToString("hh:mm tt");

                tblaudittrail a = new tblaudittrail();
                a.tablename = tbname;
                a.operation = op;
                a.fieldname = fname;
                a.occurreddate =Convert.ToDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
                a.timeoccurred = mytime;
                a.performedbyname = u.FirstName + " " + u.LastName;
                a.performedbyid = userid;
                a.oldvalue = oldv;
                a.newvalue = newv;
                a.tableId = tbid;

              await  context.tblaudittrail.AddAsync(a);
            await    context.SaveChangesAsync();
            }
        }
    }
}