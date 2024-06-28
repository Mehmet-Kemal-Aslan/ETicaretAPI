using ETicaretAPI.Application.Repositories.ProductImageFile;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.ChangeShowCaseImage
{
    public class ChangeShowCaseImageCommandHandler : IRequestHandler<ChangeShowCaseImageCommandRequest, ChangeShowCaseImageCommandResponse>
    {
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;

        public ChangeShowCaseImageCommandHandler(IProductImageFileWriteRepository productImageFileWriteRepository)
        {
            _productImageFileWriteRepository = productImageFileWriteRepository;
        }

        public async Task<ChangeShowCaseImageCommandResponse> Handle(ChangeShowCaseImageCommandRequest request, CancellationToken cancellationToken)
        {
            var query = _productImageFileWriteRepository.Table.Include(p => p.Products).SelectMany(p => p.Products, (pif, p) => new
            {
                pif,
                p
            });

            var datum = await query.FirstOrDefaultAsync(p => p.p.Id == request.ProductId && p.pif.Showcase);
            if (datum != null)
            {
                datum.pif.Showcase = false;
            }

            var image = await query.FirstOrDefaultAsync(p => p.pif.Id == request.ImageId);
            if (image != null)
            {
                image.pif.Showcase = true;
            }

            await _productImageFileWriteRepository.SaveAsync();

            return new();
        }
    }
}
