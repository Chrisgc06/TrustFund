using AutoMapper;
using TrustFund.Services.Dtos.User;
using TrustFund.Services.Dtos.Loan;
using TrustFund.Services.Dtos.Payment;
using TrustFund.Domain.Entities; // Necesario para las entidades del Dominio

namespace TrustFund.Services.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeos de User
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
                .ForMember(dest => dest.Email, opt => opt.Condition(src => src.Email != null))
                .ForMember(dest => dest.PhoneNumber, opt => opt.Condition(src => src.PhoneNumber != null));
            CreateMap<User, UserDto>();

            // Mapeos de Loan
            CreateMap<CreateLoanDto, Loan>()
                .ForMember(dest => dest.StartDate, opt => opt.Ignore()) // Se setea en el servicio
                .ForMember(dest => dest.OutstandingAmount, opt => opt.Ignore()) // Se setea en el servicio
                .ForMember(dest => dest.Status, opt => opt.Ignore()); // Se setea en el servicio

            CreateMap<UpdateLoanDto, Loan>()
                .ForMember(dest => dest.Amount, opt => opt.Condition(src => src.Amount.HasValue))
                .ForMember(dest => dest.InterestRate, opt => opt.Condition(src => src.InterestRate.HasValue))
                .ForMember(dest => dest.DueDate, opt => opt.Condition(src => src.DueDate.HasValue))
                .ForMember(dest => dest.Status, opt => opt.Condition(src => src.Status.HasValue));

            CreateMap<Loan, LoanDto>();

            // Mapeos de Payment
            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.PaymentDate, opt => opt.Ignore()); // Se setea en el servicio
            CreateMap<Payment, PaymentDto>();
        }
    }
}