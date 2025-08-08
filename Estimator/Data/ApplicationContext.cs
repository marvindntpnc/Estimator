using Microsoft.EntityFrameworkCore;

namespace Estimator.Data;

public class ApplicationContext:DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        
    }
    
    
}