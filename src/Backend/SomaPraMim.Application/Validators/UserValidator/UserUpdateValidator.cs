using FluentValidation;
using SomaPraMim.Communication.Requests.UserRequests;

namespace SomaPraMim.Application.Validators.UserValidator
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateRequest>
    {
        public UserUpdateValidator(){

        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("O nome não pode estar vazio.")
            .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.")
            .MaximumLength(150).WithMessage("O nome não pode ter mais de 150 caracteres.");
               
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("A senha não pode estar vazia.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.");
        }
    }
}