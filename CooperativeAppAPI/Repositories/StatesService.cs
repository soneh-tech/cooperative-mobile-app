namespace CooperativeAppAPI.Repositories
{
    public interface IStatesService
    {
        public Task<IEnumerable<tblState>> GetStatesAsync();
    }
    public class StatesService : IStatesService
    {
        private readonly AppDBContext context;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration configuration;
        public StatesService(AppDBContext context, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
        }

        public async Task<IEnumerable<tblState>> GetStatesAsync()
         => await context.tblState.ToListAsync();

    }
}
