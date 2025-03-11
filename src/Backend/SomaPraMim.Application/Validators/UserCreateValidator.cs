using SomaPraMim.Communication.Requests.UserRequests;
using FluentValidation;


namespace SomaPraMim.Application.Validators
{
    public class UserCreateValidator : AbstractValidator<UserCreateRequest>
    {
        public UserCreateValidator()
        {
            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("O nome não pode estar vazio.")
                .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.")
                .MaximumLength(150).WithMessage("O nome não pode ter mais de 150 caracteres.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("O e-mail não pode estar vazio.")
                .EmailAddress().WithMessage("O e-mail informado é inválido.");
               
               
            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("A senha não pode estar vazio.")
                .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.");
        }
    }
}