using Estimator.Domain.Enums;

namespace Estimator.Inerfaces;

public interface ITarifficatorService
{
    Task CreateTarifficator(IFormFile file,TarifficatorType tarifficatorType);
}