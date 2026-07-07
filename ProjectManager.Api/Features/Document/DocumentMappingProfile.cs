using AutoMapper;
using ProjectManager.Api.Features.Document.Requests;
using ProjectManager.Api.Features.Document.Responses;
using ProjectManager.Application.Document.Dto;

namespace ProjectManager.Api.Features.Document;

public class DocumentMappingProfile : Profile
{
    public DocumentMappingProfile()
    {
        CreateMap<UploadDocumentRequest, UploadDocumentDto>()
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.File.FileName))
            .ForMember(dest => dest.FileLength, opt => opt.MapFrom(src => src.File.Length));;

        CreateMap<DownloadDocumentDto, DownloadDocumentResponse>();
        
        CreateMap<DocumentsDto, DocumentsResponse>();
    }
}