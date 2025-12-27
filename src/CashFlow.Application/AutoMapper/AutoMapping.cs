using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;

namespace CashFlow.Application.AutoMapper;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        RequestToEntity();
        EntityToResponse();
    }

    private void RequestToEntity()
    {
        CreateMap<RequestExpenseJson, Expense>()
            //****************************************************************************************
            // Este trecho remove tags duplicadas antes de mapear, por conta do Distinct()
            // que pode ser necessário caso o cliente envie tags repetidas.
            // O Distinct faz parte do LINQ e retorna uma coleção sem elementos duplicados.
            //****************************************************************************************
            .ForMember(dest => dest.Tags, config => config.MapFrom(source => source.Tags.Distinct()));

        CreateMap<RequestRegisterUserJson, User>()
            .ForMember(dest => dest.Password, config => config.Ignore());

        //*********************************************************************************
        // Estou ensinando ao automapper que quando precisar mapear um Tag (enum)
        // em uma entidade Tag, ele deve criar uma instância da entidade e na 
        // propriedade value, atribuir o valor do enum.
        //*********************************************************************************
        CreateMap<Communication.Enums.Tag, Tag>()
            .ForMember(dest => dest.Value, config => config.MapFrom(source => source));
    }

    private void EntityToResponse()
    {
        CreateMap<Expense, ResponseExpenseJson>()
            //****************************************************************************************
            // O Select() percorre cada elemento da coleção Tags e retorna uma nova coleção com
            // os valores extraídos da propriedade Value de cada Tag.
            //****************************************************************************************
            .ForMember(dest => dest.Tags, config => config.MapFrom(source => source.Tags.Select(tag => tag.Value)));
        CreateMap<Expense, ResponseRegisteredExpenseJson>();
        CreateMap<Expense, ResponseShortExpenseJson>();
        CreateMap<User, ResponseUserProfileJson>();
    }
}
