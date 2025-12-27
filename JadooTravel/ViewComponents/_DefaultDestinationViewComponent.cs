

using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.DestinationDtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JadooTravel.ViewComponents
{
    public class _DefaultDestinationViewComponent:ViewComponent
    {
        private readonly IDestinationService _destinationService;
        private readonly IMapper _mapper;

        public _DefaultDestinationViewComponent(IDestinationService destinationService, IMapper mapper)
        {
            _destinationService = destinationService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values=await _destinationService.TGetAllListAsync();
            var result = _mapper.Map<List<ResultDestinationDto>>(values);
            return View(result);
        }
    }
}
